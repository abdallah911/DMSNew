using System.Web.Mvc;

namespace DMS_Authontication1.Controllers.HR
{
    [Authorize(Roles = "Admin,HR,User")]
    public class IndemnitiesPoliceController : Controller
    {

        public IndemnitiesPoliceController()
        {
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}
