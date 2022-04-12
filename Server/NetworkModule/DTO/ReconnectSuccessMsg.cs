using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class ReconnectSuccessMsg
    {
        public string Msg { get; set; }
        public ReconnectSuccessMsg(string msg)
        {
            Msg = msg;
        }
    }
}
