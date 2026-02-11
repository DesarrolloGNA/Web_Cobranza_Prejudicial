using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web_Cobranza_Prejudicial.Models;

namespace Web_Cobranza_Prejudicial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarSession()
        {
            try
            {

                /*--------------------------------------------------*/
                /*-        ELIMINAR COOKIE                         -*/
                /*--------------------------------------------------*/
                Response.Cookies.Delete("Session");
                ViewData["MessageError"] = null;
                ViewData["MessageSucces"] = null;


                return RedirectToAction("Index", "Login");

            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }











    }
}
