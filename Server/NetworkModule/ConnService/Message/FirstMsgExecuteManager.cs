using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.DTO;
using Server.NetworkModule.Interface;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;

namespace Server.NetworkModule.ConnService.Message
{
    class FirstMsgExecuteManager
    {
        static readonly Dictionary<int, Func<Socket, MsgBody, IConnPoolInternal, bool>> executeDictionary 
            = new Dictionary<int, Func<Socket, MsgBody, IConnPoolInternal, bool>>() {
                { MsgTypeConfig.MSG_TYPE_CONNECT_DEFAULT,ExecuteConnectDefault},
                { MsgTypeConfig.MSG_TYPE_CONNECT_TOKEN , ExecuteConnectToken}
            };

        /// <summary>
        /// 首次接收消息执行
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msgBody"></param>
        /// <param name="connPool"></param>
        /// <returns></returns>
        public static bool ExecuteMsg(Socket socket,MsgBody msgBody, IConnPoolInternal connPool)
        {
            if (executeDictionary.ContainsKey(msgBody.MsgType))
            {
                return executeDictionary[msgBody.MsgType](socket, msgBody, connPool);
            }
            //不在字典中的消息号进行错误处理
            else
            {
                //ConnectFailMsg failMsg = new ConnectFailMsg("消息类别错误");
                //byte[] failByte = JsonSerializer.SerializeToUtf8Bytes(failMsg);
                return false;
            }
        }

        private static bool ExecuteConnectDefault(Socket socket, MsgBody msgBody, IConnPoolInternal connPool)
        {
            return TryDefaultConnect(socket, msgBody, connPool);
        }

        private static bool ExecuteConnectToken(Socket socket,MsgBody msgBody, IConnPoolInternal connPool)
        {
            return TryTokenConnect(socket, msgBody, connPool);
        }

        private static bool TryTokenConnect(Socket socket, MsgBody msgBody, IConnPoolInternal connPool)
        {
            if (msgBody == null || msgBody?.MsgData == null)
            {
                return TryDefaultConnect(socket, null,connPool);
            }

            ConnectTokenMsg tokenMsg = JsonSerializer.Deserialize<ConnectTokenMsg>(msgBody.MsgData);
            IConn conn = connPool.TryTokenConnect(socket, tokenMsg);
            if (conn != null)
            {
                ReconnectSuccessMsg successMsg = new ReconnectSuccessMsg("已成功完成断线重连");
                byte[] successByte = JsonSerializer.SerializeToUtf8Bytes(successMsg);
                conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_CONNECT_SUCCESS, successByte));
                return true;
            }

            return TryDefaultConnect(socket, null, connPool);
        }

        private static bool TryDefaultConnect(Socket socket, MsgBody msgBody, IConnPoolInternal connPool)
        {
            //ConnectDefaultMsg defaultMsg ;
            //if (msgBody == null || msgBody?.MsgData == null)
            //{
            //    defaultMsg = new ConnectDefaultMsg("切换为正式连接");
            //}
            //else
            //{
            //    defaultMsg= JsonSerializer.Deserialize<ConnectDefaultMsg>(msgBody.MsgData);
            //}

            IConn conn = connPool.TryDefaultConnect(socket);
            if(conn != null)
            {
                ConnectSuccessMsg successMsg = new ConnectSuccessMsg("已正常连接");
                byte[] successByte = JsonSerializer.SerializeToUtf8Bytes(successMsg);
                conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_CONNECT_SUCCESS, successByte));
                return true;
            }

            return SimpleConnect(socket, connPool);
        }

        private static bool SimpleConnect(Socket socket, IConnPoolInternal connPool)
        {

            return connPool.SimpleConnect(socket);
        }

        public static void TransformDefaultConn(IConn conn)
        {
            ConnectSuccessMsg successMsg = new ConnectSuccessMsg("已正常连接");
            byte[] successByte = JsonSerializer.SerializeToUtf8Bytes(successMsg);
            conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_CONNECT_SUCCESS, successByte));
        }
    }
}
