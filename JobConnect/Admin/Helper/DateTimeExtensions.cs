using System;
using System.Data;

namespace Oms.AdminUi.Models.Extensions
{
    public static class DateTimeExtensions
    {
        public static TimeSpan ToTimeSpan(this string timeFormat)
        {
            if (string.IsNullOrEmpty(timeFormat)) return default;
            return TimeSpan.Parse(timeFormat);
        }

        public static string ToPersianDate(this DateTime dateTime)
        {
            if (dateTime==default) return string.Empty;
            return PDateTime.GetPersianDate(dateTime);
        }
        public static string ToPersianDateTime(this DateTime dateTime)
        {
            if (dateTime == default) return string.Empty;
            if (dateTime.Millisecond > 0)
            {
                return PDateTime.GetPersianDate(dateTime) + " " + dateTime.TimeOfDay.ToString("g");
            }
            else
            {
                return PDateTime.GetPersianDateTime(dateTime);
            }
        }
        public static string ToPersianDateTime(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return string.Empty;
            return ToPersianDateTime(dateTime.Value);
        }

        public static DateTime ToGregorian(this string persianDate)
        {
            if (string.IsNullOrWhiteSpace(persianDate)) return default;
            return PDateTime.GetGregorian(persianDate);
        }

        public static DateTime? ToGregorianDateTime(this string persianDate, DateTime? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(persianDate)) return defaultValue;
            var dt = PDateTime.GetGregorianDateTime(persianDate);
            if (dt == default) return defaultValue;
            return dt;
        }
        public static string RemoveDefaultTimeString(this string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString)) return dateTimeString;
            var indexOfSpace = dateTimeString.IndexOf(' ');
            if (indexOfSpace >= 1)
            {
                return dateTimeString.Substring(0, indexOfSpace);
            }

            return dateTimeString;
        }


        public static string ToTime(this DateTime date)
        {
            return date.ToString("HH:mm:ss");
        }
    }
}
