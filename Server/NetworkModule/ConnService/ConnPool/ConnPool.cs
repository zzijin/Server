using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Connect;
using Server.NetworkModule.ConnService.Message;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.ConnService.StatisticsInfo;
using Server.NetworkModule.DTO;
using Server.NetworkModule.Interface;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.NetworkModule.ConnService.ConnPool
{
    /// <summary>
    /// 链接池类，负责链接池的有关逻辑处理与对外提供方法
    /// </summary>
    class ConnPool:IConnPoolExternal,IConnPoolInternal
    {
        /// <summary>
        /// 链接池资源管理类
        /// </summary>
        private ConnPoolResourceManager resourceManager;
        /// <summary>
        /// 链接池信息
        /// </summary>
        private ConnPoolInfo poolInfo;

        public ConnPool(IServiceProvider serviceProvider)
        {
            resourceManager = new ConnPoolResourceManager(serviceProvider);
            poolInfo = new ConnPoolInfo();
        }

        public void Init()
        {
            resourceManager.Init();
            poolInfo.Init(resourceManager.GetConnInfoArray());

            StartLoopTask();
        }

        #region 线程池循环线程
        private void StartLoopTask()
        {
            Task.Run(ConnPoolLoopTaskAsync);
        }

        /// <summary>
        /// 链接池循环线程
        /// </summary>
        /// <param name="state"></param>
        private async Task ConnPoolLoopTaskAsync()
        {
            while (true)
            {
                DateTime dateTime = DateTime.Now;

                //检查等待链接队列，若正式链接列表有空闲，则依次加入
                CheackWaitQueue();
                //检查所有正式链接，并发送心跳信息和检查断线重连
                CheckUseList();

                int sleepTime = (ConnPoolConfig.CONNPOOL_LOOP_TASK_MAX_INTERVAL - (int)(DateTime.Now - dateTime).TotalMilliseconds);
                sleepTime = sleepTime > 0 ? sleepTime : 0;
                await Task.Delay(sleepTime);
            }
        }
        #endregion

        #region 建立新链接和断开链接

        /// <summary>
        /// 尝试建立正式连接
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public IConn TryDefaultConnect(Socket socket)
        {
            //若等待队列中存在，则需要排队
            if (resourceManager.WaitConnQueueSize > 0)
            {
                return null;
            }

            int index;
            if (resourceManager.FreedomConnQueueTryDequeue(out index))
            {
                LinkedListNode<int> node=resourceManager.UseConnListAdd(index);
                resourceManager[index].Connect(socket, node);

                return resourceManager[index];
            }

            return null;
        }

        /// <summary>
        /// 尝试断线重连
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="tokenMsg"></param>
        /// <returns></returns>
        public IConn TryTokenConnect(Socket socket, ConnectTokenMsg tokenMsg)
        {
            if (tokenMsg.Index > resourceManager.ConnArraySize||tokenMsg.Index<0)
            {
                return null;
            }

            if (resourceManager[tokenMsg.Index].GetConnConnectState())
            {
                return null;
            }

            if (resourceManager[tokenMsg.Index].AuthenticationToken(tokenMsg.OpenID, tokenMsg.Key, socket))
            {
                resourceManager[tokenMsg.Index].Reconnect(socket);
                return resourceManager[tokenMsg.Index];
            }

            return null;
        }

        /// <summary>
        /// 使用简单链接（排队等待）
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool SimpleConnect(Socket socket)
        {
            int waitConnQueueSize = resourceManager.WaitConnQueueSize;
            if (waitConnQueueSize > ConnPoolConfig.CONNPOOL_WAIT_QUEUE_LENGTH)
            {
                return false;
            }

            ConnSimple connSimple = new ConnSimple(socket);
            resourceManager.WaitConnQueueEnqueue(connSimple);
            connSimple.SendWaitIndex(waitConnQueueSize);

            return true;
        }

        /// <summary>
        /// 断开链接并将此链接加入空闲链接队列
        /// </summary>
        /// <param name="connNode"></param>
        public void UseConnDisconnect(LinkedListNode<int> connNode)
        {
            int index = connNode.Value;
            resourceManager.UseConnListRemove(connNode);
            resourceManager.FreedomConnQueueEnqueue(index);
        }
        #endregion

        #region 检查等待队列
        /// <summary>
        /// 检查等待队列和发送位置
        /// </summary>
        private void CheackWaitQueue()
        {
            while (true)
            {
                //若等待队列为空或空闲正式链接为空则退出
                if (resourceManager.WaitConnQueueSize == 0|| resourceManager.FreedomConnQueueSize == 0)
                {
                    return;
                }

                //尝试取出空闲正式链接
                int index;
                if (!resourceManager.FreedomConnQueueTryDequeue(out index))
                {
                    return;
                }

                //尝试取出等待链接
                ConnSimple connSimple;
                if (resourceManager.WaitConnQueueTryDequeue(out connSimple))
                {
                    //检查链接状态
                    if (connSimple.ConnState)
                    {
                        //将等待链接转换
                        LinkedListNode<int> node = resourceManager.UseConnListAdd(index);
                        resourceManager[index].Connect(connSimple.GetSocket(), node);

                        FirstMsgExecuteManager.TransformDefaultConn(resourceManager[index]);

                        continue;
                    }

                }

                resourceManager.FreedomConnQueueEnqueue(index);
                break;
            }

            //遍历等待链接队列，向其中的所有链接发送消息
            int waitIndex = 0;
            foreach(var item in resourceManager.GetWaitConnArray())
            {
                item.SendWaitIndex(waitIndex);
                waitIndex++;
            }
        }
        #endregion

        #region 检查已使用链接列表
        private void CheckUseList()
        {
            if (resourceManager.UsedConnListSize == 0)
            {
                return;
            }

            foreach (var item in resourceManager.GetUseConnArray())
            {

                CheackReconnect(item);

                SendHeartbeatMsg(item);
            }
        }

        /// <summary>
        /// 检查链接发送接收间隔后向超过间隔链接发送心跳消息
        /// </summary>
        /// <param name="conn"></param>
        private void SendHeartbeatMsg(IConn conn)
        {
            if (!conn.GetConnConnectState())
            {
                return;
            }

            MsgPack heartbeatMsg = MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_HEARTBEAT, null);
            if (conn.DetermineReceiveInterval(ConnConfig.CONN_HEARTBEAT_MSG_MAX_INTERVAL))
            {
                //此处发送心跳消息
                conn.EnqueueToWaitSendMsgQueue(heartbeatMsg);
            }
        }

        /// <summary>
        /// 将所有断开链接且未配置断线重连或断线重连等待超时的链接断开
        /// </summary>
        /// <param name="conn"></param>
        private void CheackReconnect(IConn conn)
        {
            if (conn.GetConnConnectState())
            {
                return;
            }

            if (conn.DetermineWaitReconnectInterval(ConnConfig.CONN_WAIT_RECONNECT_MAX_INTERVAL))
            {
                conn.Disconnect("重连等待重连超时");
            }

        }
        #endregion

        public ReadOnlySpan<byte> GetPublicFixedSendBuffer(int type)
        {
            return resourceManager.GetPublicFixedSendBuffer(type);
        }

        public ConnPoolInfo GetConnPoolInfoArray()
        {
            return poolInfo;
        }
    }
}
