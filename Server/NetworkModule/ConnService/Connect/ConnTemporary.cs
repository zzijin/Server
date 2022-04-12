using Microsoft.Extensions.DependencyInjection;
using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Message;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.Interface;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.NetworkModule.ConnService.Connect
{
    /// <summary>
    /// 临时链接，仅接收且只接收一条消息
    /// 此类会自动尝试在链接池中寻找对应的连接方式
    /// 服务器监听到新连接后使用此封装，然后接收首次&一次性的消息，该消息将确认新连接的连接方式，如果消息不是确认连接方式的话会立即断开
    /// 连接方式如果是断线重连则会在连接池中寻找相应链接并尝试连接，否则进入默认连接流程
    /// 连接方式如果是默认则会检查连接池中链接数，如果数量足够则尝试转换为正式链接，否则转换为简单链接
    /// </summary>
    class ConnTemporary
    {
        Socket _socket;
        IConnPoolInternal _connPool;

        public ConnTemporary(Socket socket, IServiceProvider provider)
        {
            _socket = socket;
            _connPool = provider.GetService<IConnPoolExternal>() as IConnPoolInternal;

            //设置接收超时，该设置仅对同步有效，不影响异步
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, ConnConfig.TEMPORARY_MAX_WAIT_RECEIVE_TIME);
            //立即开始一次性的接收操作
            ReceiveMessage();
        }

        private void DisConnect()
        {
            if (_socket == null)
            {
                return;
            }

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket = null;
        }

        private void ReceiveMessage()
        {
            Task.Run(()=>{
                byte[] receiveByte= new byte[MsgConfig.MSG_MAX_SIZE];
                try
                {
                    _socket.Receive(receiveByte);
                }
                catch(Exception ex)
                {
                    DisConnect();
                    return;
                }
                MsgBody msgBody = MsgParse(receiveByte);
                MsgExecute(msgBody);
            });
        }

        private MsgBody MsgParse(byte[] msg)
        {
            ConnMsgParse connMsgParse = new ConnMsgParse();
            return connMsgParse.ParseMsg(msg);
        }

        private void MsgExecute(MsgBody msgBody)
        {
            if (!FirstMsgExecuteManager.ExecuteMsg(_socket, msgBody, _connPool))
            {
                DisConnect();
            }
        }
    }
}
