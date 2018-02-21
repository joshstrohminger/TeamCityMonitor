using System;
using Windows.System.Profile;

namespace TeamCityMonitor.Views
{
    public static class ViewHelper
    {
        private static readonly Lazy<bool> AppIsIot = new Lazy<bool>(() => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT");

        public static bool IsIot => AppIsIot.Value;
    }
}
