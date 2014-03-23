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

        public VotesController(IPollRepository PollRepository, IUserIdentityService UserIdentityService, IUserRepository UserRepository, IVoteRepository VoteRepository)
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

            this.pollRepository = PollRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
            this.voteRepository = VoteRepository; 
        }

        public DataSourceResult Get(HttpRequestMessage requestMessage)
        {
            // The request is in the format GET api/YourController?{take:10,skip:0} and ParseQueryString treats it as a key without value
            DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(requestMessage.RequestUri.ParseQueryString().GetKey(0));
            var a = this.voteRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);
            return a;
        }

        public HttpResponseMessage Delete(Vote vote)
        {
            if (ModelState.IsValid && vote.VoteId > 0)
            {
                pollRepository.Delete(vote.VoteId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
    }
}