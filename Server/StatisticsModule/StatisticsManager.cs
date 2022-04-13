using Microsoft.Extensions.DependencyInjection;
using Server.NetworkModule.ConnService.StatisticsInfo;
using Server.NetworkModule.Interface;
using Server.StatisticsModule.Interface;
using System;
using System.Collections.Generic;

namespace Server.StatisticsModule
{
    class StatisticsManager: IStatisticsManager
    {
        IConnPoolExternal _connPoolExternal;
        public StatisticsManager(IServiceProvider serviceProvider)
        {
            _connPoolExternal = serviceProvider.GetService<IConnPoolExternal>();
            InitStatisticsConfiguration();
        }

        private void InitStatisticsConfiguration()
        {
            //
        }

        public void OutputConnPoolInfoToConsole()
        {
            Console.Clear();
            Console.WriteLine("链接池信息统计");

            Console.Write("链接序号");Console.CursorLeft = 20;Console.Write("链接状态");Console.CursorLeft = 40; Console.Write("链接使用时长");
            Console.CursorLeft = 60; Console.Write("链接使用次数");Console.CursorLeft = 80;Console.Write("链接接收字节数"); Console.CursorLeft = 100;
            Console.Write("链接发送字节数"); Console.CursorLeft = 120; Console.Write("链接解析数"); Console.CursorLeft = 140; Console.Write("链接执行数");
            Console.CursorLeft = 160; Console.Write("链接发送数");

            Console.WriteLine();

            foreach (var item in _connPoolExternal.GetConnPoolInfo().GetConnPoolInfo())
            {
                Console.Write(item.FixedInfoDO.FixedIndex); Console.CursorLeft = 20; 
                Console.Write(item.OnceInfoDO.OnceNode!=null); Console.CursorLeft = 40; 
                Console.Write(item.TotalInfoDO.TotalUseTime.TotalSeconds); Console.CursorLeft = 60; 
                Console.Write(item.TotalInfoDO.TotalUsedCount); Console.CursorLeft = 80; 
                Console.Write(item.TotalInfoDO.TotalReceiveBytes); Console.CursorLeft = 100;
                Console.Write(item.TotalInfoDO.TotalSendBytes); Console.CursorLeft = 120; 
                Console.Write(item.TotalInfoDO.TotalParseMsg); Console.CursorLeft = 140; 
                Console.Write(item.TotalInfoDO.TotalExecuteMsg); Console.CursorLeft = 160; 
                Console.Write(item.TotalInfoDO.TotalSendMsg);
                Console.WriteLine();
            }
        }

        public void OutputUsedConnInfoToConsole()
        {
            Console.Clear();
            Console.WriteLine("链接池信息统计");

            Console.Write("链接序号"); Console.CursorLeft = 20; Console.Write("链接状态"); Console.CursorLeft = 40; Console.Write("链接使用时长");
            Console.CursorLeft = 60; Console.Write("链接使用次数"); Console.CursorLeft = 80; Console.Write("链接接收字节数"); Console.CursorLeft = 100;
            Console.Write("链接发送字节数"); Console.CursorLeft = 120; Console.Write("链接解析数"); Console.CursorLeft = 140; Console.Write("链接执行数");
            Console.CursorLeft = 160; Console.Write("链接发送数");

            Console.WriteLine();

            foreach (var item in _connPoolExternal.GetConnPoolInfo().GetUsedConnInfo())
            {
                Console.Write(item.FixedInfoDO.FixedIndex); Console.CursorLeft = 20;
                Console.Write(item.OnceInfoDO.OnceNode != null); Console.CursorLeft = 40;
                Console.Write((DateTime.Now-item.OnceInfoDO.OnceStartUseTime).TotalSeconds); Console.CursorLeft = 60;
                Console.Write(item.TotalInfoDO.TotalUsedCount); Console.CursorLeft = 80;
                Console.Write(item.OnceInfoDO.OnceReceiveBytes); Console.CursorLeft = 100;
                Console.Write(item.OnceInfoDO.OnceSendBytes); Console.CursorLeft = 120;
                Console.Write(item.OnceInfoDO.OnceParseMsg); Console.CursorLeft = 140;
                Console.Write(item.OnceInfoDO.OnceExecuteMsg); Console.CursorLeft = 160;
                Console.Write(item.OnceInfoDO.OnceSendTimes);
                Console.WriteLine();
            }
        }


    }
}
