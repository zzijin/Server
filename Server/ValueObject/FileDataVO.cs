using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ValueObject
{
    internal class FileDataVO
    {
        private FileInfoVO fileInfo;
        private LinkedList<ReadOnlyMemory<byte>> fileData;

        public FileDataVO(FileInfoVO fileInfoVO)
        {
            this.fileInfo = fileInfoVO;
            fileData = new LinkedList<ReadOnlyMemory<byte>>();
        }

        public FileDataVO(FileInfoVO fileInfoVO,ReadOnlyMemory<byte> fileData)
        {
            this.fileInfo = fileInfoVO;
            this.fileData = new LinkedList<ReadOnlyMemory<byte>>();
            this.fileData.AddLast(fileData);
        }

        public LinkedList<ReadOnlyMemory<byte>> FileData { get => fileData; set => fileData = value; }
        internal FileInfoVO FileInfo { get => fileInfo; set => fileInfo = value; }

        public void AddFilePackData(ReadOnlyMemory<byte> data)
        {
            fileData.AddLast(data);
        }
    }

}
