using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule;
using Server.DataAccessModule.Interface;
using Server.FileAccessModule;
using Server.FileAccessModule.Interface;
using Server.LoggingModule;
using Server.LoggingModule.Interface;
using Server.NetworkModule;
using Server.NetworkModule.Interface;
using Server.StatisticsModule;
using Server.StatisticsModule.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using Server.NetworkModule.ConnService.ConnPool;
using Server.NetworkModule.ListenService;
using Microsoft.EntityFrameworkCore;
using Server.DataAccessModule.Repository;

namespace Server
{
    /// <summary>
    /// 依赖注入管理
    /// </summary>
    public class IOCManager
    {
        private IServiceProvider ServiceProvider;

        /// <summary>
        /// 创建服务
        /// </summary>
        /// <returns></returns>
        public IServiceProvider BuildServiceProvider()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();

            //添加内存缓存
            serviceDescriptors.AddMemoryCache((options)=>
            {
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(FileAccessModule.Configuration.FileConfig.CACHE_EXPIRATION_SCAN_FREQUENCY);
                options.CompactionPercentage = FileAccessModule.Configuration.FileConfig.CACHE_COMPACTION_PERCENTAGE;
                options.SizeLimit = FileAccessModule.Configuration.FileConfig.CACHE_SIZE_LIMIT;
            });

            //添加配置
            serviceDescriptors.AddSingleton<IConfiguration>(sp =>
            {
                return new ConfigurationBuilder()
                .AddJsonFile("Configuration/serversettings.json")
                .AddJsonFile("Configuration/serversettings.Network.json")
                .AddJsonFile("Configuration/serversettings.DataAccess.json")
                .AddJsonFile("Configuration/serversettings.Logging.json")
                .AddJsonFile("Configuration/serversettings.Network.json")
                .AddJsonFile("Configuration/serversettings.Statistics.json")
                .Build();
            });

            //添加服务
            serviceDescriptors.AddServerService()
                .AddDataAcessService()
                .AddFileStreamService()
                .AddLogService()
                .AddNetworkService()
                .AddStatisticsService();

            ServiceProvider = serviceDescriptors.BuildServiceProvider();
            return ServiceProvider;
        }


    }
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddServerService(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors.AddSingleton<IServerManager, ServerManager>();
        }
        public static IServiceCollection AddDataAcessService(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors.AddSingleton<IDataAccessManager,DataAccessManager>()
                .AddDbContextFactory<ServerDbContext >()
                .AddSingleton<FileInfoRepository>()
                .AddSingleton<TemporaryFileRepository>()
                .AddSingleton<FileBlockInfoRepository>();
        }

        public static IServiceCollection AddFileStreamService(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors.AddSingleton<FileAccessManager>();
        }

        public static IServiceCollection AddLogService(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors.AddSingleton<ILoggingManager, LoggingManager>();
        }
        public static IServiceCollection AddNetworkService(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors.AddSingleton<NetworkManager>()
                .AddSingleton<IConnPoolExternal,ConnPool>()
                .AddSingleton<IServerListen,ServerListen>();

        }
        public static IServiceCollection AddStatisticsService(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors.AddSingleton<StatisticsManager>();
        }
    }
}
