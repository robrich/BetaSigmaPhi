namespace BetaSigmaPhi.Web.Controllers
{
    using System.Web.Mvc;
    using BetaSigmaPhi.Web.Filters;
    using BetaSigmaPhi.Web.Models;
    using BetaSigmaPhi.Repository;
    using System;
    using System.Collections.Generic;
    using BetaSigmaPhi.Entity;
    using BetaSigmaPhi.Service;
    using System.Linq;

    [RequireAdmin]
    public class PollsController : Controller
    {
        private readonly IPollRepository pollRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userService;

        public PollsController(IPollRepository PollRepository, IUserIdentityService UserIdentityService, IUserRepository UserRepository)
        {
            if (PollRepository == null)
            {
                throw new ArgumentNullException("PollRepository");
			}

            if (UserIdentityService == null)
            {
                throw new ArgumentNullException("UserIdentityService");
            }

            if (UserRepository == null)
            {
                throw new ArgumentNullException("UserIdentityService");
            }

			this.pollRepository = PollRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
		}

        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult Category()
        {
            return this.View();
        }

        public ActionResult CurrentPolls()
        {
            PollsWithEligibleUsersViewModel polls = new PollsWithEligibleUsersViewModel();
            List<Poll> availablePolls = this.pollRepository.GetActivePolls();
            
            foreach (Poll p in availablePolls)
            {
                PollWithEligibleUsers pWithUsers = new PollWithEligibleUsers();
                pWithUsers.userPoll = p;

                User previousWinner = this.pollRepository.GetWinnerForPreviousPoll(p.PollId);
                int? userId = this.userService.GetCurrentUserId();
                List<User> eligibleUsers = this.userRepository.GetAll().Where(x => x.UserId != userId && x.UserId != previousWinner.UserId).ToList();
                pWithUsers.EligibleUsers = eligibleUsers;
            }

            return View();
        }

        [HttpPost]
        public ActionResult SubmitPoll()
        {
            return View();
        }
    }
}