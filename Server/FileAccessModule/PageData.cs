using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    class PageData:PageDataBase
    {
        private PageDataState pageCacheState;

        public PageData(PageBaseInfo pageBaseInfo,FileBaseInfo fileBaseInfo):base(pageBaseInfo, fileBaseInfo)
        {
            PageNoCacheData();
        }

        #region Get
        /// <summary>
        /// 获取本页数据
        /// </summary>
        /// <returns></returns>
        public new ReadOnlyMemory<byte> GetPageData()
        {
            if (pageCacheState == PageDataState.FullData)
            {
                return base.GetPageData();
            }
            return null;
        }

        /// <summary>
        /// 获取文件指定位置和长度的数据
        /// </summary>
        /// <param name="offset">获取偏移量，此偏移量为在文件中的偏移量</param>
        /// <param name="size">获取长度</param>
        /// <returns></returns>
        public new ReadOnlyMemory<byte> GetPageData(PageBaseInfo getInfo)
        {
            if (pageCacheState == PageDataState.FullData)
            {
                return base.GetPageData(getInfo);
            }
            return null;
        }
        #endregion

        #region Set
        /// <summary>
        /// 设置文件页仅读取操作
        /// </summary>
        /// <returns></returns>
        public new Memory<byte> SetPageData()
        {
            PageComplementCacheData();
            return base.SetPageData();
        }
        #endregion

        #region 数据状态
        /// <summary>
        /// 检查页数据状态是否正确
        /// </summary>
        /// <returns></returns>
        public bool CheckPageStateIsRight()
        {
            if (pageCacheState == PageDataState.ComplementData)
            {
                return true;
            }

            if (pageCacheState == PageDataState.FullData)
            {
                if(pageData == null)
                {
                    PageErrorCacheData();
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckPageStateIsFull()
        {
            if (pageCacheState == PageDataState.FullData)
            {
                if (pageData != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void PageNoCacheData()
        {
            pageCacheState = PageDataState.NoData;
        }

        public void PageComplementCacheData()
        {
            pageCacheState = PageDataState.ComplementData;
        }

        public void PageFullCacheData()
        {
            pageCacheState = PageDataState.FullData;
        }

        public void PageErrorCacheData()
        {
            pageCacheState = PageDataState.ErrorData;
        }
        #endregion
    }

    internal enum PageDataState
    {
        /// <summary>
        /// 无数据
        /// </summary>
        NoData,
        /// <summary>
        /// 数据补全中
        /// </summary>
        ComplementData,
        /// <summary>
        /// 数据错误
        /// 获取失败或
        /// </summary>
        ErrorData,
        /// <summary>
        /// 数据完整
        /// </summary>
        FullData
    }
}
