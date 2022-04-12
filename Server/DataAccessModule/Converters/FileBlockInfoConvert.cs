using Server.DataAccessModule.Configuration;
using Server.DataAccessModule.DO;
using Server.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.Converters
{
    internal class FileBlockInfoConvert
    {
        public static FileBlockInfoDO VOTransformToDO(FileBlockInfoVO VO)
        {
            return new FileBlockInfoDO()
            {
                FileBlockID = VO.FileBlockID,
                FileBlockSize = VO.FileBlockSize,
                FileBlockFreeSize=VO.FileBlockSize,
                FileBlockUnfailed=VO.FileBlockUnfailed,
                FileBlockLastEditUser = DBConfig.ServerUser,
                FileBlockLastEditDate = DateTime.Now,
            };
        }

        public static FileBlockInfoVO DOTransformToVO(FileBlockInfoDO fileBlockInfoDO)
        {
            return new FileBlockInfoVO(fileBlockInfoDO.FileBlockID,fileBlockInfoDO.FileBlockSize,fileBlockInfoDO.FileBlockPath,fileBlockInfoDO.FileBlockUnfailed);
        }
    }
}
