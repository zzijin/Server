using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    class TemporaryPageData:PageDataBase
    {
        private PageDataState pageCacheState;

        public TemporaryPageData(PageBaseInfo pageBaseInfo, FileBaseInfo fileBaseInfo) : base(pageBaseInfo, fileBaseInfo)
        {
            PageCollectData();
        }

        #region Set

        public new void SetPageData(PageBaseInfo setInfo, ReadOnlyMemory<byte> data)
        {
            base.SetPageData(setInfo, data);
        }

        /// <summary>
        /// 验证页数据
        /// 验证通过会将Cache状态置为Full
        /// 此函数不检查CacheState
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool VerifyPageData(ReadOnlyMemory<byte> hash)
        {
            using (SHA256 cng = SHA256.Create())
            {
                byte[] pageHash = cng.ComputeHash(pageData);
                if (Array.Equals(cng, hash))
                {
                    PageFullData();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 设置页数据状态
        /// <summary>
        /// 检查数据是否
        /// </summary>
        /// <returns></returns>
        public bool PageDataIsFull()
        {
            if (pageCacheState == PageDataState.FullData)
            {
                if (pageData == null)
                {
                    PageCollectData();
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查页数据是否已被验证
        /// </summary>
        /// <returns></returns>
        public bool PageDataIsVerify()
        {
            return pageCacheState == PageDataState.FullData || pageCacheState == PageDataState.WrittenData;
        }

        public void PageCollectData()
        {
            pageCacheState = PageDataState.CollectData;
        }

        public void PageFullData()
        {
            pageCacheState = PageDataState.FullData;
        }

        public void PageWrittenData()
        {
            pageCacheState = PageDataState.WrittenData;
        }

        #endregion

        private enum PageDataState
        {
            /// <summary>
            /// 数据收集中
            /// </summary>
            CollectData,
            /// <summary>
            /// 数据已收集完成
            /// </summary>
            FullData,
            /// <summary>
            /// 已写入数据
            /// </summary>
            WrittenData,
        }
    }
}
