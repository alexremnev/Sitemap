using System.Web.Mvc;

namespace Sitemap.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}