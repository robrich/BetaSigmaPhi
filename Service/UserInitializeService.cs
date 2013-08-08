namespace BetaSigmaPhi.Service {
	using System;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;

	public interface IUserInitializeService {
		void EnsureAdminExists();
	}

	public class UserInitializeService : IUserInitializeService {
		private readonly IUserRepository userRepository;
		private readonly ILoginService loginService;

		public UserInitializeService( IUserRepository UserRepository, ILoginService LoginService ) {
			if ( UserRepository == null ) {
				throw new ArgumentNullException( "UserRepository" );
			}
			if ( LoginService == null ) {
				throw new ArgumentNullException( "LoginService" );
			}
			this.userRepository = UserRepository;
			this.loginService = LoginService;
		}

		public void EnsureAdminExists() {
			// TODO: Check for existence of admins?
			if ( !this.userRepository.Any() ) {
				User user = new User {
					Email = "admin@example.com",
					FirstName = "Admin",
					LastName = "Admin",
					IsAdmin = true
				};
				this.loginService.SaveEmailPasswordChanges( user, "Admin", "@dm1n" );
			}
		}

	}
}
