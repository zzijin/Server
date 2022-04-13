using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule.Interface;
using Server.FileAccessModule;
using Server.LoggingModule.Interface;
using Server.NetworkModule;
using Server.NetworkModule.Interface;
using Server.StatisticsModule;
using Server.StatisticsModule.Interface;
using System;

namespace Server
{
    class ServerManager: IServerManager
    {
        IConfigurationSection serverConfiguration;
        NetworkManager networkManager;
        ILoggingManager loggingManager;
        StatisticsManager statisticsManager;
        IDataAccessManager dataAccessManager;
        FileAccessManager fileAccessManager;

        public ServerManager(IServiceProvider serviceProvider)
        {
            InitServerConfiguration();

            serverConfiguration = serviceProvider.GetService<IConfiguration>().GetSection("Server");
            networkManager = serviceProvider.GetService<NetworkManager>();
            loggingManager = serviceProvider.GetService<ILoggingManager>();
            statisticsManager = serviceProvider.GetService<StatisticsManager>();
            dataAccessManager = serviceProvider.GetService<IDataAccessManager>();
            fileAccessManager=serviceProvider.GetService<FileAccessManager>();
        }

        internal NetworkManager NetworkManager { get => networkManager; }
        internal ILoggingManager LoggingManager { get => loggingManager; }
        internal StatisticsManager StatisticsManager { get => statisticsManager; }
        internal IDataAccessManager DataAccessManager { get => dataAccessManager;}
        internal FileAccessManager FileAccessManager { get => fileAccessManager; }

        public void StartServer()
        {
            networkManager.StartNetworkService();
        }

        public void OutputConnPoolInfoToConsole()
        {
            statisticsManager.OutputConnPoolInfoToConsole();
        }

        public void OutputUsedConnInfoToConsole()
        {
            statisticsManager.OutputUsedConnInfoToConsole();
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
