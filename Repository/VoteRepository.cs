namespace BetaSigmaPhi.Repository {
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;

	public interface IVoteRepository : IRepository<Vote> {

	}

	public class VoteRepository : Repository<Vote>, IVoteRepository {

		public VoteRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory) : base(BetaSigmaPhiContextFactory) {
		}

	}
}
