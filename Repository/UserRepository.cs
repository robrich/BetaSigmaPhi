namespace BetaSigmaPhi.Repository {
    using System.Collections.Generic;
	using System.Linq;
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;

	public interface IUserRepository : IRepository<User> {
		User GetUserByEmail(string Email, bool ActiveOnly);
		User GetByAuthenticationToken(string AuthenticationToken);
		bool EmailAvailable(int UserId, string Email);
        List<User> GetEligableUsers(int PreviousWinnerId, int CurrentUserId);
	}

	public class UserRepository : Repository<User>, IUserRepository {

		public UserRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory)
			: base(BetaSigmaPhiContextFactory) {
		}

		public User GetUserByEmail(string Email, bool ActiveOnly) {
			if (string.IsNullOrEmpty(Email)) {
				return null; // No need to ask nobody for nothing
			}
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				return (
					from u in db.Users
					where u.Email == Email
					&& (u.IsActive || !ActiveOnly)
					select u
				).FirstOrDefault();
			}
		}

		public User GetByAuthenticationToken(string AuthenticationToken) {
			if (string.IsNullOrEmpty(AuthenticationToken)) {
				return null; // No need to ask nobody for nothing
			}
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				return (
					from u in db.Users
					where u.AuthenticationToken == AuthenticationToken
					select u
					).FirstOrDefault();
			}
		}

		public bool EmailAvailable(int UserId, string Email) {
			if (string.IsNullOrEmpty(Email)) {
				return false; // You asked for nothing, you got it
			}
			using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext()) {
				return !(
					from u in db.Users
					where u.UserId != UserId
					&& u.Email == Email
					select u
				).Any();
			}
		}

        public List<User> GetEligableUsers(int PreviousWinnerId, int CurrentUserId)
        {
            if (PreviousWinnerId < 1 || CurrentUserId < 1)
            {
                return null; // You asked for nothing, you got it
            }
            using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext())
            {
                return (
                    from u in db.Users
                    where u.IsActive
                    && u.UserId != CurrentUserId
                    && u.UserId != PreviousWinnerId
                    select u
                ).ToList();
            }
        }

	}
}
