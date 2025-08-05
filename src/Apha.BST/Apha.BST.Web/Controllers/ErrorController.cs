using Microsoft.AspNetCore.Mvc;

namespace Apha.BST.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied(string error)
        {
            ViewBag.ErrorMessage = error ?? "Access Denied";
            return View();
        }
    }
}
