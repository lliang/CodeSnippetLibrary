using System;

namespace CodeSnippetLibrary.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 判断当前时间是否在指定的时间段内,如果起始时间和截止时间都为DateTime.MinValue则返回True
        /// </summary>
        /// <param name="dateTime">当前时间</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <returns>Boolean 是否在指定的时间段内</returns>
        public static bool HasInTimeRange(this DateTime dateTime, DateTime startTime, DateTime endTime)
        {
            /*
             * 1.起始时间和截止时间都不是DateTime.MinValue
             * 2.起始时间不为DateTime.MinValue
             * 3.截止时间不为DateTime.MinValue
             * 4.起始时间和截止时间都为DateTime.MinValue
            */
            if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
            {
                return DateTime.Compare(dateTime, startTime) >= 0 && DateTime.Compare(dateTime, endTime) <= 0;
            }
            if (startTime != DateTime.MinValue)
            {
                return DateTime.Compare(dateTime, startTime) >= 0;
            }
            if (endTime != DateTime.MinValue)
            {
                return DateTime.Compare(dateTime, endTime) <= 0;
            }

            return true;
        }
    }
}
