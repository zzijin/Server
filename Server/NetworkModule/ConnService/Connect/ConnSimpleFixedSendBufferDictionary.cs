using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Message;
using Server.NetworkModule.DTO;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Server.NetworkModule.ConnService.Connect
{
    /// <summary>
    /// 仅用于简单链接的公用发送区
    /// </summary>
    class ConnSimpleFixedSendBufferDictionary
    {
        Dictionary<int, Memory<byte>> fixedSendBufferDictionary;
        private int freeIndex;
        private int freeSize;
        private Memory<byte> buffer;

        public ConnSimpleFixedSendBufferDictionary(Memory<byte> fixedBuffer)
        {
            fixedSendBufferDictionary = new Dictionary<int, Memory<byte>>();
            this.buffer = fixedBuffer;
            freeIndex = 0;
            freeSize = fixedBuffer.Length;
        }

        public ReadOnlySpan<byte> GetFixedSendMemory(int msgType)
        {
            if (fixedSendBufferDictionary.ContainsKey(msgType))
            {
                return fixedSendBufferDictionary[msgType].Span;
            }
            else
                return null;
        }

        public void AddConnectFailMsg()
        {
            if (freeSize > 100)
            {
                Span<byte> span = buffer.Slice(freeIndex, freeSize).Span;
                ConnectFailMsg connectFailMsg = new ConnectFailMsg("服务器已满或消息类别错误");
                byte[] failByte = JsonSerializer.SerializeToUtf8Bytes(connectFailMsg);
                int addSize = MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_CONNECT_FAIL, failByte).OutputMsgPackToSpan(span);

                fixedSendBufferDictionary.Add(MsgTypeConfig.MSG_TYPE_CONNECT_FAIL, buffer.Slice(freeIndex, addSize));
                freeIndex += addSize;
                freeSize -= addSize;
            }
        }

        public void AddHeartbeatMsg()
        {
            if (freeSize > 100)
            {
                Span<byte> span = buffer.Slice(freeIndex, freeSize).Span;

                int addSize = MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_HEARTBEAT, null).OutputMsgPackToSpan(span);

                freeIndex += addSize;
                freeSize -= addSize;
            }
        }
    }
}
