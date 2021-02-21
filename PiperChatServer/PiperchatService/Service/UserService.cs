using PiperchatService.Contract;
using PiperchatService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PiperchatService.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class UserService : IUserService
    {
       // Data Source = (localdb)\MSSQLLocalDB;Initial Catalog = master; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

        string connectionString = @"Data Source = (localdb)\MSSQLLocalDB;Initial Catalog = PiperChatDb; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string InsertUserRecord(User user)
        {
            string response = "";
            try
            {

                string query = "INSERT INTO dbo.UserTable(Name,Email,Password) values(@Name, @Email, @Password)";
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(query,con);
                //cmd.Parameters.AddWithValue("@UserId", user.UserId);
                Console.WriteLine(user.Name);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
               
                cmd.ExecuteNonQuery();
                con.Close();
                response = "Inserted Successfully";

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                response = "Error " + e;
            }
            return response;
        }

        public string UpdateUserRecord(User user)
        {
            string result = "";
            string Query = "UPDATE User SET Name=@Name,Email=@Email,Password=@Password WHERE UserId=@UserId";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand(Query, con);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            result = "Updated Successfully !";
            return result;
        }

        public string DeleteUserRecord(User user)
        {
            string Query = "DELETE FROM User Where UserId=@UserId";
            string result = "";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand(Query, con);
            cmd.Parameters.AddWithValue("@UserID", user.UserId);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            result = "Deleted Successfully!";
            return result;
        }

        public DataSet GetAllUserRecords()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                string Query = "SELECT * FROM User";

                SqlDataAdapter sda = new SqlDataAdapter(Query, con);
                sda.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex);
            }

            return ds;
        }

        public DataSet SearchUserRecord(User user)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                string Query = "SELECT * FROM User WHERE UserId=@UserId";

                SqlDataAdapter sda = new SqlDataAdapter(Query, con);
                sda.SelectCommand.Parameters.AddWithValue("@UserId", user.UserId);
                sda.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error:  " + ex);
            }
            return ds;
           
        }

        public string Validate(string email, string password)
        {
            string response="";
            try
            {


                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                string Query = "SELECT * from dbo.UserTable where Email='"+email+"' and Password='"+password+"'";
                
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand(Query, con);
                /*cmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                cmd.Parameters["@Email"].Value = email;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar);
                cmd.Parameters["@Password"].Value = password;*/
                //cmd.Parameters.AddWithValue("@Password", password);
                //cmd.Parameters.AddWithValue("@Email", email);
                SqlDataAdapter sda = new SqlDataAdapter(Query, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                int i = cmd.ExecuteNonQuery();
             
                if (dt.Rows.Count > 0)
                {
                    response = "Login Successfull";
                }
                else
                {
                    response = "Login Failed";
                }
                con.Close();
            }
            catch(Exception e)
            {
                
                response = "Error" + e;
            }
            Console.WriteLine(response);
            return response;
            
        }

        User IUserService.Validate(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
