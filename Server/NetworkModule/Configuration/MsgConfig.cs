namespace Server.NetworkModule.Configuration
{
    class MsgConfig
    {
        /// <summary>
        /// 数据包开始标识符
        /// </summary>
        public static readonly byte MSG_START_TAG = 0x98;
        /// <summary>
        /// 数据包结束标识符
        /// </summary>
        public static readonly byte MSG_END_TAG = 0x99;
        /// <summary>
        /// 数据包最大大小(字节)
        /// </summary>
        public static readonly int MSG_MAX_SIZE = 1000;

        /// <summary>
        /// 消息加密标志
        /// </summary>
        public static int MSG_FLAG_NO_ENCRYPTION = 0;
    }
}
