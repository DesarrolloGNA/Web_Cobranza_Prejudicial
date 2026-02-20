using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading;
using Web_Cobranza_Prejudicial.Models;
using static Web_Cobranza_Prejudicial.Models.Entities;


namespace Web_Cobranza_Prejudicial.Controllers
{

    [ApiController]
    [Route("api/call")]
    public class CallController : Controller
    {
        private readonly AmiService _amiService;
        private readonly Methods _methods;

        public CallController(AmiService amiService, Methods methods)
        {
            _amiService = amiService;
            _methods = methods;
        }


        [HttpPost("call")]
        public async Task<IActionResult> Call([FromQuery] int NUMERO_TELEFONO, int CARRIER, int ID_DEUDA)
        {

            String TELEFONO_COMPLETO = (CARRIER.ToString() + NUMERO_TELEFONO.ToString());


            string JsonCookie_Codificado = Request.Cookies["Session"];

            string JsonCookie_Decodificado = "";
            using (Helpers helpers = new Helpers())
            {
                JsonCookie_Decodificado = (helpers.Base64Decode(JsonCookie_Codificado.Replace("#####GNA####", "")));
            }

            oLogin outputCookie = new oLogin();
            outputCookie = JsonSerializer.Deserialize<oLogin>(JsonCookie_Decodificado);


            await _amiService.ConnectAsync();
            var result = await _amiService.ClickToCall(outputCookie.ANEXO_TELEFONICO.ToString(), TELEFONO_COMPLETO);


            //GRABO EL INTENTO DE LLAMADA

            oSP_CREATE_LOG_DISCADOR input = new oSP_CREATE_LOG_DISCADOR
            {
                ID_DEUDA = ID_DEUDA,
                ID_RESPONSABLE = outputCookie.ID_RESPONSABLE,
                ANEXO_TELEFONICO = outputCookie.ANEXO_TELEFONICO,
                CARRIER_LLAMADA = CARRIER,
                RESPUESTA = result,
                TELEFONO = NUMERO_TELEFONO
            };

            int ID_LOG_DISCADOR;
            ID_LOG_DISCADOR = await _methods.SP_CREATE_LOG_DISCADOR(input);

            return Ok(new
            {
                success = true,
                logId = ID_LOG_DISCADOR,
                amiResponse = result,
                telefono = TELEFONO_COMPLETO
            });

        }


    }
}
