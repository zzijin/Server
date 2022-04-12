namespace Server.NetworkModule.DTO
{
    class ErrorMsg
    {
        public int ErrorType { get; set; }
        public string Msg { get ; set; }
        public byte[] ErrorData { get; set; }
        public ErrorMsg(int errorType,string errorMsg,byte[] errorData)
        {
            ErrorType = errorType;
            Msg = errorMsg;
            ErrorData = errorData;
        }
    }
}
