using Microsoft.Extensions.DependencyInjection;
using Server.NetworkModule.Interface;
using System;
using Server.FileAccessModule;

namespace Server.NetworkModule.ConnService.Connect
{
    class ConnDelegate
    {
        private IConn _conn;
        private IConnPoolInternal _connPool;
        private FileAccessManager _fileAccessManager;

        public ConnDelegate(IConn conn, IServiceProvider serviceProvider)
        {
            _conn = conn;
            _connPool = serviceProvider.GetService<IConnPoolInternal>();
            _fileAccessManager = serviceProvider.GetService<FileAccessManager>();
        }

        internal IConn Conn { get => _conn; }
        internal IConnPoolInternal ConnPool { get => _connPool; }
        internal FileAccessManager FileAccessManager { get => _fileAccessManager; }
    }
}
