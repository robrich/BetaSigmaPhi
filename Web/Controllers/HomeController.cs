namespace BetaSigmaPhi.Web.Controllers {
	using System.Web.Mvc;
	using BetaSigmaPhi.Web.Filters;

	[RequireLogin]
	public class HomeController : Controller {

		public ActionResult Index() {
			return this.View();
		}

	}
}
