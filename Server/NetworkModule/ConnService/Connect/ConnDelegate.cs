using Microsoft.Extensions.DependencyInjection;
using Server.NetworkModule.Interface;
using System;

namespace Server.NetworkModule.ConnService.Connect
{
    class ConnDelegate
    {
        private IConn _conn;
        private IConnPoolInternal _connPool;
        

        public ConnDelegate(IConn conn, IServiceProvider serviceProvider)
        {
            _conn = conn;
            _connPool = serviceProvider.GetService<IConnPoolInternal>();
        }

        internal IConn Conn { get => _conn; }
        internal IConnPoolInternal ConnPool { get => _connPool; set => _connPool = value; }
    }
}
