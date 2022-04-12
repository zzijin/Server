using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Server.FileAccessModule.Configuration;

namespace Server.FileAccessModule
{
    /// <summary>
    /// 提供静态的文件路径大小检查方法及可共享的读、写操作
    /// </summary>
    class FileStreamHelper
    {
        /// <summary>
        /// 尝试生成默认大小的文件块
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDefaultFileBlock(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    fs.Seek(FileConfig.DISK_FILE_BLOCK_DEFAULT_SIZE, SeekOrigin.Begin);
                    fs.WriteByte(0);
                    fs.Flush();
                }
            }
            catch
            {

            }
        }

        public static bool CreateTemporaryFile(string path)
        {
            try
            {
                using(FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    fs.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async IAsyncEnumerable<PageBaseInfo> ReadFileDataAsync(FileStreamInfo fileStreamInfo)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileStreamInfo.FileBaseInfo.FilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    fileStream.Seek(fileStreamInfo.FileBaseInfo.FileOffset, SeekOrigin.Begin);
                    foreach (var item in fileStreamInfo.PageStreams)
                    {
                        if (await fileStream.ReadAsync(item.PageReadDatas) == item.PageBaseInfo.PageSize)
                        {
                            yield return item.PageBaseInfo;
                        }
                    }
                }
            }
            finally
            {

            }
        }

        public static async IAsyncEnumerable<PageBaseInfo> ReadPageDataAsync(FileStreamInfo fileStreamInfo)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileStreamInfo.FileBaseInfo.FilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    foreach (var item in fileStreamInfo.PageStreams)
                    {
                        if (item.PageReadDatas.Length > 0)
                        {
                            fileStream.Seek(item.PageBaseInfo.PageOffset, SeekOrigin.Begin);
                            if(await fileStream.ReadAsync(item.PageReadDatas) > 0)
                            {
                                yield return item.PageBaseInfo;
                            }
                        }
                    }
                }
            }
            finally
            {

            }
        }

        public static async IAsyncEnumerable<PageBaseInfo> WriteFileDataAsync(FileStreamInfo fileStreamInfo)
        {
            try
            {
                using(FileStream fileStream=new FileStream(fileStreamInfo.FileBaseInfo.FilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    //锁定写入的范围
                    fileStream.Lock(fileStreamInfo.FileBaseInfo.FileOffset, fileStreamInfo.FileBaseInfo.FileSize);
                    fileStream.Seek(fileStreamInfo.FileBaseInfo.FileOffset, SeekOrigin.Begin);
                    foreach(var item in fileStreamInfo.PageStreams)
                    {
                        if (item.PageWriteDatas.Length > 0)
                        {
                            if (fileStream.Position != item.PageBaseInfo.PageOffset)
                            {
                                fileStream.Position= item.PageBaseInfo.PageOffset;
                            }
                            await fileStream.WriteAsync(item.PageWriteDatas);
                            yield return item.PageBaseInfo;
                        }
                        else
                        {
                            yield break;
                        }
                    }
                    fileStream.Unlock(fileStreamInfo.FileBaseInfo.FileOffset, fileStreamInfo.FileBaseInfo.FileSize);
                    fileStream.Flush();
                }
            }
            finally
            {
                
            }
        }

        public static async Task<bool> WritePageDataAsync(FileStreamInfo fileStreamInfo)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileStreamInfo.FileBaseInfo.FilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    foreach (var item in fileStreamInfo.PageStreams)
                    {
                        if (item.PageWriteDatas.Length > 0)
                        {
                            fileStream.Seek(item.PageBaseInfo.PageOffset, SeekOrigin.Begin);
                            await fileStream.WriteAsync(item.PageWriteDatas);
                        }
                    }
                    fileStream.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckFileInfo(string path,long offset,long size)
        {
            if (!FileExistsState(path))
            {
                return false;
            }

            if (GetFileSize(path) < offset + size)
            {
                return false;
            }

            return true;
        }

        public static bool FileExistsState(string path)
        {
            return File.Exists(path);
        }

        public static long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }
    }
}
