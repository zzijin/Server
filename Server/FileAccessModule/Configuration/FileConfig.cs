using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.FileAccessModule.Configuration
{
    class FileConfig
    {
        /// <summary>
        /// 内存中允许存在的最大文件大小
        /// 超过此大小应将该文件拆分或每次访问都重新重磁盘中读取
        /// </summary>
        public static int MEMORY_FILE_MAX_SIZE = 100 * 1024 * 1024;
        /// <summary>
        /// 内存中单个文件的单页最大大小，单位byte
        /// 根据页大小分页读取，此大小应经测试程序测试后确定最佳值
        /// 值范围警告：最大大小不得大于85,000(LOH)
        /// </summary>
        public static int MEMORY_FILE_PAGE_MAX_SIZE = 32000;
        /// <summary>
        /// 磁盘中储存时文件块默认大小，单位byte
        /// 一个文件块中至少包含一个文件，根据偏移量读取不同位置的文件，防止磁盘碎片化
        /// </summary>
        public static long DISK_FILE_BLOCK_DEFAULT_SIZE = 500 * 1024 * 1024;
        /// <summary>
        /// 缓存最大缓存空间限制，单位byte
        /// </summary>
        public static long CACHE_SIZE_LIMIT = 500 * 1024 * 1024;
        /// <summary>
        /// 缓存扫描所有过期项频率(最小时间间隔),单位min
        /// </summary>
        public static int CACHE_EXPIRATION_SCAN_FREQUENCY = 10;
        /// <summary>
        /// 缓存超出缓存空间限制时，需要压缩的缓存量
        /// 压缩至CACHE_SIZE_LIMIT*(1-CACHE_COMPACTION_PERCENTAGE)大小
        /// </summary>
        public static double CACHE_COMPACTION_PERCENTAGE = 0.05;
        /// <summary>
        /// 缓存缓存绝对到期时间，单位min
        /// </summary>
        public static int CACHE_ENTRY_ABSOLUTE_EXPIRATION = 60;
        /// <summary>
        /// 缓存滑动过期时间，单位min
        /// 在滑动时间内没有被访问则被驱逐，若被访问则向后延迟一个单位时间，最终时间不超过绝对到期时间
        /// </summary>
        public static int CACHE_ENTRY_SLIDING_EXPIRATION = 20;
        /// <summary>
        /// 缓存项优先级梯度
        /// 在设置缓存项优先级时，按缓存项大小设置，越大的文件优先级越低
        /// </summary>
        public static long CACHE_ENTRY_PRIORITY_GRADIENT = 10 * 1024 * 1024;

        public static string Disk_File_Block_Base_Path = @"./FileBlock/";
        public static string Disk_Temporary_File_Base_Path = @"./TemporaryFile/";
    }
}
