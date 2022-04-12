using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.DO
{
    [Table("FileInfo")]
    internal class FileInfoDO
    {
        [Key]
        public int FileID { get; set; }
        public string FileName { get; set; }
        public int FileBlockID { get; set; }
        public long FileOffset { get; set; }
        public long FileSize { get; set; }
        public DateTime FileLastEditDate { get; set; }
        public string FileLastEditUser { get; set; }
        public bool FileUnfailed { get; set; } = true;
    }
}
