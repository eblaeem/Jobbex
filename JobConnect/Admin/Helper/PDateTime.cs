using System;
using System.Globalization;
using System.Linq;

namespace Oms.AdminUi.Models.Extensions
{
    public class PDateTime
    {
        public static string GetTime(DateTime dateTime)
        {
            return $"{dateTime.Hour:00}:{dateTime.Minute:00}";
        }

        public static string GetPersianDate(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return string.Empty;

            var persianCalendar = new PersianCalendar();

            var year = persianCalendar.GetYear(dateTime);
            var month = persianCalendar.GetMonth(dateTime);
            var day = persianCalendar.GetDayOfMonth(dateTime);

            return $"{year:0000}/{month:00}/{day:00}";
        }

        public static string GetPersianDateTime(DateTime dateTime)
        {
            if (dateTime.Date == DateTime.MinValue.Date)
                return string.Empty;

            var persianCalendar = new PersianCalendar();

            var year = persianCalendar.GetYear(dateTime);
            var month = persianCalendar.GetMonth(dateTime);
            var day = persianCalendar.GetDayOfMonth(dateTime);
            var hour = persianCalendar.GetHour(dateTime);
            var minute = persianCalendar.GetMinute(dateTime);
            var second = persianCalendar.GetSecond(dateTime);

            return $"{year:0000}/{month:00}/{day:00} {hour:00}:{minute:00}:{second:00}";
        }

        public static DateTime GetGregorian(string persianDate)
        {
            var dateParts = persianDate.Split("/");


            var persianCalendar = new PersianCalendar();
            int year = Convert.ToInt32(dateParts[0]);
            int month = Convert.ToInt32(dateParts[1]);
            int day = Convert.ToInt32(dateParts[2]);


            //var a =new DateTime(year, month, day, persianCalendar);
            return new DateTime(year, month, day, persianCalendar);
        }

        public static DateTime GetGregorianDateTime(string persianDate)
        {
            var datetime = persianDate.Split(" ");

            var dateParts = datetime[0].Split("/");

            var persianCalendar = new PersianCalendar();
            int year = Convert.ToInt32(dateParts[0]);
            int month = Convert.ToInt32(dateParts[1]);
            int day = Convert.ToInt32(dateParts[2]);

            int hour = 0;
            int minute = 0;
            int second = 0;
            var milliseconds = 0L;
            if (datetime.Length > 1)
            {
                var time = datetime.Last();
                var timeSplited = time.Split('.');
                if (timeSplited.Length > 1)
                {
                    long.TryParse(timeSplited.Last(), out milliseconds);
                }
                var timeParts = timeSplited[0].Split(":");
                hour = Convert.ToInt32(timeParts[0]);
                minute = Convert.ToInt32(timeParts[1]);
                second = Convert.ToInt32(timeParts[2]);
            }

            var convertedDateTime = new DateTime(year, month, day, hour, minute, second, persianCalendar);
            convertedDateTime = convertedDateTime.AddMilliseconds(milliseconds);
            return convertedDateTime;
        }
    }
}
