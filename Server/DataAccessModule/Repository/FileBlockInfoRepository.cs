using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule.Converters;
using Server.DataAccessModule.DO;
using Server.FileAccessModule.Configuration;
using Server.Tools;
using Server.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.Repository
{
    internal class FileBlockInfoRepository
    {
        IDbContextFactory<ServerDbContext> _dbContextFactory;
        public FileBlockInfoRepository(IServiceProvider serviceProvider)
        {
            _dbContextFactory = serviceProvider.GetService<IDbContextFactory<ServerDbContext>>();
        }

        public ExecuteBaseInfoResult GetFileBlockInfo(ref FileBlockInfoVO fileBlockInfoVO)
        {
            FileBlockInfoDO fileBlockInfoDO = FileBlockInfoConvert.VOTransformToDO(fileBlockInfoVO);

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    fileBlockInfoDO=dbContext.FileBlockInfo.Find(fileBlockInfoDO.FileBlockID);
                }

                if (fileBlockInfoDO == null)
                {
                    throw new Exception($"FileBlockInfo表当前项不存在,FileBlockID:{fileBlockInfoVO.FileBlockID}");
                }

                fileBlockInfoVO = FileBlockInfoConvert.DOTransformToVO(fileBlockInfoDO);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加FileBlockInfo信息操作成功,FileBlockInfo:{fileBlockInfoDO.FileBlockID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加FileBlockInfo信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }

        public ExecuteBaseInfoResult AddFileBlockInfo(ref FileBlockInfoVO fileBlockInfoVO)
        {
            FileBlockInfoDO fileBlockInfoDO=FileBlockInfoConvert.VOTransformToDO(fileBlockInfoVO);

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    using(var job = dbContext.Database.BeginTransaction())
                    {
                        dbContext.FileBlockInfo.Add(fileBlockInfoDO);
                        dbContext.SaveChanges();

                        fileBlockInfoDO.FileBlockPath= $@"{FileConfig.Disk_File_Block_Base_Path}{TimeTool.NowTimeToTimestamp()} {fileBlockInfoDO.FileBlockID}";
                        dbContext.FileBlockInfo.Update(fileBlockInfoDO);
                        dbContext.SaveChanges();

                        job.Commit();
                    }
                }

                fileBlockInfoVO=FileBlockInfoConvert.DOTransformToVO(fileBlockInfoDO);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加FileBlockInfo信息操作成功,FileBlockInfo:{fileBlockInfoDO.FileBlockID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加FileBlockInfo信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }

        public ExecuteBaseInfoResult UpdataFileBlockInfo(ref FileBlockInfoVO fileBlockInfoVO)
        {
            FileBlockInfoDO fileBlockInfoDO = FileBlockInfoConvert.VOTransformToDO(fileBlockInfoVO);

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    dbContext.FileBlockInfo.Update(fileBlockInfoDO);
                    dbContext.SaveChanges();
                }

                fileBlockInfoVO = FileBlockInfoConvert.DOTransformToVO(fileBlockInfoDO);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行更新FileBlockInfo信息操作成功,FileBlockInfo:{fileBlockInfoDO.FileBlockID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行更新FileBlockInfo信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }
    }
}
