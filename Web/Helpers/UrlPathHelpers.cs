namespace BetaSigmaPhi.Web.Helpers {
	using System.Web.Mvc;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Service;

	public static class UrlPathHelpers {
		private static readonly IUrlService urlService = ServiceLocator.GetService<IUrlService>(); // FRAGILE: DI not possible on static classes

		public static string JoinUrl( this HtmlHelper HtmlHelper, params string[] PathSegments ) {
			return urlService.JoinUrl( PathSegments );
		}

	}
}
