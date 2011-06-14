using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Berico.SnagL.Host.Samples.Controllers
{
    /// <summary>
    /// The default home controller for the the app
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: /Home/
        /// </summary>
        /// <returns>The default home page</returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
