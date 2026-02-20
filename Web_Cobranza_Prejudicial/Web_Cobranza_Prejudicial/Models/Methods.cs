using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading;
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
                        output.ANEXO_TELEFONICO = Convert.ToInt32(dr["ANEXO_TELEFONICO"].ToString());
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
                                output.CONTACTABILIDAD = Convert.ToInt32(dr["CONTACTABILIDAD"]?.ToString());
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





        public async Task<List<oSP_READ_LUGAR_X_ID_DEUDA>> SP_READ_LUGAR_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_LUGAR_X_ID_DEUDA> Lugar = new List<oSP_READ_LUGAR_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_LUGAR_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_LUGAR_X_ID_DEUDA output = new oSP_READ_LUGAR_X_ID_DEUDA();

                                output.ID_RESPUESTA_LUGAR = Convert.ToInt32(dr["ID_RESPUESTA_LUGAR"]?.ToString());
                                output.LUGAR = dr["LUGAR"]?.ToString();


                                Lugar.Add(output);
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
            return Lugar;
        }




        public async Task<List<oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR>> SP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR(int ID_RESPUESTA_LUGAR)
        {
            List<oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR> Contacto = new List<oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_RESPUESTA_LUGAR", SqlDbType.Int).Value = ID_RESPUESTA_LUGAR;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR output = new oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR();

                                output.ID_RESPUESTA_CONTACTO = Convert.ToInt32(dr["ID_RESPUESTA_CONTACTO"]?.ToString());
                                output.CONTACTO = dr["RESPUESTA_CONTACTO"]?.ToString();


                                Contacto.Add(output);
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
            return Contacto;
        }






        public async Task<List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO>> SP_READ_EXCUSA_X_ID_RESPUESTA_CONTACTO(int ID_RESPUESTA_CONTACTO)
        {
            List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO> Excusa = new List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_EXCUSA_X_ID_RESPUESTA_CONTACTO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_RESPUESTA_CONTACTO", SqlDbType.Int).Value = ID_RESPUESTA_CONTACTO;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO output = new oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO();

                                output.ID_RESPUESTA_EXCUSA = Convert.ToInt32(dr["ID_RESPUESTA_EXCUSA"]?.ToString());
                                output.RESPUESTA_EXCUSA = dr["RESPUESTA_EXCUSA"]?.ToString();


                                Excusa.Add(output);
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
            return Excusa;
        }


        public async Task<oSP_CREATE_GESTION_PREJUDICIAL> SP_CREATE_GESTION_PRE_JUDICIAL(iSP_CREATE_GESTION_PREJUDICIAL input)
        {

            oSP_CREATE_GESTION_PREJUDICIAL output = new oSP_CREATE_GESTION_PREJUDICIAL();
     

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_CREATE_GESTION_PRE_JUDICIAL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_LOG_DISCADOR", SqlDbType.Int).Value = input.ID_LOG_DISCADOR;
                        cmd.Parameters.Add("@ID_DEUDA_GESTION", SqlDbType.Int).Value = input.ID_DEUDA_GESTION;
                        cmd.Parameters.Add("@ID_RESPONSABLE_GESTION", SqlDbType.Int).Value = input.ID_RESPONSABLE_GESTION;
                        cmd.Parameters.Add("@ID_TELEFONO", SqlDbType.Int).Value = input.ID_TELEFONO;
                        cmd.Parameters.Add("@ID_RESPUESTA_LUGAR", SqlDbType.Int).Value = input.ID_RESPUESTA_LUGAR;
                        cmd.Parameters.Add("@ID_RESPUESTA_CONTACTO", SqlDbType.Int).Value = input.ID_RESPUESTA_CONTACTO;
                        cmd.Parameters.Add("@ID_RESPUESTA_EXCUSA", SqlDbType.Int).Value = input.ID_RESPUESTA_EXCUSA;
                        cmd.Parameters.Add("@FECHA_PROMESA", SqlDbType.DateTime).Value = input.FECHA_PROMESA;
                        cmd.Parameters.Add("@MONTO_PROMESA", SqlDbType.Int).Value = input.MONTO_PROMESA;
                        cmd.Parameters.Add("@OBSERVACION", SqlDbType.VarChar).Value = input.OBSERVACION;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
     

                                output.RETURN_VALUE = Convert.ToInt32(dr["RETURN_VALUE"]?.ToString());
                                output.MENSAJE = dr["MENSAJE"]?.ToString();

                            }
                            else
                            {
                                output.RETURN_VALUE = 0;
                                output.MENSAJE = "No fue posible Registrar Gestion.";
                            }
                        }
                    }

                    connection.Close();
                }
                catch (Exception EX)
                {
                    output.RETURN_VALUE = -1;
                    output.MENSAJE = "Error : " + EX.Message.ToString();

                    connection.Close();
                }
            }
            return output;
        }





        public async Task<List<oSP_READ_CARRIER_LLAMADAS>> SP_READ_CARRIER_LLAMADAS(int ID_DEUDA)
        {
            List<oSP_READ_CARRIER_LLAMADAS> Carrier = new List<oSP_READ_CARRIER_LLAMADAS>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_CARRIER_LLAMADAS", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_CARRIER_LLAMADAS output = new oSP_READ_CARRIER_LLAMADAS();

                                output.ID_CARRIER_LLAMADAS = Convert.ToInt32(dr["ID_CARRIER_LLAMADAS"]?.ToString());
                                output.CARRIER = Convert.ToInt32(dr["CARRIER"]?.ToString());
                                output.ORDEN_PRIORIDAD = Convert.ToInt32(dr["ORDEN_PRIORIDAD"]?.ToString());
                                output.OBSERVACION = dr["OBSERVACION"]?.ToString();
         

                                Carrier.Add(output);
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
            return Carrier;
        }





        public async Task<int> SP_CREATE_LOG_DISCADOR(oSP_CREATE_LOG_DISCADOR input)
        {
            int ID_LOG_DISCADOR = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                SqlCommand cmd = new SqlCommand("PRE.SP_CREATE_LOG_DISCADOR", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_DEUDA", input.ID_DEUDA));
                cmd.Parameters.Add(new SqlParameter("@ID_RESPONSABLE", input.ID_RESPONSABLE));
                cmd.Parameters.Add(new SqlParameter("@ANEXO_TELEFONICO", input.ANEXO_TELEFONICO));
                cmd.Parameters.Add(new SqlParameter("@CARRIER_LLAMADA", input.CARRIER_LLAMADA));
                cmd.Parameters.Add(new SqlParameter("@RESPUESTA", input.RESPUESTA));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", input.TELEFONO));

                SqlParameter VALOR_RETORNO = new SqlParameter("@RETURN_VALUE", DbType.Int32);
                VALOR_RETORNO.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(VALOR_RETORNO);


                connection.Open();
                await cmd.ExecuteNonQueryAsync();

                ID_LOG_DISCADOR = int.Parse(cmd.Parameters["@RETURN_VALUE"].Value.ToString());

                connection.Close();
            }

            return ID_LOG_DISCADOR;
        }













    }
}
