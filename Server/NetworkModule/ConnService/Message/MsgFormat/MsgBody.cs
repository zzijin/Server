using Server.Tools;
using System;

namespace Server.NetworkModule.ConnService.Message.MsgFormat
{
    ////////////消息体      
    ///        Y              操作号/消息类别         msgType               int32            4                 该消息包操作号，用于对相应消息包进行处理
    ///        Y              时间戳                  msgTime               long             8                 该消息包发出时的时间戳
    ///        Y              消息数据加密标志        msgFlag               int32            4                 该消息包主体数据的加密方式
    ///        Y              消息数据                msgData               byte[]         不定长              该消息包的主体数据
    ////////////
    /// <summary>
    /// 一个完整消息体
    /// </summary>
    class MsgBody
    {
        private int msgType;
        private long msgTime;
        private int msgFlag;
        private byte[] msgData;

        public int MsgType { get => msgType; }
        public long MsgTime { get => msgTime; }
        public int MsgFlag { get => msgFlag; }
        public byte[] MsgData { get => msgData; }

        #region 解析

        public MsgBody()
        {

        }

        /// <summary>
        /// 解析消息体数据
        /// </summary>
        /// <param name="spanDate">仅包含消息体的数据</param>
        public void InputMsgBodyFromSpan(ReadOnlySpan<byte> spanDate)
        {
            msgType = ConvertTypeTool.ByteArrayToInt32(spanDate.Slice(0, 4));
            msgTime = ConvertTypeTool.ByteArrayToLong(spanDate.Slice(4, 8));
            msgFlag = ConvertTypeTool.ByteArrayToInt32(spanDate.Slice(12, 4));
            msgData = spanDate[16..].ToArray();
        }

        /// <summary>
        /// 解析消息体数据,需换行时
        /// </summary>
        /// <param name="oneSpan"></param>
        /// <param name="twoSpan"></param>
        /// <returns></returns>
        public void InputMsgBodyFromSpan(ReadOnlySpan<byte> oneSpan, ReadOnlySpan<byte> twoSpan)
        {
            int oneSpaneSize = oneSpan.Length;
            int twoSpanSize = twoSpan.Length;

            if (oneSpaneSize < 4)
            {
                int overlenght = 4 - oneSpaneSize;
                msgType = ConvertTypeTool.ByteArrayToInt32(oneSpan.Slice(0, oneSpaneSize), twoSpan.Slice(0, overlenght));
                msgTime = ConvertTypeTool.ByteArrayToLong(twoSpan.Slice(overlenght, 8));
                overlenght += 8;
                msgFlag = ConvertTypeTool.ByteArrayToInt32(twoSpan.Slice(overlenght, 4));
                overlenght += 4;
                msgData = twoSpan[overlenght..].ToArray();
            }
            else if (oneSpaneSize < 12)
            {
                int overlenght = 12 - oneSpaneSize;
                msgType = ConvertTypeTool.ByteArrayToInt32(oneSpan.Slice(0, 4));
                msgTime = ConvertTypeTool.ByteArrayToLong(oneSpan[4..], twoSpan.Slice(0, overlenght));
                msgFlag = ConvertTypeTool.ByteArrayToInt32(twoSpan.Slice(overlenght, 4));
                overlenght += 4;
                msgData = twoSpan[overlenght..].ToArray();
            }
            else if (oneSpaneSize < 16)
            {
                int overlenght = 16 - oneSpaneSize;
                msgType = ConvertTypeTool.ByteArrayToInt32(oneSpan.Slice(0, 4));
                msgTime = ConvertTypeTool.ByteArrayToLong(oneSpan.Slice(4, 8));
                msgFlag = ConvertTypeTool.ByteArrayToInt32(oneSpan[12..], twoSpan.Slice(0, overlenght));
                msgData = twoSpan[overlenght..].ToArray();
            }
            else
            {
                msgType = ConvertTypeTool.ByteArrayToInt32(oneSpan.Slice(0, 4));
                msgTime = ConvertTypeTool.ByteArrayToLong(oneSpan.Slice(4, 8));
                msgFlag = ConvertTypeTool.ByteArrayToInt32(oneSpan.Slice(12, 4));
                int overlenght = oneSpaneSize - 16;
                int msgDataSize = oneSpaneSize + twoSpanSize - 16;
                msgData = new byte[msgDataSize];
                Buffer.BlockCopy(oneSpan[16..].ToArray(), 0, msgData, 0, overlenght);
                Buffer.BlockCopy(twoSpan.ToArray(), 0, msgData, overlenght, twoSpanSize);
            }
        }

        #endregion

        #region 装包
        public DateTime MsgTimeOfDateTime { get; set; }
        public MsgBody(int msgType, int msgFlag, byte[] msgData)
        {
            this.msgType = msgType;
            this.msgFlag = msgFlag;
            this.msgData = msgData;
            MsgTimeOfDateTime = DateTime.Now;
            this.msgTime = TimeTool.TimeToTimestamp(MsgTimeOfDateTime);
        }

        public int MsgBodySize { get => 16 + msgData.Length; }

        /// <summary>
        /// 将消息体转码为byte
        /// </summary>
        /// <param name="freeSpan"></param>
        /// <param name="offset"></param>
        /// <returns>总占用大小</returns>
        public int OutputMsgBodyToSpan(Span<byte> freeSpan, int offset)
        {
            int startIndex = offset;
            ConvertTypeTool.Int32ToSpan(msgType, freeSpan, startIndex);
            startIndex += 4;
            ConvertTypeTool.LongToSpan(msgTime, freeSpan, startIndex);
            startIndex += 8;
            ConvertTypeTool.Int32ToSpan(msgFlag, freeSpan, startIndex);
            startIndex += 4;

            Span<byte> msgDataSpan = new Span<byte>(msgData);
            msgDataSpan.CopyTo(freeSpan[startIndex..]);
            startIndex += msgData.Length;

            return startIndex - offset;
        }
        #endregion
    }
}
