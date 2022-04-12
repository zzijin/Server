using Server.NetworkModule.ConnService.Message.MsgFormat;
using System;
using System.Collections.Generic;

namespace Server.NetworkModule.ConnService.Connect
{
    class ConnMsgParse
    {
        /// <summary>
        /// 正在解析中的消息包
        /// 防止因数据不全重新接收完成后重复解析
        /// </summary>
        private MsgPack parsingMsg;

        #region Conn解析用
        /// <summary>
        /// 链接缓冲区资源管理
        /// </summary>
        private ConnBufferManager _connBufferManager;

        public ConnMsgParse(ConnBufferManager connBufferManager)
        {
            _connBufferManager = connBufferManager;
            parsingMsg = new MsgPack();
        }

        /// <summary>
        /// 解析消息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MsgBody> ParseMsg()
        {
            while (true)
            {
                //尝试解析固定头
                //若正在解析的消息包剩余长度值未知，则需解析固定头
                if (parsingMsg.MsgCount == -1)
                {
                    ReadOnlySpan<byte> oneHeadSpan, twoHeadSpan;
                    if (_connBufferManager.ReadFromReceiveBufferBySpan(parsingMsg.MsgFixedHeadSize, out oneHeadSpan, out twoHeadSpan))
                    {
                        ProcessMsgFixedHead(oneHeadSpan, twoHeadSpan);

                        _connBufferManager.FreeReceiveBuffer(parsingMsg.MsgFixedHeadSize);
                    }
                    else
                    {
                        yield break;
                    }
                }

                //解析消息体
                ReadOnlySpan<byte> oneBodySpan, twoBodySpan;
                //读取失败
                if (!_connBufferManager.ReadFromReceiveBufferBySpan(parsingMsg.MsgCount, out oneBodySpan, out twoBodySpan))
                {
                    yield break;
                }

                yield return ProcessMsgBody(oneBodySpan, twoBodySpan);
                _connBufferManager.FreeReceiveBuffer(parsingMsg.MsgCount);
                parsingMsg.Init();
            }
        }

        #endregion

        #region ConnTemporary用
        public ConnMsgParse()
        {
            parsingMsg = new MsgPack();
        }

        public MsgBody ParseMsg(byte[] msg)
        {
            if (msg.Length == 0)
            {
                return null;
            }

            ReadOnlySpan<byte> headSpan = new ReadOnlySpan<byte>(msg, 0, parsingMsg.MsgFixedHeadSize);
            ProcessMsgFixedHead(headSpan, null);

            if ((parsingMsg.MsgCount + parsingMsg.MsgFixedHeadSize) > msg.Length)
            {
                return null;
            }

            ReadOnlySpan<byte> bodySpan = new ReadOnlySpan<byte>(msg, parsingMsg.MsgFixedHeadSize, parsingMsg.MsgCount);
            return ProcessMsgBody(bodySpan, null);
        }
        #endregion

        /// <summary>
        /// 解析固定头
        /// </summary>
        /// <returns></returns>
        private void ProcessMsgFixedHead(ReadOnlySpan<byte> oneSpan,ReadOnlySpan<byte> twoSpan)
        {
            //若第二个Span无值
            if (twoSpan == null)
            {
                parsingMsg.MsgFixedHeadParser(oneSpan);
            }
            else
            {
                parsingMsg.MsgFixedHeadParser(oneSpan, twoSpan);
            }
        }

        /// <summary>
        /// 解析消息体
        /// </summary>
        /// <returns></returns>
        private MsgBody ProcessMsgBody(ReadOnlySpan<byte> oneSpan, ReadOnlySpan<byte> twoSpan)
        {
            MsgBody msgBody;
            if (twoSpan == null)
            {
                msgBody = parsingMsg.InputMsgBodyFromSpan(oneSpan);
            }
            else
            {
                msgBody = parsingMsg.InputMsgBodyFromSpan(oneSpan, twoSpan);
            }
            return msgBody;
        }
    }
}
