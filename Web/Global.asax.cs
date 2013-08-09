namespace BetaSigmaPhi.Web {
	using System.Web;
	using System.Web.Http;
	using System.Web.Http.Dispatcher;
	using System.Web.Mvc;
	using System.Web.Optimization;
	using System.Web.Routing;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.App_Start;

	public class MvcApplication : HttpApplication {

		public const string DOCUMENT_BASE_PATH = "/doc"; // FRAGILE: ASSUME: Index() == ViewNode() + pathInfo (e.g. that they start with the same url)

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			// Make MVC spit out prettier errors
			ControllerBuilder.Current.SetControllerFactory(new FuncControllerFactory(ServiceLocator.GetService));
			// Make WebAPI work
			GlobalConfiguration.Configuration.Services.Replace(typeof (IHttpControllerActivator), new FuncHttpControllerActivatorFactory(ServiceLocator.GetService));

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			FilterConfig.RegisterApiFilters(GlobalConfiguration.Configuration.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			ServiceLocator.GetService<IUserInitializeService>().EnsureAdminExists();
		}

	}
}
