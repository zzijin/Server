using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ValueObject
{
    internal class PageInfoVO
    {
        private int fileID;
        /// <summary>
        /// 文件页地址
        /// </summary>
        private int pageIndex;
        /// <summary>
        /// 文件页在文件中的偏移量
        /// 而非在文件块
        /// </summary>
        private long pageOffset;
        private int pageSize;
        /// <summary>
        /// 用于验证本页数据的哈希码
        /// </summary>
        private ReadOnlyMemory<byte> pageHashCode;
        /// <summary>
        /// 页数据校验结果
        /// </summary>
        private bool pageVerification;

        public PageInfoVO(int id,int index,long offset,int size)
        {
            this.fileID = id;
            this.pageIndex = index;
            this.pageOffset = offset;
            this.pageSize = size;
        }

        public PageInfoVO(int id,int index,long offset,int size,ReadOnlyMemory<byte> hashCode)
        {
            fileID = id;
            pageIndex = index;
            long pageOffset = offset;
            pageSize = size;
            pageHashCode = hashCode;
        }

        public int FileID { get => fileID; set => fileID = value; }
        public int PageIndex { get => pageIndex; set => pageIndex = value; }
        public long PageOffset { get => pageOffset; set => pageOffset = value; }
        public int PageSize { get => pageSize; set => pageSize = value; }
        public ReadOnlyMemory<byte> PageHashCode { get => pageHashCode; set => pageHashCode = value; }
        public bool PageVerification { get => pageVerification; set => pageVerification = value; }
    }
}
