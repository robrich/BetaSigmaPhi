namespace BetaSigmaPhi.Service {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Library;
	using BetaSigmaPhi.Repository;

	public interface IDocumentService {
		string SanitizeSlug( string Slug, string Name, int MaxLength );
		string GetSlugPath( int NodeId );
		List<Document> GetParents( int NodeId );
		void SaveModifiedNode( Document Node );
		void EnsureRootNoteExists();
	}

	public class DocumentService : IDocumentService {
		private readonly IDocumentRepository documentRepository;

		public DocumentService( IDocumentRepository DocumentRepository ) {
			if ( DocumentRepository == null ) {
				throw new ArgumentNullException( "DocumentRepository" );
			}
			this.documentRepository = DocumentRepository;
		}

		private readonly Regex goodCharsRegex = new Regex( "([^a-zA-Z0-9\\-])" );

		public string SanitizeSlug( string Slug, string Name, int MaxLength ) {
			string results = Slug;
			if ( string.IsNullOrEmpty( results ) ) {
				results = (Name ?? "").Trim();
			}
			if ( !string.IsNullOrEmpty( results ) ) {
				results = results.Trim().ToLowerInvariant().Replace( ' ', '-' ).Replace( '_', '-' ); // Pretty SEO
				results = this.goodCharsRegex.Replace( results, "" );
				results = results.Replace( "--", "-" );
				if ( results.Length > MaxLength ) {
					results = results.Substring( 0, MaxLength );
				}
			}
			return results;
		}

		// Root node's path is "", not found is null
		public string GetSlugPath( int NodeId ) {
			string results = null;

			if ( NodeId == Document.ROOT_NODE_ID ) {
				results = ""; // Blank but not null
				return results;
			}

			List<int> pieces = this.GetParentNodeIds( NodeId );
			if ( pieces.IsNullOrEmpty() ) {
				return results;
			}

			if ( pieces.Contains( Document.ROOT_NODE_ID ) ) {
				pieces.Remove( Document.ROOT_NODE_ID ); // Root node isn't part of the slug path
			}

			Dictionary<int, string> slugs = this.documentRepository.GetSlugsForNodeIds( pieces );
			if ( slugs.IsNullOrEmpty() ) {
				return results;
			}

			List<string> sortedSlugs = new List<string>();
			foreach ( int piece in pieces ) {
				if ( !slugs.ContainsKey( piece ) ) {
					return results; // Fail
				}
				sortedSlugs.Add( slugs[piece] );
			}
			if ( !sortedSlugs.IsNullOrEmpty() ) {
				results = "/" + string.Join( "/", sortedSlugs.ToArray() );
			}
			return results;
		}

		public List<Document> GetParents( int NodeId ) {
			return (
				from n in this.documentRepository.GetNodesByIds( this.GetParentNodeIds( NodeId ) )
				orderby n.Depth
				select n
			).ToList();
		}

		// public to be testable
		// results include myself
		public List<int> GetParentNodeIds( int NodeId ) {
			List<int> results = null;

			string nodeIdPath = this.documentRepository.GetNodeIdPath( NodeId );

			if ( !string.IsNullOrEmpty( nodeIdPath ) ) {
				results = (
					from s in nodeIdPath.Split( new char[] { Document.NODE_PATH_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries )
					select int.Parse( s )
				).ToList();
			}

			return results;
		}

		// TODO: Move this to a trigger or sproc to get transaction?
		public void SaveModifiedNode( Document ToSave ) {

			/* FRAGILE: ASSUME: Properties validated previously:
			 * - Slug isn't null
			 * - Slug is unique among siblings
			 * - ParentId is valid
			*/

			// Stash original data so we can compare later
			int origParentId = 0;
			string origNodeIdPath = null;
			int origDepth = 0;
			if ( !ToSave.IsNew() ) {
				Document orig = this.documentRepository.GetNode( ToSave.Id, ActiveOnly: false );
				if ( orig == null ) {
					// TODO: throw up();
				} else {
					origParentId = orig.ParentDocumentId;
					origNodeIdPath = orig.NodeIdPath;
					origDepth = orig.Depth;
				}
			} else {
				// Get a NodeId
				ToSave.NodeIdPath = "NOT NULL"; // Defeat [Required] temporarily -- safer than filling it with an incorrect NodeId
				this.documentRepository.Save( ToSave );
			}

			// New or parent id changed? Set depth and path
			if ( ToSave.IsNew() || origParentId != ToSave.ParentDocumentId ) {
				this.ResetPathAndDepth( ToSave );
			}

			// Save
			this.documentRepository.Save( ToSave );

			// Update children's depth / path (recursively)
			this.UpdateChildrenPathAndDepth( origNodeIdPath, origDepth, ToSave.NodeIdPath, ToSave.Depth );

		}

		public void ResetPathAndDepth( Document Node ) {
			Document parent = this.documentRepository.GetNode( Node.ParentDocumentId, ActiveOnly: false );
			if ( parent == null ) {
				throw new ArgumentNullException( "Node", "NodeId " + Node.ParentDocumentId + " not found" );
			}
			Node.Depth = parent.Depth + 1; // Set depth
			Node.NodeIdPath = parent.NodeIdPath + Document.NODE_PATH_SEPARATOR + Node.Id; // Set path
		}

		// public for testability, not part of the interface
		public void UpdateChildrenPathAndDepth( string origNodeIdPath, int origDepth, string newNodeIdPath, int newDepth ) {
			if ( string.IsNullOrEmpty( origNodeIdPath ) || string.IsNullOrEmpty( newNodeIdPath ) ) {
				return; // Nothing to do
			}
			if ( origNodeIdPath == newNodeIdPath && origDepth == newDepth ) {
				return; // Nothing to do
			}

			int depthChange = newDepth - origDepth;

			List<Document> children = this.documentRepository.GetRecursiveChildren( origNodeIdPath );
			if ( children.IsNullOrEmpty() ) {
				return;
			}

			foreach ( Document child in children ) {
				child.Depth += depthChange;
				child.NodeIdPath = child.NodeIdPath.Replace( origNodeIdPath, newNodeIdPath );
			}
			this.documentRepository.Save( children );
		}

		public void EnsureRootNoteExists() {

			Document root = this.documentRepository.GetById( Document.ROOT_NODE_ID );
			if ( root != null ) {
				return; // It does
			}

			// It doesn't, create it

			/* SQL:
			TRUNCATE TABLE dbo.Document
			SET IDENTITY_INSERT dbo.Document ON
			INSERT INTO dbo.Document (
				DocumentId, NodeTypeId, ParentDocumentId, NodeIdPath, Depth, Slug,
				Name, PageContent, HeadContent, ScriptContent,
				CreatedDate, ModifiedDate, IsActive
			) VALUES (
				1, 1, 1, '/1', 0, '',
				'CMS', '', NULL, NULL,
				GETDATE(), GETDATE(), 1
			)
			SET IDENTITY_INSERT dbo.Document ON
			*/

			// FRAGILE: The code below doesn't work at all, but the SQL above works just fine

			root = new Document {
				Depth = 0,
				NodeType = NodeType.Root,
				NodeIdPath = "/1",
				Name = "CMS",
				Slug = "",
				PageContent = ""
			};
			this.documentRepository.Save( root ); // FRAGILE: This fails because root's slug is blank
			root.ParentDocumentId = root.DocumentId;
			this.documentRepository.Save( root );

			if ( root.DocumentId != Document.ROOT_NODE_ID ) {
				throw new ArgumentOutOfRangeException( "Error creating root node, created id " + root.DocumentId + " instead, truncate and reseed the table" );
			}
			
		}

	}
}