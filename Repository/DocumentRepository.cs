namespace BetaSigmaPhi.Repository {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Library;

	public interface IDocumentRepository : IRepository<Document> {
		Document GetNode( int DocumentId, bool ActiveOnly );
		bool NodeExists( int DocumentId, bool ActiveOnly );
		bool ActiveNodeExistsBySlugPath( string SlugPath );
		int? GetNodeIdBySlugPath( string SlugPath, bool ActiveOnly );
		bool NodeExistsBySlugPath( string SlugPath, bool ActiveOnly );
		Document GetNodeBySlugPath( string SlugPath, bool ActiveOnly );
		List<Document> GetImediateChildren( int DocumentId, bool ActiveOnly );
		string GetNodeIdPath( int DocumentId );
		List<Document> GetNodesByIds( List<int> NodeIds );
		Dictionary<int, string> GetSlugsForNodeIds( List<int> Pieces );
		/// <summary>
		/// Are there any other children of this parent that have the identical slug?
		/// </summary>
		bool SlugUniqueAmongSiblings( int ParentNodeId, string Slug, int NodeId );
		List<Document> GetRecursiveChildren( string NodeIdPath );
		List<Document> GetNodesForDepth( int Depth, bool ActiveOnly );
	}

	public class DocumentRepository : Repository<Document>, IDocumentRepository {

		public DocumentRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory)
			: base(BetaSigmaPhiContextFactory) {
		}

		public Document GetNode( int DocumentId, bool ActiveOnly ) {
			if (DocumentId < 1) {
				return null; // You asked for nothing, you got it
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return (
					from n in db.Documents
					where n.DocumentId == DocumentId
					&& (n.IsActive || !ActiveOnly)
					select n
				).FirstOrDefault();
			}
		}

		public bool NodeExists( int DocumentId, bool ActiveOnly ) {
			if (DocumentId < 1) {
				return false; // You asked for nothing, you got it
			}
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				return (
					from n in db.Documents
					where n.DocumentId == DocumentId
					&& (n.IsActive || !ActiveOnly)
					select n
				).Any();
			}
		}

		public bool ActiveNodeExistsBySlugPath( string SlugPath ) {
			return this.GetNodeIdBySlugPath( SlugPath, ActiveOnly: true ) > 0; // null is not greater than 0
		}

		public int? GetNodeIdBySlugPath( string SlugPath, bool ActiveOnly ) {
			if ( string.IsNullOrEmpty( SlugPath ) ) {
				return null; // You asked for nothing and I agree
			}
			string[] slugParts = SlugPath.Split( new char[] { Document.NODE_PATH_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries );
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				// TODO: Is it faster to grab all matches on leaf nodes and crawl up or to walk from root down?
				int parentNodeId = Document.ROOT_NODE_ID;
				foreach ( string slug in slugParts ) {
					int currentNodeId = (
						from n in db.Documents
						where n.ParentDocumentId == parentNodeId
						&& n.Slug == slug
						&& (n.IsActive || !ActiveOnly)
						orderby n.Name // If there's more than one (shouldn't be) we'll get the first one
						select n.DocumentId
					).FirstOrDefault();
					if ( currentNodeId < 1 ) {
						return null;
					}
					parentNodeId = currentNodeId;
				}
				return parentNodeId; // Path exactly matches
			}
		}

		public bool NodeExistsBySlugPath( string SlugPath, bool ActiveOnly ) {
			return this.GetNodeIdBySlugPath( SlugPath, ActiveOnly ) > 0; // null is not greater than 0
		}

		public Document GetNodeBySlugPath( string SlugPath, bool ActiveOnly ) {
			Document result = null;
			int? nodeId = this.GetNodeIdBySlugPath( SlugPath, ActiveOnly );
			if ( nodeId > 0 ) {
				result = this.GetNode( nodeId ?? 0, ActiveOnly );
			}
			return result;
		}

		public List<Document> GetImediateChildren( int DocumentId, bool ActiveOnly ) {
			if (DocumentId < 1) {
				return new List<Document>(); // You asked for nothing, you got it
			}
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				return (
					from n in db.Documents
					where (n.IsActive || !ActiveOnly)
					&& n.ParentDocumentId == DocumentId
					&& n.DocumentId != DocumentId // exclude Constant.ROOT_NODE_ID from his own children 
					orderby n.Name
					select n
				).ToList();
			}
		}

		public string GetNodeIdPath( int DocumentId ) {
			if (DocumentId < 1) {
				return null; // You asked for nothing, you got it
			}
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				return (
					from n in db.Documents
					where n.DocumentId == DocumentId
					select n.NodeIdPath
				).FirstOrDefault();
			}
		}

		public List<Document> GetNodesByIds( List<int> NodeIds ) {
			if ( NodeIds.IsNullOrEmpty() ) {
				return null;
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				// FRAGILE: ASSUME: there aren't more than 7200 items
				return (
					from n in db.Documents
					where NodeIds.Contains( n.DocumentId )
					orderby n.Depth
					select n
				).ToList();
			}
		}

		public Dictionary<int, string> GetSlugsForNodeIds( List<int> Pieces ) {
			if ( Pieces.IsNullOrEmpty() ) {
				return null;
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				// FRAGILE: ASSUME: there aren't more than 7200 items
				return (
					from n in db.Documents
					where Pieces.Contains( n.DocumentId )
					orderby n.Depth
					select new { NodeId = n.DocumentId, n.Slug }
				).ToDictionary( i => i.NodeId, i => i.Slug );
			}
		}

		/// <summary>
		/// Are there any other children of this parent that have the identical slug?
		/// </summary>
		public bool SlugUniqueAmongSiblings( int ParentNodeId, string Slug, int NodeId ) {
			if ( string.IsNullOrEmpty( Slug ) || ParentNodeId < 1 ) {
				return false;
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return !(
					from n in db.Documents
					where n.ParentDocumentId == ParentNodeId
					&& n.DocumentId != NodeId
					&& n.Slug == Slug
					select n
				).Any();
			}
		}

		public List<Document> GetRecursiveChildren( string NodeIdPath ) {
			if ( string.IsNullOrEmpty( NodeIdPath ) ) {
				return null;
			}
			string pathStart = NodeIdPath + Document.NODE_PATH_SEPARATOR;
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return (
					from n in db.Documents
					where n.NodeIdPath.StartsWith( pathStart )
					select n
				).ToList();
			}
		}

		public List<Document> GetNodesForDepth( int Depth, bool ActiveOnly ) {
			if (Depth < 0) {
				return null; // You asked for nothing, you got it
			}
			using ( IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext() ) {
				return (
					from n in db.Documents
					where n.Depth == Depth
					&& (!ActiveOnly || n.IsActive)
					select n
				).ToList();
			}
		}

	}
}
