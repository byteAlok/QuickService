using QuickService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace QuickService.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorController : Controller
    {
        [Route("Error/{code:int?}")]
        public IActionResult HandleError(int? code)
        {
            switch (code)
            {
                case 404: 
                    return View("Error404");

                case 500:
                    return View("Error500");

                default: 
                    return View("Index");
            }
        }
    }
}
