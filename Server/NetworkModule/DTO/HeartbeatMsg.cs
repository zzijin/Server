namespace Server.NetworkModule.DTO
{
    class HeartbeatMsg
    {
        public string Msg { get; set; }

        public HeartbeatMsg(string msg)
        {
            Msg = msg;
        }
    }
}
