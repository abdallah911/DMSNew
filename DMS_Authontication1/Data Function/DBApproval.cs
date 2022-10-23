using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
namespace DMS_Authontication1.Data_Function
{/*196.221.203.129*/
    /*171.0.1.96*/
    public class DBApproval
    {
         public static string connectionStr = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)
                                            (HOST = 196.221.203.129)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDECATED)
                                            (SERVICE_NAME=ora11g)));User Id=app;Password=12369";

       
        //connection
        OracleConnection conn = new OracleConnection(connectionStr);
        //queries
        public OracleCommand cmd = new OracleCommand();

        public void SetCommand(string SQLStatement)
        {

            cmd.Connection = conn;
            cmd.CommandText = SQLStatement;
        }
        //insert or update or delete
        public bool RunNonQuery(string SQLStatement)
        {
            bool test = false;
            int x = 0;
            try
            {
                SetCommand(SQLStatement);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Open();
                }
                else if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                conn.Close();
                conn.Open();

                x = cmd.ExecuteNonQuery();
                if (x != 0)
                {
                    test = true;
                }
                return test;
            }
            //catch (Exception)
            //{
            //    conn.Close();
            //    return test;
            //}
            finally

            {
                conn.Close();
            }
        }

        //return data of one table
        public DataTable RunReader(string Selectstatement)
        {
            SetCommand(Selectstatement);
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;
        }

        public bool Founded(string Selectstatement)
        {
            conn.Open();
            SetCommand(Selectstatement);
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                return true;
            }
            return false;
        }

    }
}