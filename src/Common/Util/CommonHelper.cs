using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

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

        public static double GetMousePointer(Control obj)
        {
            var x = Mouse.GetPosition(obj).X;
            var ratio = x / obj.ActualWidth;
            return ratio;
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
