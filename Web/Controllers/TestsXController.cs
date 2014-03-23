//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using Newtonsoft.Json;
//using BetaSigmaPhi.Web.Filters;
//using BetaSigmaPhi.Web.Models;
//using BetaSigmaPhi.Repository;
//using BetaSigmaPhi.Service;
//using BetaSigmaPhi.Entity;
//using BetaSigmaPhi.Web.Models.GridHelpers;


//public class TestsXController : ApiController
//{

//        private readonly ICategoryRepository categoryRepository;
//        private readonly IPollRepository pollRepository;
//        private readonly IUserRepository userRepository;
//        private readonly IUserIdentityService userService;

//        public TestsXController(
//            IPollRepository PollRepository,
//            ICategoryRepository CategoryRepository,
//            IUserIdentityService UserIdentityService,
//            IUserRepository UserRepository
//            )
//        {
//            if (PollRepository == null)
//            {
//                throw new ArgumentNullException("PollRepository");
//            }

//            if (CategoryRepository == null)
//            {
//                throw new ArgumentNullException("CategoryRepository");
//            }

//            if (UserIdentityService == null)
//            {
//                throw new ArgumentNullException("UserIdentityService");
//            }

//            if (UserRepository == null)
//            {
//                throw new ArgumentNullException("UserIdentityService");
//            }

//            this.pollRepository = PollRepository;
//            this.categoryRepository = CategoryRepository;
//            this.userService = UserIdentityService;
//            this.userRepository = UserRepository;
//        }

//    public DataSourceResult Get(HttpRequestMessage requestMessage)
//    {
//        DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(
//            // The request is in the format GET api/products?{take:10,skip:0} and ParseQueryString treats it as a key without value
//            requestMessage.RequestUri.ParseQueryString().GetKey(0)
//        );

//        return this.categoryRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);
//    }






//}


////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Web;

////namespace BetaSigmaPhi.Web.Controllers
////{
////    public class TestsXController
////    {
////    }
////}