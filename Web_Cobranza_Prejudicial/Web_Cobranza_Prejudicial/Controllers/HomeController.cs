using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Cobranza_Prejudicial.Models;
using System.Text.Json;
using static Web_Cobranza_Prejudicial.Models.Entities;

namespace Web_Cobranza_Prejudicial.Controllers
{
    public class HomeController : Controller
    {

        private readonly Methods _methods;
        private readonly ILogger<HomeController> _logger;

        public HomeController(Methods methods, ILogger<HomeController> logger)
        {
            _methods = methods;
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
        public  IActionResult CambiarEstado(int IdEstado)
        {

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonSerializer.Deserialize<oLogin>(JsonCookie_Decodificado);


             _methods.SP_CREATE_LOG_RESPONSABLE_X_ESTADO(outputCookie.ID_RESPONSABLE, IdEstado);

            return Ok();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarSession()
        {
            try
            {
                string JsonCookie_Codificado = Request.Cookies["Session"];

                string JsonCookie_Decodificado = "";
                using (Helpers helpers = new Helpers())
                {
                    JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
                }

                oLogin outputCookie = new oLogin();
                outputCookie = JsonSerializer.Deserialize<oLogin>(JsonCookie_Decodificado);


                _methods.SP_CREATE_LOG_RESPONSABLE_X_ESTADO(outputCookie.ID_RESPONSABLE, 7);



                Response.Cookies.Delete("Session");
                return RedirectToAction("Index", "Login");

            }
            catch
            {
                Response.Cookies.Delete("Session");
                return RedirectToAction("Index", "Login");

            }

        }



    }
}
