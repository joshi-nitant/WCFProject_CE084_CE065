using PiperchatService.Contract;
using PiperchatService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PiperchatService.Service
{
    [ServiceBehavior(ConcurrencyMode =ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class MessageService : IMessageService, IUserService
    {
        private IMessageServiceCallback _callback = null;
        private ObservableCollection<User> _activeUsers;
        private Dictionary<int, IMessageServiceCallback> _clients;

        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PiperChatDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public MessageService()
        {
            _activeUsers = new ObservableCollection<User>();
            _clients = new Dictionary<int, IMessageServiceCallback>();
        }

        public void Connect(User user)
        {
            _callback = OperationContext.Current.GetCallbackChannel<IMessageServiceCallback>();

            if(_callback != null)
            {
                _clients.Add(user.UserId, _callback);
                _activeUsers.Add(user);
                _clients?.ToList().ForEach(client => client.Value.UsersConnected(_activeUsers));
            }
           
        }

        public void SendMessage(Message message)
        {
            InsertMessage(message);
            IMessageServiceCallback receiverCallBack = _clients?.First(client => client.Key == message.ReceiverId).Value;
            receiverCallBack?.ForwardToClient(message);
        }

        public ObservableCollection<User> GetConnectedUsers()
        {
            return _activeUsers;
        }

        //User services

       
        public string InsertUserRecord(User user)
        {
            string response = "";
            try
            {

                string query = "INSERT INTO dbo.UserTable(Name,Email,Password,About,ContactNo,Location) values(@Name, @Email, @Password, @About, @ContactNo, @Location)";
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                //cmd.Parameters.AddWithValue("@UserId", user.UserId);
                Console.WriteLine(user.Name);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@About", user.About);
                cmd.Parameters.AddWithValue("@ContactNo", user.ContactNo);
                cmd.Parameters.AddWithValue("@Location", user.Location);
                cmd.ExecuteNonQuery();
                con.Close();
                response = "Inserted Successfully";

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                response = "Error " + e;
            }
            return response;
        }

        private void InsertMessage(Message message)
        {
            string query = "INSERT INTO dbo.Message(SenderId,ReceiverId,message,messgaeTime) values(@SenderId,@ReceiverId,@message,@messageTime)";
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            //cmd.Parameters.AddWithValue("@UserId", user.UserId);
            cmd.Parameters.AddWithValue("@SenderId",message.SenderId);
            cmd.Parameters.AddWithValue("@ReceiverId", message.ReceiverId);
            cmd.Parameters.AddWithValue("@message", message.MessageSent);
            cmd.Parameters.AddWithValue("@messageTime", message.TimeSent);

            cmd.ExecuteNonQuery();
            con.Close();
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

        public User Validate(string email, string password)
        {
            User user = null;
            try
            {


                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                string Query = "SELECT * from dbo.UserTable where Email='" + email + "' and Password='" + password + "'";

                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand(Query, con);
                SqlDataAdapter sda = new SqlDataAdapter(Query, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                int i = cmd.ExecuteNonQuery();

                foreach (DataRow row in dt.Rows)
                {
                    user = new User();
                    user.UserId = (int)row["UserId"];
                    user.Name = (string)row["Name"];
                    user.Email = (string)row["Email"];
                    user.Password = (string)row["Password"];
                    user.ContactNo = (string)row["ContactNo"];
                    user.About = (string)row["About"];
                    user.Location = (string)row["Location"];
                }

                con.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
               
            }
    
            return user;
        }

        public void Disconnect(User user)
        {
            _callback = OperationContext.Current.GetCallbackChannel<IMessageServiceCallback>();

            if (_callback != null)
            {
                _clients.Remove(user.UserId);
                //_clients.Add(user.UserId, _callback);
                //_activeUsers.Add(user);
                _activeUsers.Remove(_activeUsers.First(us => us.UserId == user.UserId));
                _clients?.ToList().ForEach(client => client.Value.UsersConnected(_activeUsers));
            }
        }
    }
}
