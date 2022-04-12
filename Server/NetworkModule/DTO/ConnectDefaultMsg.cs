using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class ConnectDefaultMsg
    {
        public string Msg { get; set; }

        public ConnectDefaultMsg(string msg)
        {
            Msg = msg;
        }
    }
}
