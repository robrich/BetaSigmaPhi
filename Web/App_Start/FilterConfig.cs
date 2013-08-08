namespace BetaSigmaPhi.Web.App_Start {
	using System.Web.Http.Filters;
	using System.Web.Mvc;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Web.Filters;

	public static class FilterConfig {

		public static void RegisterGlobalFilters( GlobalFilterCollection Filters ) {
			Filters.Add(ServiceLocator.GetService<HandleAndLogErrorAttribute>());
		}

		public static void RegisterApiFilters( HttpFilterCollection Filters ) {
			
		}

	}
}
