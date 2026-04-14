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

                    using SqlCommand cmd = new SqlCommand("PRE.SP_READ_RESPONSABLE_VALIDA_SESION", connection);
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
                                output.FECHA_ULTIMO_PAGO = Convert.ToDateTime(dr["FECHA_ULTIMO_PAGO"]?.ToString());

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

            List<oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA> Gestiones = new List<oSP_READ_GESTIONES_PREJUDICIALES_X_ID_DEUDA>();


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
                                output.ID_TIPO_CONTACTABILIDAD = Convert.ToInt32(dr["ID_TIPO_CONTACTABILIDAD"]?.ToString());
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





        public async Task<List<oSP_READ_LUGAR_X_ID_DEUDA>> SP_READ_LUGAR_X_ID_DEUDA(int ID_DEUDA, int DISCADOR = 0, int BLOQUEO_LEY = 0,int REGULARIZADOS = 0)
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
                        cmd.Parameters.Add("@DISCADOR", SqlDbType.Int).Value = DISCADOR;
                        cmd.Parameters.Add("@BLOQUEO_LEY", SqlDbType.Int).Value = BLOQUEO_LEY;
                        cmd.Parameters.Add("@REGULARIZADOS", SqlDbType.Int).Value = REGULARIZADOS;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_LUGAR_X_ID_DEUDA output = new oSP_READ_LUGAR_X_ID_DEUDA();

                                output.ID_RESPUESTA_LUGAR = Convert.ToInt32(dr["ID_RESPUESTA_LUGAR"]?.ToString());
                                output.LUGAR = dr["LUGAR"]?.ToString();
                                output.ID_TIPO_RESPUESTA_LUGAR = Convert.ToInt32(dr["ID_TIPO_RESPUESTA_LUGAR"]?.ToString());

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




        public async Task<List<oSP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR>> SP_READ_CONTACTO_X_ID_RESPUESTA_LUGAR(int ID_RESPUESTA_LUGAR, int BLOQUEO_LEY = 0, int REGULARIZADOS=0)
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
                        cmd.Parameters.Add("@BLOQUEO_LEY", SqlDbType.Int).Value = BLOQUEO_LEY;
                        cmd.Parameters.Add("@REGULARIZADOS", SqlDbType.Int).Value = REGULARIZADOS;
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






        public async Task<List<oOBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO>> SP_READ_EXCUSA_X_ID_RESPUESTA_CONTACTO(int ID_RESPUESTA_CONTACTO,int BLOQUEO_LEY = 0,int REGULARIZADOS = 0)
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
                        cmd.Parameters.Add("@BLOQUEO_LEY", SqlDbType.Int).Value = BLOQUEO_LEY;
                        cmd.Parameters.Add("@REGULARIZADOS", SqlDbType.Int).Value = REGULARIZADOS;
                        
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








        public async Task<List<oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA>> SP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA> Ofertas = new List<oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA output = new oSP_READ_CAMPAÑAS_OFERTAS_X_ID_DEUDA();

                                output.ID_CAMPAÑAS_OFERTAS = Convert.ToInt32(dr["ID_CAMPAÑAS_OFERTAS"]?.ToString());
                                output.ID_MANDANTE = Convert.ToInt32(dr["ID_MANDANTE"]?.ToString());
                                output.ID_DEUDA = Convert.ToInt32(dr["ID_DEUDA"]?.ToString());
                                output.FECHA_INICIO_CAMPAÑA = Convert.ToDateTime(dr["FECHA_INICIO_CAMPAÑA"]?.ToString());
                                output.FECHA_FIN_CAMPAÑA = Convert.ToDateTime(dr["FECHA_FIN_CAMPAÑA"]?.ToString());
                                output.PRIORIDAD = Convert.ToInt32(dr["PRIORIDAD"]?.ToString());
                                output.DESCRIPCION = dr["DESCRIPCION"]?.ToString();

                                Ofertas.Add(output);
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
            return Ofertas;
        }




        public async Task<List<oSP_READ_DIRECCION_X_ID_DEUDA>> SP_READ_DIRECCION_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_DIRECCION_X_ID_DEUDA> Direccion = new List<oSP_READ_DIRECCION_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_DIRECCION_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_DIRECCION_X_ID_DEUDA output = new oSP_READ_DIRECCION_X_ID_DEUDA();

                                output.ID_DIRECCION = Convert.ToInt32(dr["ID_DIRECCION"]?.ToString());
                                output.ID_MANDANTE = Convert.ToInt32(dr["ID_MANDANTE"]?.ToString());
                                output.DIRECCION = dr["DIRECCION"]?.ToString();
                                output.ID_COMUNA = Convert.ToInt32(dr["ID_COMUNA"]?.ToString());
                                output.FECHA_CREACION = Convert.ToDateTime(dr["FECHA_CREACION"]?.ToString());
                                output.OBSERVACION = dr["OBSERVACION"]?.ToString();
                                output.FECHA_ACTUALIZACION = Convert.ToDateTime(dr["FECHA_ACTUALIZACION"]?.ToString());
                                output.ID_CLIENTE = Convert.ToInt32(dr["ID_CLIENTE"]?.ToString());
                                output.ID_ESTADO_DIRECCION = Convert.ToInt32(dr["ID_ESTADO_DIRECCION"]?.ToString());
                                output.ESTADO_DIRECCION = dr["ESTADO_DIRECCION"]?.ToString();
                                output.CIUDAD = dr["CIUDAD"]?.ToString();
                                output.COMUNA = dr["COMUNA"]?.ToString();
                                output.REGION = dr["REGION"]?.ToString();
                                output.NUM_REGION = dr["NUM_REGION"]?.ToString();


                                Direccion.Add(output);
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
            return Direccion;
        }

        public async Task<List<oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA>> SP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA> Gestiones = new List<oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA output = new oSP_READ_GESTIONES_JUDICIALES_X_ID_DEUDA();

                                output.ID_GESTIONES_JUDICIALES = Convert.ToInt32(dr["ID_GESTIONES_JUDICIALES"]?.ToString());
                                output.FECHA_REGISTRO = Convert.ToDateTime(dr["FECHA_REGISTRO"]?.ToString());
                                output.NOMBRE_RESPONSABLE = dr["NOMBRE_RESPONSABLE"]?.ToString();
                                output.RESPUESTA_CONTACTO = dr["RESPUESTA_CONTACTO"]?.ToString();
                                output.RESPUESTA_EXCUSA = dr["RESPUESTA_EXCUSA"]?.ToString();
                                output.OBSERVACION = dr["OBSERVACION"]?.ToString();
                                output.TIPO_GESTION = dr["TIPO_GESTION"]?.ToString();
                                output.FECHA_DILIGENCIA = Convert.ToDateTime(dr["FECHA_DILIGENCIA"]?.ToString());
                                output.VALOR_DILIGENCIA = Convert.ToInt32(dr["VALOR_DILIGENCIA"]?.ToString());

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






        public async Task<List<oSP_READ_DEUDAS_X_ID_DEUDA>> SP_READ_DEUDAS_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_DEUDAS_X_ID_DEUDA> Deuda = new List<oSP_READ_DEUDAS_X_ID_DEUDA>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_DEUDAS_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_DEUDAS_X_ID_DEUDA output = new oSP_READ_DEUDAS_X_ID_DEUDA();

                                output.ID_DEUDA = Convert.ToInt32(dr["ID_DEUDA"]?.ToString());
                                output.FOLIO = dr["FOLIO"]?.ToString();
                                output.FECHA_VENCIMIENTO = Convert.ToDateTime(dr["FECHA_VENCIMIENTO"]?.ToString());
                                output.VALOR = Convert.ToInt32(dr["VALOR"]?.ToString());
                                output.FACULTAD = dr["FACULTAD"]?.ToString();
                                output.SEDE = dr["SEDE"]?.ToString();
                                output.MOTIVO = dr["MOTIVO"]?.ToString();
                                output.FECHA_CARGA = Convert.ToDateTime(dr["FECHA_CARGA"]?.ToString());
                                output.NRO_JUICIO = Convert.ToInt32(dr["NRO_JUICIO"]?.ToString());
                                output.RUT_ALUMNO = Convert.ToInt32(dr["RUT_ALUMNO"]?.ToString());
                                output.DV_ALUMNO = dr["DV_ALUMNO"]?.ToString();
                                output.NOMBRE_ALUMNO = dr["NOMBRE_ALUMNO"]?.ToString();
                                output.ESTADO_DEUDA = dr["ESTADO_DEUDA"]?.ToString();

                                Deuda.Add(output);
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
            return Deuda;
        }





        public async Task<List<oSP_READ_PAGO_X_ID_DEUDA>> SP_READ_PAGO_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_PAGO_X_ID_DEUDA> Pagos = new List<oSP_READ_PAGO_X_ID_DEUDA>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_PAGO_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_PAGO_X_ID_DEUDA output = new oSP_READ_PAGO_X_ID_DEUDA();

                                output.ID_PAGO = Convert.ToInt32(dr["ID_PAGO"]?.ToString());
                                output.MONTO_PAGO = Convert.ToInt32(dr["MONTO_PAGO"]?.ToString());
                                output.FECHA_PAGO = Convert.ToDateTime(dr["FECHA_PAGO"]?.ToString());
                                output.TIPO_PAGO = dr["TIPO_PAGO"]?.ToString();
                                output.FOLIO = dr["FOLIO"]?.ToString();
                                Pagos.Add(output);
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
            return Pagos;
        }



        public async Task<oSP_READ_BANNER_X_ID_DEUDA> SP_READ_BANNER_X_ID_DEUDA(int ID_DEUDA)
        {
            oSP_READ_BANNER_X_ID_DEUDA Banner = new oSP_READ_BANNER_X_ID_DEUDA();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_BANNER_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {

                                Banner.FOLIO = dr["FOLIO"]?.ToString();
                                Banner.ESTADO_DEUDA = dr["ESTADO_DEUDA"]?.ToString();
                                Banner.NOMBRE_MANDANTE = dr["NOMBRE_MANDANTE"]?.ToString();
                                Banner.ESTADO_JUDICIAL = dr["ESTADO_JUDICIAL"]?.ToString();
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
            return Banner;
        }






        public async Task<oSP_CREATE_TELEFONO_PREJUDICIAL> SP_CREATE_TELEFONO_PRE_JUDICIAL(iSP_CREATE_TELEFONO_PREJUDICIAL input, int ID_RESPONSABLE)
        {

            oSP_CREATE_TELEFONO_PREJUDICIAL output = new oSP_CREATE_TELEFONO_PREJUDICIAL();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_CREATE_TELEFONO_PRE_JUDICIAL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = input.ID_DEUDA_TELEFONO;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;
                        cmd.Parameters.Add("@TELEFONO", SqlDbType.Int).Value = input.TELEFONO_PRE;


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
                                output.MENSAJE = "No fue posible Registrar Telefono.";
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





        public async Task<oSP_CREATE_EMAIL_PREJUDICIAL> SP_CREATE_EMAIL_PRE_JUDICIAL(iSP_CREATE_EMAIL_PREJUDICIAL input, int ID_RESPONSABLE)
        {

            oSP_CREATE_EMAIL_PREJUDICIAL output = new oSP_CREATE_EMAIL_PREJUDICIAL();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_CREATE_EMAIL_PRE_JUDICIAL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = input.ID_DEUDA_EMAIL;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;
                        cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar).Value = input.EMAIL_PRE;


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
                                output.MENSAJE = "No fue posible Registrar Telefono.";
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








        public async Task<List<oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA>> SP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA(int ID_DEUDA, int ID_RESPUESTA_EXCUSA)
        {
            List<oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA> Telefonos = new List<oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;
                        cmd.Parameters.Add("@ID_RESPUESTA_EXCUSA", SqlDbType.Int).Value = ID_RESPUESTA_EXCUSA;
                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA output = new oSP_READ_TELEFONOS_X_ID_RESPUESTA_EXCUSA();
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




        public async Task<oSP_VALIDA_BOTONERA_X_ID_DEUDA> SP_VALIDA_BOTONERA_X_ID_DEUDA(int ID_DEUDA)
        {

            oSP_VALIDA_BOTONERA_X_ID_DEUDA Output = new oSP_VALIDA_BOTONERA_X_ID_DEUDA();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_VALIDA_BOTONERA_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;

                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {

                                Output.ESTADO_DEUDA = Convert.ToInt32(dr["ESTADO_DEUDA"]?.ToString());
                                Output.BLOQUEO_LEY = Convert.ToInt32(dr["BLOQUEO_LEY"]?.ToString());

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
            return Output;
        }



        public async Task<List<oSP_READ_ALERTA_X_ID_DEUDA>> SP_READ_ALERTA_X_ID_DEUDA(int ID_DEUDA)
        {
            List<oSP_READ_ALERTA_X_ID_DEUDA> Alertas = new List<oSP_READ_ALERTA_X_ID_DEUDA>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_ALERTA_X_ID_DEUDA", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = ID_DEUDA;
                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_ALERTA_X_ID_DEUDA output = new oSP_READ_ALERTA_X_ID_DEUDA();

                                output.ID_ALERTAS = Convert.ToInt32(dr["ID_ALERTAS"]?.ToString());
                                output.TIPO_ALERTA = Convert.ToInt32(dr["TIPO_ALERTA"]?.ToString());
                                output.TITULO_ALERTA = dr["TITULO_ALERTA"]?.ToString();
                                output.MENSAJE_ALERTA = dr["MENSAJE_ALERTA"]?.ToString();
                                output.PRIORIDAD = Convert.ToInt32(dr["PRIORIDAD"]?.ToString());

                                Alertas.Add(output);
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
            return Alertas;
        }



        public  void SP_CREATE_LOG_RESPONSABLE_X_ESTADO(int ID_RESPONSABLE,int ID_ESTADO_CONEXION)
        {
          

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_CREATE_LOG_RESPONSABLE_X_ESTADO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;
                        cmd.Parameters.Add("@ID_ESTADO_CONEXION", SqlDbType.Int).Value = ID_ESTADO_CONEXION;
                        connection.Open();

                        cmd.ExecuteNonQuery();

                    }

                    connection.Close();
                }
                catch
                {
                    connection.Close();
                }
            }
 
        }



        public async Task<List<oSP_READ_LISTA_ESTADO_PRE_JUDICIAL>> SP_READ_LISTA_ESTADO_PRE_JUDICIAL(int ID_DEUDA)
        {
            List<oSP_READ_LISTA_ESTADO_PRE_JUDICIAL> Estados_PreJudiciales = new List<oSP_READ_LISTA_ESTADO_PRE_JUDICIAL>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_LISTA_ESTADO_PRE_JUDICIAL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_LISTA_ESTADO_PRE_JUDICIAL output = new oSP_READ_LISTA_ESTADO_PRE_JUDICIAL();

                                output.ID_ESTADO_JUDICIAL = Convert.ToInt32(dr["ID_ESTADO_JUDICIAL"]?.ToString());
                                output.ESTADO_JUDICIAL = dr["ESTADO_JUDICIAL"]?.ToString();


                                Estados_PreJudiciales.Add(output);
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
            return Estados_PreJudiciales;
        }



        public async Task<oSP_UPDATE_ESTADO_PREJUDICIAL> SP_UPDATE_ESTADO_PREJUDICIAL(iSP_UPDATE_ESTADO_PREJUDICIAL input, int ID_RESPONSABLE)
        {

            oSP_UPDATE_ESTADO_PREJUDICIAL output = new oSP_UPDATE_ESTADO_PREJUDICIAL();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_UPDATE_ESTADO_PREJUDICIAL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DEUDA", SqlDbType.Int).Value = input.ID_DEUDA_ESTADO_PREJUDICIAL;
                        cmd.Parameters.Add("@ID_ESTADO_JUDICIAL", SqlDbType.Int).Value = input.ID_ESTADO_JUDICIAL;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;


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
                                output.MENSAJE = "No fue posible Registrar Telefono.";
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




        public async Task<List<oSP_READ_ESTADO_EMAIL>> SP_READ_ESTADO_EMAIL()
        {
            List<oSP_READ_ESTADO_EMAIL> Estados_Email = new List<oSP_READ_ESTADO_EMAIL>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_ESTADO_EMAIL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_ESTADO_EMAIL output = new oSP_READ_ESTADO_EMAIL();

                                output.ID_ESTADO_EMAIL = Convert.ToInt32(dr["ID_ESTADO_EMAIL"]?.ToString());
                                output.ESTADO_EMAIL = dr["ESTADO_EMAIL"]?.ToString();


                                Estados_Email.Add(output);
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
            return Estados_Email;
        }



        public async Task<oSP_UPDATE_ESTADO_EMAIL> SP_UPDATE_ESTADO_EMAIL(iSP_UPDATE_ESTADO_EMAIL input, int ID_RESPONSABLE)
        {

            oSP_UPDATE_ESTADO_EMAIL output = new oSP_UPDATE_ESTADO_EMAIL();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_UPDATE_ESTADO_EMAIL", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_EMAIL", SqlDbType.Int).Value = input.ID_EMAIL_ESTADO_EMAIL;
                        cmd.Parameters.Add("@ID_ESTADO_EMAIL", SqlDbType.Int).Value = input.ID_ESTADO_EMAIL;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;


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
                                output.MENSAJE = "No fue posible Registrar Telefono.";
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







        public async Task<List<oSP_READ_ESTADO_DIRECCION>> SP_READ_ESTADO_DIRECCION()
        {
            List<oSP_READ_ESTADO_DIRECCION> Estados_direccion = new List<oSP_READ_ESTADO_DIRECCION>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_ESTADO_DIRECCION", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_ESTADO_DIRECCION output = new oSP_READ_ESTADO_DIRECCION();

                                output.ID_ESTADO_DIRECCION = Convert.ToInt32(dr["ID_ESTADO_DIRECCION"]?.ToString());
                                output.ESTADO_DIRECCION = dr["ESTADO_DIRECCION"]?.ToString();


                                Estados_direccion.Add(output);
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
            return Estados_direccion;
        }


        public async Task<oSP_UPDATE_ESTADO_DIRECCION> SP_UPDATE_ESTADO_DIRECCION(iSP_UPDATE_ESTADO_DIRECCION input, int ID_RESPONSABLE)
        {

            oSP_UPDATE_ESTADO_DIRECCION output = new oSP_UPDATE_ESTADO_DIRECCION();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_UPDATE_ESTADO_DIRECCION", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_DIRECCION", SqlDbType.Int).Value = input.ID_DIRECCION_ESTADO_DIRECCION;
                        cmd.Parameters.Add("@ID_ESTADO_DIRECCION", SqlDbType.Int).Value = input.ID_ESTADO_DIRECCION;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;


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
                                output.MENSAJE = "No fue posible Registrar Telefono.";
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






        public async Task<List<oSP_READ_ESTADO_TELEFONO>> SP_READ_ESTADO_TELEFONO()
        {
            List<oSP_READ_ESTADO_TELEFONO> Estados_Telefono = new List<oSP_READ_ESTADO_TELEFONO>();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_READ_ESTADO_TELEFONO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                oSP_READ_ESTADO_TELEFONO output = new oSP_READ_ESTADO_TELEFONO();

                                output.ID_ESTADO_TELEFONO= Convert.ToInt32(dr["ID_ESTADO_TELEFONO"]?.ToString());
                                output.ESTADO_TELEFONO = dr["ESTADO_TELEFONO"]?.ToString();


                                Estados_Telefono.Add(output);
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
            return Estados_Telefono;
        }



        public async Task<oSP_UPDATE_ESTADO_TELEFONO> SP_UPDATE_ESTADO_TELEFONO(iSP_UPDATE_ESTADO_TELEFONO input, int ID_RESPONSABLE)
        {

            oSP_UPDATE_ESTADO_TELEFONO output = new oSP_UPDATE_ESTADO_TELEFONO();


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {

                    using (SqlCommand cmd = new SqlCommand("PRE.SP_UPDATE_ESTADO_TELEFONO", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID_TELEFONO", SqlDbType.Int).Value = input.ID_TELEFONO_ESTADO_TELEFONO;
                        cmd.Parameters.Add("@ID_ESTADO_TELEFONO", SqlDbType.Int).Value = input.ID_ESTADO_TELEFONO;
                        cmd.Parameters.Add("@ID_RESPONSABLE", SqlDbType.Int).Value = ID_RESPONSABLE;


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
                                output.MENSAJE = "No fue posible Registrar Telefono.";
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



    }
}
