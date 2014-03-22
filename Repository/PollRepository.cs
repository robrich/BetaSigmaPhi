namespace BetaSigmaPhi.Repository {
    using BetaSigmaPhi.DataAccess;
    using BetaSigmaPhi.Entity;
    using System.Collections.Generic;
    using System.Linq;

	public interface IPollRepository : IRepository<Poll> {
        User GetWinnerForPoll(int PollId);
        User GetWinnerForPreviousPoll(int currentPollId);
        List<Poll> GetActivePolls();
        bool HasReachedPollingLimit(int pollId, int userId);
	}

	public class PollRepository : Repository<Poll>, IPollRepository {

		public PollRepository(IBetaSigmaPhiContextFactory BetaSigmaPhiContextFactory) : base(BetaSigmaPhiContextFactory) {
		}

        public User GetWinnerForPoll(int pollId)
        {
            using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext())
            {
                User winner = null;

                if (db.Votes.Count() > 0)
                {
                    winner = (
                        from v in db.Votes
                        where v.PollId == pollId
                        && v.IsActive
                        group v by v.ElectedUserId into g
                        orderby g.Count() descending
                        select g.First().ElectedUser
                    ).FirstOrDefault();
                }

                return winner;
            }
        }

        public User GetWinnerForPreviousPoll(int currentPollId)
        {
            using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext())
            {
                User previousWinner = null;
                Poll currentPoll = db.Polls.Where(x => x.PollId == currentPollId).FirstOrDefault();
                Poll previousPoll = db.Polls.Where(y => y.CategoryId == currentPoll.CategoryId && y.PollId != currentPollId).OrderByDescending(z => z.EndDate).FirstOrDefault();
                
                if(previousPoll != null)
                {
                    previousWinner = GetWinnerForPoll(previousPoll.PollId);
                }

                return previousWinner;
            }
        }
        
        public List<Poll> GetActivePolls()
        {
            using (IBetaSigmaPhiContext db = this.BetaSigmaPhiContextFactory.GetContext())
            {
                List<Poll> activePolls = db.Polls.Where(x => x.IsActive && x.IsOpenForVoting).ToList();
                return activePolls;
            }             
        }

        public bool HasReachedPollingLimit(int pollId, int userId)
        {
            //TODO: Pending changes from Rob
            return false;
        }
    }
}
