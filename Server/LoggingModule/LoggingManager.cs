using Server.LoggingModule.Interface;

namespace Server.LoggingModule
{
    class LoggingManager: ILoggingManager
    {
        public LoggingManager()
        {
            InitLogConfiguration();


        }

        /// <summary>
        /// 初始化日志模块配置
        /// </summary>
        private void InitLogConfiguration()
        {
            LoadLogConfiguration();
        }
        /// <summary>
        /// 从配置文件中加载日志文件配置
        /// </summary>
        private void LoadLogConfiguration()
        {

        }
    }
}
