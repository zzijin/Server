using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class FileInfoMsg
    {
        public int FileID { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string Msg { get; set; }

        public FileInfoMsg(string fileName, long fileSize)
        {
            FileName = fileName;
            FileSize = fileSize;
        }

        public FileInfoMsg(int fileID,string msg)
        {
            FileID = fileID;
            Msg = msg;
        }

        public FileInfoMsg(int fileID,string fileName,long fileSize,string msg)
        {
            FileID = fileID;
            FileName = fileName;
            FileSize = fileSize;
            Msg = msg;
        }
    }
}
