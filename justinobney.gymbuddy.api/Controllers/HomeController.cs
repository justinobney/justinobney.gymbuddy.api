using System.Web.Mvc;

namespace justinobney.gymbuddy.api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
