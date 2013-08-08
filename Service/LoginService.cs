namespace BetaSigmaPhi.Service {
	using System;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Library;
	using BetaSigmaPhi.Repository;

	public interface ILoginService {
		User ValidateUser( string Email, string Password );
		void SaveEmailPasswordChanges( User User, string EmailFromGui, string PasswordFromGui );
	}

	public class LoginService : ILoginService {
		private readonly IUserRepository userRepository;
		private readonly IHashHelper hashHelper;
		private readonly ISettingRepository settingRepository;

		public LoginService( IUserRepository UserRepository, IHashHelper HashHelper, ISettingRepository SettingRepository ) {
			if ( UserRepository == null ) {
				throw new ArgumentNullException( "UserRepository" );
			}
			if ( HashHelper == null ) {
				throw new ArgumentNullException( "HashHelper" );
			}
			if ( SettingRepository == null ) {
				throw new ArgumentNullException( "SettingRepository" );
			}
			this.userRepository = UserRepository;
			this.hashHelper = HashHelper;
			this.settingRepository = SettingRepository;
		}

		public User ValidateUser( string Email, string Password ) {
			User result = null;
			User user = this.userRepository.GetUserByEmail( Email, ActiveOnly: true );

			if ( user == null || !user.IsActive ) {
				return null; // No such username / password combination
			}
			if ( this.IsLockedOut( user ) ) {
				// TODO: Increment LoginFailCount? You won't be any more locked out by spinning up your LoginFailCount to insanity-speed.
				return null; // You're locked out
			}
			if ( this.HashedPassMatches( user, Password ) ) {
				// Auth succeeded
				this.ClearFailedCount( user );
				result = user;
			} else {
				// Auth failed
				this.IncrementFailedCount( user );
				result = null; // Auth failed
			}
			return result;
		}

		// public for testing, not part of interface
		public bool IsLockedOut( User User ) {
			if ( User == null ) {
				throw new ArgumentNullException( "User" );
			}

			if ( User.LoginFailCount <= this.settingRepository.MaxLoginFailCount ) {
				return false;
			}
			if ( User.LoginFailStartDate == null ) {
				return false;
			}
			if ( User.LoginFailStartDate.Value.Add( this.settingRepository.LoginFailWindow ) <= DateTime.Now ) {
				return false;
			}

			return true;
		}

		// public for testing, not part of interface
		public bool HashedPassMatches( User User, string PasswordFromGui ) {
			if ( string.IsNullOrEmpty( User.Salt ) || string.IsNullOrEmpty( User.Password ) ) {
				throw new ArgumentNullException( "User", "UserId " + User.UserId + " has a blank password or salt" );
			}
			if ( string.IsNullOrEmpty( User.Salt ) || string.IsNullOrEmpty( User.Password ) ) {
				throw new ArgumentNullException( "User", "UserId " + User.UserId + " has a blank password or salt" );
			}
			string hashedPass = this.hashHelper.GenerateHash( this.settingRepository.HashType, PasswordFromGui, User.Salt );
			return (hashedPass == User.Password);
		}

		// public for testing, not part of interface
		public void ClearFailedCount( User User ) {
			if ( User.LoginFailStartDate != null || User.LoginFailCount > 0 ) {
				User.LoginFailStartDate = null;
				User.LoginFailCount = 0;
				this.userRepository.Save( User );
			} else {
				// Already clear
			}
		}

		// public for testing, not part of interface
		public void IncrementFailedCount( User User ) {
			if ( User.LoginFailStartDate != null && User.LoginFailStartDate.Value.Add( this.settingRepository.LoginFailWindow ) > DateTime.Now ) {
				User.LoginFailCount++;
			} else {
				User.LoginFailCount = 1;
				User.LoginFailStartDate = DateTime.Now;
			}
			this.userRepository.Save( User );
		}

		public void SetPassword( User User, string PasswordFromGui ) {
			User.Salt = this.hashHelper.GenerateSalt( this.settingRepository.MinSaltLength, this.settingRepository.MaxSaltLength );
			User.Password = this.hashHelper.GenerateHash( this.settingRepository.HashType, PasswordFromGui, User.Salt );
		}

		public void SaveEmailPasswordChanges( User User, string EmailFromGui, string PasswordFromGui ) {
			if ( string.IsNullOrEmpty( EmailFromGui ) ) {
				throw new ArgumentNullException( "EmailFromGui" );
			}
			if ( !string.Equals( User.Email, EmailFromGui, StringComparison.InvariantCultureIgnoreCase ) || !string.IsNullOrEmpty( PasswordFromGui ) ) {
				User.AuthenticationToken = Guid.NewGuid().ToString( "N" ); // Reset authentication token, effectively kill their cookie
			}
			User.Email = EmailFromGui;
			if ( !string.IsNullOrEmpty( PasswordFromGui ) ) {
				this.SetPassword( User, PasswordFromGui );
			}
			this.userRepository.Save( User );
		}

	}
}
