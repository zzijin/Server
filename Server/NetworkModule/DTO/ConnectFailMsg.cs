namespace Server.NetworkModule.DTO
{
    class ConnectFailMsg
    {
        public string Msg { get; set; }

        public ConnectFailMsg(string msg)
        {
            Msg = msg;
        }
    }
}
