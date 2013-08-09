namespace BetaSigmaPhi.Web.Controllers {
	using System;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.Filters;
	using BetaSigmaPhi.Web.Models;

	[RequireAdmin]
	public class DocumentBranchController : Controller {
		private readonly IDocumentRepository documentRepository;
		private readonly IDocumentService documentService;

		public DocumentBranchController( IDocumentRepository DocumentRepository, IDocumentService DocumentService ) {
			if ( DocumentRepository == null ) {
				throw new ArgumentNullException( "DocumentRepository" );
			}
			if ( DocumentService == null ) {
				throw new ArgumentNullException( "DocumentService" );
			}
			this.documentRepository = DocumentRepository;
			this.documentService = DocumentService;
		}

		public ActionResult Add( int id = 0 ) {
			int parentDocumentId = id;
			if ( parentDocumentId < Document.ROOT_NODE_ID ) {
				parentDocumentId = Document.ROOT_NODE_ID;
			}
			if ( !this.documentRepository.NodeExists( parentDocumentId, ActiveOnly: false ) ) {
				return this.View( "NotFound" );
			}
			DocumentBranchEditViewModel model = new DocumentBranchEditViewModel {
				ParentDocumentId = parentDocumentId
			};
			this.SetupBreadcrumb( model );
			return this.View( "Edit", model );
		}

		[HttpPost]
		public ActionResult Add( DocumentBranchEditViewModel Model ) {
			if ( Model == null ) {
				return this.View( "NotFound" );
			}
			return this.SaveBranchNode( Model );
		}

		public ActionResult Edit( int id ) {
			Document node = this.documentRepository.GetNode( id, ActiveOnly: false );
			if ( node == null || node.DocumentId == Document.ROOT_NODE_ID ) {
				return this.View( "NotFound" );
			}
			DocumentBranchEditViewModel model = this.NodeToBranchModel( node );
			this.SetupBreadcrumb( model );
			return this.View( "Edit", model );
		}

		[HttpPost]
		public ActionResult Edit( DocumentBranchEditViewModel Model ) {
			if ( Model == null ) {
				return this.View( "NotFound" );
			}
			return this.SaveBranchNode( Model );
		}

		private DocumentBranchEditViewModel NodeToBranchModel( Document Node ) {
			return new DocumentBranchEditViewModel {
				DocumentId = Node.DocumentId,
				ParentDocumentId = Node.ParentDocumentId,
				Name = Node.Name,
				Slug = Node.Slug,
				IsActive = Node.IsActive,
			};
		}

		private void BranchModelToNode( DocumentBranchEditViewModel Model, Document Node ) {
			Node.Name = Model.Name;
			Node.Slug = this.documentService.SanitizeSlug( Model.Slug, Model.Name, Document.SLUG_MAX_LENGTH );
			Node.IsActive = Model.IsActive;
		}

		private ActionResult SaveBranchNode( DocumentBranchEditViewModel Model ) {
			if ( Model == null ) {
				// Fix your errors and try again
				return this.View( "NotFound" );
			}
			if ( Model.DocumentId == Document.ROOT_NODE_ID ) {
				this.ModelState.AddModelError( "DocumentId", "Can't set root node" );
			}
			if ( !this.documentRepository.NodeExists( Model.ParentDocumentId, ActiveOnly: false ) ) {
				this.ModelState.AddModelError( "ParentDocumentId", "Node has an invalid parent" );
			}
			if ( !this.ModelState.IsValid ) {
				// Fix your errors and try again
				this.SetupBreadcrumb( Model );
				return this.View( "Edit", Model );
			}

			Document node = null;
			if ( Model.DocumentId > 0 ) {
				node = this.documentRepository.GetNode( Model.DocumentId, false );
				if ( node == null ) {
					return this.View( "NotFound" );
				}
			} else {
				node = new Document {
					NodeType = NodeType.Branch,
					ParentDocumentId = Model.ParentDocumentId
				};
			}

			this.BranchModelToNode( Model, node );
			bool uniqueSlug = this.documentRepository.SlugUniqueAmongSiblings( node.ParentDocumentId, node.Slug, node.DocumentId );
			if ( !uniqueSlug ) {
				this.ModelState.AddModelError( "Slug", "Slug is not unique for nodes in this category" );
				return this.View( "Edit", Model );
			}

			this.documentService.SaveModifiedNode( node );

			// Saved successfully
			return this.RedirectToAction( "Index", "Document" );
		}

		private void SetupBreadcrumb( DocumentBranchEditViewModel Model ) {
			int nodeId = Model.DocumentId;
			if ( nodeId < 1 ) {
				nodeId = Model.ParentDocumentId;
			}
			Model.LastNodeIsLink = true;
			Model.PathNodes = this.documentService.GetParents( nodeId );
		}

	}
}
