using System.Collections.Generic;
using System.Linq;

namespace Server.NetworkModule.ConnService.StatisticsInfo
{
    /// <summary>
    /// 链接池信息
    /// </summary>
    class ConnPoolInfo
    {
        IEnumerable<ConnInfo> connInfos;

        public ConnPoolInfo()
        {

        }

        public void Init(IEnumerable<ConnInfo> connInfos)
        {
            this.connInfos = connInfos;
        }

        #region 单链接信息查询索引
        /// <summary>
        /// 查询已使用链接的本次连接统计信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ConnOnceInfoDO> GetConnOnceInfo()
        {
            var query = from connInfo in connInfos
                        where connInfo.OnceInfoDO!=null
                         select connInfo.OnceInfoDO;
            return query;
        }
        /// <summary>
        /// 查询所有连接的总统计信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ConnTotalInfoDO> GetConnTotalInfo()
        {
            var query = from connInfo in connInfos
                        select connInfo.TotalInfoDO;
            return query;
        }
        #endregion

        #region 链接池信息查询索引

        #endregion
    }
}
