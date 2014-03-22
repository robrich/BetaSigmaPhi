namespace BetaSigmaPhi.Repository {
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;

	public interface IPollRepository : IRepository<Poll> {

	}

	public class PollRepository : Repository<Poll>, IPollRepository {

		public PollRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory) : base(BetaSigmaPhiContextFactory) {
		}

	}
}
