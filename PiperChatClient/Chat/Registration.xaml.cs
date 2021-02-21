using Chat.PiperChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chat
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }
        private void SignUp(object sender, RoutedEventArgs e)
        {
            User user = new User();
            user.Name = TName .Text;
            user.Email = TEmail.Text;
            user.ContactNo = TContact.Text;
            user.About = TAbout.Text;
            user.Password = TPassword.Password;
            user.Location = TLocation.Text;

            UserServiceClient client = new UserServiceClient();
            string response = client.InsertUserRecord(user);
            Console.WriteLine(response);
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

       
    }
}


