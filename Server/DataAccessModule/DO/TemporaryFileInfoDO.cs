using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.DO
{
    [Table("TemporaryFileInfo")]
    internal class TemporaryFileInfoDO
    {
        [Key]
        public int TemporaryFileID { get; set; }
        public string TemporaryFileName { get; set; }
        public string TemporaryFilePath { get; set; }
        public long TemporaryFileSize { get; set; }
        public DateTime TemporaryFileLastEditDate { get; set; }
        public string TemporaryFileLastEditUser { get; set; }
        public bool TemporaryFileUnfailed { get; set; } = true;
    }
}
