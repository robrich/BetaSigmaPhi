namespace BetaSigmaPhi.Web.Controllers {
	using System.Web.Mvc;
	using BetaSigmaPhi.Web.Filters;

	[RequireAdmin]
	public class SomeController : Controller {

		public ActionResult Index() {
			return this.View();
		}

	}
}
