using System;
using System.Runtime.Serialization;

namespace PiperchatService.Models
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public int SenderId { get; set; }
        [DataMember]
        public int ReceiverId { get; set; }
        [DataMember]
        public string MessageSent { get; set; }
        [DataMember]
        public DateTime TimeSent { get; set; }

    }
}