using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using TGS_PAGE.Utils;

namespace WebApplication1.Utils
{
    public  class OracleHelper :IDBHelper
    {
        public string cstr { get; set; }
        
        public  DataTable Query(string sql, string[] key, object[] value)
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

        public  DataTable Query(string sql)
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

        public  void  Execute(string sql)
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