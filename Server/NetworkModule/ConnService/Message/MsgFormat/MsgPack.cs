using Server.NetworkModule.Configuration;
using Server.Tools;
using System;

namespace Server.NetworkModule.ConnService.Message.MsgFormat
{
    ///*************************************************** 消息包格式
    ////////////固定头
    ///     是否使用              名称                  变量名              类型         长度(字节)            说明
    ///        Y              消息包开始标识符        msgStartTag           byte             1                 包头标识，固定值
    ///        Y              主消息体大小            msgCount               int32           4                 该消息包剩余长度(消息体+固定尾)
    ////////////消息体      
    ///        Y              操作号/消息类别         msgType               int32            4                 该消息包操作号，用于对相应消息包进行处理
    ///        Y              时间戳                  msgTime               long             8                 该消息包发出时的时间戳
    ///        Y              消息数据加密标志        msgFlag               int32            4                 该消息包主体数据的加密方式
    ///        Y              消息数据                msgData               byte[]         不定长              该消息包的主体数据
    ////////////固定尾
    ///        Y              消息包结束标识符        msgEndTag             byte             1                 包尾标识符
    ///*************************************************** 消息包格式

    /// <summary>
    /// 一个完整消息包
    /// 用于包装和解析
    /// </summary>
    class MsgPack
    {
        #region 消息包构成
        private MsgFixedHead msgFixedHead;
        private MsgBody msgBody;
        private MsgFixedFoot msgFixedFoot;
        #endregion

        #region 解析
        /// <summary>
        /// 消息包最小大小
        /// 指示当接收消息累积到多大大小时可以开始解析
        /// </summary>
        public readonly int MsgPackMinSize = 22;
        /// <summary>
        /// 消息包固定头大小
        /// </summary>
        public readonly int MsgFixedHeadSize = MsgFixedHead.MsgFixedHeadSize;
        /// <summary>
        /// 指示该消息包除固定头外的剩余长度
        /// 该量在执行完成固定头解析后有值，否则为-1
        /// </summary>
        public int MsgCount { get => msgFixedHead.msgCount; }

        public MsgPack()
        {
            msgFixedHead = new MsgFixedHead();
            msgFixedFoot = new MsgFixedFoot();

            Init();
        }

        public void Init()
        {
            msgFixedHead.Init();
        }

        /// <summary>
        ///  消息固定头解析函数
        ///  在解析前需使用ValidateLength验证数据长度是否不少于消息包最小长度
        /// </summary>
        /// <param name="spanDate"></param>
        /// <returns></returns>
        public void MsgFixedHeadParser(ReadOnlySpan<byte> spanDate)
        {
            if (msgFixedHead.msgCount != -1)
            {
                return;
            }

            //验证首字节是否为标识符，注：单个Span长度至少长度为1
            if (spanDate[0] != MsgConfig.MSG_START_TAG)
            {
                throw new ApplicationException("消息包首标识符错误");
            }

            msgFixedHead.msgCount = ConvertTypeTool.ByteArrayToInt32(spanDate[1..]);
        }
        /// <summary>
        /// 消息固定头解析函数
        /// 在解析前应先验证可解析数据长度是否不少于消息包最小长度
        /// </summary>
        /// <param name="oneSpan"></param>
        /// <param name="twoSpan"></param>
        /// <param name="msgBody"></param>
        /// <returns></returns>
        public void MsgFixedHeadParser(ReadOnlySpan<byte> oneSpan, ReadOnlySpan<byte> twoSpan)
        {
            //若固定头长度不为-1，则已解析了长度信息
            if (msgFixedHead.msgCount != -1)
            {
                return;
            }

            //注：由于在使用此函数前已经进行了数据长度验证，故两Span长度和必然不小于22
            int receiveDataOneSize = oneSpan.Length;
            //验证首字节是否为标识符，注：单个Span长度至少长度为1
            if (oneSpan[0] != MsgConfig.MSG_START_TAG)
            {
                throw new ApplicationException("消息包首标识符错误");
            }

            //oneSpan长度可能性：1，2，3，4，>4
            switch (receiveDataOneSize)
            {
                ///oneSpan只有一个字符，则表示剩余长度符均在twoSpan
                case 1: msgFixedHead.msgCount = ConvertTypeTool.ByteArrayToInt32(twoSpan[0], twoSpan[1], twoSpan[2], twoSpan[3]); break;
                case 2: msgFixedHead.msgCount = ConvertTypeTool.ByteArrayToInt32(oneSpan[1], twoSpan[0], twoSpan[1], twoSpan[2]); break;
                case 3: msgFixedHead.msgCount = ConvertTypeTool.ByteArrayToInt32(oneSpan[1], oneSpan[2], twoSpan[0], twoSpan[1]); break;
                case 4: msgFixedHead.msgCount = ConvertTypeTool.ByteArrayToInt32(oneSpan[1], oneSpan[2], oneSpan[3], twoSpan[0]); break;
                default: msgFixedHead.msgCount = ConvertTypeTool.ByteArrayToInt32(oneSpan[1], oneSpan[2], oneSpan[3], oneSpan[4]); break;
            }
        }

        /// <summary>
        /// 消息体解析函数
        /// 传入的span应只包含剩余长度消息
        /// 在解析前应先验证可解析数据长度是否不少于该消息包剩余长度
        /// </summary>
        /// <param name="spanDate"></param>
        /// <param name="msgBody"></param>
        /// <returns></returns>
        public MsgBody InputMsgBodyFromSpan(ReadOnlySpan<byte> spanDate)
        {
            MsgBody msgBody;
            if (spanDate[spanDate.Length - 1] != MsgConfig.MSG_END_TAG)
            {
                throw new ApplicationException("消息包尾标识符错误");
            }
            msgBody = new MsgBody();
            msgBody.InputMsgBodyFromSpan(spanDate[0..^1]);

            return msgBody;
        }

        /// <summary>
        /// 消息体解析函数
        /// 传入的span应只包含剩余长度消息
        /// 在解析前应先验证可解析数据长度是否不少于该消息包剩余长度
        /// </summary>
        /// <param name="oneSpan"></param>
        /// <param name="twoSpan"></param>
        /// <returns></returns>
        public MsgBody InputMsgBodyFromSpan(ReadOnlySpan<byte> oneSpan, ReadOnlySpan<byte> twoSpan)
        {
            MsgBody msgBody;
            if (twoSpan[twoSpan.Length - 1] != MsgConfig.MSG_END_TAG)
            {
                throw new ApplicationException("消息包尾标识符错误");
            }
            msgBody = new MsgBody();
            //若twoSpan只含一个值，该值为尾标识符
            if (twoSpan.Length == 1)
            {
                msgBody.InputMsgBodyFromSpan(oneSpan[..]);
            }
            else
            {
                msgBody.InputMsgBodyFromSpan(oneSpan, twoSpan[0..^1]);
            }
            return msgBody;
        }

        #endregion

        #region 装包
        public MsgBody MsgBody { get => msgBody; }

        public MsgPack(MsgBody msgBody)
        {
            this.msgBody = msgBody;
        }

        public int OutputMsgPackToSpan(Span<byte> freeSpan)
        {
            msgFixedHead.msgCount = msgBody.OutputMsgBodyToSpan(freeSpan, MsgFixedHead.MsgFixedHeadSize) + MsgFixedFoot.MsgFixedFootSize;
            msgFixedHead.WriteMsgFixedHeadToSpan(freeSpan, 0);
            msgFixedFoot.WriteMsgFixedFootToSpan(freeSpan, msgFixedHead.msgCount + MsgFixedHead.MsgFixedHeadSize - MsgFixedFoot.MsgFixedFootSize);
            return MsgFixedHead.MsgFixedHeadSize + msgFixedHead.msgCount;
        }

        public int MsgPackSize { get => msgBody.MsgBodySize + MsgFixedHead.MsgFixedHeadSize + MsgFixedFoot.MsgFixedFootSize; }

        #endregion

        struct MsgFixedHead
        {
            public byte msgStartTag { get => MsgConfig.MSG_START_TAG; }
            public int msgCount;

            public void Init()
            {
                msgCount = -1;
            }

            public void WriteMsgFixedHeadToSpan(Span<byte> freeSpan, int offset)
            {
                freeSpan[0] = msgStartTag;
                ConvertTypeTool.Int32ToSpan(msgCount, freeSpan, 1);
            }

            public byte[] MsgFixedHeadForByteArray
            {
                get
                {
                    byte[] msgFixedHeadByte = new byte[MsgFixedHeadSize];
                    msgFixedHeadByte[0] = msgStartTag;
                    Buffer.BlockCopy(ConvertTypeTool.Int32ToByteArray(msgCount), 0, msgFixedHeadByte, 1, 4);
                    return msgFixedHeadByte;
                }
            }

            public static readonly int MsgFixedHeadSize = 5;
        }

        struct MsgFixedFoot
        {
            public byte msgEndTag { get => MsgConfig.MSG_END_TAG; }

            public void WriteMsgFixedFootToSpan(Span<byte> freeSpan, int offset)
            {
                freeSpan[offset] = msgEndTag;
            }

            public byte[] MsgFixedHeadForByteArray
            {
                get
                {
                    byte[] msgFixedHeadByte = new byte[1];
                    msgFixedHeadByte[0] = msgEndTag;
                    return msgFixedHeadByte;
                }
            }

            public static readonly int MsgFixedFootSize = 1;
        }
    }
}
