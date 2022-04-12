using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Message;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.DTO;
using Server.NetworkModule.Interface;
using System;
using System.Net.Sockets;
using System.Text.Json;

namespace Server.NetworkModule.ConnService.Connect
{
    /// <summary>
    /// 简单连接，仅发送
    /// 此链接在队列/链表中排队等待接入，如果仅运行服务器向客户端发送消息
    /// 此链接满足条件可转换为正式链接
    /// </summary>
    class ConnSimple
    {
        private Socket _socket;

        public bool ConnState { get => _socket != null; }
        public Socket GetSocket() { return _socket; }

        public ConnSimple(Socket socket)
        {
            _socket = socket;
        }

        public void SendWaitIndex(int index)
        {
            if (_socket == null)
            {
                return;
            }

            ConnectWaitMsg waitMsg = new ConnectWaitMsg(index, "请排队等待");
            byte[] waitMsgBytes = JsonSerializer.SerializeToUtf8Bytes(waitMsg);
            MsgPack msgPack=MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_CONNECT_WAIT, waitMsgBytes);
            byte[] msg = new byte[msgPack.MsgPackSize];
            msgPack.OutputMsgPackToSpan(msg);

            SendMsg(msg);
        }
        private void SendMsg(byte[] msg)
        {
            try
            {
                _socket.Send(msg);
            }
            catch (Exception ex)
            {
                DisConnect();
            }
        }

        private void SendMsg(ReadOnlySpan<byte> msg)
        {
            try
            {
                _socket.Send(msg);
            }
            catch (Exception ex)
            {
                DisConnect();
            }
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
    }
}
