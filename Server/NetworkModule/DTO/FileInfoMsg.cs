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
        public bool OperatingState { get; set; }
    }
}
