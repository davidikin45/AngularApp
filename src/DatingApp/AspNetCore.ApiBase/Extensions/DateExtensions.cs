using AspNetCore.ApiBase.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace AspNetCore.ApiBase.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime ToEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        //Json.NET returns all dates as Unspecified

        //UTC > LocalTime = Fine
        //LocalTime > UTC = Fine

        //Unspecified(UTC) > LocalTime
        //Unspecified(LocalTime) > UTC

        public static DateTime ToConfigLocalTime(this IHtmlHelper htmlhelper, DateTime utcDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ);
        }

        public static string ToConfigLocalTimeString(this IHtmlHelper htmlhelper, DateTime utcDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return String.Format("{0} ({1})", TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortDateString(), appSettings.TimezoneAbbr);
        }

        public static string ToConfigLocalTimeStringNoTimezone(this IHtmlHelper htmlhelper, DateTime utcDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return String.Format("{0}", TimeZoneInfo.ConvertTimeFromUtc(utcDT, istTZ).ToShortDateString());
        }

        public static DateTime FromConfigLocalTimeToUTC(this IHtmlHelper htmlhelper, DateTime localConfigDT)
        {
            var appSettings = (AppSettings)htmlhelper.ViewContext.HttpContext.RequestServices.GetService(typeof(AppSettings));
            var istTZ = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Timezone);
            return TimeZoneInfo.ConvertTimeToUtc(localConfigDT, istTZ);
        }
    }
}
