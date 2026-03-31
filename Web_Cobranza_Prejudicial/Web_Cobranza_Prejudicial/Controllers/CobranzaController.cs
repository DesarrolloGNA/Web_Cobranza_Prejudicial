using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
                outputCookie =  JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);


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


            try
            {

                string JsonCookie_Codificado = Request.Cookies["Session"];
                string JsonCookie_Decodificado = "";
                using (Helpers helpers = new Helpers())
                {
                    JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
                }

                oLogin outputCookie = new oLogin();

                outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

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


                    if (string.IsNullOrWhiteSpace(TIPO_BUSQUEDA) ||
                        string.IsNullOrWhiteSpace(VALOR_BUSCAR))
                    {
                        return BadRequest("Parámetros inválidos");
                    }

                    var output = _methods.SP_READ_DEUDA_X_FILTROS(TIPO_BUSQUEDA, VALOR_BUSCAR);

                    return PartialView("_Deuda", output);

                }
                else
                {
                    /*--------------------------------------------------*/
                    /*-        ELIMINAR COOKIE                         -*/
                    /*--------------------------------------------------*/
                    Response.Cookies.Delete("Session");
                    ViewData["MessageError"] = "Session Expirada.";
                    ViewData["MessageSucces"] = null;

                    return RedirectToAction("Index", "Login");
                }

            }
            catch
            {
                /*--------------------------------------------------*/
                /*-        ELIMINAR COOKIE                         -*/
                /*--------------------------------------------------*/
                Response.Cookies.Delete("Session");
                ViewData["MessageError"] = "Session Expirada.";
                ViewData["MessageSucces"] = null;

                //return RedirectToAction("Index", "Login");
                return RedirectToAction("CerrarSession", "Home");

       

            }


        }


        [HttpGet]
        public async Task<IActionResult> _Informacion(int ID_DEUDA)
        {


            INFORMACION Info  = new INFORMACION();

            Info.DATOS_PERSONALES = await _methods.SP_READ_DATOS_PERSONALES_X_ID_DEUDA(ID_DEUDA);
            Info.TELEFONOS = await _methods.SP_READ_TELEFONOS_X_ID_DEUDA(ID_DEUDA);
            Info.EMAILS = await _methods.SP_READ_EMAIL_X_ID_DEUDA(ID_DEUDA);
            Info.OFERTAS = await _methods.SP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA(ID_DEUDA);
            Info.DIRECCIONES = await _methods.SP_READ_DIRECCION_X_ID_DEUDA(ID_DEUDA);
            Info.ALERTAS = await _methods.SP_READ_ALERTA_X_ID_DEUDA(ID_DEUDA);


            return PartialView("_Informacion", Info);
        }



        [HttpGet]
        public async Task<IActionResult> _Banner(int ID_DEUDA)
        {


            oSP_READ_BANNER_X_ID_DEUDA Banner = new oSP_READ_BANNER_X_ID_DEUDA();
            Banner = await _methods.SP_READ_BANNER_X_ID_DEUDA(ID_DEUDA);

            return PartialView("_Banner", Banner);
        }




        [HttpGet]
        public async Task<IActionResult> _Botonera(int ID_DEUDA)
        {

            oSP_VALIDA_BOTONERA_X_ID_DEUDA Output = new oSP_VALIDA_BOTONERA_X_ID_DEUDA();

      
            Output = await _methods.SP_VALIDA_BOTONERA_X_ID_DEUDA(ID_DEUDA);

  

            return PartialView("_Botonera", Output);
        }



        [HttpGet]
        public async Task<IActionResult> _Gestiones(int ID_DEUDA)
        {

            GESTIONES Gestiones = new GESTIONES();

            Gestiones.GESTIONES_PREJUDICIALES = await _methods.SP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA(ID_DEUDA);
            Gestiones.GESTIONES_JUDICIALES = await _methods.SP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA(ID_DEUDA);
            Gestiones.DEUDAS = await _methods.SP_READ_DEUDAS_X_ID_DEUDA(ID_DEUDA);
            Gestiones.PAGOS = await _methods.SP_READ_PAGO_X_ID_DEUDA(ID_DEUDA);

            return PartialView("_Gestiones", Gestiones);
        }






        [HttpGet]
        public async Task<IActionResult> _RegistrarGestion(int ID_DEUDA,int DISCADOR = 0, int BLOQUEO_LEY = 0)
        {
            

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);


            obj_REGISTRAR_GESTION objRegistrarGestion = new obj_REGISTRAR_GESTION();
            objRegistrarGestion.ID_DEUDA = ID_DEUDA;
            objRegistrarGestion.ID_RESPONSABLE = outputCookie.ID_RESPONSABLE;



            objRegistrarGestion.LUGAR = await _methods.SP_READ_LUGAR_X_ID_DEUDA(ID_DEUDA, DISCADOR, BLOQUEO_LEY);






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
        public async Task<IActionResult> OBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO(int ID_RESPUESTA_CONTACTO, int BLOQUEO_LEY = 0)
        {

            List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO> Contacto = new List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO>();

            Contacto = await _methods.SP_READ_EXCUSA_X_ID_RESPUESTA_CONTACTO(ID_RESPUESTA_CONTACTO, BLOQUEO_LEY);

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


            if (Input.ID_TELEFONO==0)
            {
                Input.ID_TELEFONO = 1;
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


        [HttpGet]
        public async Task<IActionResult> _RegistrarTelefono()
        {
            return PartialView("_RegistrarTelefono");
        }



        [HttpGet]
        public async Task<IActionResult> _RegistrarEmail()
        {

            return PartialView("_RegistrarEmail");
        }



        [HttpPost]
        public async Task<IActionResult> Create_Telefono_Prejudicial(iSP_CREATE_TELEFONO_PREJUDICIAL Input)
        {

            oSP_CREATE_TELEFONO_PREJUDICIAL output = new oSP_CREATE_TELEFONO_PREJUDICIAL();

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

            output = await _methods.SP_CREATE_TELEFONO_PRE_JUDICIAL(Input, outputCookie.ID_RESPONSABLE);

            return Ok(output);

        }



        [HttpPost]
        public async Task<IActionResult> Create_Email_Prejudicial(iSP_CREATE_EMAIL_PREJUDICIAL Input)
        {

            oSP_CREATE_EMAIL_PREJUDICIAL output = new oSP_CREATE_EMAIL_PREJUDICIAL();

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

            output = await _methods.SP_CREATE_EMAIL_PRE_JUDICIAL(Input, outputCookie.ID_RESPONSABLE);

            return Ok(output);

        }






        [HttpGet]
        public async Task<IActionResult> OBTENER_TELEFONOS_X_ID_RESPUESTA_EXCUSA(int ID_DEUDA, int ID_RESPUESTA_EXCUSA)
        {

            List<oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA> Contacto = new List<oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA>();

            Contacto = await _methods.SP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA(ID_DEUDA,ID_RESPUESTA_EXCUSA);
 

            return Ok(Contacto);

        }





        [HttpGet]
        public async Task<IActionResult> _LeerCampañas(int ID_DEUDA)
        {

            INFORMACION Info = new INFORMACION();

            Info.OFERTAS = await _methods.SP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA(ID_DEUDA);


            return PartialView("_ListadoCampañas", Info);
        }




        [HttpGet]
        public async Task<IActionResult> _LeerAlertas(int ID_DEUDA)
        {

            INFORMACION Info = new INFORMACION();

            Info.ALERTAS = await _methods.SP_READ_ALERTA_X_ID_DEUDA(ID_DEUDA);


            return PartialView("_ListadoAlertas", Info);
        }




        [HttpGet]
        public async Task<IActionResult> _LeerEstadoPreJudicial(int ID_DEUDA, string ESTADO_ACTUAL)
        {

            OBJ_MODIFICAR_ESTADO_PREJUDICIAL Estados_Prejudicial = new OBJ_MODIFICAR_ESTADO_PREJUDICIAL();

            Estados_Prejudicial.ESTADOS_PREJUDICIAL = await _methods.SP_READ_LISTA_ESTADO_PRE_JUDICIAL(ID_DEUDA);
            Estados_Prejudicial.ID_DEUDA = ID_DEUDA;
            Estados_Prejudicial.ESTADO_ACTUAL = ESTADO_ACTUAL;


            return PartialView("_EstadoPreJudicial", Estados_Prejudicial);
        }




        [HttpPost]
        public async Task<IActionResult> Update_Estado_PreJudicial(iSP_UPDATE_ESTADO_PREJUDICIAL Input)
        {

            oSP_UPDATE_ESTADO_PREJUDICIAL output = new oSP_UPDATE_ESTADO_PREJUDICIAL();

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

            output = await _methods.SP_UPDATE_ESTADO_PREJUDICIAL(Input, outputCookie.ID_RESPONSABLE);

            return Ok(output);

        }



        [HttpGet]
        public async Task<IActionResult> _LeerEstadoEmaill(int ID_EMAIL,string ESTADO_ACTUAL_EMAIL,string EMAIL)
        {

            OBJ_MODIFICAR_ESTADO_EMAIL Estados_Email = new OBJ_MODIFICAR_ESTADO_EMAIL();

            Estados_Email.ESTADO_EMAIL = await _methods.SP_READ_ESTADO_EMAIL();
            Estados_Email.ID_EMAIL = ID_EMAIL;
            Estados_Email.ESTADO_ACTUAL_EMAIL = ESTADO_ACTUAL_EMAIL;
            Estados_Email.EMAIL = EMAIL;

            return PartialView("_EstadoEmail", Estados_Email);
        }


        [HttpPost]
        public async Task<IActionResult> Update_Estado_Email(iSP_UPDATE_ESTADO_EMAIL Input)
        {

            oSP_UPDATE_ESTADO_EMAIL output = new oSP_UPDATE_ESTADO_EMAIL();

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

            output = await _methods.SP_UPDATE_ESTADO_EMAIL(Input, outputCookie.ID_RESPONSABLE);

            return Ok(output);

        }


        [HttpGet]
        public async Task<IActionResult> _LeerEstadoDireccion(int ID_DIRECCION, string ESTADO_ACTUAL_DIRECCION, string DIRECCION, string COMUNA)
        {

            OBJ_MODIFICAR_ESTADO_DIRECCION Estados_Direccion = new OBJ_MODIFICAR_ESTADO_DIRECCION();

            Estados_Direccion.ESTADO_DIRECCION = await _methods.SP_READ_ESTADO_DIRECCION();
            Estados_Direccion.ID_DIRECCION = ID_DIRECCION;
            Estados_Direccion.ESTADO_ACTUAL_DIRECCION = ESTADO_ACTUAL_DIRECCION;
            Estados_Direccion.DIRECCION = DIRECCION;
            Estados_Direccion.COMUNA = COMUNA;

            return PartialView("_EstadoDireccion", Estados_Direccion);
        }



        [HttpPost]
        public async Task<IActionResult> Update_Estado_Direccion(iSP_UPDATE_ESTADO_DIRECCION Input)
        {

            oSP_UPDATE_ESTADO_DIRECCION output = new oSP_UPDATE_ESTADO_DIRECCION();

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

            output = await _methods.SP_UPDATE_ESTADO_DIRECCION(Input, outputCookie.ID_RESPONSABLE);

            return Ok(output);

        }


        [HttpGet]
        public async Task<IActionResult> _LeerEstadoTelefono(int ID_TELEFONO, string ESTADO_ACTUAL_TELEFONO, int TELEFONO)
        {

            OBJ_MODIFICAR_ESTADO_TELEFONO Estados_telefono = new OBJ_MODIFICAR_ESTADO_TELEFONO();

            Estados_telefono.ESTADO_TELEFONO= await _methods.SP_READ_ESTADO_TELEFONO();
            Estados_telefono.ID_TELEFONO = ID_TELEFONO;
            Estados_telefono.ESTADO_ACTUAL_TELEFONO = ESTADO_ACTUAL_TELEFONO;
            Estados_telefono.TELEFONO = TELEFONO;


            return PartialView("_EstadoTelefono", Estados_telefono);
        }




        

        [HttpPost]
        public async Task<IActionResult> Update_Estado_Telefono(iSP_UPDATE_ESTADO_TELEFONO Input)
        {

            oSP_UPDATE_ESTADO_TELEFONO output = new oSP_UPDATE_ESTADO_TELEFONO();

            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonConvert.DeserializeObject<oLogin>(JsonCookie_Decodificado);

            output = await _methods.SP_UPDATE_ESTADO_TELEFONO(Input, outputCookie.ID_RESPONSABLE);

            return Ok(output);

        }



    }

}

