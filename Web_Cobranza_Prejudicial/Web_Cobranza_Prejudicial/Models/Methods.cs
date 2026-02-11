using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using static Web_Cobranza_Prejudicial.Models.Entities;

namespace Web_Cobranza_Prejudicial.Models
{
    public class Methods : IDisposable
    {

        /// <summary>
        /// DESTRUCTOR
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.SuppressFinalize(this);
        }

        private readonly string _connectionString;

        public Methods(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        /// <summary>
        ///  Validar Usuario
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Retorna Objeto con token de validacion</returns>
        public oLogin Login(iLogin input, string DIR_IP)
        {
            oLogin output = new oLogin();

  
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using SqlCommand cmd = new SqlCommand("GNA.WL_SP_READ_RESPONSABLE_VALIDA_SESION", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@USUARIO", input.USUARIO));
                    cmd.Parameters.Add(new SqlParameter("@CONTRASEÑA", input.CONTRASEÑA));
                    cmd.Parameters.Add(new SqlParameter("@DIR_IP", DIR_IP));
                    connection.Open();
                    using SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        output.RETURN_VALUE = Convert.ToInt32(dr["RETURN_VALUE"].ToString());
                        output.NOMBRE_RESPONSABLE = dr["NOMBRE_RESPONSABLE"].ToString();
                        output.PERFIL_RESPONSABLE = dr["PERFIL_RESPONSABLE"].ToString();
                        output.ID_RESPONSABLE = Convert.ToInt32(dr["ID_RESPONSABLE"].ToString());
                        output.ID_PERFIL_RESPONSABLE = Convert.ToInt32(dr["ID_PERFIL_RESPONSABLE"].ToString());
                        output.TOKEN = dr["TOKEN"].ToString();
                    }

                    connection.Close();

                    output.MENSAJE = output.RETURN_VALUE > 0
                    ? "Usuario Logueado Correctamente"
                    : "Usuario y/o Contraseña Erróneo";
                }
                catch (Exception EX)
                {
                    output.RETURN_VALUE = -1;
                    output.MENSAJE = "Error de Conexión," + EX.Message;
                    connection.Close();
                }


            }

            return output;
        }

        /// <summary>
        ///  Valida Session Activa
        /// </summary>
        public Boolean WL_VALIDA_SESSION_ACTIVA(string TOKEN, string IP)
        {
            Boolean ESTADO_CONEXION = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {


                SqlCommand cmd = new SqlCommand("GNA.WL_VALIDA_SESSION_ACTIVA", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DIR_IP", IP));
                cmd.Parameters.Add(new SqlParameter("@TOKEN", TOKEN));

                SqlParameter VALOR_RETORNO = new SqlParameter("@RETURN_VALUE", DbType.Int32);
                VALOR_RETORNO.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(VALOR_RETORNO);


                connection.Open();
                cmd.ExecuteNonQuery();

                ESTADO_CONEXION = cmd.Parameters["@RETURN_VALUE"].Value.ToString() == "1" ? true : false;


                connection.Close();


            }

            return ESTADO_CONEXION;
        }



        public List<oSP_READ_DEUDA_X_FILTROS> SP_READ_DEUDA_X_FILTROS(string TIPO_BUSQUEDA, string VALOR_BUSCAR)
        {
            List<oSP_READ_DEUDA_X_FILTROS> output = new List<oSP_READ_DEUDA_X_FILTROS>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("PRE.SP_READ_DEUDA_X_FILTROS", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_BUSQUEDA", TIPO_BUSQUEDA));
                    cmd.Parameters.Add(new SqlParameter("@VALOR_BUSCAR", VALOR_BUSCAR));
                    connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        oSP_READ_DEUDA_X_FILTROS REGISTRO = new oSP_READ_DEUDA_X_FILTROS();
                        REGISTRO.NOMBRE = dr["NOMBRE"].ToString();
                        REGISTRO.NOMBRE_MANDANTE = dr["NOMBRE_MANDANTE"].ToString();
                        REGISTRO.RUT = Convert.ToInt32(dr["RUT"].ToString());
                        REGISTRO.DV = dr["DV"].ToString();
                        REGISTRO.ID_DEUDA = Convert.ToInt32(dr["ID_DEUDA"].ToString());
                        REGISTRO.FOLIO = dr["FOLIO"].ToString();
                        REGISTRO.FECHA_VENCIMIENTO = Convert.ToDateTime(dr["FECHA_VENCIMIENTO"].ToString());
                        REGISTRO.VALOR = Convert.ToInt32(dr["VALOR"].ToString());
                        REGISTRO.ESTADO_DEUDA = dr["ESTADO_DEUDA"].ToString();

                        output.Add(REGISTRO);
                    }
                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }



            return output;

        }


        public async Task<oSP_READ_DATOS_PERSONALES_X_ID_DEUDA> SP_READ_DATOS_PERSONALES_X_ID_DEUDA(int ID_DEUDA)
        {
            oSP_READ_DATOS_PERSONALES_X_ID_DEUDA output = new oSP_READ_DATOS_PERSONALES_X_ID_DEUDA();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_DATOS_PERSONALES_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {

                                output.NOMBRE_TUTOR = dr["NOMBRE_TUTOR"]?.ToString();
                                output.NOMBRE_MANDANTE = dr["NOMBRE_MANDANTE"]?.ToString();
                                output.RUT_TUTOR = dr["RUT_TUTOR"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RUT_TUTOR"]);
                                output.DV_TUTOR = dr["DV_TUTOR"]?.ToString();
                                output.NOMBRE_ALUMNO = dr["NOMBRE_ALUMNO"]?.ToString();
                                output.RUT_ALUMNO = dr["RUT_ALUMNO"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RUT_ALUMNO"]);
                                output.DV_ALUMNO = dr["DV_ALUMNO"]?.ToString();
                                output.ID_DEUDA = dr["ID_DEUDA"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID_DEUDA"]);
                                output.ESTADO_JUICIO = dr["ESTADO_JUICIO"]?.ToString();
                                output.ESTADO_GESTION = dr["ESTADO_GESTION"]?.ToString();
                              
                            }
                        }
                    }

                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return output;
        }



        public async Task<List<oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA>> SP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA(int ID_DEUDA)
        {
            
            List<oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA> Gestiones =  new List<oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA output = new oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA();

                                output.ID_GESTIONES_PREJUDICIALES = Convert.ToInt32(dr["ID_GESTIONES_PREJUDICIALES"]?.ToString());
                                output.FECHA_REGISTRO = Convert.ToDateTime(dr["FECHA_REGISTRO"]?.ToString());
                                output.OBSERVACION = dr["OBSERVACION"]?.ToString();
                                output.FECHA_PROMESA = Convert.ToDateTime(dr["FECHA_PROMESA"]?.ToString());
                                output.MONTO_PROMESA = Convert.ToInt32(dr["MONTO_PROMESA"]?.ToString());
                                output.TELEFONO = Convert.ToInt32(dr["TELEFONO"]?.ToString());
                                output.NOMBRE_RESPONSABLE = dr["NOMBRE_RESPONSABLE"]?.ToString();
                                output.LUGAR = dr["LUGAR"]?.ToString();
                                output.RESPUESTA_EXCUSA = dr["RESPUESTA_EXCUSA"]?.ToString();
                                output.RESPUESTA_CONTACTO = dr["RESPUESTA_CONTACTO"]?.ToString();
                                output.CODIGO = dr["CODIGO"]?.ToString();
                                Gestiones.Add(output);
                            }
                        }
                    }

                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return Gestiones;
        }



        public async Task<List<oSP_READ_TELEFONOS_X_ID_DEUDA>> SP_READ_TELEFONOS_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_TELEFONOS_X_ID_DEUDA> Telefonos = new List<oSP_READ_TELEFONOS_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_TELEFONOS_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_TELEFONOS_X_ID_DEUDA output = new oSP_READ_TELEFONOS_X_ID_DEUDA();
                                output.ID_TELEFONO = Convert.ToInt32(dr["ID_TELEFONO"]?.ToString());
                                output.TELEFONO = Convert.ToInt32(dr["TELEFONO"]?.ToString());
                                output.C_CD = Convert.ToInt32(dr["C_CD"]?.ToString());
                                output.C_CI = Convert.ToInt32(dr["C_CI"]?.ToString());
                                output.C_SC = Convert.ToInt32(dr["C_SC"]?.ToString());
                                output.OBSERVACION = dr["OBSERVACION"]?.ToString();
                                output.FECHA_ACTUALIZACION = Convert.ToDateTime(dr["FECHA_ACTUALIZACION"]?.ToString());
                                output.ESTADO_TELEFONO = dr["ESTADO_TELEFONO"]?.ToString();
                                Telefonos.Add(output);  
                            }
                        }
                    }

                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return Telefonos;
        }







        public async Task<List<oSP_READ_EMAIL_X_ID_DEUDA>> SP_READ_EMAIL_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_EMAIL_X_ID_DEUDA> Emails = new List<oSP_READ_EMAIL_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_EMAIL_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_EMAIL_X_ID_DEUDA output = new oSP_READ_EMAIL_X_ID_DEUDA();

                                output.ID_EMAIL = Convert.ToInt32(dr["ID_EMAIL"]?.ToString());
                                output.EMAIL = dr["EMAIL"]?.ToString();
                                output.ESTADO_EMAIL = dr["ESTADO_EMAIL"]?.ToString();
                                output.FECHA_ACTUALIZACION = Convert.ToDateTime(dr["FECHA_ACTUALIZACION"]?.ToString());

                                Emails.Add(output);
                            }
                        }
                    }

                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
            return Emails;
        }






    }
}
