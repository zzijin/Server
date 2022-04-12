using Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server.NetworkModule.DTO
{
    class ConnectTokenMsg
    {
        public byte[] OpenID { get; set; }
        public byte[] Key { get; set; }

        [JsonIgnore]
        public int Index { get
            {
                if (OpenID==null?true:(OpenID.Length < 4))
                {
                    return -1;
                }
                ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(OpenID[^4..]);
                return ConvertTypeTool.ByteArrayToInt32(span);
            } 
        }

        public ConnectTokenMsg(byte[] openID,byte[] key)
        {
            OpenID = openID;
            Key = key;
        }
    }
}
