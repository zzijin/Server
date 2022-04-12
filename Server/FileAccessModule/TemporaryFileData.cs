using Server.ValueObject;
using Server.FileAccessModule.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule
{
    class TemporaryFileData:FileDataBase
    {
        private FileDataState fileCacheState;

        public TemporaryFileData(FileInfoVO fileInfoVO) : base(fileInfoVO)
        {
            FileNoData();
        }

        public new void Init()
        {
            FileBuildingData();

            int index = 0;
            foreach (var item in InitFileBaseInfos())
            {
                filePages[index] = new TemporaryPageData(item, fileBaseInfo);
                index++;
            }

            FileCollectData();
        }

        #region Set
        public new ExecuteBaseInfoResult SetFileData(ref FileDataVO fileDataVO)
        {
            if (fileCacheState == FileDataState.FullData)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Success,
                    ExecuteResultMsg = "目标文件已完成收集"
                };
            }

            if (fileDataVO.FileInfo.FileOffset >= FileSize)
            {
                return new ExecuteBaseInfoResult()
                {
                    ExecuteResultState = ExecuteState.Error,
                    ExecuteResultMsg = "设置数据越界"
                };
            }

            foreach (var item in GetPageBaseInfos(fileDataVO.FileInfo))
            {
                (filePages[item.PageNumber] as TemporaryPageData).SetPageData(item, fileDataVO.FileData.First.Value);
            }

            return new ExecuteBaseInfoResult()
            {
                ExecuteResultState = ExecuteState.Success,
                ExecuteResultMsg = "设置数据成功"
            };
        }
        #endregion

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

        /// <summary>
        /// 在文件加入内存时自动调用
        /// </summary>
        /// <returns></returns>
        public Task ReadFileAllDataAsync()
        {
            return Task.Run(async() =>
            {
                List<PageStreamInfo> allPages = GetAllPageToRead().ToList();

                await foreach (var item in FileStreamHelper.ReadFileDataAsync(new FileStreamInfo()
                {
                    FileBaseInfo = fileBaseInfo,
                    PageStreams = allPages
                }))
                {
                    (filePages[item.PageNumber] as TemporaryPageData).PageWrittenData();
                }
            });
        }
        #endregion

        #region Write
        /// <summary>
        /// 将拥有完整数据的页进行整理集合为迭代器
        /// </summary>
        /// <returns></returns>
        private IEnumerable<PageStreamInfo> GetFullPageToWrite()
        {
            return from tempPage in (from page in filePages select (page as TemporaryPageData))
            where tempPage.PageDataIsFull()
            orderby tempPage.PageBaseInfo.PageNumber
            select new PageStreamInfo()
            {
                PageBaseInfo = tempPage.PageBaseInfo,
                PageWriteDatas = tempPage.GetPageData()
            };
        }

        /// <summary>
        /// 将Full文件页写入磁盘
        /// 在该文件被逐出内存时自动调用
        /// </summary>
        /// <returns></returns>
        public Task WriteFileDataAsync()
        {
            return Task.Run(async() =>
            {
                List<PageStreamInfo> allPages = GetFullPageToWrite().ToList();

                await foreach (var item in FileStreamHelper.WriteFileDataAsync(new FileStreamInfo()
                {
                    FileBaseInfo = fileBaseInfo,
                    PageStreams = allPages
                }))
                {
                    (filePages[item.PageNumber] as TemporaryPageData).PageWrittenData();
                }
            });
        }
        #endregion

        #region 验证文件数据、更新文件状态
        /// <summary>
        /// 页数据验证
        /// </summary>
        /// <returns></returns>
        public ExecuteBaseInfoResult PageDataVerification(ref PageInfoVO pageInfoVO)
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

            pageInfoVO.PageVerification = (filePages[pageInfoVO.PageIndex] as TemporaryPageData).VerifyPageData(pageInfoVO.PageHashCode);
            return new ExecuteBaseInfoResult()
            {
                ExecuteResultState = ExecuteState.Success
            };
        }


        public async void ConvertToFileAsync(FileInfoVO fileInfoVO)
        {
            fileBaseInfo.FileID = fileInfoVO.FileID;
            fileBaseInfo.FileSize = fileInfoVO.FileSize;
            fileBaseInfo.FilePath = fileInfoVO.FilePath;
            fileBaseInfo.FileOffset = fileInfoVO.FileOffset;
            //更新每页的数据偏移量
            foreach(var page in filePages)
            {
                page.PageBaseInfo.PageOffset += fileBaseInfo.FileOffset;
                (page as TemporaryPageData).PageFullData();
            }

            await WriteFileDataAsync();
        }

        public LinkedList<PageInfoVO> CheckFileState()
        {
            LinkedList<PageInfoVO> list = new LinkedList<PageInfoVO>();
            foreach(var item in (from page in filePages
                                  where !(page as TemporaryPageData).PageDataIsVerify()
                                  select page))
            {
                list.AddLast(new PageInfoVO(fileBaseInfo.FileID, item.PageNumber, item.PageOffset, item.PageSize));
            }

            if (list.Count() > 0)
            {
                FileCollectData();
            }
            else
            {
                FileFullCacheData();
            }

            return list;
        }

        #endregion

        #region 设置文件数据状态
        public void FileNoData()
        {
            fileCacheState = FileDataState.NoData;
        }

        public void FileBuildingData()
        {
            fileCacheState = FileDataState.Building;
        }

        public void FileCollectData()
        {
            fileCacheState = FileDataState.CollectData;
        }

        public void FileFullCacheData()
        {
            fileCacheState = FileDataState.FullData;
        }
        #endregion

        public new void Disposable()
        {
            WriteFileDataAsync();
        }

        /// <summary>
        /// 构造临时文件，
        /// 若文件存在则读取，若文件不存在则创建
        /// 在构造前应从数据库获取信息
        /// </summary>
        /// <param name="fileInfoVO"></param>
        /// <returns></returns>
        public static TemporaryFileData BuildTemporaryFile(FileInfoVO fileInfoVO)
        {
            TemporaryFileData fileData = new TemporaryFileData(fileInfoVO);
            fileData.Init();

            //尝试创建临时文件，若文件存在则改为读取
            if (FileStreamHelper.FileExistsState(fileInfoVO.FilePath))
            {
                fileData.ReadFileAllDataAsync();
            }
            else
            {
                FileStreamHelper.CreateTemporaryFile(fileInfoVO.FilePath);
            }
            return fileData;
        }

        private enum FileDataState
        {
            /// <summary>
            /// 没有任何数据
            /// </summary>
            NoData,
            /// <summary>
            /// 文件构造中
            /// </summary>
            Building,
            /// <summary>
            /// 数据收集中
            /// </summary>
            CollectData,
            /// <summary>
            /// 数据已收集完成
            /// </summary>
            FullData,
        }
    }
}
