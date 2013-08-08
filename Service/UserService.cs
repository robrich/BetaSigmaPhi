namespace BetaSigmaPhi.Service {
	using System;
	using BetaSigmaPhi.Entity;

	public interface IUserService {
		bool UserHasRole( User User, Role Role );
		bool UserIsAdmin( User User );
	}

	public class UserService : IUserService {

		public bool UserHasRole( User User, Role Role ) {
			bool result = false;
			if ( User != null ) {
				switch ( Role ) {
					case Role.Admin:
						result = User.IsAdmin;
						break;
					default:
						throw new ArgumentOutOfRangeException( "Role", Role, Role + " is not a " + typeof( Role ).Name );
				}
			}
			return result;
		}

		public bool UserIsAdmin( User User ) {
			return User != null && User.IsAdmin;
		}
		
	}
}
