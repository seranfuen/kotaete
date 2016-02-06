using Resources;
using System;

namespace KotaeteMVC.Helpers
{
    public static class TimeHelper
    {
        public static string GetTimeAgo(DateTime dateTime)
        {
            var diff = DateTime.Now - dateTime;
            if (diff.Days > 0)
            {
                return diff.Days == 1 ? MainGlobal.OneDayAgo : string.Format(MainGlobal.DaysAgo, diff.Days);
            }
            else if (diff.Hours > 0)
            {
                return diff.Hours == 1 ? MainGlobal.OneHourAgo : string.Format(MainGlobal.HoursAgo, diff.Hours);
            }
            else if (diff.Minutes >= 5)
            {
                return string.Format(MainGlobal.MinutesAgo, diff.Minutes);
            }
            else
            {
                return MainGlobal.JustNow;
            }
        }
    }
}