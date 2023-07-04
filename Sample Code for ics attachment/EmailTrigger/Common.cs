using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;

namespace EmailTrigger
{
    public static class Common
    {

        private static string Decrypt(string value)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(value));
        }

        public static string GetConnectionString(string connectionStringName)
        {
            return Decrypt(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }

        public static void GetExpiryDates()
        {

            string con = GetConnectionString("Development");

            string storedProcedure = "doap_get_pwd_expiry_date";

            using (SqlConnection connection = new SqlConnection(con))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(storedProcedure, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    Program.CreateCSV(dt);
                }
            }
        }

    
        }
    }

