namespace BetaSigmaPhi.Web.Controllers {
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Web.Http;
	using BetaSigmaPhi.Web.Filters;

	[RequireLoginApi]
	public class SomeApiController : ApiController {

		// Get list
		public IEnumerable<string> Get() {
			return new string[] {"value1", "value2"};
		}

		// Get
		public HttpResponseMessage Get( int id ) {
			string item = "the object";
			HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, item);
			return response;
		}

		// Create
		public void Post( [FromBody] string value ) {
		}

		// Update
		public void Put( int id, [FromBody] string value ) {
		}

		// Delete
		public void Delete( int id ) {
		}

	}
}
