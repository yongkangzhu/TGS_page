using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Utils
{
    public  class DBHelper
    {

        private static string cstr { get; set; }
        
        static DBHelper()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false).Build();
            cstr = config.GetSection("appconfig").GetSection("connstr").Value;
            //cstr = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST={ConfigurationManager.AppSettings["dbip"]})(PORT=1521))(CONNECT_DATA=(SID = MESTST500)));User Id=sajet;Password=tech;";
            //cstr = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=10.132.49.7)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME = mesdb)));User Id=sajet;Password=tech;";
        }

        public static DataTable Query(string sql, string[] key, object[] value)
        {
            DataTable data = new DataTable();
            using (OracleConnection conn = new OracleConnection(cstr))
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                for (int i = 0; i < key.Length; i++)
                {
                    //cmd.Parameters.Add(key[i], value[i]);
                    cmd.Parameters.Add(new OracleParameter(key[i], value[i]));
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                OracleDataReader dr = cmd.ExecuteReader();

                data.Load(dr);
                conn.Close();
            }
            return data;
        }

        public static DataTable Query(string sql)
        {
            DataTable data = new DataTable();
                                 
            using (OracleConnection conn = new OracleConnection(cstr))
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                OracleDataReader dr = cmd.ExecuteReader();

                data.Load(dr);
            }
                               
            return data;
        }

        public static void  Execute(string sql)
        {           
            using (OracleConnection conn = new OracleConnection(cstr))
            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.ExecuteNonQuery();
            }
            
                      
        }

    }
}