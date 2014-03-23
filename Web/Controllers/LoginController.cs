namespace BetaSigmaPhi.Web.Controllers {
	using System;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.Models;

	public class LoginController : Controller {
		private readonly ILoginService loginService;
		private readonly IUserIdentityRepository userIdentityRepository;

		public LoginController( ILoginService LoginService, IUserIdentityRepository UserIdentityRepository ) {
			if ( LoginService == null ) {
				throw new ArgumentNullException( "LoginService" );
			}
			if ( UserIdentityRepository == null ) {
				throw new ArgumentNullException( "UserIdentityRepository" );
			}
			this.loginService = LoginService;
			this.userIdentityRepository = UserIdentityRepository;
		}

		public ActionResult Index() {
			// TODO: If they're already authenticated, just send them on their way?
			return this.View( new LoginModel() );
		}

		[HttpPost]
		public ActionResult Index( LoginModel Model ) {
			if ( this.ModelState.IsValid && Model != null ) {
				Model.Message = null;

				// Attempt the login
				User user = this.loginService.ValidateUser( Model.Email, Model.Password );
				if ( user != null ) {
					// Login worked
					this.userIdentityRepository.Login( user.AuthenticationToken, Model.RememberMe );

					string redirectUrl = Model.ReturnUrl;
					if ( !string.IsNullOrWhiteSpace( redirectUrl ) ) {
						// validate the url is on this site
						Uri url = null;
						if ( !Uri.TryCreate( redirectUrl, UriKind.Relative, out url ) ) {
							redirectUrl = null;
						}
					}
					if ( !string.IsNullOrWhiteSpace( redirectUrl ) ) {
						return this.Redirect( redirectUrl );
					}

					return this.RedirectToAction( "Index", "Home" );
				}
			}
			// Login failed
			if ( Model == null ) {
				Model = new LoginModel();
			}
			Model.Message = "Invalid username or password";
			return this.View( Model );
		}

		public ActionResult Logout() {
			this.userIdentityRepository.Logout();
			return this.RedirectToAction( "Index", "Home" );
		}

	}
}
