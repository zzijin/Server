using Microsoft.Extensions.DependencyInjection;
using Server.DataAccessModule.Repository;
using Server.FileAccessModule.Configuration;
using Server.FileAccessModule.Interface;
using Server.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    class FileAccessManager: IFileAccessManager
    {
        private FileInfoRepository fileInfoRepository;
        private TemporaryFileRepository temporaryFileRepository;
        private FileBlockInfoRepository fileBlockInfoRepository;
        private MemoryCacheManager memoryCacheManager;

        public FileAccessManager(IServiceProvider serviceProvider)
        {
            InitFileConfiguration();
            fileInfoRepository=serviceProvider.GetService<FileInfoRepository>();
            temporaryFileRepository=serviceProvider.GetService<TemporaryFileRepository>();
            fileBlockInfoRepository=serviceProvider.GetService<FileBlockInfoRepository>();

            memoryCacheManager=new MemoryCacheManager(serviceProvider);
        }

        /// <summary>
        /// 初始化文件模块配置
        /// </summary>
        private void InitFileConfiguration()
        {
            //
        }

        public ExecuteBaseInfoResult GetFile(ref FileDataVO fileDataVO)
        {
            FileInfoVO fileInfoVO = fileDataVO.FileInfo;
            FileData fileData = null;

            ExecuteBaseInfoResult executeBaseInfoResult = memoryCacheManager.GetCacheEntryData(ref fileInfoVO, out fileData);
            fileDataVO.FileInfo = fileInfoVO;
            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait || executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
            {
                return executeBaseInfoResult;
            }

            fileData.GetFileData(ref fileDataVO);
            return executeBaseInfoResult;
        }

        public ExecuteBaseInfoResult SetTemporaryFile(ref FileDataVO fileDataVO)
        {
            FileInfoVO fileInfoVO = fileDataVO.FileInfo;
            TemporaryFileData temporaryFileData = null;

            ExecuteBaseInfoResult executeBaseInfoResult = memoryCacheManager.GetCacheEntryTemporaryFileData(ref fileInfoVO, out temporaryFileData);
            fileDataVO.FileInfo = fileInfoVO;
            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait || executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
            {
                return executeBaseInfoResult;
            }

            temporaryFileData.SetFileData(ref fileDataVO);
            return executeBaseInfoResult;
        }

        public ExecuteBaseInfoResult CreateTemporaryFile(ref FileDataVO fileDataVO)
        {
            FileInfoVO fileInfoVO = fileDataVO.FileInfo;

            return temporaryFileRepository.AddTemporaryFileInfo(ref fileInfoVO);
        }

        public ExecuteBaseInfoResult ConvertTemporaryFile(ref FileInfoVO fileInfoVO,out LinkedList<PageInfoVO> pageInfos)
        {
            pageInfos = null;
            TemporaryFileData temporaryFileData = null;
            FileInfoVO temporaryFileinfoVO = fileInfoVO;
            ExecuteBaseInfoResult executeBaseInfoResult = memoryCacheManager.GetCacheEntryTemporaryFileData(ref fileInfoVO, out temporaryFileData);
            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait || executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
            {
                return executeBaseInfoResult;
            }

            pageInfos = temporaryFileData.CheckFileState();
            if (pageInfos.Count>0)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg="当前文件未全部经过验证",
                };
            }

            executeBaseInfoResult=fileInfoRepository.AddFileInfo(ref fileInfoVO);
            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
            {
                return executeBaseInfoResult;
            }
            else if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait)
            {
                FileBlockInfoVO fileBlockInfoVO = new FileBlockInfoVO(FileConfig.DISK_FILE_BLOCK_DEFAULT_SIZE);

                ExecuteBaseInfoResult executeBaseInfoResult1 = fileBlockInfoRepository.AddFileBlockInfo(ref fileBlockInfoVO);
                if (executeBaseInfoResult1.ExecuteResultState == ExecuteState.Error)
                {
                    return executeBaseInfoResult1;
                }

                FileStreamHelper.CreateDefaultFileBlock(fileBlockInfoVO.FileBlockPath);
                fileBlockInfoVO.FileBlockUnfailed = true;
                fileBlockInfoRepository.UpdataFileBlockInfo(ref fileBlockInfoVO);

                return executeBaseInfoResult;
            }
            else
            {
                temporaryFileData.ConvertToFileAsync(fileInfoVO);
                temporaryFileRepository.DeleteTemporaryFileInfo(ref temporaryFileinfoVO);
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Success,
                };
            }
        }

        public ExecuteBaseInfoResult PageDataVerification(ref PageInfoVO pageInfoVO)
        {
            FileInfoVO fileInfoVO = new FileInfoVO(pageInfoVO.FileID);
            TemporaryFileData temporaryFileData = null;

            ExecuteBaseInfoResult executeBaseInfoResult = memoryCacheManager.GetCacheEntryTemporaryFileData(ref fileInfoVO, out temporaryFileData);
            if (executeBaseInfoResult.ExecuteResultState == ExecuteState.Wait || executeBaseInfoResult.ExecuteResultState == ExecuteState.Error)
            {
                return executeBaseInfoResult;
            }

            return temporaryFileData.PageDataVerification(ref pageInfoVO);
        }
    }
}
