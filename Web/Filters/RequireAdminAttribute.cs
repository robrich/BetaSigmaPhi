namespace BetaSigmaPhi.Web.Filters {
	using System;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Service;

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
	public class RequireAdminAttribute : ActionFilterAttribute {
		private readonly IUserIdentityService userIdentityService;
		private readonly IUserService userService;

		public RequireAdminAttribute( IUserIdentityService UserIdentityService, IUserService UserService ) {
			if ( UserIdentityService == null ) {
				throw new ArgumentNullException( "UserIdentityService" );
			}
			if ( UserService == null ) {
				throw new ArgumentNullException( "UserService" );
			}
			this.userIdentityService = UserIdentityService;
			this.userService = UserService;
		}

		public RequireAdminAttribute()
			: this( ServiceLocator.GetService<IUserIdentityService>(), ServiceLocator.GetService<IUserService>() ) {
		}

		public override void OnActionExecuting( ActionExecutingContext Context ) {
			if ( !this.UserIsAdmin() ) {
				//Context.Result = new HttpStatusCodeResult( HttpStatusCode.Unauthorized );
				// If you're under-authenticated, this "doesn't exist": great for employee pages, for customer-facing pages, return 401 instead
				Context.Result = new ViewResult {
					ViewName = "NotFound"
				};
			}

			base.OnActionExecuting( Context );
		}

		private bool UserIsAdmin() {
			User user = this.userIdentityService.GetCurrentUser();
			return this.userService.UserIsAdmin( user );
		}

	}
}
