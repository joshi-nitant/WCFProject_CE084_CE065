using PiperchatService.Models;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace PiperchatService.Contract
{
    [ServiceContract()]
    public interface IMessageServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ForwardToClient(Message message);

        [OperationContract(IsOneWay = true)]
        void UsersConnected(ObservableCollection<User> users);
    }
}
