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
    internal class FileInfoConvert
    {
        public static FileInfoDO VOTransformToDO(FileInfoVO VO)
        {
            return new FileInfoDO()
            {
                FileID = VO.FileID,
                FileName = VO.FileName,
                FileOffset = VO.FileOffset,
                FileSize = VO.FileSize,
                FileLastEditUser = DBConfig.ServerUser,
                FileLastEditDate = DateTime.Now,
            };
        }

        public static FileInfoVO DOTransformToVO(FileInfoDO fileInfoDO,FileBlockInfoDO fileBlockInfoDO)
        {
            return new FileInfoVO(fileInfoDO.FileID, fileInfoDO.FileName,fileBlockInfoDO.FileBlockPath,fileInfoDO.FileOffset,fileInfoDO.FileSize);
        }
    }
}
