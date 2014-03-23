﻿namespace BetaSigmaPhi.Web.Controllers
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

    [RequireLogin]
    public class PollsController : Controller
    {
        private readonly IPollRepository pollRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userService;
        private readonly ICategoryRepository categoryRepository;
        private readonly IFrequencyRepository frequencyRepository;
        private readonly IVoteRepository voteRepository;

        public PollsController(IPollRepository PollRepository, IUserIdentityService UserIdentityService, IUserRepository UserRepository,
            ICategoryRepository CategoryRepository, IFrequencyRepository FrequencyRepository, IVoteRepository VoteRepository)
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

            if (CategoryRepository == null)
            {
                throw new ArgumentNullException("CategoryRepository");
            }

            if (FrequencyRepository == null)
            {
                throw new ArgumentNullException("FrequencyRepository");
            }

            if (VoteRepository == null)
            {
                throw new ArgumentNullException("VoteRepository");
            }

			this.pollRepository = PollRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
            this.categoryRepository = CategoryRepository;
            this.frequencyRepository = FrequencyRepository;
            this.voteRepository = VoteRepository;
		}

        [RequireAdmin]
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult Category()
        {
            return this.View();
        }

        #region User Polls
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
                List<User> eligibleUsers = this.userRepository.GetEligableUsers(previousWinner == null ? 0 : previousWinner.UserId, userId ?? 0);
                pWithUsers.EligibleUsers = eligibleUsers;
                polls.avaiablePollsWithEligibleUsers.Add(pWithUsers);
            }

            return View(polls);
        }

        [HttpPost]
        public ActionResult SubmitPoll()
        {            
            return View();
        }
        #endregion User Polls

        #region Frequency

        [RequireAdmin]
        public ActionResult Frequencies()
        { 
            return View(); 
        }

        [RequireAdmin]
        public ActionResult EditFrequency()
        { 
            return View(); 
        }

        [RequireAdmin]
        public ActionResult DeleteFrequency()
        { 
            return View(); 
        }

        [RequireAdmin]
        public ActionResult AddFrequency()
        { 
            return View(); 
        }

        #endregion Frequency

        #region Admin Polls

        [RequireAdmin]
        public ActionResult GetAllPolls()
        {
            List<Poll> allPolls = this.pollRepository.GetAll();
            return View(allPolls); 
        }

        [RequireAdmin]
        public ActionResult EditPoll(int PollId)
        { 
            return View(); 
        }

        [RequireAdmin]
        [HttpPost]
        public ActionResult DeletePoll(int PollId)
        { 
            return View(); 
        }

        [RequireAdmin]
        public ActionResult AddPoll()
        { 
            return View(); 
        }

        [RequireAdmin]
        [HttpPost]
        [ActionName("AddPoll")]
        public ActionResult AddPollPost()
        {
            return View();
        }

        #endregion Admin Polls
    }
}