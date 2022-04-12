using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    /// <summary>
    /// 页数据基础类
    /// 需要继承使用
    /// </summary>
    class PageDataBase
    {
        private protected FileBaseInfo fileInfo;
        /// <summary>
        /// 页在文件中的页数
        /// </summary>
        private protected PageBaseInfo pageBaseInfo;
        /// <summary>
        /// 页数据
        /// </summary>
        private protected byte[] pageData;

        public int PageNumber { get => pageBaseInfo.PageNumber; }
        public long PageOffset { get => pageBaseInfo.PageOffset; }
        public int PageSize { get => pageBaseInfo.PageSize; }
        public long PageEndIndex { get => (pageBaseInfo.PageEndIndex); }
        public int FileID { get => fileInfo.FileID; }
        public string FilePath { get => fileInfo.FilePath; }
        public long FileOffset { get => fileInfo.FileOffset; }
        public long FileSize { get => fileInfo.FileSize; }
        public PageBaseInfo PageBaseInfo { get => pageBaseInfo; }

        public PageDataBase(PageBaseInfo pageBaseInfo, FileBaseInfo filebaseInfo)
        {
            this.pageBaseInfo= pageBaseInfo;
            fileInfo = filebaseInfo;
        }

        #region Get

        /// <summary>
        /// 获取本页数据
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<byte> GetPageData()
        {
            if (pageData == null)
            {
                return null;
            }
            return new ReadOnlyMemory<byte>(pageData);
        }

        /// <summary>
        /// 获取文件指定位置和长度的数据
        /// 获取的数据均位于本页
        /// </summary>
        /// <param name="offset">获取偏移量，此偏移量为在文件中的偏移量</param>
        /// <param name="size">获取长度</param>
        /// <returns></returns>
        public ReadOnlyMemory<byte> GetPageData(PageBaseInfo getInfo)
        {
            if (pageData == null)
            {
                return null;
            }
            return new ReadOnlyMemory<byte>(pageData).Slice((int)(getInfo.PageOffset - pageBaseInfo.PageOffset), getInfo.PageSize);
        }

        #endregion

        #region Set

        /// <summary>
        /// 设置本页数据
        /// </summary>
        /// <returns></returns>
        public Memory<byte> SetPageData()
        {
            if (pageData == null)
            {
                pageData = new byte[pageBaseInfo.PageSize];
            }

            return new Memory<byte>(pageData);
        }

        /// <summary>
        /// 将数据设置到文件的指定位置
        /// 设置的数据均位于本页
        /// </summary>
        /// <param name="offset">存入偏移位置，此偏移量为在文件中的偏移量</param>
        /// <param name="data">存入数据</param>
        public void SetPageData(PageBaseInfo setInfo, ReadOnlyMemory<byte> data)
        {
            if (pageData == null)
            {
                pageData = new byte[pageBaseInfo.PageSize];
            }

            Memory<byte> dst = new Memory<byte>(pageData).Slice((int)(setInfo.PageOffset - pageBaseInfo.PageOffset), data.Length);
            data.CopyTo(dst);
        }

        #endregion
    }
}
