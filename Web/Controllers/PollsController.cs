namespace BetaSigmaPhi.Web.Controllers
{
    using System.Web.Mvc;
    using BetaSigmaPhi.Web.Filters;

    [RequireAdmin]
    public class PollsController : Controller
    {

        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult Category()
        {
            return this.View();
        }

    }
}