using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    class FileStreamInfo
    {
        public FileBaseInfo FileBaseInfo { get; set; }
        public IEnumerable<PageStreamInfo> PageStreams { get; set; }
    }
    class PageStreamInfo
    {
        public PageBaseInfo PageBaseInfo { get; set; }
        public ReadOnlyMemory<byte> PageWriteDatas { get; set; }
        public Memory<byte> PageReadDatas { get; set; }
    }
}
