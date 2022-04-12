namespace Server.NetworkModule.Configuration
{
    class ListenConfig
    {
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static readonly int LISTEN_PORT = 24242;
        /// <summary>
        /// 排队等待连接数量
        /// </summary>
        public static readonly int LISTEN_BACKLOG = 20;
    }
}
