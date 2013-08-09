namespace BetaSigmaPhi.Web.App_Start {
	using System.Web.Http;
	using System.Web.Mvc;
	using System.Web.Routing;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Web.Filters;

	public static class RouteConfig {

		public static void RegisterRoutes( RouteCollection routes ) {
			routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

			routes.MapRoute(
				name: "Logout",
				url: "Logout",
				defaults: new { controller = "Login", action = "Logout" }
			);

			string pageBasePath = MvcApplication.DOCUMENT_BASE_PATH.Length > 1 ? MvcApplication.DOCUMENT_BASE_PATH.Substring(1) + "/" : "";

			// FRAGILE: If a real controller/view exists and a CMS page exists, the CMS page wins
			routes.MapRoute(
				"DocumentView", // Route name
				pageBasePath + "{*pathInfo}", // URL with parameters
				new { controller = "Document", action = "Index" }, // Parameter defaults
				new { pathInfo = ServiceLocator.GetService<DocumentSlugExistsConstraint>(), method = new HttpMethodConstraint(new string[] { "GET" }) } // Constraints
			);

			routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}

	}
}
