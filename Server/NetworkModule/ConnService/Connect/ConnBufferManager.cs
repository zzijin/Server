using Server.NetworkModule.Configuration;
using System;

namespace Server.NetworkModule.ConnService.Connect
{
    /*
     * 该类可独立出Conn类
     * 物理分配过程:
     * 在物理上所有链接均使用同一个byte数组作为收发缓冲区，在线程池初始化时，线程池会按照配置文件中的链接池链接个数和每个链接收发缓冲区大小初始化byte数组并分配给所有链接
     * 缓冲区一旦分配，直到应用结束运行前都不会再发生改变
     * 每个链接根据分配到的数组起始地址和配置文件中收发区域大小再由ConnBuffer类分配为接收缓冲区和发生缓冲区，他们的信息由ConnBuffer.BufferInfo类记录
     * 逻辑使用过程:
     * 在逻辑上每个链接分配到的缓冲区分为接收和发送两个缓冲区，ConnBuffer和ConnBuffer.BufferInfo类将这两个缓冲区在使用时经由逻辑运算视为独立的环形数组，从而达到重复使用缓冲区的目的
     * ConnBuffer类负责管理缓冲区，提供函数实现对缓冲区的操作，ConnBuffer.BufferInfo类记录了单个(接收或发送)缓冲区的起始地址，中止地址，大小，写入位置，读取位置，空闲大小等信息
     * 在操作缓冲区时必须使用ConnBuffer的相应函数，在使用这些函数对缓冲区进行写入、读取数据时，是根据对ConnBuffer.BufferInfo类记录的信息进行计算的结果实现写入和读取
     *
     * 所有链接使用同一个数组作为缓冲区作用:
     * 1.避免了内存碎片化
     * 2.避免了反复创建数组的消耗，提高内存利用率
     * 3.优化断线重连功能(客户端在移动设备上可能会经常意外断开链接，断线重连是必须做到的功能)，已接收的数据和未发送的数据存储在固定的缓冲区中，在意外断开时不会被清除，重连后可以很容易得继续完成
     * 4.在流量高峰期也能提高一定的削峰能力
    */
    /// <summary>
    /// 单个链接的收发缓冲区。
    /// 该缓冲区的接收和发送缓冲区是都是线程不安全的
    /// </summary>
    class ConnBufferManager
    {
        byte[] buffer;
        /// <summary>
        /// 接收缓冲区位置信息
        /// </summary>
        ConnBufferInfo receiveBufferInfo;
        /// <summary>
        /// 发送缓冲区位置信息
        /// </summary>
        ConnBufferInfo sendBufferInfo;
        /// <summary>
        /// 发送缓冲区锁
        /// </summary>
        //private readonly object sendBufferLock = new object();

        public ConnBufferManager()
        {
            receiveBufferInfo = new ConnBufferInfo();
            sendBufferInfo = new ConnBufferInfo();
        }

        /// <summary>
        /// 为该链接设置收发缓冲区
        /// </summary>
        /// <param name="bufferPool">公用缓冲池</param>
        /// <param name="startIndex">该链接起始缓冲位置</param>
        /// <param name="receiveEventArgs">该链接接收SocketAsyncEventArgs</param>
        /// <param name="sendEventArgs">该链接发送SocketAsyncEventArgs</param>
        public void Init(byte[] bufferPool, int startIndex)
        {
            buffer = bufferPool;

            receiveBufferInfo.SetInfo(startIndex, ConnConfig.CONN_RECEIVE_BUFFER_SIZE);
            sendBufferInfo.SetInfo(receiveBufferInfo.bufferEndIndex, ConnConfig.CONN_SEND_BUFFER_SIZE);
        }

        /// <summary>
        /// 新连接接入，重置缓存区状态
        /// </summary>
        public void ClearBufferInfo()
        {
            sendBufferInfo.InitInfo();
            receiveBufferInfo.InitInfo();
        }

        #region 接收缓冲区操作

        /// <summary>
        /// 从接收缓冲区取出未未写入的区域Memory
        /// 以用于写入
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        /// <returns>false表示缓冲区已满,true表示缓冲区设置完成</returns>
        public Memory<byte> WriteToReceiveBufferByMemory()
        {
            if (receiveBufferInfo.freeBufferSize > 0)
            {
                if (receiveBufferInfo.freeStartIndex >= receiveBufferInfo.useStartIndex)
                {
                   return new Memory<byte>(buffer,receiveBufferInfo.freeStartIndex, receiveBufferInfo.bufferEndIndex - receiveBufferInfo.freeStartIndex);
                }
                else
                {
                    return new Memory<byte>(buffer,receiveBufferInfo.freeStartIndex, receiveBufferInfo.useStartIndex - receiveBufferInfo.freeStartIndex);
                }
            }
            else
                return null;
        }

        /// <summary>
        /// 异步接收完成后重新设置接收缓冲区位置信息
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        public void UseReceiveBuffer(int useBuffer)
        {
            receiveBufferInfo.UseBuffer(useBuffer);
        }

        /// <summary>
        /// 解析完成后重新设置接收缓冲区位置信息
        /// </summary>
        public void FreeReceiveBuffer(int freeBuffer)
        {
            receiveBufferInfo.FreeBuffer(freeBuffer);
        }

        /// <summary>
        /// 从接收缓冲区中读取指定长度的数据
        /// 以用于解析
        /// </summary>
        /// <param name="getSize">需获取的长度</param>
        /// <param name="oneSpan">需获取的数据的内存区域</param>
        /// <param name="twoSpan">若需从头读取，此Span有值</param>
        /// <returns>false表示接受缓冲区可读数据长度不够，true表示读取成功</returns>
        public bool ReadFromReceiveBufferBySpan(int getSize, out ReadOnlySpan<byte> oneSpan, out ReadOnlySpan<byte> twoSpan)
        {
            //如果需获取的长度大于接受缓冲区已接受的长度，则需等待
            if (getSize > receiveBufferInfo.useBufferSize)
            {
                oneSpan = null;
                twoSpan = null;
                return false;

            }
            else
            {
                //若始写入位置大于始读取位置，则始读取至始写入间为可读取数据
                if (receiveBufferInfo.freeStartIndex > receiveBufferInfo.useStartIndex)
                {
                    oneSpan = new ReadOnlySpan<byte>(buffer, receiveBufferInfo.useStartIndex, getSize);
                    twoSpan = null;
                }
                //若始写入位置小于等于始读取位置，则可读取数据有两段:
                //1.始读取至接受缓冲区尾 
                //2.接受缓冲区头至发送始写入位置
                else
                {
                    int one = receiveBufferInfo.bufferEndIndex - receiveBufferInfo.useStartIndex;
                    //若范围一大于等于获取长度，则需获取数据均在范围一
                    if (getSize <= one)
                    {
                        oneSpan = new ReadOnlySpan<byte>(buffer, receiveBufferInfo.useStartIndex, getSize);
                        twoSpan = null;
                    }
                    //否则需将范围一数据与范围二数据结合
                    else
                    {
                        int two = getSize - one;
                        oneSpan = new ReadOnlySpan<byte>(buffer, receiveBufferInfo.useStartIndex, one);
                        twoSpan = new ReadOnlySpan<byte>(buffer, receiveBufferInfo.bufferStartIndex, two);
                    }
                }
                return true;
            }

        }

        /// <summary>
        /// 验证接收缓冲区已接收数据大小是否足够
        /// </summary>
        /// <param name="size">比较大小</param>
        /// <returns></returns>
        //public bool ValidateReceiveUseSize(int size)
        //{
        //    return size > receiveBufferInfo.useBufferSize;
        //}
        /// <summary>
        /// 验证接收缓冲区是否还有空闲空间
        /// </summary>
        /// <returns></returns>
        public bool ValidateReceiveFreeSize()
        {
            return receiveBufferInfo.freeBufferSize > 0;
        }
        #endregion

        #region 发送缓冲区操作
        /// <summary>
        /// 从发送缓冲区取出已记录数据Memory
        /// 以用于发送
        /// </summary>
        /// <returns></returns>
        public Memory<byte> ReadFormSendBufferByMemory()
        {
            if (sendBufferInfo.useBufferSize > 0)
            {
                return new Memory<byte>(buffer,sendBufferInfo.useStartIndex, sendBufferInfo.useBufferSize);
            }
            else
                return null;
        }

        /// <summary>
        /// 更改发送缓冲区使用范围标识
        /// </summary>
        /// <param name="useBuffer">本次使用大小</param>
        public void UseSendBuffer(int useBuffer)
        {
            sendBufferInfo.UseBuffer(useBuffer);
        }

        /// <summary>
        /// 根据预估的大小取出相应的发送缓冲区Span
        /// 以用于写入
        /// </summary>
        /// <param name="useSize">预估大小</param>
        /// <param name="freeSpan">可使用内存范围</param>
        /// <returns></returns>
        public bool WriteToSendBufferBySpan(int useSize, out Span<byte> freeSpan)
        {
            if (sendBufferInfo.freeBufferSize > useSize)
            {
                freeSpan = new Span<byte>(buffer, sendBufferInfo.freeStartIndex, useSize);
                return true;
            }
            freeSpan = null;
            return false;
        }
        /// <summary>
        /// 释放接收缓冲区
        /// </summary>
        public void FreeSendBuffer()
        {
            sendBufferInfo.InitInfo();
        }

        /// <summary>
        /// 验证发送缓冲区是否还能放入指定大小数据
        /// </summary>
        /// <param name="size">指定大小</param>
        public bool ValidateSendFreeSize(int size)
        {
            return sendBufferInfo.freeBufferSize >= size;
        }
        /// <summary>
        /// 验证发送缓冲区是否还有未发送数据
        /// </summary>
        /// <returns></returns>
        public bool ValidateSendUseSize()
        {
            return sendBufferInfo.useBufferSize > 0;
        }
        #endregion

        struct ConnBufferInfo
        {
            /// <summary>
            ///  缓冲区起始位置
            /// </summary>
            public int bufferStartIndex;
            /// <summary>
            /// 缓冲区终止位置(缓冲区可写入位置不包含此位)
            /// </summary>
            public int bufferEndIndex;
            /// <summary>
            /// 缓冲区大小
            /// </summary>
            public int bufferSize;
            /// <summary>
            /// 空闲缓冲区起始位置\始写入位置(指示缓冲区从此位开始为可使用字节段)
            /// </summary>
            public int freeStartIndex;
            /// <summary>
            /// 已使用缓冲区起始位置\始读取位置(指示缓冲区从此位开始为已使用字段)
            /// </summary>
            public int useStartIndex;
            /// <summary>
            /// 缓冲区空闲大小
            /// </summary>
            public int freeBufferSize;
            /// <summary>
            /// 缓冲区已使用大小
            /// </summary>
            public int useBufferSize
            {
                get
                {
                    return bufferSize - freeBufferSize;
                }
            }

            public void SetInfo(int bufferStartIndex, int bufferSize)
            {
                this.bufferStartIndex = bufferStartIndex;
                this.bufferSize = bufferSize;
                bufferEndIndex = bufferStartIndex + bufferSize;
                InitInfo();
            }

            public void InitInfo()
            {
                freeStartIndex = bufferStartIndex;
                useStartIndex = bufferStartIndex;
                freeBufferSize = bufferSize;
            }

            public void UseBuffer(int useSize)
            {
                freeStartIndex += useSize;
                if (freeStartIndex >= bufferEndIndex)
                {
                    freeStartIndex = freeStartIndex - bufferEndIndex + bufferStartIndex;
                }
                freeBufferSize -= useSize;
            }

            public void FreeBuffer(int freeSize)
            {
                useStartIndex += freeSize;
                if (useStartIndex >= bufferEndIndex)
                {
                    useStartIndex = useStartIndex - bufferEndIndex + bufferStartIndex;
                }
                freeBufferSize += freeSize;
            }
        }
    }
}
