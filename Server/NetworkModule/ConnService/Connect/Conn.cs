using Microsoft.Extensions.DependencyInjection;
using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Message;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.ConnService.StatisticsInfo;
using Server.NetworkModule.Interface;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.NetworkModule.ConnService.Connect
{
    /// <summary>
    /// 链接类，每个客户端与服务器的链接都被封装为一个Conn类
    /// 该类可以取出作为客户端链接
    /// </summary>
    class Conn:IConn
    {
        #region 链接包含的一些类
        private Socket _socket;
        private SocketAsyncEventArgs receiveEventArgs;
        private SocketAsyncEventArgs sendEventArgs;
        /// <summary>
        /// 缓冲区资源管理器
        /// </summary>
        ConnBufferManager connBufferManager;
        /// <summary>
        /// 消息解析
        /// </summary>
        private ConnMsgParse msgParse;
        /// <summary>
        /// 消息队列
        /// </summary>
        private ConnMsgQueue connMsgQueue;
        /// <summary>
        /// 该链接信息
        /// </summary>
        internal ConnInfo connInfo;
        /// <summary>
        /// 本次链接信息
        /// </summary>
        internal ConnToken connToken;
        #endregion

        #region 链接提供的委托方法
        private ConnDelegate connDelegate;
        #endregion

        #region 链接引用的外部接口
        private IConnPoolInternal _connPool;
        #endregion

        public Conn(IServiceProvider serviceProvider)
        {
            connMsgQueue = new ConnMsgQueue();
            connBufferManager = new ConnBufferManager();
            connDelegate = new ConnDelegate(this,serviceProvider);

            //为方法委托赋值
            receiveEventArgs = new SocketAsyncEventArgs();
            sendEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.Completed += Receive_Completed;
            sendEventArgs.Completed += Send_Completed;

            _connPool = serviceProvider.GetService<IConnPoolExternal>() as IConnPoolInternal;
        }

        public void Init(int index, byte[] buffer, int bufferStartIndex)
        {
            connInfo = new ConnInfo(index);
            //初始化缓冲区
            connBufferManager.Init(buffer, bufferStartIndex);
            //初始化消息解析器
            msgParse = new ConnMsgParse(connBufferManager);
        }

        #region 使用该链接和停用该链接
        /// <summary>
        /// 为新连接使用该链接
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="connNode"></param>
        /// <returns></returns>
        public void Connect(Socket socket, LinkedListNode<int> connNode)
        {
            if (_socket != null)
            {
                Disconnect("该链接被再次使用");
            }

            _socket = socket;
            connInfo.Connect(connNode);
            //清空消息队列
            connMsgQueue.Clear();
            //清空缓冲区信息
            connBufferManager.ClearBufferInfo();
            //重新设置缓冲
            receiveEventArgs.SetBuffer(connBufferManager.WriteToReceiveBufferByMemory());
            sendEventArgs.SetBuffer(connBufferManager.ReadFormSendBufferByMemory());

            //设置好该链接后开始异步接收
            StartReceiveAsync();
            StartExecuteMsgQueue();
        }

        public Object DisconnectLock = new Object();
        /// <summary>
        /// 停止使用该链接
        /// </summary>
        public void Disconnect(string msg)
        {
            lock (DisconnectLock)
            {
                if (_socket != null || connInfo.OnceInfoDO.OnceNode != null)
                {
                    connToken = null;

                    //禁用接收和发送
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    //移除socket
                    _socket = null;

                    _connPool.UseConnDisconnect(connInfo.OnceInfoDO.OnceNode);
                    connInfo.DisConnect("停止使用该链接:" + msg);
                }
            }
        }
        #endregion

        #region 断线重连和恢复连接

        /// <summary>
        /// 链接连接状态
        /// </summary>
        public bool GetConnConnectState()
        { 
            return _socket != null;
        }
        /// <summary>
        /// 链接使用状态
        /// 当链接未处于断线重连和连接时，返回false
        /// </summary>
        /// <returns></returns>
        public bool GetConnUseState()
        {
            return !(_socket == null && connToken == null);
        }

        /// <summary>
        /// 恢复连接
        /// </summary>
        public void Reconnect(Socket socket)
        {
            _socket = socket;
            connToken.Reconnect();
            //恢复发送和接收
            StartReceiveAsync();
            StartExecuteMsgQueue();
        }

        /// <summary>
        /// 意外断开连接
        /// </summary>
        private void AccidentalDisConnect(string msg)
        {
            //如果未配置断线重连则断开
            if (connToken == null)
            {
                Disconnect(msg+"未配置断线重连");
                return;
            }

            //如果断开则不管
            if (_socket == null)
            {
                return;
            }

            //返回false表示此客户端网络动荡，则断开
            if (!connToken.AccidentalDisconnect())
            {
                Disconnect("网络动荡，重连次数过多，断开连接");
                return;
            }


            connInfo.AddConnWaitReconnectInfo();
            _socket.Close();
            _socket = null;
        }
        #endregion

        #region 生成令牌、认证令牌和删除令牌
        public bool GenerateToken(byte[] clientID,out byte[] openID,out byte[] key)
        {
            if (connToken != null)
            {
                openID = null;
                key = null;
                return false;
            }

            connToken = new ConnToken();
            connToken.GenerateToken(connInfo.FixedInfoDO.FixedIndex, clientID, out openID, out key);

            return true;
        }
        public bool AuthenticationToken(byte[] openID, byte[] clientKey, Socket socket)
        {
            if (connToken == null)
            {
                return false;
            }

            if(connToken.AuthenticationKey(openID, clientKey))
            {
                return true;
            }
            return false;
        }
        public void DeleteToken()
        {

        }

        public bool DetermineWaitReconnectInterval(int interval)
        {
            return _socket != null ? false : connToken.DetermineWaitReconnectInterval(interval);
        }
        #endregion

        #region 接收消息
        private void StartReceiveAsync()
        {
            //重新设置接收缓冲区
            receiveEventArgs.SetBuffer(connBufferManager.WriteToReceiveBufferByMemory());
            if (_socket == null)
            {
                AccidentalDisConnect("socket无对象:" + connInfo.FixedInfoDO.FixedIndex);
                return;
            }
            //异步接收，返回值为true表示此IO操作为挂起状态，false表示此IO操作为同步
            bool willRaiseEvent = _socket.ReceiveAsync(receiveEventArgs);
            if (!willRaiseEvent)
            {
                ReceiveCompleted(receiveEventArgs);
            }
        }

        private void ReceiveCompleted(SocketAsyncEventArgs e)
        {
            //检查远程主机是否断开连接
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                connInfo.AddConnReceiveBytesInfo(e.BytesTransferred);
                //改变缓冲区位置
                connBufferManager.UseReceiveBuffer(e.BytesTransferred);

                //将接收到的数据进行解析
                ParseReceiveBuffer();
            }
            else
            {
                //断开连接
                AccidentalDisConnect("接收{传输字节:" + e.BytesTransferred + ";操作结果:" + e.SocketError + "}");
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            ReceiveCompleted(e);
        }

        #endregion

        #region 解析缓存区并将得到的消息包放入接收消息队列
        /// <summary>
        /// 处理缓存区并将得到的消息包放入接收消息队列
        /// </summary>
        private void ParseReceiveBuffer()
        {
            try
            {
                foreach (var item in msgParse.ParseMsg())
                {
                    EnqueueToWaitExecuteMsgQueue(item);
                }
            }
            catch (Exception ex)
            {
                Disconnect("处理消息出错" + ex.ToString());
            }

            StartReceiveAsync();
        }


        public void EnqueueToWaitExecuteMsgQueue(MsgBody msgBody)
        {
            if (msgBody != null)
            {
                connMsgQueue.EnqueueToWaitExecuteMsgQueue(msgBody);
                connInfo.AddConnParseMsgInfo();
            }
        }
        #endregion

        #region 执行消息队列
        /// <summary>
        /// 开始执行消息队列
        /// </summary>
        private void StartExecuteMsgQueue()
        {
            Task.Run(ExecuteMsgAsync);
        }
        /// <summary>
        /// 执行消息队列
        /// </summary>
        private async Task ExecuteMsgAsync()
        {
            if (_socket != null)
            {
                if ((!connMsgQueue.IsNotNullExecuteMsgQueue) && (!connMsgQueue.IsNotNullWaitSendMsgQueue))
                {
                    await Task.Delay(ConnConfig.CONN_NO_MSG_THREAD_SLEEP_TIME);
                }

                MsgBody msgBody;
                while (connMsgQueue.DequeueInWaitExecuteMsgQueue(out msgBody))
                {
                    connInfo.AddConnExecuteMsgInfo();
                    //Console.WriteLine("开始处理消息");
                    MsgExecuteManager.ExecuteMsg(msgBody, connDelegate);
                }

                StartProcessSend(null);
            }
        }
        #endregion

        #region 发送
        /// <summary>
        /// 准备发送
        /// </summary>
        /// <param name="msgPack"></param>
        private void StartProcessSend(MsgPack msgPack)
        {
            MsgPack peekPack;

            //1.将携带数据放入等待发送队列
            EnqueueToWaitSendMsgQueue(msgPack);


            //2.判断缓冲区是否有数据
            if (connBufferManager.ValidateSendUseSize())
            {
                //有则进行4
                goto TryPeekMsg;
            }
            else
            {
                //无则进行3
                goto FirstTryPeekMsg;
            }

        //3.首次尝试获取等待消息队列首消息包
        FirstTryPeekMsg:
            {
                //无数据则尝试从等待消息队列取出首消息
                if (connMsgQueue.DequeueInWaitSendMsgQueue(out peekPack))
                {
                    //如果成功取出则记录消息包内时间戳
                    WriteMsgPackToSendBuff(peekPack);
                    //成功进行4
                    goto TryPeekMsg;
                }
                //无消息则退出
                else
                {
                    //失败退出
                    goto ExecuteMsg;
                }
            }
        //4.无数据则尝试从等待消息队列取出首消息
        TryPeekMsg:
            {
                //尝试从等待消息队列获取首消息(不移除)
                if (connMsgQueue.DequeueInWaitSendMsgQueue(out peekPack))
                {
                    //成功进行5
                    goto TryWriteMsg;
                }
                //若等待发送消息队列无消息
                else
                {
                    //失败进行6
                    goto NoMsgTrySend;
                }
            }

        //5.判断缓冲区能否写入下一个数据包
        TryWriteMsg:
            {
                //判断缓冲区能否写入下一个数据包
                if (connBufferManager.ValidateSendFreeSize(peekPack.MsgPackSize))
                {
                    //写入数据包
                    WriteMsgPackToSendBuff(peekPack);
                    //然后进行7
                    goto TrySend;
                }
                else
                {
                    connMsgQueue.EnqueueToWaitSendMsgQueue(peekPack);
                    //失败进行发送
                    goto StartSend;
                }
            }

        //6.判断上次发送到现在的时间间隔是否超过发送延时(此时等待发送消息队列无数据)
        NoMsgTrySend:
            {
                //判断上次发送到现在的时间间隔是否超过发送延时
                if (DetermineSendInterval(ConnConfig.CONN_SEND_MAX_INTERVAL))
                {
                    //超出发送
                    goto StartSend;
                }
                else
                {
                    //未超出执行消息
                    goto ExecuteMsg;
                }
            }

        //7.判断上次发送到现在的时间间隔是否超过发送延时
        TrySend:
            {
                //判断上次发送到现在的时间间隔是否超过发送延时
                if (DetermineSendInterval(ConnConfig.CONN_SEND_MAX_INTERVAL))
                {
                    //超出发送
                    goto StartSend;
                }
                else
                {
                    goto TryPeekMsg;
                }
            }

        //执行消息
        ExecuteMsg:
            {
                StartExecuteMsgQueue();
                return;
            }

        //开始发送数据
        StartSend:
            {
                StartSendAsync();
                return;
            }
        }

        /// <summary>
        /// 将一个消息包写入发送缓冲区
        /// </summary>
        /// <param name="msgPack"></param>
        private void WriteMsgPackToSendBuff(MsgPack msgPack)
        {
            Span<byte> freeSpan;
            connBufferManager.WriteToSendBufferBySpan(msgPack.MsgPackSize, out freeSpan);
            int useBuffSize = msgPack.OutputMsgPackToSpan(freeSpan);
            //更改缓冲区位置
            connBufferManager.UseSendBuffer(useBuffSize);

            connInfo.AddConnSendMsgInfo();
        }

        public bool DetermineReceiveInterval(int interval)
        {
            return (DateTime.Now - connInfo.OnceInfoDO.OnceLastReceiveTime).TotalMilliseconds > interval;
        }

        /// <summary>
        /// 判断发送消息间隔是否大于某值
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool DetermineSendInterval(int interval)
        {
            return (DateTime.Now - connInfo.OnceInfoDO.OnceLastSendTime).TotalMilliseconds > interval;
        }

        /// <summary>
        /// 将消息放入等待发送队列
        /// </summary>
        public void EnqueueToWaitSendMsgQueue(MsgPack msgPack)
        {
            if (msgPack != null)
            {
                connMsgQueue.EnqueueToWaitSendMsgQueue(msgPack);
            }
        }

        private void StartSendAsync()
        {
            //Console.WriteLine("开始发送");
            sendEventArgs.SetBuffer(connBufferManager.ReadFormSendBufferByMemory());
            if (_socket == null)
            {
                AccidentalDisConnect("socket无对象");
                return;
            }
            bool willRaiseEvent = _socket.SendAsync(sendEventArgs);
            if (!willRaiseEvent)
            {
                SendCompleted(sendEventArgs);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="e"></param>
        private void SendCompleted(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                connInfo.AddConnSendBytesInfo(e.BytesTransferred);
                connBufferManager.FreeSendBuffer();
                //Console.WriteLine("发送成功:"+e.BytesTransferred);
                StartProcessSend(null);
            }
            else
            {
                //断开链接 
                AccidentalDisConnect("发送{传输字节:" + e.BytesTransferred + ";操作结果:" + e.SocketError + "}");
            }
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            SendCompleted(e);
        }
        #endregion

        public ConnInfo GetConnInfo()
        {
            return connInfo;
        }
    }
}
