using PiperchatService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PiperchatService.Contract
{
    [ServiceContract(CallbackContract =typeof(IMessageServiceCallback),SessionMode = SessionMode.Required)]
    public interface IMessageService
    {
        [OperationContract(IsOneWay = true)]
        void Connect(User user);

        [OperationContract(IsOneWay = true)]
        void SendMessage(Message message);

        [OperationContract(IsOneWay = false)]
        ObservableCollection<User> GetConnectedUsers();

        [OperationContract]
        void Disconnect(User user);


    }
}
