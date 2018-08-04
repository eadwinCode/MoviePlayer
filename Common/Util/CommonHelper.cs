using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Util
{
    public static class CommonHelper
    {
        public static string SetDuration(double p)
        {
            int hour;
            int minute;
            int seconds;
            hour = (int)(p / 3600);
            minute = (int)(p % 3600) / 60;
            seconds = (int)((p % 3600) % 60);
            return string.Format("{0:00}:{1:00}:{2:00}", hour, minute, seconds);
        }

        public static string SetPlayerTitle(string heading, string p)
        {
            //var filename = Path.GetFileName(p);
            return (heading + " - " + p);
        }

        public static string GetFileName(string filepath)
        {
            return Path.GetFileName(filepath);
        }
    }
}
