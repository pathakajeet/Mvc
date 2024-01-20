using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 15000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            Console.ReadKey();
        }
        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           
            try
            {
                SqlConnection con = new SqlConnection();
                
                con = new SqlConnection(Properties.Settings.Default.ConStr);                
                SqlCommand cmd = new SqlCommand("validate_employee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", "test");
                cmd.Parameters.AddWithValue("@Password", "test");
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
