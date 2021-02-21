using Chat.PiperChat;
using Chat.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, PiperChat.IMessageServiceCallback
    {

        Dictionary<int, List<MessageViewModel>> chatList;
        Dictionary<int, int> currentMessageCount;
        InstanceContext instanceContext;
        MessageServiceClient client;
        User currentSelected;

        public MainWindow()
        {
            instanceContext = new InstanceContext(this);
            client = new MessageServiceClient(instanceContext);
            chatList = new Dictionary<int, List<MessageViewModel>>();
            currentMessageCount = new Dictionary<int, int>();

            Global.CurrentLoggedInUser.PublicKey = Global.rsaAlgorithm.PublicKey;
            client.Connect(Global.CurrentLoggedInUser);
            InitializeComponent();
            emptyChat();
        }

        private void emptyChat()
        {
            MessagePanel.Children.Clear();
            TAbout.Text = Global.CurrentLoggedInUser.About;
            TLocation.Text = Global.CurrentLoggedInUser.Location;
            TContact.Text = Global.CurrentLoggedInUser.ContactNo;
            TUserName.Text = Global.CurrentLoggedInUser.Name;
            TEmail.Text = Global.CurrentLoggedInUser.Email;
            TCurrentChatName.Text = "PiperChat";
        }

        private void EstablishConnection(object sender, RoutedEventArgs e)
        {

        }

        void incrementMessageCount(int id)
        {
            if (currentMessageCount.ContainsKey(id))
            {
                int count = currentMessageCount[id];
                count++;
                currentMessageCount[id] = count;
            }
            else
            {
                currentMessageCount.Add(id, 1);
            }

        }



        public void ForwardToClient(Message message)
        {
            MessageViewModel messageViewModel = new MessageViewModel();
            messageViewModel.message = message;
            messageViewModel.plainText = Global.rsaAlgorithm.Decrypt(message.MessageSent);

            if (chatList.ContainsKey(message.SenderId))
            {
                List<MessageViewModel> messageList = chatList[message.SenderId];
                messageList.Add(messageViewModel);
                chatList[message.SenderId] = messageList;
            }
            else
            {
                List<MessageViewModel> messageList = new List<MessageViewModel>();
                messageList.Add(messageViewModel);
                chatList.Add(message.SenderId, messageList);
            }
            incrementMessageCount(message.SenderId);

            if (currentSelected != null && message.SenderId == currentSelected.UserId)
            {
                RenderMessageGrid(message.SenderId);
            }

            //MessagePanel.Children.Add(new UserControlMessageReceived(message));
        }

        public void UsersConnected(User[] users)
        {
            User receiver = (User)(ActiveUserList.SelectedItem);
            List<User> userList = users.Where(user => user.UserId != Global.CurrentLoggedInUser.UserId).ToList<User>();
            if (userList.Count == 0)
            {
                emptyChat();
            }else if(receiver!=null && users.FirstOrDefault(user => user.UserId == receiver.UserId) == null)
            {
                emptyChat();
            }
            ActiveUserList.ItemsSource = userList;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            User receiver = (User)(ActiveUserList.SelectedItem);
            Message message = new Message();
            message.MessageSent = Global.rsaAlgorithm.Encrypt(TxtMessage.Text, receiver.PublicKey);
            message.SenderId = Global.CurrentLoggedInUser.UserId;
            message.ReceiverId = receiver.UserId;
            message.TimeSent = DateTime.Now;
            client.SendMessage(message);

            MessageViewModel messageViewModel = new MessageViewModel();
            messageViewModel.message = message;
            messageViewModel.plainText = TxtMessage.Text;

            if (chatList.ContainsKey(message.ReceiverId))
            {
                List<MessageViewModel> messageList = chatList[message.ReceiverId];
                messageList.Add(messageViewModel);
                chatList[message.ReceiverId] = messageList;
            }
            else
            {
                List<MessageViewModel> messageList = new List<MessageViewModel>();
                messageList.Add(messageViewModel);
                chatList.Add(message.ReceiverId, messageList);
            }

            RenderMessageGrid(message.ReceiverId);
            //MessagePanel.Children.Add(new UserControlMessageSent(message));
            TxtMessage.Text = "";
        }

        void RenderMessageGrid(int id)
        {
            MessagePanel.Children.Clear();
            List<MessageViewModel> messageOfUser = chatList.First(kvp => kvp.Key == id).Value;
            foreach (MessageViewModel messageViewModel in messageOfUser)
            {
                if (messageViewModel.message.SenderId == Global.CurrentLoggedInUser.UserId)
                {
                    MessagePanel.Children.Add(new UserControlMessageSent(messageViewModel));
                }
                else
                {
                    MessagePanel.Children.Add(new UserControlMessageReceived(messageViewModel));
                }
            }
        }

        private void ActiveUserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User user = (User)ActiveUserList.SelectedItem;
            if (user != null)
            {
                currentSelected = user;
                TCurrentChatName.Text = user.Name;
                TAbout.Text = user.About;
                TLocation.Text = user.Location;
                TContact.Text = user.ContactNo;
                TUserName.Text = user.Name;
                TEmail.Text = user.Email;

                currentMessageCount[user.UserId] = 0;

            }
            if (user != null && chatList.ContainsKey(user.UserId))
            {
                RenderMessageGrid(user.UserId);
            }
            else
            {
                MessagePanel.Children.Clear();
            }
        }


        private void Disconnect(object sender, RoutedEventArgs e)
        {
            client.Disconnect(Global.CurrentLoggedInUser);
            this.Close();
        }
    }
}
