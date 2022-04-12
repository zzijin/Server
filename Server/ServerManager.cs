using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.LoggingModule.Interface;
using Server.NetworkModule.Interface;
using System;

namespace Server
{
    class ServerManager: IServerManager
    {
        IConfigurationSection serverConfiguration;
        INetworkManager networkManager;
        ILoggingManager loggingManager;

        public ServerManager(IServiceProvider serviceProvider)
        {
            serverConfiguration = serviceProvider.GetService<IConfiguration>().GetSection("Server");
            networkManager = serviceProvider.GetService<INetworkManager>();
            loggingManager = serviceProvider.GetService<ILoggingManager>();
            InitServerConfiguration();
        }

        public void StartServer()
        {
            networkManager.StartNetworkService();
        }

        private void InitServerConfiguration()
        {
            //
        }

        private void InitThreadPoolConfiguration()
        {
            if (serverConfiguration.GetValue<bool>("ThreadPool:IsSet"))
            {
                
            }

        }
    }
}
