using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using BetaSigmaPhi.Web.Filters;
using BetaSigmaPhi.Repository;
using BetaSigmaPhi.Entity;
using BetaSigmaPhi.Web.Models.GridHelpers;

namespace BetaSigmaPhi.Web.Controllers
{
    [RequireAdmin]
    public class CategoryController : ApiController
    {

        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository CategoryRepository)
        {
            if (CategoryRepository == null)
            {
                throw new ArgumentNullException("CategoryRepository");
            }
            this.categoryRepository = CategoryRepository;
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

            if (category == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            try
            {
                this.categoryRepository.Delete(category.CategoryId);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, category);

        }

    }

}


        //public DataSourceResult Get(HttpRequestMessage requestMessage)
        //{
        //    // The request is in the format GET api/YourController?{take:10,skip:0} and ParseQueryString treats it as a key without value
        //    DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(requestMessage.RequestUri.ParseQueryString().GetKey(0));
        //    return this.categoryRepository.GetActive().AsQueryable().ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);
        //}

        //public HttpResponseMessage Post(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        categoryRepository.Save(category);
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.CategoryId }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //}

        //public HttpResponseMessage Update(int CategoryId, Category category)
        //{
        //    if (ModelState.IsValid && CategoryId == category.CategoryId)
        //    {
        //        try
        //        {
        //            categoryRepository.Save(category);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //}

        //public HttpResponseMessage Delete(int CategoryId)
        //{

        //    Category category = this.categoryRepository.GetById(CategoryId);

        //    if (category == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    try
        //    {
        //        this.categoryRepository.Delete(CategoryId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, category);

        //}