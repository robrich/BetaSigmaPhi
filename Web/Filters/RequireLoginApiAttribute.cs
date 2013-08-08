namespace BetaSigmaPhi.Web.Filters {
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using BetaSigmaPhi.Infrastructure;
	using BetaSigmaPhi.Repository;

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
	public class RequireLoginApiAttribute : AuthorizeAttribute {
		private readonly IUserIdentityRepository userIdentityRepository;

		public RequireLoginApiAttribute( IUserIdentityRepository UserIdentityRepository ) {
			if ( UserIdentityRepository == null ) {
				throw new ArgumentNullException( "UserIdentityRepository" );
			}
			this.userIdentityRepository = UserIdentityRepository;
		}

		public RequireLoginApiAttribute()
			: this( ServiceLocator.GetService<IUserIdentityRepository>() ) {
		}

		public override void OnAuthorization( HttpActionContext Context ) {
			if ( !this.userIdentityRepository.IsAuthenticated() ) {
				Context.Response = new HttpResponseMessage( HttpStatusCode.Unauthorized );
			}
		}

	}
}
