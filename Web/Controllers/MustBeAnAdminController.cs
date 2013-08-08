namespace BetaSigmaPhi.Web.Controllers {
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Web.Filters;

	[RequireRole( Role.Admin )]
	[RequireAdmin]
	public class MustBeAnAdminController : Controller {

		public ActionResult Index() {
			return this.View();
		}

	}
}
