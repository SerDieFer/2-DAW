using Microsoft.AspNetCore.Mvc;

namespace Vestigio.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
