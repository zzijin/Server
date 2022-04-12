using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ValueObject
{
    internal class FileBlockInfoVO
    {
        private int fileBlockID;
        private long fileBlockSize;
        private string fileBlockPath;
        private bool fileBlockUnfailed;

        public FileBlockInfoVO(long size)
        {
            fileBlockSize = size;
            fileBlockUnfailed = false;
        }

        public FileBlockInfoVO(int id,long size,string path,bool unfailed)
        {
            fileBlockID = id;
            fileBlockSize= size;
            fileBlockPath = path;
            fileBlockUnfailed= unfailed;
        }

        public int FileBlockID { get => fileBlockID; set => fileBlockID = value; }
        public string FileBlockPath { get => fileBlockPath; set => fileBlockPath = value; }
        public long FileBlockSize { get => fileBlockSize; set => fileBlockSize = value; }
        public bool FileBlockUnfailed { get => fileBlockUnfailed; set => fileBlockUnfailed = value; }
    }
}
