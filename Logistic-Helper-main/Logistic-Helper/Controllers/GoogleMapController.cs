using Microsoft.AspNetCore.Mvc;

namespace LogisticHelper.Controllers
{
    public class GoogleMapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}