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
    public class CategoryController : ApiController
    {

        private readonly ICategoryRepository categoryRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserIdentityService userService;

        public CategoryController(ICategoryRepository CategoryRepository, IUserIdentityService UserIdentityService, IUserRepository UserRepository)
        {
            if (CategoryRepository == null)
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

            this.categoryRepository = CategoryRepository;
            this.userService = UserIdentityService;
            this.userRepository = UserRepository;
        }

        public DataSourceResult Get(HttpRequestMessage requestMessage)
        {
            // The request is in the format GET api/YourController?{take:10,skip:0} and ParseQueryString treats it as a key without value
            DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(requestMessage.RequestUri.ParseQueryString().GetKey(0));
            return this.categoryRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);
        }

        public HttpResponseMessage Post(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Save(category);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.CategoryId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }


        public HttpResponseMessage Delete(Category category)
        {
            if (ModelState.IsValid && category.CategoryId > 0)
            {
                categoryRepository.Delete(category.CategoryId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

    }

}