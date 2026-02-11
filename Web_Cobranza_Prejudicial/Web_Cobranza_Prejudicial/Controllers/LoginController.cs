using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web_Cobranza_Prejudicial.Models;
using static Web_Cobranza_Prejudicial.Models.Entities;

namespace Web_Cobranza_Prejudicial.Controllers
{
    public class LoginController : Controller
    {



        private readonly Methods _methods;

        public LoginController(Methods methods)
        {
            _methods = methods;
        }



        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(iLogin Input)
        {
            Input.USUARIO = Input.USUARIO.Replace("'", "").Replace("--", "");
            Input.CONTRASEÑA = Input.CONTRASEÑA.Replace("'", "").Replace("--", "");



            if (!ModelState.IsValid)
            {
                Message message = new Message();

                if (string.IsNullOrEmpty(Input.CONTRASEÑA))
                {

                    ViewData["Message"] = "Contraseña es requerida";
                    message = null;
                }
                else
                {

                    ViewData["Message"] = "Usuario es requerido";
                    message = null;
                }
                return View();
            }

            /*--------------------------------------------------*/
            /*-        OBTENER LA DIRECCION IP                 -*/
            /*--------------------------------------------------*/
            string DIR_IP = "";
            using (Helpers helpers = new Helpers())
            {
                DIR_IP = helpers.DIRECCION_IP();
                
            }

            /*--------------------------------------------------*/
            /*-                 VALIDAR USUARIO                -*/
            /*--------------------------------------------------*/

            //using (Methods methods = new Methods())
            //{
            //    output = methods.Login(Input, DIR_IP);
            //}

            oLogin output = _methods.Login(Input, DIR_IP);


            if (output.RETURN_VALUE == 1)
            {

                /*--------------------------------------------------*/
                /*-        ELIMINAR COOKIE                         -*/
                /*--------------------------------------------------*/
                Response.Cookies.Delete("Session");


                /*--------------------------------------------------*/
                /*-                 CREAR COOKIE                   -*/
                /*--------------------------------------------------*/
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                };
                string OutputJson;
                using (Helpers helpers = new Helpers())
                {
                    OutputJson = (helpers.Base64Encode(JsonConvert.SerializeObject(output)) + "#####GNA####");
                }



                Response.Cookies.Append("Session", OutputJson, cookieOptions);
                output = null;
                return RedirectToAction("Index", "Cobranza");

            }
            else if (output.RETURN_VALUE == 0)
            {
                /*--------------------------------------------------*/
                /*-        ELIMINAR COOKIE                         -*/
                /*--------------------------------------------------*/
                Response.Cookies.Delete("Session");

                ViewData["Message"] = output.MENSAJE;
                output = null;
                return View();
            }
            else
            {

                /*--------------------------------------------------*/
                /*-        ELIMINAR COOKIE                         -*/
                /*--------------------------------------------------*/
                Response.Cookies.Delete("Session");
                ViewData["Message"] = output.MENSAJE;
                output = null;
                return View();

            }
        }







    }
}
