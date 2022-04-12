using Server.DataAccessModule.Configuration;
using Server.DataAccessModule.DO;
using Server.DataAccessModule.Interface;
using Server.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.Converters
{
    internal class TemporaryFileInfoConvert
    {
        public static TemporaryFileInfoDO VOTransformToDO(FileInfoVO VO)
        {
            return new TemporaryFileInfoDO()
            {
                TemporaryFileID = VO.FileID,
                TemporaryFileName = VO.FileName,
                TemporaryFilePath = VO.FilePath,
                TemporaryFileSize = VO.FileSize,
                TemporaryFileLastEditUser = DBConfig.ServerUser,
                TemporaryFileLastEditDate = DateTime.Now,
            };
        }

        public static FileInfoVO DOTransformToVO(TemporaryFileInfoDO DO)
        {
            return new FileInfoVO(DO.TemporaryFileID, DO.TemporaryFileName, DO.TemporaryFilePath, DO.TemporaryFileSize);
        }
    }
}
