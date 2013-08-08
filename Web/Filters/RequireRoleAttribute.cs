namespace BetaSigmaPhi.Web.Filters {
	using System;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Service;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class RequireRoleAttribute : ActionFilterAttribute {
		private readonly IUserIdentityService userIdentityService;
		private readonly IUserService userService;

		private readonly Role role;

		public RequireRoleAttribute(Role Role) {
			this.userIdentityService = ServiceLocator.GetService<IUserIdentityService>();
			this.userService = ServiceLocator.GetService<IUserService>();
			this.role = Role;
		}

		public override void OnActionExecuting(ActionExecutingContext Context) {
			if (!this.UserHasRole()) {
				Context.Result = new RedirectResult("/Login");
			}

			base.OnActionExecuting(Context);
		}

		private bool UserHasRole() {
			User user = this.userIdentityService.GetCurrentUser();
			return this.userService.UserHasRole(user, this.role);
		}

	}
}
