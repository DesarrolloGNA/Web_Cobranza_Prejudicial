using System.ComponentModel.DataAnnotations;

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

        }






    }
}
