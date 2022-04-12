using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class ConnectSuccessMsg
    {
        public string Msg { get; set; }

        public ConnectSuccessMsg(string msg)
        {
            Msg = msg;
        }
    }
}
