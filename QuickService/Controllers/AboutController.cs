using Microsoft.AspNetCore.Mvc;

namespace QuickService.Controllers
{
    public class AboutController : Controller
    {
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult FrequentlyAskedQuestion()
        {
            return View();
        }
        public IActionResult MissionVision()
        {
            return View();
        }
        public IActionResult OurStory()
        {
            return View();
        }
        public IActionResult Pricing()
        {
            return View();
        }
        public IActionResult PrivacyPolicy()
        {
            return View();
        }
        public IActionResult TermsAndConditions()
        {
            return View();
        }
        public IActionResult WhyChooseUs()
        {
            return View();
        }
    }
}
