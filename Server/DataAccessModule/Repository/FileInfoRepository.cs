using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule.Configuration;
using Server.DataAccessModule.Converters;
using Server.DataAccessModule.DO;
using Server.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.Repository
{
    class FileInfoRepository
    {
        IDbContextFactory<ServerDbContext> _dbContextFactory;
        public FileInfoRepository(IServiceProvider serviceProvider)
        {
            _dbContextFactory = serviceProvider.GetService<IDbContextFactory<ServerDbContext>>();
        }

        public ExecuteBaseInfoResult GetFileInfo(ref FileInfoVO fileInfoVO)
        {
            FileInfoDO fileInfoDO = FileInfoConvert.VOTransformToDO(fileInfoVO);
            FileBlockInfoDO firstFileBlockInfo;
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    fileInfoDO=dbContext.FileInfo.Find(fileInfoDO.FileID);
                    firstFileBlockInfo=dbContext.FileBlockInfo.Find(fileInfoDO.FileBlockID);
                }

                if (fileInfoDO == null)
                {
                    throw new Exception($"FileInfo表当前项不存在,FileID:{fileInfoVO.FileID}");
                }

                fileInfoVO = FileInfoConvert.DOTransformToVO(fileInfoDO, firstFileBlockInfo);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加TemporaryFile信息操作成功,FileID:{fileInfoDO.FileID}",
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

        public ExecuteBaseInfoResult AddFileInfo(ref FileInfoVO fileInfoVO)
        {
            FileInfoDO fileInfoDO=FileInfoConvert.VOTransformToDO(fileInfoVO);
            FileBlockInfoDO firstFileBlockInfo;
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    using (var job = dbContext.Database.BeginTransaction())
                    {
                        //寻找一个剩余空间大于存入文件模块
                        firstFileBlockInfo = (from b in dbContext.FileBlockInfo 
                                                  where b.FileBlockFreeSize>fileInfoDO.FileSize&&b.FileBlockUnfailed==true
                                                  orderby b.FileBlockID 
                                                  select b)
                                                  .FirstOrDefault();

                        //若最新的FileBlock已不满足存储条件
                        if (firstFileBlockInfo==null)
                        {
                            return new ExecuteBaseInfoResult()
                            {
                                ExecuteResultMsg = $"执行添加TemporaryFile信息操作暂停，需要新增文件块",
                                ExecuteResultState = ExecuteState.Wait,
                            };
                        }

                        fileInfoDO.FileOffset = firstFileBlockInfo.FileBlockSize - firstFileBlockInfo.FileBlockFreeSize;
                        fileInfoDO.FileBlockID = firstFileBlockInfo.FileBlockID;
                        dbContext.FileInfo.Add(fileInfoDO);

                        firstFileBlockInfo.FileBlockFreeSize -= fileInfoDO.FileSize;
                        dbContext.FileBlockInfo.Update(firstFileBlockInfo);

                        dbContext.SaveChanges();

                        job.Commit();
                    }
                }

                fileInfoVO = FileInfoConvert.DOTransformToVO(fileInfoDO, firstFileBlockInfo);

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = $"执行添加TemporaryFile信息操作成功,FileID:{fileInfoDO.FileID}",
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

        //public ExecuteBaseInfoResult UpdataFileInfo(FileInfoVO fileInfoVO)
        //{
        //    FileInfoDO fileInfoDO = FileInfoConvert.VOTransformToDO(fileInfoVO);

        //}

        //public ExecuteBaseInfoResult DeleteFileInfo(FileInfoVO fileInfoVO)
        //{
        //    FileInfoDO fileInfoDO = FileInfoConvert.VOTransformToDO(fileInfoVO);

        //}
    }

}
