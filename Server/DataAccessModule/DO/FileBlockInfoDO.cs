using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.DO
{
    [Table("FileBlockInfo")]
    internal class FileBlockInfoDO
    {
        [Key]
        public int FileBlockID { get; set; }
        public string FileBlockPath { get; set; }
        public long FileBlockSize { get; set; }
        public long FileBlockFreeSize { get; set; }
        public DateTime FileBlockLastEditDate { set; get; }
        public string FileBlockLastEditUser { get;set; }
        public bool FileBlockUnfailed { get; set; } = true;
    }
}
