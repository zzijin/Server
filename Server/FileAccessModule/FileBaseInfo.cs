using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    class FileBaseInfo
    {
        private int fileID;
        private string filePath;
        private long fileOffset;
        private long fileSize;

        public int FileID { get => fileID; set => fileID = value; }
        public string FilePath { get => filePath; set => filePath = value; }
        public long FileOffset { get => fileOffset; set => fileOffset = value; }
        public long FileSize { get => fileSize; set => fileSize = value; }
        public long FileEndIndex { get=> fileOffset + fileSize; }
    }
}
