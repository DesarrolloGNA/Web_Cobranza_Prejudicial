using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Web_Cobranza_Prejudicial.Models
{
    public class Helpers : IDisposable
    {


        /// <summary>
        /// DESTRUCTOR
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public string DIRECCION_IP()
        {
            string IP = "";

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());// objeto para guardar la ip
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    IP = ip.ToString();
                }
            }
            return IP;
        }

        /// <summary>
        /// LISTA TO DATASET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IList<T> list)
        {

            try
            {

                Type elementType = typeof(T);
                DataSet ds = new DataSet();
                DataTable t = new DataTable();
                ds.Tables.Add(t);


                //add a column to table for each public property on T
                foreach (var propInfo in elementType.GetProperties())
                {
                    Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                    t.Columns.Add(propInfo.Name, ColType);
                }

                //go through each property on T and add each value to the table
                foreach (T item in list)
                {
                    DataRow row = t.NewRow();

                    foreach (var propInfo in elementType.GetProperties())
                    {
                        row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                    }

                    t.Rows.Add(row);
                }

                return t;
            }
            catch
            {
                throw;
            }
        }



        public string Limpiar_Nombre_Archivo(string input)
        {
            StringBuilder resultado = new StringBuilder();

            foreach (char c in input)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '_') || (c == '.'))
                {
                    resultado.Append(c);
                }
                else if (c >= 'A' && c <= 'Z') // Convertimos mayúsculas a minúsculas
                {
                    resultado.Append(char.ToLower(c));
                }
            }

            return resultado.ToString();
        }
        public static string MD5(string word)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(word));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        public static string Token()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray()) i *= ((int)b + 1);
            return MD5(string.Format("{0:x}", i - DateTime.Now.Ticks));
        }

        public string Base64Encode(string word)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(word);
            return Convert.ToBase64String(byt);
        }
        public string Base64Decode(string word)
        {
            byte[] b = Convert.FromBase64String(word);
            return System.Text.Encoding.UTF8.GetString(b);
        }
        public static string SHA1(string str)
        {
            SHA1 sha1 = SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }


        /// <summary>
        ///  Encriptar un parametro string a SHA256
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Retorna parametro string encriptado</returns>
        public string SHA256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public static string SHA384(string str)
        {
            SHA384 sha384 = SHA384Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha384.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        public static string SHA512(string str)
        {
            SHA512 sha512 = SHA512Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha512.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }




    }
}
