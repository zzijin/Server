using Server.NetworkModule.Configuration;
using Server.NetworkModule.ConnService.Connect;
using Server.NetworkModule.Interface;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.NetworkModule.ListenService
{
    /// <summary>
    /// 服务端监听类
    /// </summary>
    class ServerListen:IServerListen
    {
        private Socket listener;
        private IServiceProvider _provider;
        SocketAsyncEventArgs acceptEventArg;

        public ServerListen(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void StartListenServer()
        {
            acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.Completed += AcceptEventArg_Completed;

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, ListenConfig.LISTEN_PORT);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(iPEndPoint);

            //设置排队等待连接的最大数量
            listener.Listen(ListenConfig.LISTEN_BACKLOG);

            //开始监听
            StartAccept();
            Console.WriteLine("开始监听");
        }

        private void StartAccept()
        {
            // 清除使用的套接字对象
            acceptEventArg.AcceptSocket = null;

            //异步接收接入尝试时，返回值为true表示此IO操作为挂起状态，false表示此IO操作为同步
            //true时会出发SocketAsyncEventArgs.Completed事件，false则不会，所以此处需判断为false时，指定方法
            bool willRaiseEvent = listener.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            ConnTemporary connTemporary = new ConnTemporary(e.AcceptSocket, _provider);

            //处理下一个接入请求
            StartAccept();
        }
    }
}
