using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Connect;
using Server.NetworkModule.ConnService.StatisticsInfo;
using Server.NetworkModule.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Server.NetworkModule.ConnService.ConnPool
{
    /// <summary>
    /// 线程池资源管理
    /// 封装链接池所有资源并对链接池提供功能型操作
    /// </summary>
    class ConnPoolResourceManager
    {
        #region 正式链接相关
        /// <summary>
        /// 正式链接集合
        /// </summary>
        private IConn[] connArray;
        /// <summary>
        /// 空闲链接位置的线程安全队列
        /// </summary>
        private ConcurrentQueue<int> freedomConnQueue;
        /// <summary>
        /// 已使用链接位置的有锁列表
        /// </summary>
        private LockLinkedList<int> usedConnList;
        /// <summary>
        /// 缓冲池
        /// </summary>
        private byte[] bufferPool;
        #endregion

        #region 等待链接
        /// <summary>
        /// 等待链接队列
        /// </summary>
        private ConcurrentQueue<ConnSimple> waitConnQueue;
        #endregion

        #region 其它
        /// <summary>
        /// 公用固定发送消息字典
        /// </summary>
        private ConnSimpleFixedSendBufferDictionary fixedSendBuffer;
        /// <summary>
        /// 服务提供
        /// </summary>
        private IServiceProvider _serviceProvider;
        #endregion

        #region 对链接池提供的字段属性
        public IConn this[int index]
        {
            get { return connArray[index]; }
        }

        public int ConnArraySize { get => connArray.Length; }

        public int UsedConnListSize{get=>usedConnList.Count; }

        public int FreedomConnQueueSize{get=>freedomConnQueue.Count;}

        public int WaitConnQueueSize { get => waitConnQueue.Count; }


        public IEnumerable<IConn> GetConnArray()
        {
            return from item in connArray select item;
        }

        public IEnumerable<ConnInfo> GetConnInfoArray()
        {
            return from item in connArray select item.GetConnInfo();
        }

        public IEnumerable<IConn> GetUseConnArray()
        {
            return from item in usedConnList select connArray[item];
        }

        public IEnumerable<ConnSimple> GetWaitConnArray()
        {
            return from item in waitConnQueue select item;
        }


        #endregion

        #region 初始化资源
        public ConnPoolResourceManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        public void Init()
        {
            //初始化链接池变量
            connArray = new Conn[ConnPoolConfig.CONNPOOL_SIZE];

            //初始化缓冲池
            //缓存池大小=链接池大小*(单个链接接收+发送缓冲区大小)+固定发送缓冲区
            bufferPool = new byte[ConnPoolConfig.CONNPOOL_SIZE *
                (ConnConfig.CONN_RECEIVE_BUFFER_SIZE + ConnConfig.CONN_SEND_BUFFER_SIZE) + ConnConfig.CONN_FIXED_SEND_BUFFER_SIZE];

            //初始化公用固定发送缓冲区
            fixedSendBuffer = new ConnSimpleFixedSendBufferDictionary(
                new Memory<byte>(bufferPool,
                ConnPoolConfig.CONNPOOL_SIZE * (ConnConfig.CONN_RECEIVE_BUFFER_SIZE + ConnConfig.CONN_SEND_BUFFER_SIZE),
                ConnConfig.CONN_FIXED_SEND_BUFFER_SIZE)
                );

            //初始化集合
            InitConnArray();

            //初始化固定字典
            InitFixedSendDictionary();
        }


        private void InitConnArray()
        {
            //初始化空闲队列
            freedomConnQueue = new ConcurrentQueue<int>();
            //初始化正式链接链表
            usedConnList = new LockLinkedList<int>();
            //初始化等待队列
            waitConnQueue = new ConcurrentQueue<ConnSimple>();

            //循环初始化每个链接
            for (int i = 0; i < connArray.Length; i++)
            {
                //初始化链接
                connArray[i] = new Conn(_serviceProvider);
                connArray[i].Init(i, bufferPool, i * (ConnConfig.CONN_RECEIVE_BUFFER_SIZE + ConnConfig.CONN_SEND_BUFFER_SIZE));
                freedomConnQueue.Enqueue(i);
            }
        }
        private void InitFixedSendDictionary()
        {
            fixedSendBuffer.AddConnectFailMsg();
        }
        #endregion

        #region 链接池资源对外（对链接池）提供的方法
        public ReadOnlySpan<byte> GetPublicFixedSendBuffer(int type)
        {
            return fixedSendBuffer.GetFixedSendMemory(type);
        }
        public LinkedListNode<int> UseConnListAdd(int index)
        {
            return usedConnList.AddLast(index);
        }
        public void UseConnListRemove(LinkedListNode<int> node)
        {
            usedConnList.Remove(node);
        }
        public void FreedomConnQueueEnqueue(int index)
        {
            freedomConnQueue.Enqueue(index);
        }
        public bool FreedomConnQueueTryDequeue(out int index)
        {
            if (freedomConnQueue.TryDequeue(out index))
            {
                return true;
            }

            return false;
        }
        public void WaitConnQueueEnqueue(ConnSimple connSimple)
        {
            waitConnQueue.Enqueue(connSimple);
        }
        public bool WaitConnQueueTryDequeue(out ConnSimple connSimple)
        {
            if (waitConnQueue.TryDequeue(out connSimple))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
