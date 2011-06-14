using System.Web.Mvc;

namespace Berico.SnagL.Host.Samples.Controllers
{
    /// <summary>
    /// Contains action methods for demoing the web API functionality
    /// </summary>
    public class WebApiDemoController : Controller
    {
        /// <summary>
        /// GET: /WebApiDemoController/
        /// </summary>
        /// <returns>The view page for the web API demo</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
