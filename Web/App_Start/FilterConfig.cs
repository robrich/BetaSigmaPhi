namespace BetaSigmaPhi.Web.App_Start {
	using System.Web.Http.Filters;
	using System.Web.Mvc;

	public static class FilterConfig {

		public static void RegisterGlobalFilters( GlobalFilterCollection Filters ) {
			Filters.Add( new HandleErrorAttribute() );
		}

		public static void RegisterApiFilters( HttpFilterCollection Filters ) {
			
		}

	}
}
