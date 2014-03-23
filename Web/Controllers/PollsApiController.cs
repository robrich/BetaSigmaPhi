//####################################################################################################################################
// Sql script does not have VoteCountPerFrequency(int) on table need to add this field manually for now
//####################################################################################################################################

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
        private readonly ICategoryRepository categoryRepository;
        private readonly IFrequencyRepository frequencyRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userService;

        public PollsApiController(IPollRepository PollRepository, ICategoryRepository CategoryRepository,  IFrequencyRepository FrequencyRepository, IUserIdentityService UserIdentityService, IUserRepository UserRepository)
        {
            if (PollRepository == null)
            {
                throw new ArgumentNullException("CategoryRepository");
            }

            if (CategoryRepository == null)
            {
                throw new ArgumentNullException("CategoryRepository");
            }

            if (FrequencyRepository == null)
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
            this.categoryRepository = CategoryRepository;
            this.frequencyRepository = FrequencyRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
        }

        public DataSourceResult Get(HttpRequestMessage requestMessage)
        {
            // The request is in the format GET api/YourController?{take:10,skip:0} and ParseQueryString treats it as a key without value
            DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(requestMessage.RequestUri.ParseQueryString().GetKey(0));

            //this.pollRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);


            var query = (from tPoll in this.pollRepository.GetActive()
                        join tCategory in this.categoryRepository.GetActive() on tPoll.CategoryId equals tCategory.CategoryId
                        //join tFrequency in this.frequencyRepository.GetAll() on tPoll.Frequency equals tFrequency
                        select new PollModel
                        {
                            PollId = tPoll.PollId,
                            StartDate = tPoll.StartDate,
                            EndDate = tPoll.EndDate,
                            CategoryId = tCategory.CategoryId,
                            Category = tCategory.Name,
                            FrequencyId = (int)tPoll.Frequency,
                            VoteCountPerFrequency = tPoll.VoteCountPerFrequency
                        });

            return query.AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);


        }

        public HttpResponseMessage Post(PollModel pollModel)
        {

            if (ModelState.IsValid)
            {
                Poll o = new Poll();
                Category c = this.categoryRepository.GetById(pollModel.CategoryId);

                o.StartDate = pollModel.StartDate;
                o.EndDate = pollModel.EndDate;
                o.CategoryId = pollModel.CategoryId;
                o.Category = c;
                o.Frequency = (Frequency)pollModel.FrequencyId;
                o.VoteCountPerFrequency = pollModel.VoteCountPerFrequency;
                pollRepository.Save(o);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, pollModel);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = pollModel.PollId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

        public HttpResponseMessage Delete(PollModel pollModel)
        {
            if (ModelState.IsValid && pollModel.PollId > 0)
            {
                pollRepository.Delete(pollModel.PollId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

    }

}