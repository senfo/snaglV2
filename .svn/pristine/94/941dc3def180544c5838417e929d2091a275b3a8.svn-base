using System.IO;
using System.Web.Mvc;

namespace Berico.SnagL.Host.Samples.Controllers
{
    public class LiveDataController : Controller
    {
        /// <summary>
        /// GET: /LiveData/
        /// </summary>
        /// <returns>Returns the live data SnagL demo page</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Test action method that returns XML to test SnagL Live Data
        /// </summary>
        /// <param name="id">ID of the file for which to return</param>
        /// <returns>GraphML to be rendered by SnagL</returns>
        public ActionResult Details(int id)
        {
            string content;

            // The code bellow is terrible, but it's intended to demonstrate functionality
            using (StreamReader reader = new StreamReader(Path.Combine(Server.MapPath("~/Content"), "GraphML", string.Format("3wheel_sample{0}.xml", id))))
            {
                content = reader.ReadToEnd();
            }

            return new ContentResult
            {
                Content = content,
                ContentType = "text/xml"
            };
        }
    }
}
