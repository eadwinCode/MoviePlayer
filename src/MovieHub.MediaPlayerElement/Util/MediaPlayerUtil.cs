using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace MovieHub.MediaPlayerElement.Util
{
    internal class MediaPlayerUtil
    {
        public static void ExecuteTimerAction(Action callback, long millisecond)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(millisecond)
            };
            dispatcherTimer.Tick += (s, e) =>
            {
                dispatcherTimer.Stop();
                callback.Invoke();
            };
            dispatcherTimer.Start();
        }
    }
}
