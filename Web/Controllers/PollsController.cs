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

        [RequireAdmin]
        public ActionResult Category()
        {
            return this.View();
        }

        [RequireAdmin]
        public ActionResult EditPolls()
        {
            return this.View();
        }

        [RequireAdmin]
        public ActionResult Votes()
        {
            return this.View();
        }

        #region User Polls
        public ActionResult CurrentPolls()
        {
            PollsWithEligibleUsersViewModel polls = new PollsWithEligibleUsersViewModel();
            polls.avaiablePollsWithEligibleUsers = new List<PollWithEligibleUsers>();
            List<Poll> availablePolls = this.pollRepository.GetActivePolls();
            
            foreach (Poll p in availablePolls)
            {
                PollWithEligibleUsers pWithUsers = new PollWithEligibleUsers();
                pWithUsers.userPoll = p;

                p.Category = this.categoryRepository.GetById(p.CategoryId);

                User previousWinner = this.pollRepository.GetWinnerForPreviousPoll(p.PollId);
                int? userId = this.userService.GetCurrentUserId();
                List<User> eligibleUsers = this.userRepository.GetEligableUsers(previousWinner == null ? 0 : previousWinner.UserId, userId ?? 0);
                pWithUsers.EligibleUsers = eligibleUsers;
                polls.avaiablePollsWithEligibleUsers.Add(pWithUsers);
            }

            return View(polls);
        }

        [HttpPost]
        [ActionName("CurrentPolls")]
        public ActionResult CurrentPollsPost()
        {
            string pollId = this.Request["selectedPollId"];
            string electedUserId = this.Request["electedUserId"];
            int currentUser = this.userService.GetCurrentUserId() ?? 0;

            if (!string.IsNullOrEmpty(pollId) && !string.IsNullOrEmpty(electedUserId) && currentUser > 0)
            {
                if (this.voteRepository.HasReachedPollingLimit(Convert.ToInt32(pollId), currentUser))
                {
                    ViewData[pollId] = "You have reached the maximum voting limit.";
                }
                else
                {
                    int result = this.voteRepository.Save(new Vote { ElectedUserId = int.Parse(electedUserId), PollId = int.Parse(pollId), VoteDate = DateTime.Now, VoterUserId = currentUser, ModifiedDate = DateTime.Now, CreatedDate = DateTime.Now });
                    if (result > 0)
                    {
                        ViewData[pollId] = "Your poll is submitted successfully.";
                    }
                }
            }
            else
            {
                ViewData[pollId] = "Please select a member and submit your vote.";
            }

            //Get Poll Information again
            PollsWithEligibleUsersViewModel polls = new PollsWithEligibleUsersViewModel();
            polls.avaiablePollsWithEligibleUsers = new List<PollWithEligibleUsers>();
            List<Poll> availablePolls = this.pollRepository.GetActivePolls();

            foreach (Poll p in availablePolls)
            {
                PollWithEligibleUsers pWithUsers = new PollWithEligibleUsers();
                pWithUsers.userPoll = p;

                p.Category = this.categoryRepository.GetById(p.CategoryId);

                User previousWinner = this.pollRepository.GetWinnerForPreviousPoll(p.PollId);
                int? userId = this.userService.GetCurrentUserId();
                List<User> eligibleUsers = this.userRepository.GetEligableUsers(previousWinner == null ? 0 : previousWinner.UserId, userId ?? 0);
                pWithUsers.EligibleUsers = eligibleUsers;
                polls.avaiablePollsWithEligibleUsers.Add(pWithUsers);
            }

            return View("CurrentPolls", polls);
        }
        #endregion User Polls

        #region Admin Polls

        [RequireAdmin]
        public ActionResult GetAllPolls()
        {
            List<Poll> allPolls = this.pollRepository.GetActive();
            return View(allPolls); 
        }

        [RequireAdmin]
        public ActionResult EditPoll(int PollId)
        {
            List<Category> pollCategories = new List<Category>();
            pollCategories = categoryRepository.GetActive();
            ViewBag.Categories = pollCategories;

            Poll currentPoll = this.pollRepository.GetById(PollId);
            return View(currentPoll); 
        }

        [RequireAdmin]
        [ActionName("EditPoll")]
        [HttpPost]
        public ActionResult EditPollPost(Poll modifiedPoll)
        {
            List<Category> pollCategories = new List<Category>();
            pollCategories = categoryRepository.GetActive();
            ViewBag.Categories = pollCategories;
            int result = this.pollRepository.Save(modifiedPoll);
            if (result > 0)
            {
                ViewBag.Message = "The record is successfully edited.";
            }
            else
            {
                ViewBag.Message = "An error occured and the record failed to update. Please check the information entered or try again later.";
            }
            return View(modifiedPoll);            
        }

        [RequireAdmin]
        public ActionResult DeletePoll(int PollId)
        {
            try
            {
                this.pollRepository.Delete(PollId);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Sorry, an error occurred while deleting the Poll";
            }

            return RedirectToAction("GetAllPolls"); 
        }

        [RequireAdmin]
        public ActionResult AddPoll()
        {
            List<Category> pollCategories = new List<Category>();
            pollCategories = categoryRepository.GetActive();

            ViewBag.Categories = pollCategories;

            return View(); 
        }

        [RequireAdmin]
        [HttpPost]
        [ActionName("AddPoll")]
        public ActionResult AddPollPost(Poll myPoll)
        {
            List<Category> pollCategories = new List<Category>();
            pollCategories = categoryRepository.GetActive();
            ViewBag.Categories = pollCategories;

            int result = this.pollRepository.Save(myPoll);
            if (result > 0)
            {
                ViewBag.Message = "The poll was successfully saved.";
            }
            return View(myPoll);
        }

        #endregion Admin Polls

        #region Category

        [RequireAdmin]
        public ActionResult GetCategories()
        {
            List<Category> allCategories = this.categoryRepository.GetActive();
            return View(allCategories);
        }

        [RequireAdmin]
        public ActionResult EditCategory(int CategoryId)
        {
            Category editItem = this.categoryRepository.GetById(CategoryId);
            return View(editItem);
        }

        [RequireAdmin]
        [ActionName("EditCategory")]
        [HttpPost]
        public ActionResult EditCategoryPost(Category category)
        {
            int result = this.categoryRepository.Save(category);
            if (result > 0)
            {
                ViewBag.Message = "The record is successfully edited.";
                return RedirectToAction("GetCategories");
            }
            return View();
        }

        [RequireAdmin]
        public ActionResult DeleteCategory(int CategoryId)
        {
            try
            {
                this.categoryRepository.Delete(CategoryId);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Sorry, an error occurred while deleting the category.";
            }

            return RedirectToAction("GetCategories");
        }

        [RequireAdmin]
        public ActionResult AddCategory()
        {
            return View();
        }

        [RequireAdmin]
        [HttpPost]
        [ActionName("AddCategory")]
        public ActionResult AddCategoryPost(Category model)
        {
            int categoryId = this.categoryRepository.Save(new Category { Name = model.Name});
            if (categoryId > 0)
            { 
                ViewBag.Message = "The category was successfully created.";
                return RedirectToAction("GetCategories");
            }
            return View();
        }

        #endregion Category

        #region Votes

        [RequireAdmin]
        public ActionResult GetAllVotes(int PollId)
        {
            List<Vote> votes = this.voteRepository.GetAll();
            return View();
        }

        [RequireAdmin]
        public ActionResult GetWinnerForPoll(int PollId)
        {
            return View();
        }

        [RequireAdmin]
        public ActionResult DeleteVote(int PollId)
        {
            try
            {
                this.voteRepository.Delete(PollId);
            }
            catch (Exception e)
            {
                ViewBag.Error = "Sorry, an error occured. " + e.ToString();
            }

            return View();
        }

        #endregion Votes
    }
}