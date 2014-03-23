namespace BetaSigmaPhi.Repository {
	using BetaSigmaPhi.DataAccess;
	using BetaSigmaPhi.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;

	public interface IVoteRepository : IRepository<Vote> {
        bool HasReachedPollingLimit(int pollId, int userId);
	}

	public class VoteRepository : Repository<Vote>, IVoteRepository {

		public VoteRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory) : base(BetaSigmaPhiContextFactory) {
		}

        public bool HasReachedPollingLimit(int pollId, int userId)
        {
            using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext())
            {
                int pollLimit = db.Polls.Where(x => x.PollId == pollId).Select(y => y.VoteCountPerFrequency).FirstOrDefault();
                List<int> userVotes = db.Votes.Where(x => x.PollId == pollId && x.VoterUserId == userId).Select(y => y.VoteId).ToList(); 
                List<Poll> activePolls = db.Polls.Where(x => x.IsActive && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

                if (userVotes != null && userVotes.Count >= pollLimit)
                {
                    return true;
                }

                return false;
            } 
        }
    }
}
