using System;

namespace Server.Tools
{
    class TimeTool
    {
        public static long NowTimeToTimestamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds);
        }
        public static long TimeToTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds);
        }
    }
}
