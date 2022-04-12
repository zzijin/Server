using Server.NetworkModule.Interface;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Server.NetworkModule
{
    class NetworkManager: INetworkManager
    {
        IServerListen _connListen;
        IConnPoolExternal _connPool;

        public NetworkManager(IServiceProvider serviceProvider)
        {
            _connPool = serviceProvider.GetService<IConnPoolExternal>();
            _connListen = serviceProvider.GetService<IServerListen>();

            InitNetworkConfiguration();
        }

        /// <summary>
        /// 初始化有关网络模块的相关配置
        /// </summary>
        private void InitNetworkConfiguration()
        {
            LoadNetworkConfiguration();
        }

        /// <summary>
        /// 从配置文件中加载有关网络模块的相关配置
        /// </summary>
        private void LoadNetworkConfiguration()
        {

        }

        public void StartNetworkService()
        {
            _connPool.Init();
            _connListen.StartListenServer();

            Console.WriteLine("开启服务");
        }
    }
}
