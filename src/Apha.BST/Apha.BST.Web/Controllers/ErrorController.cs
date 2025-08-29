using Apha.BST.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace Apha.BST.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
           
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            string message = "An unexpected error occurred. Please try again later.";

            // You can customize the message based on exception type, etc.
            if (exceptionFeature?.Error != null)
            {
                message = exceptionFeature.Error.Message;
            }

            var model = new ErrorViewModel
            {
               
                ErrorMessage = message
            };

            return View(model);
        }
    }
}
