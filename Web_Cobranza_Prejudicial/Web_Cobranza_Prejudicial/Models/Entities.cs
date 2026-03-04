using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Web_Cobranza_Prejudicial.Models
{
    public class Entities
    {


        public class iLogin
        {
            [Required]
            public String USUARIO { get; set; }

            [Required]
            public String CONTRASEÑA { get; set; }

        }
        public class Message
        {
            public bool VISIBLE { get; set; }
            public String MENSAJE { get; set; }

        }

        public class oLogin
        {
            public int RETURN_VALUE { get; set; }
            public String NOMBRE_RESPONSABLE { get; set; }
            public String PERFIL_RESPONSABLE { get; set; }
            public int ID_RESPONSABLE { get; set; }
            public int ID_PERFIL_RESPONSABLE { get; set; }
            public String MENSAJE { get; set; }
            public String TOKEN { get; set; }

            public int ANEXO_TELEFONICO { get; set; }

        }


        public class oSP_READ_DEUDA_X_FILTROS
        {
            public String NOMBRE { get; set; }
            public String NOMBRE_MANDANTE { get; set; }
            public int RUT { get; set; }
            public String DV { get; set; }
            public int ID_DEUDA { get; set; }
            public String FOLIO { get; set; }
            public DateTime FECHA_VENCIMIENTO { get; set; }
            public int VALOR { get; set; }
            public String ESTADO_DEUDA { get; set; }
        }


        public class oSP_READ_DATOS_PERSONALES_X_ID_DEUDA
        {
            public String NOMBRE_TUTOR { get; set; }

            public String NOMBRE_MANDANTE { get; set; }
            public int RUT_TUTOR { get; set; }
            public String DV_TUTOR { get; set; }
            public String NOMBRE_ALUMNO { get; set; }
            public int RUT_ALUMNO { get; set; }
            public String DV_ALUMNO { get; set; }
            public int ID_DEUDA { get; set; }
            public String ESTADO_JUICIO { get; set; }
            public String ESTADO_GESTION { get; set; }
        }



        public class INFORMACION
        {
            public oSP_READ_DATOS_PERSONALES_X_ID_DEUDA DATOS_PERSONALES { get; set; }
            public List<oSP_READ_TELEFONOS_X_ID_DEUDA> TELEFONOS { get; set; }
            public List<oSP_READ_EMAIL_X_ID_DEUDA> EMAILS { get; set; }
            public List<oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA> OFERTAS { get; set; }
            public List<oSP_READ_DIRECCION_X_ID_DEUDA> DIRECCIONES { get; set; }
            
        }




        public class oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA
        {
            public int ID_GESTIONES_PREJUDICIALES { get; set; }
            public DateTime FECHA_REGISTRO { get; set; }
            public String OBSERVACION { get; set; }
            public DateTime FECHA_PROMESA { get; set; }
            public int MONTO_PROMESA { get; set; }
            public int TELEFONO { get; set; }
            public String NOMBRE_RESPONSABLE { get; set; }
            public String LUGAR { get; set; }
            public String RESPUESTA_EXCUSA { get; set; }
            public String RESPUESTA_CONTACTO { get; set; }
            public String CODIGO { get; set; }

        }



        public class GESTIONES
        {
            public List<oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA> GESTIONES_PREJUDICIALES { get; set; }
            public List<oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA> GESTIONES_JUDICIALES { get; set; }
            public List<oSP_READ_DEUDAS_X_ID_DEUDA> DEUDAS { get; set; }
            public List<oSP_READ_PAGO_X_ID_DEUDA> PAGOS { get; set; }
        }



        public class oSP_READ_TELEFONOS_X_ID_DEUDA
        {
            public int ID_TELEFONO { get; set; }
            public int TELEFONO { get; set; }
            public int C_CD { get; set; }
            public int C_CI { get; set; }
            public int C_SC { get; set; }
            public String OBSERVACION { get; set; }
            public DateTime FECHA_ACTUALIZACION { get; set; }
            public String ESTADO_TELEFONO { get; set; }
            public int CONTACTABILIDAD { get; set; }

        }



        public class oSP_READ_EMAIL_X_ID_DEUDA
        {

            public int ID_EMAIL { get; set; }
            public String EMAIL { get; set; }
            public String ESTADO_EMAIL { get; set; }
            public DateTime FECHA_ACTUALIZACION { get; set; }

        }





        public class obj_REGISTRAR_GESTION
        {
            public int ID_DEUDA { get; set; }
            public int ID_RESPONSABLE { get; set; }
            public List<oSP_READ_TELEFONOS_X_ID_DEUDA> TELEFONOS { get; set; }
            public List<oSP_READ_LUGAR_X_ID_DEUDA> LUGAR { get; set; }
        }

        public class oSP_READ_LUGAR_X_ID_DEUDA
        {
            public int ID_RESPUESTA_LUGAR { get; set; }
            public string LUGAR { get; set; }


        }


        public class oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR
        {
            public int ID_RESPUESTA_CONTACTO { get; set; }
            public string CONTACTO { get; set; }


        }



        public class oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO
        {
            public int ID_RESPUESTA_EXCUSA { get; set; }
            public string RESPUESTA_EXCUSA { get; set; }


        }



        public class iSP_CREATE_GESTION_PREJUDICIAL
        {
            [Required]
            public int ID_LOG_DISCADOR { get; set; }

            [Required]
            public int ID_DEUDA_GESTION { get; set; }
            [Required]
            public int ID_RESPONSABLE_GESTION { get; set; }
            [Required]
            public int ID_TELEFONO { get; set; }

            [Required]
            public int ID_RESPUESTA_LUGAR { get; set; }
            [Required]
            public int ID_RESPUESTA_CONTACTO { get; set; }
            [Required]
            public int ID_RESPUESTA_EXCUSA { get; set; }

            public DateTime FECHA_PROMESA { get; set; }
            public string MONTO_PROMESA { get; set; }

            [Required]
            public string OBSERVACION { get; set; }


        }





        public class oSP_CREATE_GESTION_PREJUDICIAL
        {
            public int RETURN_VALUE { get; set; }
            public string MENSAJE { get; set; }
        }



        public class DISCADOR
        {
            public List<oSP_READ_CARRIER_LLAMADAS> CARRIER_LLAMADAS { get; set; }
            public List<oSP_READ_TELEFONOS_X_ID_DEUDA> TELEFONOS { get; set; }

        }


        public class oSP_READ_CARRIER_LLAMADAS
        {
            public int ID_CARRIER_LLAMADAS { get; set; }
            public int CARRIER { get; set; }
            public int ORDEN_PRIORIDAD { get; set; }
            public string OBSERVACION { get; set; }

        }


        public class oSP_CREATE_LOG_DISCADOR
        {
            public int ID_DEUDA { get; set; }
            public int ID_RESPONSABLE { get; set; }
            public int ANEXO_TELEFONICO { get; set; }
            public int CARRIER_LLAMADA { get; set; }
            public string RESPUESTA { get; set; }
            public int TELEFONO { get; set; }

        }



        public class oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA
        {
            public int ID_CAMPAÑAS_OFERTAS { get; set; }
            public int ID_MANDANTE { get; set; }
            public int ID_DEUDA { get; set; }
            public DateTime FECHA_INICIO_CAMPAÑA { get; set; }
            public DateTime FECHA_FIN_CAMPAÑA { get; set; }
            public int PRIORIDAD { get; set; }
            public String DESCRIPCION { get; set; }

        }



        public class oSP_READ_DIRECCION_X_ID_DEUDA
        {
            public int ID_DIRECCION { get; set; }
            public int ID_MANDANTE { get; set; }
            public String DIRECCION { get; set; }
            public int ID_COMUNA { get; set; }
            public DateTime FECHA_CREACION { get; set; }
            public String OBSERVACION { get; set; }
            public DateTime FECHA_ACTUALIZACION { get; set; }
            public int ID_CLIENTE { get; set; }
            public int ID_ESTADO_DIRECCION { get; set; }
            public String ESTADO_DIRECCION { get; set; }
            public String CIUDAD { get; set; }
            public String COMUNA { get; set; }
            public String REGION { get; set; }
            public String NUM_REGION { get; set; }

        }

        public class oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA
        {
            public int ID_GESTIONES_JUDICIALES { get; set; }
            public DateTime FECHA_REGISTRO { get; set; }
            public String NOMBRE_RESPONSABLE { get; set; }
            public String RESPUESTA_CONTACTO { get; set; }
            public String RESPUESTA_EXCUSA { get; set; }
            public String OBSERVACION { get; set; }
            public String TIPO_GESTION { get; set; }
            public DateTime FECHA_DILIGENCIA { get; set; }
            public int VALOR_DILIGENCIA { get; set; }

        }



        public class oSP_READ_DEUDAS_X_ID_DEUDA
        {

            public int ID_DEUDA { get; set; }
            public String FOLIO { get; set; }
            public DateTime FECHA_VENCIMIENTO { get; set; }
            public int VALOR { get; set; }
            public String FACULTAD { get; set; }
            public String SEDE { get; set; }
            public String MOTIVO { get; set; }
            public DateTime FECHA_CARGA { get; set; }
            public int NRO_JUICIO { get; set; }
            public int RUT_ALUMNO { get; set; }
            public String DV_ALUMNO { get; set; }
            public String NOMBRE_ALUMNO { get; set; }
            public String ESTADO_DEUDA { get; set; }

        }


        public class oSP_READ_PAGO_X_ID_DEUDA
        {
            public int ID_PAGO { get; set; }
            public int MONTO_PAGO { get; set; }
            public DateTime FECHA_PAGO { get; set; }
            public String TIPO_PAGO { get; set; }
        }



        public class oSP_READ_BANNER_X_ID_DEUDA
        {
            public String FOLIO { get; set; }
            public String ESTADO_DEUDA { get; set; }
            public String NOMBRE_MANDANTE { get; set; }

           
        }









    }
}
