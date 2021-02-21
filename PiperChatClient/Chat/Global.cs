using Chat.PiperChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public static class Global
    {
        private static User _currentLoggedInUser;

        static Global()
        {
            
        }
        public static RsaAlgorithm rsaAlgorithm { get; set; }
        public static User CurrentLoggedInUser
        {
            get
            {
                return (_currentLoggedInUser);
            }
            set
            {
                _currentLoggedInUser = value;
            }
        }
    }
}
