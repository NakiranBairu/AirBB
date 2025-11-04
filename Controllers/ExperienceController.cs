using Microsoft.AspNetCore.Mvc;

namespace AirBB.Controllers
{
    public class ExperienceController : Controller
    {
        public IActionResult List(string id = "All")
        {
            return Content($"Public Area - Experience Controller - List Action - ID: {id}");
        }
    }
}