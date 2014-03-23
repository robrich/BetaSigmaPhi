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
    public class PollsApiController : ApiController
    {
        private readonly IPollRepository pollRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userService;

        public PollsApiController(IPollRepository PollRepository, IUserIdentityService UserIdentityService, IUserRepository UserRepository)
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

            this.pollRepository = PollRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
        }

        public DataSourceResult Get(HttpRequestMessage requestMessage)
        {
            // The request is in the format GET api/YourController?{take:10,skip:0} and ParseQueryString treats it as a key without value
            DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(requestMessage.RequestUri.ParseQueryString().GetKey(0));
            var a = this.pollRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);
            return a;
        }

        public HttpResponseMessage Post(Poll poll)
        {
            if (ModelState.IsValid)
            {
                pollRepository.Save(poll);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, poll);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = poll.PollId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

        public HttpResponseMessage Delete(Poll poll)
        {
            if (ModelState.IsValid && poll.PollId > 0)
            {
                pollRepository.Delete(poll.PollId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

    }

}