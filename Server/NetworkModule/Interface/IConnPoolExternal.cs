using Server.NetworkModule.ConnService.StatisticsInfo;

namespace Server.NetworkModule.Interface
{
    /// <summary>
    /// 线程池对外方法
    /// </summary>
    interface IConnPoolExternal
    {
        public void Init();
        public ConnPoolInfo GetConnPoolInfo();
    }
}
