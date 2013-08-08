namespace BetaSigmaPhi.Web.Helpers {
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Service;

	public static class CurrentUserHelpers {
		private static readonly IUserIdentityRepository UserIdentityRepository = ServiceLocator.GetService<IUserIdentityRepository>();
		private static readonly IUserIdentityService UserIdentityService = ServiceLocator.GetService<IUserIdentityService>();

		public static bool CurrentUserIsAuthenticated( this HtmlHelper HtmlHelper ) {
			return UserIdentityRepository.IsAuthenticated();
		}

		public static string CurrentUserFirstName( this HtmlHelper HtmlHelper ) {
			User user = UserIdentityService.GetCurrentUser();
			return user != null ? user.FirstName : null;
		}

	}
}
