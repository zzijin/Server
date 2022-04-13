using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Server.ValueObject;
using Server.FileAccessModule.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.DataAccessModule.Repository;
using Server.FileAccessModule.StatisticsInfo;

namespace Server.FileAccessModule
{
    /*
     * 进度记录：读已完成，写入需考虑流程
     * 写入流程1：文件记录在缓存中，直到完全再进行验证写入操作，坏处：缓存有过期时间，长时间未上传被清除（无法断点续传）
     * 写入流程2：文件直接获得全部信息直接同时写入到缓存和磁盘，完成后标记状态
     * 写入流程3：文件记录到临时位置，完成后迁移并标记
     */
    /// <summary>
    /// 内存缓冲管理类，封装了对内存缓存的基本操作
    /// </summary>
    class MemoryCacheManager
    {
        private IMemoryCache cache;
        private FileInfoRepository fileInfoRepository;
        private TemporaryFileRepository temporaryFileRepository;
        private FileBlockInfoRepository fileBlockInfoRepository;
        private MemoryCacheInfo memoryCacheInfo;

        public MemoryCacheManager(IServiceProvider serviceProvider)
        {
            cache = serviceProvider.GetService<IMemoryCache>();
            fileInfoRepository= serviceProvider.GetService<FileInfoRepository>();
            temporaryFileRepository= serviceProvider.GetService<TemporaryFileRepository>();
            fileBlockInfoRepository= serviceProvider.GetService<FileBlockInfoRepository>();
            memoryCacheInfo = new MemoryCacheInfo();
        }

        /// <summary>
        /// 设置内存缓存项
        /// 由单个线程上传数据而来，始终是单线程，不需要考虑多线程
        /// </summary>
        /// <param name="fileInfoEntity"></param>
        public ExecuteBaseInfoResult GetCacheEntryTemporaryFileData(ref FileInfoVO fileInfoVO, out TemporaryFileData temporaryFile)
        {
            //若内存中存在则直接设置
            if(cache.TryGetValue<TemporaryFileData>(fileInfoVO.FileID,out temporaryFile))
            {
                return new ExecuteBaseInfoResult
                {
                    ExecuteResultState = ExecuteState.Success,
                };
            }

            //内存不存在则需要创建,先从数据库查询信息
            if(temporaryFileRepository.GetTemporaryFileInfo(ref fileInfoVO).ExecuteResultState == ExecuteState.Success)
            {
                FileInfoVO fileInfo = fileInfoVO;
                temporaryFile = cache.GetOrCreate<TemporaryFileData>(fileInfoVO.FileID, (entryOptions) =>
                {
                    TemporaryFileData temporaryFile = TemporaryFileData.BuildTemporaryFile(fileInfo);

                    entryOptions.RegisterPostEvictionCallback(FreeTempCacheCallBack);
                    entryOptions.SlidingExpiration = TimeSpan.FromMinutes(FileConfig.CACHE_ENTRY_SLIDING_EXPIRATION);
                    entryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(FileConfig.CACHE_ENTRY_ABSOLUTE_EXPIRATION);
                    entryOptions.Size = fileInfo.FileSize;

                    return temporaryFile;
                });
                memoryCacheInfo.AddMemoryEnTry(fileInfo.FileSize);

                return new ExecuteBaseInfoResult
                {
                    ExecuteResultState = ExecuteState.Success,
                };
            }
            else
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultMsg = "未知序号的文件",
                    ExecuteResultState = ExecuteState.Error,
                };
            }
            
        }

        /// <summary>
        /// 从内存缓存中删除临时文件数据
        /// </summary>
        /// <param name="fileInfoVO"></param>
        public void DeleteCacheEntryTemporaryFileData(ref FileInfoVO fileInfoVO)
        {
            cache.Remove(fileInfoVO.FileID);
        }

        /// <summary>
        /// 获取缓存项数据
        /// </summary>
        /// <param name="fileDataVO"></param>
        /// <returns></returns>
        public ExecuteBaseInfoResult GetCacheEntryData(ref FileInfoVO fileInfoVO,out FileData fileData)
        {
            if (cache.TryGetValue(fileInfoVO.FileID, out fileData))
            {
                //fileData.GetFileData(ref fileDataVO);
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Success,
                };
            }

            if (fileInfoRepository.GetFileInfo(ref fileInfoVO).ExecuteResultState == ExecuteState.Success)
            {
                if (FileStreamHelper.CheckFileInfo(fileInfoVO.FilePath, fileInfoVO.FileOffset, fileInfoVO.FileSize))
                {

                    AddCacheEntry(fileInfoVO);

                    return new ExecuteBaseInfoResult()
                    {
                        ExecuteResultState = ExecuteState.Wait,
                    };
                }
                else
                {
                    return new ExecuteBaseInfoResult()
                    {
                        ExecuteResultState = ExecuteState.Error,
                        ExecuteResultMsg = "数据库信息错误，请联系管理员"
                    };
                }
            }
            else
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg = "请求的文件信息错误"
                };
            }
        }

        /// <summary>
        /// 设置缓存项
        /// 多个线程同时设置统一缓存项可能会创建多次，不影响结果但可能影响性能，已尝试降低发生概率
        /// </summary>
        /// <param name="fileInfoVO"></param>
        private void AddCacheEntry(FileInfoVO fileInfoVO)
        {
            //若已被创建则返回
            if (cache.Get(fileInfoVO.FileID) != null)
            {
                return;
            }

            var options = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(FileConfig.CACHE_ENTRY_SLIDING_EXPIRATION),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(FileConfig.CACHE_ENTRY_ABSOLUTE_EXPIRATION),
                Size = fileInfoVO.FileSize
            };

            //注册驱逐回调
            options.RegisterPostEvictionCallback(FreeCacheCallBack);

            //构建并从磁盘读取文件
            //异步从磁盘读取所有文件
            FileData fileDataInfo = FileData.BuildFile(fileInfoVO);

            cache.Set<FileData>(fileDataInfo.FileID, fileDataInfo,options);
            memoryCacheInfo.AddMemoryEnTry(fileInfoVO.FileSize);
        }

        /// <summary>
        /// 逐出缓存回调
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        /// <param name="state"></param>
        private void FreeCacheCallBack(object key, object value, EvictionReason reason, object state)
        {
            FileData fileData=value as FileData;

            memoryCacheInfo.RemoveMemoryEntry(fileData.FileSize);
            //$"缓存:{key}已被逐出"
            fileData.Disposable();
        }

        // <summary>
        /// 逐出缓存回调
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        /// <param name="state"></param>
        private void FreeTempCacheCallBack(object key, object value, EvictionReason reason, object state)
        {
            TemporaryFileData tempData=value as TemporaryFileData;

            memoryCacheInfo.RemoveMemoryEntry(tempData.FileSize);

            //$"缓存:{key}已被逐出"
            (value as TemporaryFileData).Disposable();
        }

        Object cacheWriteLock = new();
        /// <summary>
        /// 设置缓存项
        /// 添加时加锁，防止同一对象被多次创建，可能影响性能
        /// </summary>
        /// <param name="fileInfoVO"></param>
        public void SafeAddCacheEntryFormDisk(FileInfoVO fileInfoVO)
        {
            lock (cacheWriteLock)
            {
                AddCacheEntry(fileInfoVO);
            }
        }
    }
}
