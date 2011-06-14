using System.Web.Mvc;

namespace Berico.SnagL.Host.Samples.Controllers
{
    /// <summary>
    /// Contains action methods for demonstrating the ExternalResource functionality
    /// </summary>
    public class ExternalResourceController : Controller
    {
        /// <summary>
        /// GET: /ExternalResource/
        /// </summary>
        /// <returns>The ExternalResource view page</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
