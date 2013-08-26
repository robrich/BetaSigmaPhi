namespace BetaSigmaPhi.Web.Controllers {
	using System.Web.Mvc;
	using BetaSigmaPhi.Web.Filters;

	[RequireAdmin]
	public class AdminController : Controller {

		public ActionResult Index() {
			return this.View();
		}

	}
}
