using Microsoft.AspNetCore.Mvc;

namespace QuickService.Controllers
{
    public class ServicesController : Controller
    {
        [Route("Services/")]
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult AcRepair()
        {
            return View();
        }
        public IActionResult ElectricalWork()
        {
            return View();
        }
        public IActionResult GeyserRepair()
        {
            return View();
        }
        public IActionResult LedTvRepair()
        {
            return View();
        }
        public IActionResult MicrowaveRepair()
        {
            return View();
        }
        public IActionResult RefrigeratorRepair()
        {
            return View();
        }
        public IActionResult RoPurifierRepair()
        {
            return View();
        }
        public IActionResult TreadmillRepair()
        {
            return View();
        }
        public IActionResult WashingMachineRepair()
        {
            return View();
        }
    }
}
