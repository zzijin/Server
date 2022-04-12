using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule.Configuration;
using Server.DataAccessModule.Converters;
using Server.DataAccessModule.DO;
using Server.DataAccessModule.Interface;
using Server.FileAccessModule.Configuration;
using Server.ValueObject;

namespace Server.DataAccessModule.Repository
{
    class TemporaryFileRepository
    {
        IDbContextFactory<ServerDbContext > _dbContextFactory;
        public TemporaryFileRepository(IServiceProvider serviceProvider)
        {
            _dbContextFactory = serviceProvider.GetService<IDbContextFactory<ServerDbContext>>();
        }

        public ExecuteBaseInfoResult GetTemporaryFileInfo(ref FileInfoVO fileInfoVO)
        {
            TemporaryFileInfoDO fileInfoDO = TemporaryFileInfoConvert.VOTransformToDO(fileInfoVO);

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    fileInfoDO=(from temp in dbContext.TemporaryFileInfo 
                                where temp.TemporaryFileID == fileInfoDO.TemporaryFileID&&temp.TemporaryFileUnfailed select temp)
                                .ToList().FirstOrDefault();
                }

                if (fileInfoDO == null)
                {
                    throw new Exception($"TemporaryFileInfo表当前项不存在,TemporaryFileID:{fileInfoVO.FileID}");
                }

                fileInfoVO = TemporaryFileInfoConvert.DOTransformToVO(fileInfoDO);
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行查询TemporaryFile信息操作成功,TemporaryFileID:{fileInfoDO.TemporaryFileID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行查询TemporaryFile信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }

        public ExecuteBaseInfoResult AddTemporaryFileInfo(ref FileInfoVO fileInfoVO)
        {
            TemporaryFileInfoDO fileInfoDO= TemporaryFileInfoConvert.VOTransformToDO(fileInfoVO);

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    fileInfoDO = dbContext.Add<TemporaryFileInfoDO>(fileInfoDO).Entity;
                    dbContext.SaveChanges();

                    fileInfoDO.TemporaryFilePath = $@"{FileConfig.Disk_Temporary_File_Base_Path}{fileInfoDO.TemporaryFileID} {fileInfoDO.TemporaryFileName}";
                    dbContext.Update<TemporaryFileInfoDO>(fileInfoDO);
                    dbContext.SaveChanges();
                }
                
                fileInfoVO = TemporaryFileInfoConvert.DOTransformToVO(fileInfoDO);
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加TemporaryFile信息操作成功,TemporaryFileID:{fileInfoDO.TemporaryFileID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加TemporaryFile信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }

        public ExecuteBaseInfoResult DeleteTemporaryFileInfo(ref FileInfoVO fileInfoVO)
        {
            TemporaryFileInfoDO fileInfoDO = TemporaryFileInfoConvert.VOTransformToDO(fileInfoVO);
            fileInfoDO.TemporaryFileUnfailed = false;

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    dbContext.Update<TemporaryFileInfoDO>(fileInfoDO);
                    dbContext.SaveChanges();
                }

                fileInfoVO = TemporaryFileInfoConvert.DOTransformToVO(fileInfoDO);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行删除TemporaryFile信息操作成功,TemporaryFileID:{fileInfoDO.TemporaryFileID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行删除TemporaryFile信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }

        public ExecuteBaseInfoResult UpdataTemporaryFileInfo(ref FileInfoVO fileInfoVO)
        {
            TemporaryFileInfoDO fileInfoDO = TemporaryFileInfoConvert.VOTransformToDO(fileInfoVO);

            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    dbContext.Update<TemporaryFileInfoDO>(fileInfoDO);
                    dbContext.SaveChanges();
                }

                fileInfoVO = TemporaryFileInfoConvert.DOTransformToVO(fileInfoDO);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行更新TemporaryFile信息操作成功,TemporaryFileID:{fileInfoDO.TemporaryFileID}",
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            catch (Exception ex)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行更新TemporaryFile信息操作失败,Error:{Environment.NewLine}[{ex}]",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
        }
    }
}
