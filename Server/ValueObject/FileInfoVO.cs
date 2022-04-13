using Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ValueObject
{
    internal class FileInfoVO
    {
        private int fileID;
        private string fileName;
        private string filePath;
        private long fileOffset;
        private long fileSize;

        public FileInfoVO(int id)
        {
            fileID = id;
        }

        public FileInfoVO(string name, long size)
        {
            if (name == null || name?.Length == 0)
            {
                name = @$"NoName {TimeTool.NowTimeToTimestamp()}";
            }
            fileName = name;
            fileSize = size;
        }

        public FileInfoVO(int id, string name, string path,long offset,long size)
        {
            fileID = id;
            fileName = name;
            filePath = path;
            fileOffset = offset;
            fileSize = size;
        }

        public FileInfoVO(int id, long offset, long size)
        {
            fileID = id;
            fileSize = size;
            fileOffset=offset;
        }

        public FileInfoVO(int id, string name, string path, long size)
        {
            fileID = id;
            fileName = name;
            filePath = path;
            fileSize = size;
        }

        public int FileID { get => fileID; set => fileID = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public string FilePath { get => filePath; set => filePath = value; }
        /// <summary>
        /// 获取或设置时，此值指在文件中的偏移量，而非在文件块
        /// </summary>
        public long FileOffset { get => fileOffset; set => fileOffset = value; }
        public long FileSize { get => fileSize; set => fileSize = value; }
    }
}
