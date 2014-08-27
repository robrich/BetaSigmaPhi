namespace BetaSigmaPhi.Web.App_Start {
	using System.Web.Optimization;

	// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
	public static class BundleConfig {

		public static void RegisterBundles( BundleCollection bundles ) {
            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                        "~/js/jquery.datetimepicker.js",
                        "~/js/jquery-{version}.js"));

			bundles.Add( new ScriptBundle( "~/js/jqueryval" ).Include(
						"~/js/jquery.unobtrusive*",
						"~/js/jquery.validate*" ) );

			bundles.Add( new StyleBundle( "~/css/site" ).Include( "~/css/site.css" ) );

            //Added by MDE 3/22/2014 support for kendoui ############################################################################################################################
            bundles.Add(new StyleBundle("~/css/kendouicss").Include("~/css/kendoui/kendo.common.min.css", "~/css/kendoui/kendo.rtl.min.css", "~/css/kendoui/kendo.highcontrast.min.css"));
            bundles.Add(new ScriptBundle("~/js/kendouijs").Include("~/js/kendoui/console.js", "~/js/kendoui/kendo.web.min.js"));
            //Added by MDE 3/22/2014 support for kendoui ############################################################################################################################

		}

	}

}
