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
	public class RequireRoleApiAttribute : AuthorizeAttribute {
		private readonly IUserIdentityService userIdentityService;
		private readonly IUserService userService;

		private readonly Role role;

		public RequireRoleApiAttribute( Role Role ) {
			this.userIdentityService = ServiceLocator.GetService<IUserIdentityService>();
			this.userService = ServiceLocator.GetService<IUserService>();
			this.role = Role;
		}

		public override void OnAuthorization( HttpActionContext Context ) {
			if ( !this.UserHasRole() ) {
				Context.Response = new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}
		}

		private bool UserHasRole() {
			User user = this.userIdentityService.GetCurrentUser();
			return this.userService.UserHasRole( user, this.role );
		}

	}
}
