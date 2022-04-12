namespace Server.NetworkModule.Configuration
{
    class ConnConfig
    {
        #region 临时连接设置
        /// <summary>
        /// 最大等待接收时间
        /// </summary>
        public static int TEMPORARY_MAX_WAIT_RECEIVE_TIME = 60*1000;

        #endregion

        #region 简单连接设置
        /// <summary>
        /// 最大发送消息间隔
        /// </summary>
        public static int SIMPLE_SEND_MAX_INTERVAL = 1000;
        #endregion

        #region 正式连接设置
        /// <summary>
        /// 预留发送缓冲区大小
        /// </summary>
        public static int CONN_FIXED_SEND_BUFFER_SIZE = 1000;
        /// <summary>
        /// 链接接收缓冲区大小，单位byte
        /// </summary>
        public static int CONN_RECEIVE_BUFFER_SIZE = 10 * 1000;
        /// <summary>
        /// 链接发送缓冲区大小，单位byte
        /// </summary>
        public static int CONN_SEND_BUFFER_SIZE = 1 * 1024;
        /// <summary>
        ///  链接发送时间间隔，单位ms
        /// </summary>
        public static int CONN_SEND_MAX_INTERVAL = 50;
        /// <summary>
        ///  链接无消息休眠时间
        /// </summary>
        public static int CONN_NO_MSG_THREAD_SLEEP_TIME = 150;
        /// <summary>
        /// 心跳消息包发送最大间隔,单位毫秒
        /// </summary>
        public static int CONN_HEARTBEAT_MSG_MAX_INTERVAL = 60 * 1000;
        /// <summary>
        /// 断线重连最大等等时间间隔，单位毫秒
        /// </summary>
        public static int CONN_WAIT_RECONNECT_MAX_INTERVAL = 60 * 1000;
        #endregion
    }
}
