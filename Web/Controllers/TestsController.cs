
//using System.Web.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Web.Http;
//using System.Net.Http;
//using System.Web.Services;
//using Newtonsoft.Json;
//using BetaSigmaPhi.Web.Filters;
//using BetaSigmaPhi.Web.Models;
//using BetaSigmaPhi.Repository;
//using BetaSigmaPhi.Service;
//using BetaSigmaPhi.Entity;
//using BetaSigmaPhi.Service;

//using System;
//﻿using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;


//namespace BetaSigmaPhi.Web.Controllers
//{

//    //[RequireAdmin]
//    public class TestsController : Controller
//    {
//        private readonly ICategoryRepository categoryRepository;
//        private readonly IPollRepository pollRepository;
//        private readonly IUserRepository userRepository;
//        private readonly IUserIdentityService userService;

//        public TestsController(
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
//        public ActionResult Index()
//        {
//            Category o = new Category();
//            o.Name = "TestCategory";
//            this.categoryRepository.Save(o);
//            return this.View();
//        }

//        /// <summary>
//        /// Reads the available categorys to provide data for the Kendo Grid
//        /// </summary>
//        /// <returns>All available categorys</returns>

//        //public DataSourceResult GetCategory(HttpRequestMessage requestMessage)
//        //{
//        //    DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(
//        //        // The request is in the format GET /api/DBLogs?{take:10,skip:0} and ParseQueryString treats it as a key without value
//        //    requestMessage.RequestUri.ParseQueryString().GetKey(0));

//        //    return this.categoryRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);

//        //}

//    }

//}