using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Connect;
using Server.NetworkModule.ConnService.Message.MsgFormat;
using Server.NetworkModule.DTO;
using Server.ValueObject;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Server.NetworkModule.ConnService.Message
{
    /// <summary>
    /// 消息执行
    /// </summary>
    class MsgExecuteManager
    {
        static readonly Dictionary<int, Action<MsgBody, ConnDelegate>> executeDictionary 
            = new Dictionary<int, Action<MsgBody, ConnDelegate>>() {
                { MsgTypeConfig.MSG_TYPE_HEARTBEAT,ExecuteHeartbeatMsg },
                { MsgTypeConfig.MSG_TYPE_TEST , ExecuteTestMsg },
                { MsgTypeConfig.MSG_TYPE_CREATE_TOKEN,ExecuteCreateToken },
                {MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_INFO,ExecuteDownloadFileInfo},
                {MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_DATA,ExecuteDownloadFileData},
                {MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_VERIFY,ExecuteDownloadFileVerify},
                {MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_INFO,ExecuteUploadInfo},
                {MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_DATA,ExecuteUploadData},
                {MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_VERIFY,ExecuteUploadverify},
                {MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_OVER,ExecuteUploadOver}
            };

        public static void ExecuteMsg(MsgBody msgBody, ConnDelegate connDelegate)
        {
            if (executeDictionary.ContainsKey(msgBody.MsgType))
            {
                executeDictionary[msgBody.MsgType](msgBody, connDelegate);
            }
            //不在字典中的消息号进行错误处理
            else
            {
                ExecuteErrorMsg("未知的消息号", msgBody, connDelegate);
            }
        }

        private static void ExecuteTestMsg(MsgBody data, ConnDelegate connDelegate)
        {
            TestMsg testMsg = JsonSerializer.Deserialize<TestMsg>(data.MsgData);
            testMsg.AddSequenceNumber();
            byte[] backData = JsonSerializer.SerializeToUtf8Bytes(testMsg);

            connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_TEST, backData));
        }

        static void ExecuteHeartbeatMsg(MsgBody data, ConnDelegate connDelegate)
        {
            //回发心跳消息不用处理
        }

        private static void ExecuteCreateToken(MsgBody data, ConnDelegate connDelegate)
        {
            ConnTokenMsg tokenMsg = JsonSerializer.Deserialize<ConnTokenMsg>(data.MsgData);

            byte[] openID, key;
            if(connDelegate.Conn.GenerateToken(tokenMsg.ClientID, out openID, out key))
            {
                tokenMsg.OpenID = openID;
                tokenMsg.Key = key;
                tokenMsg.Msg = $"生成令牌成功,[openID:{openID.Length}],[key:{key.Length}]";
            }
            else
            {
                tokenMsg.Msg = "生成令牌失败,令牌可能已存在";
            }

            byte[] backData = JsonSerializer.SerializeToUtf8Bytes(tokenMsg);
            connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_CREATE_TOKEN, backData));
        }

        static void ExecuteErrorMsg(string errorInfo,MsgBody data, ConnDelegate connDelegate)
        {
            ErrorMsg errorMsg = new ErrorMsg(data.MsgType,errorInfo, data.MsgData);
            byte[] backData = JsonSerializer.SerializeToUtf8Bytes(errorMsg);

            connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_ERROR, backData));
        }


        private static void ExecuteUploadOver(MsgBody data, ConnDelegate connDelegate)
        {
            FileInfoMsg fileInfoMsg=JsonSerializer.Deserialize<FileInfoMsg>(data.MsgData);
            
            FileInfoVO fileInfoVO=new FileInfoVO(fileInfoMsg.FileID);
            LinkedList<PageInfoVO> pageInfos;
            ExecuteBaseInfoResult executeBaseInfoResult=connDelegate.FileAccessManager.ConvertTemporaryFile(ref fileInfoVO, out pageInfos);

            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                fileInfoMsg.OperatingState= (executeBaseInfoResult.ExecuteResultState == ExecuteState.Success);

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    fileInfoMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                byte[] backData = JsonSerializer.SerializeToUtf8Bytes(fileInfoMsg);
                connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_VERIFY, backData));

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    if (pageInfos.Count > 0)
                    {
                        foreach (PageInfoVO pageInfo in pageInfos)
                        {

                            PageInfoMsg backPagesMsg = new PageInfoMsg()
                            {
                                FileInfoMsg = fileInfoMsg,
                                PageNumber = pageInfo.PageIndex,
                                Pageoffset = pageInfo.PageOffset,
                                PageSize = pageInfo.PageSize,
                                OperatingState = false,
                            };

                            backData = JsonSerializer.SerializeToUtf8Bytes(backPagesMsg);
                            connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_REMAIN, backData));
                        }
                    }
                }
            }
        }

        private static void ExecuteUploadverify(MsgBody data, ConnDelegate connDelegate)
        {
            PageInfoMsg pageInfoMsg = JsonSerializer.Deserialize<PageInfoMsg>(data.MsgData);

            PageInfoVO pageInfoVO = new PageInfoVO(pageInfoMsg.FileInfoMsg.FileID, pageInfoMsg.PageNumber, pageInfoMsg.Pageoffset, pageInfoMsg.PageSize,pageInfoMsg.PageHashCode);
            ExecuteBaseInfoResult executeBaseInfoResult=connDelegate.FileAccessManager.PageDataVerification(ref pageInfoVO);

            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                pageInfoMsg.OperatingState = (executeBaseInfoResult.ExecuteResultState == ExecuteState.Success);
                pageInfoMsg.PageHashCode = null;

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    pageInfoMsg.FileInfoMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                byte[] backData = JsonSerializer.SerializeToUtf8Bytes(pageInfoMsg);
                connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_VERIFY, backData));
            }
        }

        private static void ExecuteUploadData(MsgBody data, ConnDelegate connDelegate)
        {
            FileDataMsg fileDataMsg=JsonSerializer.Deserialize<FileDataMsg>(data.MsgData);

            FileDataVO fileDataVO = new FileDataVO(
                new FileInfoVO(fileDataMsg.FileID, fileDataMsg.FileOffset, fileDataMsg.FileSize),
                fileDataMsg.FileData);
            ExecuteBaseInfoResult executeBaseInfoResult=connDelegate.FileAccessManager.SetTemporaryFileData(ref fileDataVO);

            if(executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                fileDataMsg.FileData = null;
                fileDataMsg.OperatingState = (executeBaseInfoResult.ExecuteResultState==ExecuteState.Success);

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    fileDataMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                byte[] backData = JsonSerializer.SerializeToUtf8Bytes(fileDataMsg);
                connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_DATA, backData));
            }
        }

        private static void ExecuteUploadInfo(MsgBody data, ConnDelegate connDelegate)
        {
            FileInfoMsg fileInfoMsg=JsonSerializer.Deserialize<FileInfoMsg>(data.MsgData);
            FileInfoVO fileInfoVO=new FileInfoVO(fileInfoMsg.FileName,fileInfoMsg.FileSize);

            ExecuteBaseInfoResult executeBaseInfoResult=connDelegate.FileAccessManager.CreateTemporaryFile(ref fileInfoVO);

            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                fileInfoMsg.FileID=fileInfoVO.FileID;
                fileInfoMsg.OperatingState = (executeBaseInfoResult.ExecuteResultState == ExecuteState.Success);

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    fileInfoMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                byte[] backData = JsonSerializer.SerializeToUtf8Bytes(fileInfoMsg);
                connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_INFO, backData));
            }
        }

        private static void ExecuteDownloadFileVerify(MsgBody data, ConnDelegate connDelegate)
        {
            PageInfoMsg pageInfoMsg = JsonSerializer.Deserialize<PageInfoMsg>(data.MsgData);
            PageInfoVO pageInfoVO=new PageInfoVO(pageInfoMsg.FileInfoMsg.FileID,pageInfoMsg.PageNumber,pageInfoMsg.Pageoffset,pageInfoMsg.PageSize);

            ExecuteBaseInfoResult executeBaseInfoResult = connDelegate.FileAccessManager.GetFileVerification(ref pageInfoVO);

            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                pageInfoMsg.PageHashCode = pageInfoVO.PageHashCode.ToArray();
                pageInfoMsg.OperatingState = (executeBaseInfoResult.ExecuteResultState == ExecuteState.Success);

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    pageInfoMsg.FileInfoMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                byte[] backData = JsonSerializer.SerializeToUtf8Bytes(pageInfoMsg);
                connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_UPLOAD_INFO, backData));
            }
        }

        private static void ExecuteDownloadFileData(MsgBody data, ConnDelegate connDelegate)
        {
            FileDataMsg fileDataMsg=JsonSerializer.Deserialize<FileDataMsg>(data.MsgData);
            FileDataVO fileDataVO = new FileDataVO(new FileInfoVO(fileDataMsg.FileID, fileDataMsg.FileOffset, fileDataMsg.FileSize));

            ExecuteBaseInfoResult executeBaseInfoResult = connDelegate.FileAccessManager.GetFileData(ref fileDataVO);

            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                fileDataMsg.OperatingState = (executeBaseInfoResult.ExecuteResultState == ExecuteState.Success);

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    fileDataMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                if(executeBaseInfoResult.ExecuteResultState == ExecuteState.Success)
                {
                    foreach(var packData in fileDataVO.FileData)
                    {
                        fileDataMsg.FileData = packData.ToArray();

                        byte[] backData = JsonSerializer.SerializeToUtf8Bytes(fileDataMsg);
                        connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_INFO, backData));
                    }
                }
            }
        }

        private static void ExecuteDownloadFileInfo(MsgBody data, ConnDelegate connDelegate)
        {
            FileInfoMsg fileInfoMsg= JsonSerializer.Deserialize<FileInfoMsg>(data.MsgData);
            FileInfoVO fileInfoVO = new FileInfoVO(fileInfoMsg.FileID);

            ExecuteBaseInfoResult executeBaseInfoResult = connDelegate.FileAccessManager.GetFileInfo(ref fileInfoVO);

            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                connDelegate.Conn.EnqueueToWaitExecuteMsgQueue(data);
            }
            else
            {
                fileInfoMsg.FileName = fileInfoVO.FileName;
                fileInfoMsg.FileSize= fileInfoVO.FileSize;
                fileInfoMsg.OperatingState = (executeBaseInfoResult.ExecuteResultState == ExecuteState.Success);

                if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
                {
                    fileInfoMsg.Msg = executeBaseInfoResult.ExecuteResultMsg;
                }

                byte[] backData = JsonSerializer.SerializeToUtf8Bytes(fileInfoMsg);
                connDelegate.Conn.EnqueueToWaitSendMsgQueue(MsgPackManager.PackMsg(MsgTypeConfig.MSG_TYPE_FILE_DOWNLOAD_INFO, backData));
            }
        }
    }
}
