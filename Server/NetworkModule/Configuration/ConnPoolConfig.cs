namespace Server.NetworkModule.Configuration
{
    class ConnPoolConfig
    {
        /// <summary>
        /// 链接池大小
        /// </summary>
        public static int CONNPOOL_SIZE = 1000;
        /// <summary>
        /// 链接等待最长队列
        /// </summary>
        public static int CONNPOOL_WAIT_QUEUE_LENGTH = 500;
        /// <summary>
        /// 链接池循环线程最大运行间隔时间
        /// </summary>
        public static int CONNPOOL_LOOP_TASK_MAX_INTERVAL = 1000;
    }
}
