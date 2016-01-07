using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Helpers
{
    public static class StringHelper
    {
        public static List<string> SplitLines(this string str)
        {
            return str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
        }
    }
}