using System;
using Windows.System.Profile;

namespace TeamCityMonitor.Views
{
    public static class ViewHelper
    {
        private static readonly Lazy<bool> AppIsIot = new Lazy<bool>(() => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT");

        public static bool IsIot => AppIsIot.Value;

        public static string ToAgeString(this TimeSpan age)
        {
            var seconds = age.TotalSeconds;
            var minutes = age.TotalMinutes;
            var hours = age.TotalHours;
            var days = age.TotalDays;

            if (seconds < 5) return "now";
            if (seconds < 60) return $"{seconds:0}s ago";
            if (minutes < 60) return $"{minutes:0}m ago";
            if (hours < 24) return $"{hours:0}h ago";
            return $"{days:0} days ago";
        }
    }
}
