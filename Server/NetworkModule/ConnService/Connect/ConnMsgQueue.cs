using Server.NetworkModule.ConnService.Message.MsgFormat;
using System.Collections.Concurrent;

namespace Server.NetworkModule.ConnService.Connect
{
    /// <summary>
    /// 单个链接的消息队列
    /// </summary>
    class ConnMsgQueue
    {
        ConcurrentQueue<MsgBody> waitExecuteMsgQueue;
        ConcurrentQueue<MsgPack> waitSendMsgQueue;
        //private int waitProcessesMsgQueueMaxNum;
        //private int waitSendMsgQueueMaxNum;

        public ConnMsgQueue()
        {
            waitExecuteMsgQueue = new ConcurrentQueue<MsgBody>();
            waitSendMsgQueue = new ConcurrentQueue<MsgPack>();
        }

        public void Clear()
        {
            
            waitExecuteMsgQueue.Clear();
            waitSendMsgQueue.Clear();
        }

        #region 等待处理消息队列
        /// <summary>
        /// 将任务加入等待处理队列
        /// </summary>
        /// <param name="msgBody"></param>
        public void EnqueueToWaitExecuteMsgQueue(MsgBody msgBody)
        {
            waitExecuteMsgQueue.Enqueue(msgBody);
        }

        /// <summary>
        /// 尝试从待处理消息队列中取出任务
        /// </summary>
        /// <param name="msgBody"></param>
        /// <returns>true表示成功取出,false表示取出失败</returns>
        public bool DequeueInWaitExecuteMsgQueue(out MsgBody msgBody)
        {
            return waitExecuteMsgQueue.TryDequeue(out msgBody);
        }
        public bool IsNotNullExecuteMsgQueue { get => waitExecuteMsgQueue.Count > 0; }
        #endregion

        #region 等待发送消息队列方法
        /// <summary>
        /// 将消息加入等待发送队列
        /// </summary>
        /// <param name="msgPack"></param>
        public void EnqueueToWaitSendMsgQueue(MsgPack msgPack)
        {
            waitSendMsgQueue.Enqueue(msgPack);
        }

        /// <summary>
        /// 尝试从待发送消息队列中取出消息
        /// </summary>
        /// <param name="msgPack"></param>
        /// <returns></returns>
        public bool DequeueInWaitSendMsgQueue(out MsgPack msgPack)
        {
                return waitSendMsgQueue.TryDequeue(out msgPack);
        }

        public bool IsNotNullWaitSendMsgQueue { get => waitSendMsgQueue.Count > 0; }
        #endregion
    }
}
