using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class ConnectWaitMsg
    {
        public int QueueNumber { get; set; }

        public string Msg { get; set; }

        public ConnectWaitMsg(int queueNumber,string msg)
        {
            QueueNumber = queueNumber;Msg = msg;
        }
    }
}
