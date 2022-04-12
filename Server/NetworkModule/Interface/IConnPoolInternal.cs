using Server.NetworkModule.DTO;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.NetworkModule.Interface
{
    /// <summary>
    /// 线程池对内（模块内）方法
    /// </summary>
    interface IConnPoolInternal
    {
        public IConn TryDefaultConnect(Socket socket);
        public IConn TryTokenConnect(Socket socket, ConnectTokenMsg tokenMsg);
        public bool SimpleConnect(Socket socket);
        public ReadOnlySpan<byte> GetPublicFixedSendBuffer(int type);
        public void UseConnDisconnect(LinkedListNode<int> connNode);
    }
}
