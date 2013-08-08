namespace BetaSigmaPhi.Web.App_Start {
	using System.Web.Http;
	using System.Web.Mvc;
	using System.Web.Routing;

	public static class RouteConfig {

		public static void RegisterRoutes( RouteCollection routes ) {
			routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

			routes.MapRoute(
				name: "Logout",
				url: "Logout",
				defaults: new { controller = "Login", action = "Logout" }
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
