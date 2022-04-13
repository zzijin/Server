using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    internal class PageInfoMsg
    {
        public FileInfoMsg FileInfoMsg { get; set; }
        public int PageNumber { get; set; }
        public long Pageoffset { get; set; }
        public int PageSize { get; set; }
        public byte[] PageHashCode { get; set; }
        public bool OperatingState { get; set; }
    }
}
