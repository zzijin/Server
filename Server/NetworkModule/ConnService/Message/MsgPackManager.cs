using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.DTO;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Server.NetworkModule.ConnService.Message
{
    /// <summary>
    /// 消息封装
    /// </summary>
    class MsgPackManager
    {
        static readonly Dictionary<int, Func<int, byte[], MsgPack>> packDictionary 
            = new Dictionary<int, Func<int, byte[], MsgPack>>(){
                { MsgTypeConfig.MSG_TYPE_HEARTBEAT, PackHeartbeatMsg },
                { MsgTypeConfig.MSG_TYPE_CONNECT_FAIL,PackFailMsg },
                //{MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_INFO,PackDownloadFileInfo},
                //{MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_DATA,PackDownloadFileData},
                //{MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_VERIFY,PackDownloadFileVerify},
                //{MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_INFO,PackUploadInfo},
                //{MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_DATA,PackUploadData},
                //{MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_VERIFY,PackUploadverify},
                //{MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_OVER,PackUploadOver}

        };

        public static MsgPack PackMsg(int msgType, byte[] msgData)
        {
            //需特殊封装的消息
            if (packDictionary.ContainsKey(msgType))
            {
                return packDictionary[msgType](msgType, msgData);
            }
            //其他默认封装的消息
            else
            {
                return PackDefaultMsg(msgType, msgData);
            }
        }

        /// <summary>
        /// 按默认规则封装消息
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="msgData"></param>
        /// <returns></returns>
        static MsgPack PackDefaultMsg(int msgType, byte[] msgData)
        {
            return new MsgPack(new MsgBody(msgType, MsgConfig.MSG_FLAG_NO_ENCRYPTION, msgData));
        }

        static MsgPack PackHeartbeatMsg(int msgType, byte[] msgData)
        {
            if (msgData == null)
            {
                HeartbeatMsg heartbeatMsg = new HeartbeatMsg("这是一条心跳消息");
                msgData = JsonSerializer.SerializeToUtf8Bytes(heartbeatMsg);
            }
            return new MsgPack(new MsgBody(msgType, MsgConfig.MSG_FLAG_NO_ENCRYPTION, msgData));
        }

        static MsgPack PackFailMsg(int msgType, byte[] msgData)
        {
            if (msgData == null)
            {
                ConnectFailMsg failMsg = new ConnectFailMsg("首消息包类型错误或是接收消息超时");
                msgData = JsonSerializer.SerializeToUtf8Bytes(failMsg);
            }
            return new MsgPack(new MsgBody(msgType, MsgConfig.MSG_FLAG_NO_ENCRYPTION, msgData));
        }

        //private static MsgPack PackUploadOver(int msgType, byte[] msgData)
        //{
            
        //}

        //private static MsgPack PackUploadverify(int msgType, byte[] msgData)
        //{
        //    throw new NotImplementedException();
        //}

        //private static MsgPack PackUploadData(int msgType, byte[] msgData)
        //{
        //    throw new NotImplementedException();
        //}

        //private static MsgPack PackUploadInfo(int msgType, byte[] msgData)
        //{
        //    throw new NotImplementedException();
        //}

        //private static MsgPack PackDownloadFileVerify(int msgType, byte[] msgData)
        //{
        //    throw new NotImplementedException();
        //}

        //private static MsgPack PackDownloadFileData(int msgType, byte[] msgData)
        //{
        //    throw new NotImplementedException();
        //}

        //private static MsgPack PackDownloadFileInfo(int msgType, byte[] msgData)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
