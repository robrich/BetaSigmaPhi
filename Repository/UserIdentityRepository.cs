namespace BetaSigmaPhi.Repository {
	using System.Security.Principal;
	using System.Threading;
	using System.Web;
	using System.Web.Hosting;
	using System.Web.Security;

	public interface IUserIdentityRepository {
		void Login( string AuthenticationToken, bool IsPersistent );
		void Logout();
		bool IsAuthenticated();
		string GetCurrentAuthenticationToken();
	}

	/// <summary>
	/// The current user pulled from 
	/// </summary>
	public class UserIdentityRepository : IUserIdentityRepository {

		public void Login( string AuthenticationToken, bool IsPersistent ) {
			FormsAuthentication.SetAuthCookie( AuthenticationToken, IsPersistent );
		}

		public void Logout() {
			FormsAuthentication.SignOut();
		}

		private IIdentity GetIdentity() {
			// Reflectored out of System.Web.Security.Membership
			if ( HostingEnvironment.IsHosted ) {
				HttpContext context1 = HttpContext.Current;
				if ( context1 != null && context1.User != null ) {
					return context1.User.Identity;
				}
			} else {
				IPrincipal principal1 = Thread.CurrentPrincipal;
				if ( principal1 != null && principal1.Identity != null ) {
					return principal1.Identity;
				}
			}
			return null;
		}

		public bool IsAuthenticated() {
			IIdentity identity = this.GetIdentity();
			return identity != null ? identity.IsAuthenticated : false;
		}

		public string GetCurrentAuthenticationToken() {
			IIdentity identity = this.GetIdentity();
			return identity != null ? identity.Name : null;
		}

	}
}
