namespace Server.NetworkModule.DTO
{
    class TestMsg
    {
        public int SequenceNumber { get; set; }
        public string Msg { get; set; }

        public void AddSequenceNumber()
        {
            SequenceNumber++;
        }
    }
}
