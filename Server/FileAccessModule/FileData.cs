using Server.ValueObject;
using Server.FileAccessModule.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    
    /// <summary>
    /// 文件数据信息
    /// 本类用于缓存文件块文件
    /// </summary>
    class FileData:FileDataBase
    {
        private FileDataState fileCacheState;

        /// <summary>
        /// 文件的基本信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <param name="offer"></param>
        /// <param name="size"></param>
        public FileData(FileInfoVO fileInfoVO) :base(fileInfoVO)
        {
            FileNoCacheData();
        }

        public new void Init()
        {
            FileBuildingCacheData();

            int index = 0;
            foreach (var item in InitFileBaseInfos())
            {
                filePages[index] = new PageData(item, fileBaseInfo);
                index++;
            }
        }

        #region Read
        
        private IEnumerable<PageStreamInfo> GetAllPageToRead()
        {
            foreach (var item in filePages)
            {
                yield return new PageStreamInfo()
                {
                    PageBaseInfo = item.PageBaseInfo,
                    PageReadDatas = item.SetPageData()
                };
            }
        }

        private IEnumerable<PageStreamInfo> GetNeedPageToRead()
        {
            foreach(var item in filePages)
            {
                if(!(item as PageData).CheckPageStateIsRight())
                {
                    yield return new PageStreamInfo()
                    {
                        PageBaseInfo = item.PageBaseInfo,
                        PageReadDatas = item.SetPageData()
                    };
                }
            }
        }
        
        private async Task TryReadFileDataAsync()
        {
            if (fileCacheState == FileDataState.Building)
            {
                return;
            }

            FileBuildingCacheData();
            List<PageStreamInfo> errorPages = GetNeedPageToRead().ToList();

            await foreach(var item in FileStreamHelper.ReadPageDataAsync(new FileStreamInfo()
            {
                FileBaseInfo = fileBaseInfo,
                PageStreams= errorPages
            }))
            {
                (filePages[item.PageNumber] as PageData).PageFullCacheData();
            }

            CheckPageState(errorPages);
        }

        private async Task ReadFileAllDataAsync()
        {
            List<PageStreamInfo> allPages = GetAllPageToRead().ToList();

            await foreach(var item in FileStreamHelper.ReadPageDataAsync(new FileStreamInfo()
            {
                FileBaseInfo = fileBaseInfo,
                PageStreams = allPages
            }))
            {
                (filePages[item.PageNumber] as PageData).PageFullCacheData();
            }

            CheckPageState(allPages);
        }

        private void CheckPageState(List<PageStreamInfo> checkPages)
        {
            foreach(var item in checkPages)
            {
                PageData pageData = filePages[item.PageBaseInfo.PageNumber] as PageData;
                if (!pageData.CheckPageStateIsFull())
                {
                    FilePartCacheData();
                    return;
                }
            }
        }

        #endregion

        #region Get
        public ExecuteBaseInfoResult GetFileData(ref FileDataVO fileDataVO)
        {
            if (fileDataVO.FileInfo.FileOffset >= FileSize)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg = "获取数据越界"
                };
            }

            foreach (var item in base.GetFileData(fileDataVO.FileInfo))
            {
                fileDataVO.AddFilePackData(item);
            }

            if (fileDataVO.FileData.Count > 0)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Success,
                    ExecuteResultMsg = "获取数据成功"
                };
            }
            else
            {
                //页面数据缺失，当状态不为Building，需检查和加载文件数据
                if (fileCacheState == FileDataState.PartData || fileCacheState == FileDataState.FullData)
                {
                    Task task = TryReadFileDataAsync();
                }

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Wait,
                    ExecuteResultMsg = "获取数据失败，相应数据缺失"
                };
            }
        }

        public ExecuteBaseInfoResult GetPageHashCode(ref PageInfoVO pageInfoVO)
        {
            if (pageInfoVO.PageIndex < 0 || pageInfoVO.PageIndex >= filePages.Length)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg = "执行出错，页号越界",
                };
            }

            if (filePages[pageInfoVO.PageIndex].PageOffset != (pageInfoVO.PageOffset + FileOffset) || filePages[pageInfoVO.PageIndex].PageSize != pageInfoVO.PageSize)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg = "执行出错，页数据错误",
                };
            }

            ReadOnlyMemory<byte> hashCode = (filePages[pageInfoVO.PageIndex] as PageData).GetPageHashCode();
            if (hashCode.Length>0)
            {
                pageInfoVO.PageHashCode= hashCode;
                pageInfoVO.PageVerification = true;

                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Success
                };
            }
            else
            {
                pageInfoVO.PageVerification = false;
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg="当前页面丢失"
                };
            }

        }
        #endregion

        #region 设置文件数据状态
        public void FileNoCacheData()
        {
            fileCacheState = FileDataState.NoData;
        }

        public void FileBuildingCacheData()
        {
            fileCacheState = FileDataState.Building;
        }

        public void FilePartCacheData()
        {
            fileCacheState = FileDataState.PartData;
        }

        public void FileFullCacheData()
        {
            fileCacheState = FileDataState.FullData;
        }
        #endregion

        public new void Disposable()
        {

        }

        public static FileData BuildFile(FileInfoVO fileInfoVO)
        {
            FileData fileData=new FileData(fileInfoVO);
            fileData.Init();
            Task task = fileData.ReadFileAllDataAsync();

            return fileData;
        }

        private enum FileDataState
        {
            /// <summary>
            /// 没有任何页数据
            /// </summary>
            NoData,
            /// <summary>
            /// 正在构造/读取文件中
            /// </summary>
            Building,
            /// <summary>
            /// 只有部分数据,数据存在缺失
            /// </summary>
            PartData,
            /// <summary>
            /// 数据完全
            /// </summary>
            FullData,
        }
    }
}
