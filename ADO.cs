using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace OpenTableApp
{

    class ADO
    {
        //declaration of sql objects
        public SqlConnection con = new SqlConnection();
        public SqlCommand cmd = new SqlCommand();
        public SqlCommand cmd1 = new SqlCommand();
        public SqlDataReader? dr;
        public SqlDataReader dr2;
        public DataTable dt = new DataTable();

        //delcation of the connect method
        public void CONNECT()
        {
            if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
            {
                con.ConnectionString = "Data Source=DESKTOP-F6ODC3B;Initial Catalog=opentable;Integrated Security=True";
                con.Open();
            }
        }
        //declation of the desconnect method
        public void DECONNECT()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}
