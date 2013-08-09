namespace BetaSigmaPhi.Web.Controllers {
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.Models;

	public class DocumentController : Controller {
		private readonly IDocumentRepository documentRepository;
		private readonly IDocumentService documentService;

		public DocumentController( IDocumentRepository DocumentRepository, IDocumentService DocumentService ) {
			if ( DocumentRepository == null ) {
				throw new ArgumentNullException( "DocumentRepository" );
			}
			if ( DocumentService == null ) {
				throw new ArgumentNullException( "DocumentService" );
			}
			this.documentRepository = DocumentRepository;
			this.documentService = DocumentService;
		}

		[HttpGet]
		public ActionResult Index( string pathInfo = "" ) {
			bool activeOnly = true; // TODO: Let admins view inactive pages

			Document node = null;
			if ( string.IsNullOrEmpty( pathInfo ) ) {
				node = this.documentRepository.GetNode( Document.ROOT_NODE_ID, activeOnly );
			} else {
				node = this.documentRepository.GetNodeBySlugPath( pathInfo, activeOnly );
			}
			if ( node == null ) {
				return this.View( "NotFound" );
			}

			List<Document> pathNodes = this.documentService.GetParents( node.DocumentId );

			// Collect url pieces
			string thisSlugPath = this.documentService.GetSlugPath( node.DocumentId );

			switch ( node.NodeType ) {
				case NodeType.Root:
				case NodeType.Branch:
					// Collect children
					List<Document> children = this.documentRepository.GetImediateChildren( node.DocumentId, activeOnly );

					return this.View( "ViewBranchNode", new DocumentBranchViewModel {
						Document = node,
						NodeSlugPath = thisSlugPath,
						Children = children,
						// Breadcrumb:
						PathNodes = pathNodes,
					} );

				case NodeType.Leaf:
					// Show this node
					return this.View( "ViewLeafNode", new DocumentLeafViewModel {
						Document = node,
						NodeSlugPath = thisSlugPath,
						// Breadcrumb:
						PathNodes = pathNodes,
					} );

				default:
					throw new ArgumentOutOfRangeException( "NodeType", node.NodeType + " is not a valid NodeType" );
			}
		}

	}
}
