using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Helpers
{
    public static class TimeHelper
    {
        public static string GetTimeAgo(this Controller controller, DateTime dateTime)
        {
            var diff = DateTime.Now - dateTime;
            if (diff.Days > 0)
            {
                return diff.Days + (diff.Days == 1 ? " day ago" : " days ago");
            } else if (diff.Hours > 0)
            {
                return diff.Hours + (diff.Hours == 1 ? " hour ago" : " hours ago");
            }
            else if (diff.Minutes > 5)
            {
                return diff.Minutes + " minutes ago";
            } else
            {
                return " just now";
            }
        }
    }
}