namespace BetaSigmaPhi.Web.App_Start {
	using System.Web.Optimization;

	// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
	public static class BundleConfig {

		public static void RegisterBundles( BundleCollection bundles ) {
			bundles.Add( new ScriptBundle( "~/js/jquery" ).Include(
						"~/js/jquery-{version}.js" ) );

			bundles.Add( new ScriptBundle( "~/js/jqueryval" ).Include(
						"~/js/jquery.unobtrusive*",
						"~/js/jquery.validate*" ) );

			bundles.Add( new StyleBundle( "~/css/site" ).Include( "~/css/site.css" ) );

		}

	}
}
