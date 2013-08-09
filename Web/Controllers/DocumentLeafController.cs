namespace BetaSigmaPhi.Web.Controllers {
	using System;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.Filters;
	using BetaSigmaPhi.Web.Models;

	[RequireAdmin]
	public class DocumentLeafController : Controller {
		private readonly IDocumentRepository documentRepository;
		private readonly IDocumentService documentService;
		private readonly IUrlService urlService;

		public DocumentLeafController( IDocumentRepository DocumentRepository, IDocumentService DocumentService, IUrlService UrlService ) {
			if ( DocumentRepository == null ) {
				throw new ArgumentNullException( "DocumentRepository" );
			}
			if ( DocumentService == null ) {
				throw new ArgumentNullException( "DocumentService" );
			}
			if ( UrlService == null ) {
				throw new ArgumentNullException( "UrlService" );
			}
			this.documentRepository = DocumentRepository;
			this.documentService = DocumentService;
			this.urlService = UrlService;
		}

		public ActionResult Add( int id = 0 ) {
			int parentDocumentId = id;
			if ( parentDocumentId < Document.ROOT_NODE_ID ) {
				parentDocumentId = Document.ROOT_NODE_ID;
			}
			if ( !this.documentRepository.NodeExists( parentDocumentId, ActiveOnly: false ) ) {
				return this.View( "NotFound" );
			}
			DocumentLeafEditViewModel model = new DocumentLeafEditViewModel {
				ParentDocumentId = parentDocumentId
			};
			this.SetupBreadcrumb( model );
			return this.View( "Edit", model );
		}

		[HttpPost]
		public ActionResult Add( DocumentLeafEditViewModel Model ) {
			if ( Model == null ) {
				return this.View( "NotFound" );
			}
			return this.SaveLeafNode( Model, Model.ParentDocumentId );
		}

		public ActionResult Edit( int id ) {
			Document node = this.documentRepository.GetById( id );
			if ( node == null || node.DocumentId == Document.ROOT_NODE_ID ) {
				return this.View( "NotFound" );
			}
			DocumentLeafEditViewModel model = this.NodeToLeafModel( node );
			this.SetupBreadcrumb( model );
			return this.View( "Edit", model );
		}

		[HttpPost]
		public ActionResult Edit( DocumentLeafEditViewModel Model ) {
			if ( Model == null ) {
				return this.View( "NotFound" );
			}
			return this.SaveLeafNode( Model, Model.DocumentId );
		}

		private DocumentLeafEditViewModel NodeToLeafModel( Document Document ) {
			DocumentLeafEditViewModel model = new DocumentLeafEditViewModel {
				DocumentId = Document.DocumentId,
				ParentDocumentId = Document.ParentDocumentId,
				Name = Document.Name,
				Slug = Document.Slug,
				IsActive = Document.IsActive,
				PageContent = Document.PageContent,
				HeadContent = Document.HeadContent,
				ScriptContent = Document.ScriptContent
			};
			return model;
		}

		private void LeafModelToNode( DocumentLeafEditViewModel Model, Document Document ) {

			Document.Name = Model.Name;
			Document.Slug = Model.Slug;
			Document.IsActive = Model.IsActive;
			Document.PageContent = Model.PageContent;
			Document.HeadContent = Model.HeadContent;
			Document.ScriptContent = Model.ScriptContent;
		}

		private ActionResult SaveLeafNode( DocumentLeafEditViewModel Model, int DestinationDocumentId ) {
			
			if ( Model == null ) {
				// Fix your errors and try again
				if ( this.Request.IsAjaxRequest() ) {
					return this.HttpNotFound();
				} else {
					return this.View( "NotFound" );
				}
			}
			if ( Model.DocumentId == Document.ROOT_NODE_ID ) {
				this.ModelState.AddModelError( "DocumentId", "Can't set root node" );
			}
			if ( !this.documentRepository.NodeExists( Model.ParentDocumentId, ActiveOnly: false ) ) {
				this.ModelState.AddModelError( "ParentDocumentId", "Document has an invalid parent" );
			}
			if ( !this.ModelState.IsValid ) {
				// Fix your errors and try again
				if ( this.Request.IsAjaxRequest() ) {
					return this.Json( new { Success = false, Reason = "Data validation failure", Validation = this.ModelState, Model } );
				} else {
					this.SetupBreadcrumb( Model );
					return this.View( "Edit", Model );
				}
			}

			Document node = null;
			if ( Model.DocumentId > 0 ) {
				node = this.documentRepository.GetById( Model.DocumentId );
				if ( node == null ) {
					if ( this.Request.IsAjaxRequest() ) {
						return this.HttpNotFound();
					} else {
						return this.View( "NotFound" );
					}
				}
			} else {
				// Add new node
				node = new Document {
					NodeType = NodeType.Leaf,
					ParentDocumentId = Model.ParentDocumentId
				};
			}

			// Copy Model to Enties
			this.LeafModelToNode( Model, node );

			// Did they leave anything blank?
			node.Slug = this.documentService.SanitizeSlug( node.Slug, node.Name, Document.SLUG_MAX_LENGTH );

			bool uniqueSlug = this.documentRepository.SlugUniqueAmongSiblings( node.ParentDocumentId, node.Slug, node.DocumentId );
			if ( !uniqueSlug ) {
				this.ModelState.AddModelError( "Slug", "Slug is not unique for nodes in this category" );
				if ( this.Request.IsAjaxRequest() ) {
					return this.Json( new { Success = false, Reason = "Data validation failure", Validation = this.ModelState, Model } );
				} else {
					return this.View( "Edit", Model );
				}
			}

			this.documentService.SaveModifiedNode( node );
			// Saved successfully

			string pathInfo = this.documentService.GetSlugPath( DestinationDocumentId );
			string url = this.urlService.JoinUrl( MvcApplication.DOCUMENT_BASE_PATH, pathInfo );

			Model = this.NodeToLeafModel( node ); // Reset model
			if ( this.Request.IsAjaxRequest() ) {
				return this.Json( new { Success = true, Redirect = url, Model } );
			} else if ( !string.IsNullOrEmpty( url ) ) {
				return this.Redirect( url );
			} else {
				return this.View( "Edit", Model );
			}
		}

		private void SetupBreadcrumb( DocumentLeafEditViewModel Model ) {
			int DocumentId = Model.DocumentId;
			if ( DocumentId < 1 ) {
				DocumentId = Model.ParentDocumentId;
			}
			Model.LastNodeIsLink = true;
			Model.PathNodes = this.documentService.GetParents( DocumentId );
		}

	}
}
