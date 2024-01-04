using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.Data;
using System.Data.SqlClient;


namespace SEWTVDisplay
{

    public class ConnectToSQL : Activity
    {
        public DataTable dt = new DataTable();
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand _cmd;
        private string StrCon = null;
        private string _error;
        SqlCommand cmd = new SqlCommand();
        public static string IpSQL = "";
        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public SqlConnection Connection
        {
            get { return Conn; }
        }

        public SqlCommand CMD
        {
            get { return _cmd; }
            set { _cmd = value; }
        }
        public ConnectToSQL()
        {
            ISharedPreferences pre = GetSharedPreferences("server", FileCreationMode.Private);
            string ch = pre.GetString("chuoi", "").ToString();
            if (ch != "")
            {
                StrCon = ch;
                CSDL.chuoi = ch;
            }
            else StrCon = "Data Source=192.168.50.253;Initial Catalog=DtradeProduction;Persist Security Info=True;User ID=sa;Password=Sql4116!";
            Conn = new SqlConnection(StrCon);
        }

        public bool OpenConn()
        {
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    //Toast.MakeText(this,  "dd" + ValueStore.ip, ToastLength.Long).Show();
                    // Conn = new SqlConnection(ValueStore.ipsql);
                    Conn.Open();
                }
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                return false;
            }
            return true;
        }

        public bool CloseConn()
        {
            try
            {
                if (Conn.State == ConnectionState.Open)
                    //  Conn = new SqlConnection(ValueStore.ipsql);
                    Conn.Close();
            }
            catch (Exception ex)
            {
                // ValueStore.msg = "Server down !";
                _error = ex.Message;
                return false;
            }
            return true;
        }


        public DataTable GetData(string st)
        {
            DataTable dt = new DataTable();
            cmd.CommandText = st;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            try
            {
                OpenConn();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                string mex = ex.Message;
                cmd.Dispose();

                CloseConn();
            }
            return dt;
        }


        public void InsertItem(string st)
        {
            try
            {
                // connectionString = Resources.GetText(Resource.String.conn);
                SqlCommand cmd = new SqlCommand();
                //  SqlConnection conn = new SqlConnection(connectionString);
                cmd.CommandText = st;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = Connection;
                OpenConn();

                cmd.ExecuteNonQuery();
                Toast.MakeText(this, "Insert Ok !!", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                string mex = ex.Message;
                cmd.Dispose();
                CloseConn();
            }
        }

    }

}