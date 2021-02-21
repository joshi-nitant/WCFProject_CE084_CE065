using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PiperchatService.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string About { get; set; }

        [DataMember]
        public string ContactNo { get; set; }

        [DataMember]
        public RSAParameters PublicKey { get; set; }

    }
}
