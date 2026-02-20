using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web_Cobranza_Prejudicial.Models;
using static Web_Cobranza_Prejudicial.Models.Entities;

namespace Web_Cobranza_Prejudicial.Controllers
{
    public class CobranzaController : Controller
    {

        private readonly Methods _methods;

        public CobranzaController(Methods methods)
        {
            _methods = methods;
        }




        public IActionResult Index()
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


                /*--------------------------------------------------*/
                /*-        VALIDA SESION ACTIVA                    -*/
                /*--------------------------------------------------*/
                string IP;
                using (Helpers helpers = new Helpers())
                {
                    IP = helpers.DIRECCION_IP();
                }

                Boolean ESTADO_SESSION = false;

                ESTADO_SESSION = _methods.WL_VALIDA_SESSION_ACTIVA(outputCookie.TOKEN, IP);

            

                if (ESTADO_SESSION)
                {
                    ViewData["Log_Usuario"] = outputCookie.NOMBRE_RESPONSABLE.ToString().ToUpper();
                    ViewData["Log_Perfil"] = outputCookie.PERFIL_RESPONSABLE.ToString().ToUpper();
                    ViewData["Anexo_Telefonico"] = outputCookie.ANEXO_TELEFONICO.ToString().ToUpper();

                    return View();
                }
                else
                {

                    /*--------------------------------------------------*/
                    /*-        ELIMINAR COOKIE                         -*/
                    /*--------------------------------------------------*/
                    Response.Cookies.Delete("Session");

                    return RedirectToAction("Index", "Login");
                }

            }
            catch
            {
                /*--------------------------------------------------*/
                /*-        ELIMINAR COOKIE                         -*/
                /*--------------------------------------------------*/
                Response.Cookies.Delete("Session");

                return RedirectToAction("Index", "Login");
            }
        }



        [HttpGet]
        public IActionResult _Deuda(string TIPO_BUSQUEDA, string VALOR_BUSCAR)
        {
            if (string.IsNullOrWhiteSpace(TIPO_BUSQUEDA) ||
                string.IsNullOrWhiteSpace(VALOR_BUSCAR))
            {
                return BadRequest("Parámetros inválidos");
            }

            var output = _methods.SP_READ_DEUDA_X_FILTROS(TIPO_BUSQUEDA, VALOR_BUSCAR);

            return PartialView("_Deuda", output);
        }


        [HttpGet]
        public async Task<IActionResult> _Informacion(int ID_DEUDA)
        {


            INFORMACION Info  = new INFORMACION();

            Info.DATOS_PERSONALES = await _methods.SP_READ_DATOS_PERSONALES_X_ID_DEUDA(ID_DEUDA);
            Info.TELEFONOS = await _methods.SP_READ_TELEFONOS_X_ID_DEUDA(ID_DEUDA);
            Info.EMAILS = await _methods.SP_READ_EMAIL_X_ID_DEUDA(ID_DEUDA);
            



            return PartialView("_Informacion", Info);
        }


        [HttpGet]
        public async Task<IActionResult> _Botonera()
        {

         

            return PartialView("_Botonera");
        }



        [HttpGet]
        public async Task<IActionResult> _Gestiones(int ID_DEUDA)
        {

            GESTIONES Gestiones = new GESTIONES();

            Gestiones.GESTIONES_PREJUDICIALES = await _methods.SP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA(ID_DEUDA);

            return PartialView("_Gestiones", Gestiones);
        }






        [HttpGet]
        public async Task<IActionResult> _RegistrarGestion(int ID_DEUDA)
        {


            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonSerializer.Deserialize<oLogin>(JsonCookie_Decodificado);


            obj_REGISTRAR_GESTION objRegistrarGestion = new obj_REGISTRAR_GESTION();
            objRegistrarGestion.ID_DEUDA = ID_DEUDA;
            objRegistrarGestion.ID_RESPONSABLE = outputCookie.ID_RESPONSABLE;
            objRegistrarGestion.TELEFONOS = await _methods.SP_READ_TELEFONOS_X_ID_DEUDA(ID_DEUDA);
            objRegistrarGestion.LUGAR = await _methods.SP_READ_LUGAR_X_ID_DEUDA(ID_DEUDA);





            return PartialView("_RegistrarGestion", objRegistrarGestion);
        }









        [HttpGet]
        public async Task<IActionResult> OBTENER_CONTACTO_X_LUGAR(int ID_RESPUESTA_LUGAR)
        {

            List<oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR> Contacto = new List<oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR>();

            Contacto = await _methods.SP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR(ID_RESPUESTA_LUGAR);

            return Ok(Contacto);

        }

        [HttpGet]
        public async Task<IActionResult> OBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO(int ID_RESPUESTA_CONTACTO)
        {

            List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO> Contacto = new List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO>();

            Contacto = await _methods.SP_READ_EXCUSA_X_ID_RESPUESTA_CONTACTO(ID_RESPUESTA_CONTACTO);

            return Ok(Contacto);

        }

        [HttpPost]
        public async Task<IActionResult>Create_Gestion_Prejudicial(iSP_CREATE_GESTION_PREJUDICIAL Input)
        {

            oSP_CREATE_GESTION_PREJUDICIAL output = new oSP_CREATE_GESTION_PREJUDICIAL();

            if(Input.FECHA_PROMESA.ToString("dd-MM-yyyy") == "01-01-0001")
            {
                Input.FECHA_PROMESA = DateTime.Parse("01-01-1900");
            }


            output = await _methods.SP_CREATE_GESTION_PRE_JUDICIAL(Input);

            return Ok(output);

        }





        [HttpGet]
        public async Task<IActionResult> _Discador(int ID_DEUDA)
        {

             DISCADOR Discador = new DISCADOR();
             Discador.TELEFONOS = await _methods.SP_READ_TELEFONOS_X_ID_DEUDA(ID_DEUDA);
             Discador.CARRIER_LLAMADAS = await _methods.SP_READ_CARRIER_LLAMADAS(ID_DEUDA);



            return PartialView("_DISCADOR", Discador);
        }







    }
}
