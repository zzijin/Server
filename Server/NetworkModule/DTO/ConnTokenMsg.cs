using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class ConnTokenMsg
    {
        public byte[] ClientID { get; set; }
        public byte[] OpenID { get; set; }
        public byte[] Key { get; set; }
        public string Msg { get; set; }

        public ConnTokenMsg(byte[] clientID,byte[] openID,byte[] key,string msg)
        {
            ClientID = clientID;
            OpenID = openID;
            Key = key;
            Msg = msg;
        }
    }
}
