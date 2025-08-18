using Microsoft.AspNetCore.Mvc;

namespace Apha.BST.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
           
            return View();
        }
    }
}
