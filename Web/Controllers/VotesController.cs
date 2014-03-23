using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using BetaSigmaPhi.Web.Filters;
using BetaSigmaPhi.Web.Models;
using BetaSigmaPhi.Repository;
using BetaSigmaPhi.Service;
using BetaSigmaPhi.Entity;
using BetaSigmaPhi.Web.Models.GridHelpers;

namespace BetaSigmaPhi.Web.Controllers
{
    [RequireAdmin]
    public class VotesController : ApiController
    {
        private readonly IPollRepository pollRepository;
        private readonly IVoteRepository voteRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userService;
        private readonly ICategoryRepository categoryRepository;

        public VotesController(IPollRepository PollRepository, IUserIdentityService UserIdentityService,
            IUserRepository UserRepository, IVoteRepository VoteRepository, ICategoryRepository CategoryRepository)
        {
            if (PollRepository == null)
            {
                throw new ArgumentNullException("CategoryRepository");
            }

            if (UserIdentityService == null)
            {
                throw new ArgumentNullException("UserIdentityService");
            }

            if (UserRepository == null)
            {
                throw new ArgumentNullException("UserIdentityService");
            }

            if (VoteRepository == null)
            {
                throw new ArgumentNullException("VoteRepository");
            }

            if (CategoryRepository == null)
            {
                throw new ArgumentNullException("CategoryRepository");
            }

            this.pollRepository = PollRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
            this.voteRepository = VoteRepository;
            this.categoryRepository = CategoryRepository;
        }

        public DataSourceResult Get(HttpRequestMessage requestMessage)
        {
            // The request is in the format GET api/YourController?{take:10,skip:0} and ParseQueryString treats it as a key without value
            DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(requestMessage.RequestUri.ParseQueryString().GetKey(0));
            var a = this.voteRepository.GetActive();

            //return a;

            var query = (from tVote in this.voteRepository.GetActive()
                         join tUser in this.userRepository.GetActive() on tVote.VoterUserId equals tUser.UserId
                         join tElectedUser in this.userRepository.GetActive() on tVote.ElectedUserId equals tElectedUser.UserId
                         select new VoteModel
                         {
                             ElectedUser = tElectedUser.FirstName + " " + tElectedUser.LastName,
                             ElectedUserId = tVote.ElectedUserId,
                             PollId = tVote.PollId,
                             VoteDate = tVote.VoteDate,
                             VoteId = tVote.VoteId,
                             VoterUser = tUser.FirstName + " " +tUser.LastName,
                             VoterUserId = tVote.VoterUserId
                         });

            return query.AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);
        }

        public HttpResponseMessage Delete(VoteModel vote)
        {
            if (ModelState.IsValid && vote.VoteId > 0)
            {
                voteRepository.Delete(vote.VoteId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
    }
}