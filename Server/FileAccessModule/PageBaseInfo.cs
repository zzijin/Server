using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    internal class PageBaseInfo
    {
        /// <summary>
        /// 页在文件中的页数
        /// </summary>
        private int pageNumber;
        /// <summary>
        /// 页在文件中的偏移量
        /// </summary>
        private long pageOffset;
        /// <summary>
        /// 页大小
        /// </summary>
        private int pageSize;

        /// <summary>
        /// 页在文件中的页数
        /// </summary>
        public int PageNumber { get => pageNumber; set => pageNumber = value; }
        /// <summary>
        /// 页在文件中的偏移量
        /// </summary>
        public long PageOffset { get => pageOffset; set => pageOffset = value; }
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get => pageSize; set => pageSize = value; }
        /// <summary>
        /// 页末尾位置
        /// </summary>
        public long PageEndIndex { get => (int)(pageOffset + pageSize); }
    }
}
