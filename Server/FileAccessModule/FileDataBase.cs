using Server.ValueObject;
using Server.FileAccessModule.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    /*
     * 一、此处需区分文件块、文件、文件页、临时文件
     * 文件块是指将文件存储在磁盘时的状态，一个文件块可能包含多个文件，是系统的永久性文件，由数据库存储文件信息
     * 文件指一个完整的文件，文件是内存缓存中的基本单位，
     * 文件页指在读取文件时按页大小分次读取
     * 临时文件指用户在上传时没有进行完全hash验证时临时存储，储存时间过长或移动到文件块后会被清理
     * 
     * 二、设计说明
     * FileData包含了文件信息和PageData
     * 设置fileCacheState、fileDiskState分别指示缓存状态和磁盘状态
     * 只有检查了文件包含的所有页数据的相应状态，文件的状态才能为Full(所有页都通过验证)
     * 
     * PageData包含了文件信息、页信息及数据
     * PageData封装了对页数据的操作：Set&Get
     * 
     * 
     * 三、文件|临时文件
     * 文件有Get、Read、Write操作
     * 临时文件仅有Set、Read操作
     */
    /// <summary>
    /// 文件数据基础类
    /// 需要继承使用
    /// </summary>
    class FileDataBase
    {
        private protected FileBaseInfo fileBaseInfo;
        private protected PageDataBase[] filePages;

        public int FileID { get => fileBaseInfo.FileID; }
        public string FilePath { get => fileBaseInfo.FilePath; }
        public long FileOffset { get => fileBaseInfo.FileOffset; }
        public long FileSize { get => fileBaseInfo.FileSize; }

        public FileDataBase(FileInfoVO fileInfoVO)
        {
            fileBaseInfo.FileID = fileInfoVO.FileID; fileBaseInfo.FilePath = fileInfoVO.FilePath;
            fileBaseInfo.FileOffset = fileInfoVO.FileOffset; fileBaseInfo.FileSize = fileInfoVO.FileSize;

            int pageSize = (int)(fileBaseInfo.FileSize % FileConfig.MEMORY_FILE_PAGE_MAX_SIZE == 0 ? 
                fileBaseInfo.FileSize / FileConfig.MEMORY_FILE_PAGE_MAX_SIZE : fileBaseInfo.FileSize / FileConfig.MEMORY_FILE_PAGE_MAX_SIZE + 1);
            filePages = new PageDataBase[pageSize];
        }

        /// <summary>
        /// 初始化文件页信息
        /// </summary>
        /// <returns></returns>
        private protected IEnumerable<PageBaseInfo> InitFileBaseInfos()
        {
            int blockSize = filePages.Length;
            //初始化其数据块
            for (int i = 0; i < blockSize - 1; i++)
            {
                yield return new PageBaseInfo()
                {
                    PageNumber = i,
                    PageOffset = i * FileConfig.MEMORY_FILE_PAGE_MAX_SIZE,
                    PageSize = FileConfig.MEMORY_FILE_PAGE_MAX_SIZE
                };
            }

            int lastNumber = blockSize - 1;
            yield return new PageBaseInfo()
            {
                PageNumber = lastNumber,
                PageOffset = lastNumber * FileConfig.MEMORY_FILE_PAGE_MAX_SIZE,
                PageSize = (int)(fileBaseInfo.FileSize - (lastNumber * FileConfig.MEMORY_FILE_PAGE_MAX_SIZE))
            };
        }

        private protected virtual void Init()
        {
            int index = 0;
            foreach (var item in InitFileBaseInfos()) 
            {
                filePages[index] = new PageDataBase(item, fileBaseInfo);
                index++;
            }
        }

        #region Read
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private protected IEnumerable<PageStreamInfo> ReadFileData()
        {
            foreach(var item in filePages)
            {
                yield return new PageStreamInfo()
                {
                    PageBaseInfo = item.PageBaseInfo,
                    PageReadDatas = item.SetPageData()
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private protected IEnumerable<PageStreamInfo> ReadPageData(int index)
        {
            yield return new PageStreamInfo()
            {
                PageBaseInfo = filePages[index].PageBaseInfo,
                PageReadDatas =filePages[index].SetPageData()
            };
        }

        #endregion

        #region Write

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private protected IEnumerable<PageStreamInfo> WriteFileData()
        {
            foreach (var item in filePages)
            {
                ReadOnlyMemory<byte> pageData = item.GetPageData();
                if (pageData.Length > 0)
                {
                    yield return new PageStreamInfo()
                    {
                        PageBaseInfo = item.PageBaseInfo,
                        PageWriteDatas = pageData
                    };
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private protected IEnumerable<PageStreamInfo> WritePageData(int index)
        {
            ReadOnlyMemory<byte> pageData = filePages[index].GetPageData();
            if (pageData.Length > 0)
            {
                yield return new PageStreamInfo()
                {
                    PageBaseInfo = filePages[index].PageBaseInfo,
                    PageWriteDatas = pageData
                };
            }
            else
            {
                yield break;
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// 获取文件指定数据
        /// </summary>
        /// <param name="fileInfoVO"></param>
        private protected IEnumerable<ReadOnlyMemory<byte>> GetFileData(FileInfoVO fileInfoVO)
        {
            //if (fileInfoVO.FileOffset < FileSize)
            //{
                foreach (var item in GetPageBaseInfos(fileInfoVO))
                {
                    ReadOnlyMemory<byte> pageData = filePages[item.PageNumber].GetPageData(item);
                    if (pageData.Length > 0)
                    {
                        yield return pageData;
                    }
                    else
                    {
                        yield break;
                    }
                }
            //}
            yield break;
        }

        #endregion

        #region Set

        /// <summary>
        /// 设置文件指定数据
        /// 此方法需要重写，此供示例
        /// </summary>
        private protected ExecuteBaseInfoResult SetFileData(ref FileDataVO fileDataVO)
        {
            if (fileDataVO.FileInfo.FileOffset >= FileSize)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg = "获取数据越界"
                };
            }

            foreach (var item in GetPageBaseInfos(fileDataVO.FileInfo))
            {
                filePages[item.PageNumber].SetPageData(item, fileDataVO.FileData.First.Value);
            }

            return new ExecuteBaseInfoResult()
            {
                ExecuteResultState= ExecuteState.Success,
                ExecuteResultMsg="插入数据成功"
            };
        }

        #endregion

        #region 查询文件相关页

        /// <summary>
        /// 根据页号获取页数据
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        private protected PageDataBase GetFilePageData(int pageNumber)
        {
            return filePages[pageNumber];
        }


        /// <summary>
        /// 设置和获取页数据时
        /// 根据获取位置计算的相关信息
        /// </summary>
        /// <param name="fileDataEntity"></param>
        /// <returns></returns>
        private protected IEnumerable<PageBaseInfo> GetPageBaseInfos(FileInfoVO fileInfoVO)
        {
            long needLenth = fileInfoVO.FileSize;
            //此处偏移量指在文件中的偏移量，需转换为文件块偏移量
            long offset = fileInfoVO.FileOffset + fileBaseInfo.FileOffset;
            int pageIndex = (int)(fileInfoVO.FileOffset / FileConfig.MEMORY_FILE_PAGE_MAX_SIZE);

            do
            {
                int pageSize = (int)(((needLenth + offset) - (filePages[pageIndex].PageEndIndex)) > 0 ?
                        (filePages[pageIndex].PageEndIndex - offset) : (offset + needLenth - filePages[pageIndex].PageOffset));
                yield return new PageBaseInfo()
                {
                    PageNumber = pageIndex,
                    PageOffset = offset,
                    PageSize = pageSize
                };
                pageIndex++;
                needLenth -= pageSize;
                offset = filePages[pageIndex].PageOffset;
            }
            while (needLenth > 0 && pageIndex < filePages.Length);
        }
        #endregion

        #region 释放文件
        public virtual void Disposable()
        {

        }
        #endregion
    }

}
