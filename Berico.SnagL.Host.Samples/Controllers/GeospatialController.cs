using System.Web.Mvc;

namespace Berico.SnagL.Host.Samples.Controllers
{
    /// <summary>
    /// Contains action methods for the geospatial demo
    /// </summary>
    public class GeospatialController : Controller
    {
        /// <summary>
        /// GET: /Geospatial/
        /// </summary>
        /// <returns>The Index view</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
