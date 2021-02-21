using PiperchatService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace PiperchatService.Contract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {
       
        [OperationContract]
        string InsertUserRecord(User user);

        [OperationContract]
        string UpdateUserRecord(User user);

        [OperationContract]
        string DeleteUserRecord(User user);

        [OperationContract]
        DataSet SearchUserRecord(User user);


        [OperationContract]
        DataSet GetAllUserRecords();

        [OperationContract]
        User Validate(string email, string password);
    }
}
