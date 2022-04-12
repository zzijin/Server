using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.ConnService.StatisticsInfo;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.NetworkModule.Interface
{
    interface IConn
    {
        public bool GetConnConnectState();
        public bool GetConnUseState();
        public void Init(int index, byte[] buffer, int bufferStartIndex);
        public void Connect(Socket socket, LinkedListNode<int> connNode);
        public void Disconnect(string msg);
        public void EnqueueToWaitExecuteMsgQueue(MsgBody msgBody);
        public bool DetermineReceiveInterval(int interval);
        public bool DetermineSendInterval(int interval);
        public bool DetermineWaitReconnectInterval(int interval);
        public void EnqueueToWaitSendMsgQueue(MsgPack msgPack);
        public bool GenerateToken(byte[] clientID, out byte[] openID, out byte[] key);
        public bool AuthenticationToken(byte[] openID, byte[] clientKey,Socket socket);
        public void DeleteToken();
        public void Reconnect(Socket socket);
        public ConnInfo GetConnInfo();
    }
}
