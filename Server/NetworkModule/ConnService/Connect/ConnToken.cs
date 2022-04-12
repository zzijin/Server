using Server.Tools;
using System;
using System.Security.Cryptography;

namespace Server.NetworkModule.ConnService.Connect
{
    /// <summary>
    /// 本次连接的令牌
    /// 用于断线重连，加密解密等
    /// </summary>
    class ConnToken
    {
        /// <summary>
        /// 用于查询的客户端唯一ID
        /// </summary>
        byte[] openID;
        /// <summary>
        /// 用于核对客户端的验证密钥
        /// </summary>
        byte[] key;
        /// <summary>
        /// 用于标识链接是否断开
        /// </summary>
        //bool connConnectState;
        /// <summary>
        /// 记录意外断开链接时间
        /// </summary>
        DateTime lastDisconnectTime;
        /// <summary>
        /// 网络动荡次数
        /// 当限定时间内触发断线重连，此计数加1
        /// </summary>
        int netUnstableTimes;

        //public bool ConnConnectState { get => connConnectState; }

        private bool NetStableTotalState { get => netUnstableTimes > 10; }

        private bool NetUnstable { get => (DateTime.Now - lastDisconnectTime).TotalSeconds > 20; }

        public ConnToken()
        {
            lastDisconnectTime = DateTime.Now;
            //connConnectState = true;
            netUnstableTimes = 0;
        }

        public void Reconnect()
        {
            //connConnectState = true;
        }

        public bool AccidentalDisconnect()
        {
            if (NetStableTotalState)
            {
                return false;
            }

            if(NetUnstable)
            {
                netUnstableTimes++;
            }

            //connConnectState = false;
            lastDisconnectTime = DateTime.Now;
            return true;
        }

        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <param name="openID"></param>
        /// <returns>生成的密钥</returns>
        public void GenerateToken(int connIndex,byte[] clientID,out byte[] openID,out byte[] key)
        {
            //将客户端ID与链接编号结合为唯一ID
            int clientIDLength = clientID.Length;
            this.openID = new byte[4 + clientIDLength];
            Buffer.BlockCopy(clientID, 0, this.openID, 0, clientIDLength);
            Span<byte> openSpan = new Span<byte>(this.openID);
            ConvertTypeTool.Int32ToSpan(connIndex, openSpan, clientIDLength);

            //使用哈希得到验证密钥
            using (SHA256 cng = SHA256.Create())
            {
                this.key = cng.ComputeHash(this.openID);
            }

            openID = this.openID;
            key = this.key;
        }

        /// <summary>
        /// 验证Token
        /// 只有唯一ID和密钥均验证通过才能恢复链接
        /// </summary>
        /// <param name="clientKey"></param>
        /// <returns></returns>
        public bool AuthenticationKey(byte[] openID,byte[] clientKey)
        {
            if (Array.Equals(clientKey, this.key)&&Array.Equals(openID,this.openID))
            {
                Reconnect();
                return true;
            }

            return false;
        }

        public bool DetermineWaitReconnectInterval(int interval)
        {
            return (DateTime.Now - lastDisconnectTime).TotalMilliseconds > interval;
        }
    }
}
