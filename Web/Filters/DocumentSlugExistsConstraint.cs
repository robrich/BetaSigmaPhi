namespace BetaSigmaPhi.Web.Filters {
	using System;
	using System.Web;
	using System.Web.Routing;
	using BetaSigmaPhi.Repository;

	public class DocumentSlugExistsConstraint : IRouteConstraint {
		private readonly IDocumentRepository documentRepository;

		public DocumentSlugExistsConstraint( IDocumentRepository DocumentRepository ) {
			if ( DocumentRepository == null ) {
				throw new ArgumentNullException( "DocumentRepository" );
			}
			this.documentRepository = DocumentRepository;
		}

		public bool Match( HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection ) {

			string slug = (values.ContainsKey( parameterName ) ? values[parameterName] : null) as string;
			if ( string.IsNullOrEmpty( slug ) ) {
				return true; // Document.ROOT_NODE_ID is valid
			}

			return this.documentRepository.ActiveNodeExistsBySlugPath( slug );
		}

	}
}