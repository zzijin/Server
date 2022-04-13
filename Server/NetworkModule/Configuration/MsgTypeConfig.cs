using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.Configuration
{
    class MsgTypeConfig
    {
        /// <summary>
        /// 测试消息
        /// </summary>
        public static int MSG_TYPE_TEST = 0;

        #region 客户端连接类型，是收到的第一条消息
        /// <summary>
        /// 使用默认方式连接
        /// </summary>
        public static int MSG_TYPE_CONNECT_DEFAULT=1;
        /// <summary>
        /// 使用令牌尝试重连
        /// </summary>
        public static int MSG_TYPE_CONNECT_TOKEN = 2;
        /// <summary>
        /// 连接成功
        /// </summary>
        public static int MSG_TYPE_CONNECT_SUCCESS = 5;
        /// <summary>
        /// 连接成功
        /// </summary>
        public static int MSG_TYPE_RECONNECT_SUCCESS = 6;
        /// <summary>
        /// 连接排队中
        /// </summary>
        public static int MSG_TYPE_CONNECT_WAIT = 7;
        /// <summary>
        /// 连接失败
        /// </summary>
        public static int MSG_TYPE_CONNECT_FAIL = 8;
        /// <summary>
        /// 使用令牌重连失败
        /// </summary>
        public static int MSG_TYPE_CONNECT_TOKEN_FAIL = 9;
        #endregion

        #region 通讯模块功能消息类型
        /// <summary>
        /// 心跳消息
        /// </summary>
        public static int MSG_TYPE_HEARTBEAT = 10;
        /// <summary>
        /// 创建令牌
        /// </summary>
        public static int MSG_TYPE_CREATE_TOKEN = 11;
        /// <summary>
        /// 删除令牌
        /// </summary>
        public static int MSG_TYPE_DELETE_TOKEN = 12;
        /// <summary>
        /// 消息类型错误
        /// </summary>
        public static int MSG_TYPE_ERROR = 21;
        #endregion

        #region 默认文件消息类型
        /// <summary>
        /// 文件下载消息
        /// </summary>
        public static int MSG_TYPE_FILE_DOWNLOAD_INFO = 30;
        /// <summary>
        /// 文件下载数据
        /// </summary>
        public static int MSG_TYPE_FILE_DOWNLOAD_DATA = 31;
        /// <summary>
        /// 文件下载验证
        /// </summary>
        public static int MSG_TYPE_FILE_DOWNLOAD_VERIFY = 32;
        /// <summary>
        /// 文件上传信息
        /// </summary>
        public static int MSG_TYPE_FILE_UPLOAD_INFO = 40;
        /// <summary>
        /// 文件上传数据
        /// </summary>
        public static int MSG_TYPE_FILE_UPLOAD_DATA = 41;
        /// <summary>
        /// 文件上传验证
        /// </summary>
        public static int MSG_TYPE_FILE_UPLOAD_VERIFY = 42;
        /// <summary>
        /// 文件上传剩余数据
        /// </summary>
        public static int MSG_TYPE_FILE_UPLOAD_REMAIN = 43;
        /// <summary>
        /// 文件上传完毕
        /// </summary>
        public static int MSG_TYPE_FILE_UPLOAD_OVER = 44;
        #endregion
    }
}
