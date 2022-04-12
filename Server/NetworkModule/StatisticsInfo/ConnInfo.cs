using System;
using System.Collections.Generic;
using System.Threading;

namespace Server.NetworkModule.ConnService.StatisticsInfo
{
    /// <summary>
    /// 用于记录连接的相关数据
    /// </summary>
    class ConnInfo
    {
        ConnFixedInfoDO fixedInfo;
        ConnOnceInfoDO onceInfo;
        ConnTotalInfoDO totalInfo;

        public ConnInfo(int connIndex)
        {
            fixedInfo = new ConnFixedInfoDO(connIndex);
            onceInfo = new ConnOnceInfoDO();
            totalInfo = new ConnTotalInfoDO();
        }

        public void Connect(LinkedListNode<int> node)
        {
            onceInfo.Init(node);

            totalInfo.AddTotalUsedCount();

            //Console.WriteLine("使用第" + connIndex + "链接");
        }

        public void DisConnect(string msg)
        {
            if (onceInfo == null)
            {
                return;
            }

            if (onceInfo.OnceNode != null)
            {
                totalInfo.AddTotalInfo(onceInfo);
                Console.WriteLine($"第{fixedInfo.FixedIndex}个链接断开,使用时长:{(long)(DateTime.Now - onceInfo.OnceStartUseTime).TotalMilliseconds}毫秒;断开原因:{msg}");
                onceInfo.ClearOnceNode();
            }
        }

        #region 添加统计消息
        public void AddConnReceiveBytesInfo(int addSize)
        {
            onceInfo.AddOnceReceiveTimes();
            onceInfo.AddOnceReceiveBytes(addSize);
            onceInfo.SetOnceLastReceiveTime();
            //Console.WriteLine("接收消息,大小:" + addSize + ";该链接总计接收:" + connTotalReceiveBytes);
        }

        public void AddConnSendBytesInfo(int addSize)
        {
            onceInfo.AddOnceSendTimes();
            onceInfo.AddOnceSendBytes(addSize);
            onceInfo.SetOnceLastSendTime();
            //Console.WriteLine("发送消息,大小:" + addSize + ";该链接总计发送:" + connTotalSendBytes);
        }

        public void AddConnParseMsgInfo()
        {
            onceInfo.AddOnceParseMsg();
            //Console.WriteLine("解析消息个数:" + connTotalParseMsg);
        }

        public void AddConnExecuteMsgInfo()
        {
            onceInfo.AddOnceExecuteMsg();
            //Console.WriteLine("处理消息个数:" + connTotalProcessMsg);
        }

        public void AddConnSendTimesInfo()
        {
            onceInfo.AddOnceSendTimes();
            //Console.WriteLine("发送消息次数:" + connTotalSendMsg);
        }

        public void AddConnSendMsgInfo()
        {
            onceInfo.AddOnceSendMsg();
        }

        public void AddConnWaitReconnectInfo()
        {
            onceInfo.AddOnceWaitConnectTimes();
        }
        #endregion

        #region 链接信息查询
        public ConnFixedInfoDO FixedInfoDO { get { return fixedInfo; } }
        public ConnOnceInfoDO OnceInfoDO { get { return onceInfo; } }
        public ConnTotalInfoDO TotalInfoDO { get { return totalInfo; } }
        #endregion
    }

    /// <summary>
    /// 链接的固有信息(该链接初始化时即固定，直到到程序结束都不会发送改变)
    /// </summary>
    public class ConnFixedInfoDO
    {
        /// <summary>
        /// 该链接在链接池中的地址
        /// </summary>
        public int FixedIndex { get; private set; }

        public ConnFixedInfoDO(int connIndex)
        {
            this.FixedIndex = connIndex;
        }
    }

    /// <summary>
    /// 链接的单次连接的信息
    /// </summary>
    public class ConnOnceInfoDO
    {
        private LinkedListNode<int> onceNode;
        private DateTime onceStartUseTime;
        private long onceReceiveBytes;
        private long onceReceiveTimes;
        private long onceSendBytes;
        private long onceParseMsg;
        private long onceSendTimes;
        private long onceSendMsg;
        private long onceExecuteMsg;
        private DateTime onceLastReceiveTime;
        private DateTime onceLastSendTime;
        private int onceWaitReconnectTimes;

        /// <summary>
        /// 当前连接在已使用链接链表中的节点信息
        /// </summary>
        public LinkedListNode<int> OnceNode { get => onceNode; }
        /// <summary>
        /// 当前链接使用起始时间
        /// </summary>
        public DateTime OnceStartUseTime { get => onceStartUseTime; }
        /// <summary>
        /// 链接总接收字节数
        /// </summary>
        public long OnceReceiveBytes { get => onceReceiveBytes; }
        /// <summary>
        /// 链接总发送字节数
        /// </summary>
        public long OnceSendBytes { get => onceSendBytes; }
        /// <summary>
        /// 链接总解析消息数量
        /// </summary>
        public long OnceParseMsg { get => onceParseMsg; }
        /// <summary>
        /// 链接总发送消息数量
        /// </summary>
        public long OnceSendTimes { get => onceSendTimes; }
        /// <summary>
        /// 执行请求个数
        /// </summary>
        public long OnceExecuteMsg { get => onceExecuteMsg; }
        /// <summary>
        /// 上次接收的时间
        /// </summary>
        public DateTime OnceLastReceiveTime { get => onceLastReceiveTime; }
        /// <summary>
        /// 上次发送的时间
        /// </summary>
        public DateTime OnceLastSendTime { get => onceLastSendTime; }
        /// <summary>
        /// 重连次数
        /// </summary>
        public int OnceWaitReconnectTimes { get => onceWaitReconnectTimes; }
        public long OnceSendMsg { get => onceSendMsg; }
        public long OnceReceiveTimes { get => onceReceiveTimes; }

        public void Init(LinkedListNode<int> node)
        {
            onceStartUseTime = DateTime.Now;
            this.onceNode = node;
            onceReceiveBytes = 0; onceReceiveBytes = 0; onceParseMsg = 0;
            onceSendTimes = 0; onceExecuteMsg = 0; onceWaitReconnectTimes = 0; onceSendMsg = 0;
            SetOnceLastReceiveTime();
        }

        public void ClearOnceNode()
        {
            onceNode = null;
        }

        public void AddOnceReceiveBytes(int addSize)
        {
            Interlocked.Add(ref onceReceiveBytes,addSize);
            //Console.WriteLine("接收消息,大小:" + addSize + ";该链接总计接收:" + connTotalReceiveBytes);
        }

        public void AddOnceReceiveTimes()
        {
           Interlocked.Increment(ref onceReceiveTimes);
        }

        public void AddOnceSendBytes(int addSize)
        {
            Interlocked.Add(ref onceSendBytes, addSize);
            //Console.WriteLine("发送消息,大小:" + addSize + ";该链接总计发送:" + connTotalSendBytes);
        }

        public void AddOnceParseMsg()
        {
            Interlocked.Increment(ref onceParseMsg);
            //Console.WriteLine("解析消息个数:" + connTotalParseMsg);
        }

        public void AddOnceExecuteMsg()
        {
            Interlocked.Increment(ref onceExecuteMsg);
            //Console.WriteLine("处理消息个数:" + connTotalProcessMsg);
        }

        public void AddOnceSendTimes()
        {
            Interlocked.Increment(ref onceSendTimes);
            //Console.WriteLine("发送消息总数:" + connTotalSendMsg);
        }

        public void SetOnceLastReceiveTime()
        {
            onceLastReceiveTime = DateTime.Now;
        }

        public void SetOnceLastSendTime()
        {
            onceLastSendTime = DateTime.Now;
        }

        public void AddOnceWaitConnectTimes()
        {
            onceWaitReconnectTimes++;
        }

        public void AddOnceSendMsg()
        {
            Interlocked.Increment(ref onceSendMsg);
        }
    }

    /// <summary>
    /// 链接的总计统计信息
    /// </summary>
    public class ConnTotalInfoDO
    {
        /// <summary>
        /// 链接使用次数
        /// </summary>
        public int TotalUsedCount { get; private set; }
        /// <summary>
        /// 链接总接收字节数
        /// </summary>
        public long TotalReceiveBytes { get; private set; }
        /// <summary>
        /// 链接总发送字节数
        /// </summary>
        public long TotalSendBytes { get; private set; }
        /// <summary>
        /// 链接总使用时间
        /// </summary>
        public TimeSpan TotalUseTime { get; private set; }
        /// <summary>
        /// 链接总解析消息数量
        /// </summary>
        public long TotalParseMsg { get; private set; }
        /// <summary>
        /// 链接总发送消息数量
        /// </summary>
        public long TotalSendMsg { get; private set; }
        /// <summary>
        /// 处理信息个数
        /// </summary>
        public long TotalExecuteMsg { get; private set; }

        public ConnTotalInfoDO()
        {
            TotalUsedCount = 0;
            TotalReceiveBytes = 0;
            TotalSendBytes = 0;
            TotalUseTime = new TimeSpan(0);
            TotalParseMsg = 0;
            TotalSendMsg = 0;
            TotalExecuteMsg = 0;
        }

        public void AddTotalUsedCount()
        {
            TotalUsedCount++;
        }

        public void AddTotalInfo(ConnOnceInfoDO onceInfo)
        {
            TotalUseTime += DateTime.Now - onceInfo.OnceStartUseTime;
            TotalReceiveBytes += onceInfo.OnceReceiveBytes;
            TotalSendBytes += onceInfo.OnceSendBytes;
            TotalParseMsg += onceInfo.OnceParseMsg;
            TotalSendMsg += onceInfo.OnceSendTimes;
            TotalExecuteMsg += onceInfo.OnceExecuteMsg;
        }
    }
}
