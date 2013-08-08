namespace BetaSigmaPhi.Web.Filters {
	using System;
	using System.Net;
	using System.Web.Mvc;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Repository;

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
	public class RequireLoginAttribute : ActionFilterAttribute {
		private readonly IUserIdentityRepository userIdentityRepository;

		public RequireLoginAttribute( IUserIdentityRepository UserIdentityRepository ) {
			if ( UserIdentityRepository == null ) {
				throw new ArgumentNullException( "UserIdentityRepository" );
			}
			this.userIdentityRepository = UserIdentityRepository;
		}

		public RequireLoginAttribute()
			: this( ServiceLocator.GetService<IUserIdentityRepository>() ) {
		}

		public override void OnActionExecuting( ActionExecutingContext Context ) {
			if ( !this.userIdentityRepository.IsAuthenticated() ) {
				Context.Result = new HttpStatusCodeResult( HttpStatusCode.Unauthorized );
			}

			base.OnActionExecuting( Context );
		}

	}
}
