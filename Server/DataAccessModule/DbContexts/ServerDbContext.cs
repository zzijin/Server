using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule.Configuration;
using Server.DataAccessModule.DO;

namespace Server.DataAccessModule
{
    
    class ServerDbContext : DbContext
    {
        public DbSet<TemporaryFileInfoDO> TemporaryFileInfo { get; set; }
        public DbSet<FileBlockInfoDO> FileBlockInfo { get; set; }
        public DbSet<FileInfoDO> FileInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@$"Server={DBConfig.SQLServerName};Database={DBConfig.SQLServerDatabase};Trusted_Connection=SSPI");
        }
    }
}
