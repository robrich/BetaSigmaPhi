namespace BetaSigmaPhi.Service {
	using System;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;

	public interface IUserIdentityService {
		int? GetCurrentUserId();
		User GetCurrentUser();
	}

	public class UserIdentityService : IUserIdentityService {
		private readonly IUserIdentityRepository userIdentityRepository;
		private readonly IUserRepository userRepository;

		public UserIdentityService(IUserIdentityRepository UserIdentityRepository, IUserRepository UserRepository) {
			if ( UserIdentityRepository == null ) {
				throw new ArgumentNullException( "UserIdentityRepository" );
			}
			if ( UserRepository == null ) {
				throw new ArgumentNullException( "UserRepository" );
			}
			this.userIdentityRepository = UserIdentityRepository;
			this.userRepository = UserRepository;
		}

		public int? GetCurrentUserId() {
			User user = this.GetCurrentUser();
			return user != null ? user.UserId : (int?)null;
		}

		public User GetCurrentUser() {
			string token = this.userIdentityRepository.GetCurrentAuthenticationToken();
			User user = this.userRepository.GetByAuthenticationToken( token );
			if ( user != null && !user.IsActive ) {
				user = null;
			}
			return user;
		}

	}
}
