namespace BetaSigmaPhi.Web.Filters {
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Service;

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
	public class RequireAdminApiAttribute : AuthorizeAttribute {
		private readonly IUserIdentityService userIdentityService;
		private readonly IUserService userService;

		public RequireAdminApiAttribute( IUserIdentityService UserIdentityService, IUserService UserService ) {
			if ( UserIdentityService == null ) {
				throw new ArgumentNullException( "UserIdentityService" );
			}
			if ( UserService == null ) {
				throw new ArgumentNullException( "UserService" );
			}
			this.userIdentityService = UserIdentityService;
			this.userService = UserService;
		}

		public RequireAdminApiAttribute()
			: this( ServiceLocator.GetService<IUserIdentityService>(), ServiceLocator.GetService<IUserService>() ) {
		}

		public override void OnAuthorization( HttpActionContext Context ) {
			if ( !this.UserIsAdmin() ) {
				//Context.Response = new HttpResponseMessage( HttpStatusCode.NotFound );
				// If you're under-authenticated, this "doesn't exist": great for employee pages, for customer-facing pages, return 401 instead
				Context.Response = new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}
		}

		private bool UserIsAdmin() {
			User user = this.userIdentityService.GetCurrentUser();
			return this.userService.UserIsAdmin( user );
		}

	}
}
