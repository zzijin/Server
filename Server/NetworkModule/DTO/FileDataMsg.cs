using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    internal class FileDataMsg
    {
        public int FileID { get; set; }
        public long FileOffset { get;set; }
        public int FileSize { get; set; }
        public byte[] FileData { get; set; }
        public string Msg { get; set; }
        public bool OperatingState { get; set; }
    }
}
