using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Movies.MovieServices.Services
{
    public class DispatcherService : IDispatcherService
    {
        private Dispatcher dispatcher = Application.Current.Dispatcher;
        public void BeginInvokeDispatchAction(Action action)
        {
            dispatcher.BeginInvoke(action,DispatcherPriority.Background);
        }

        public void ExecuteTimerAction(Action callback, long millisecond)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(millisecond);
            dispatcherTimer.Tick += (s, e) =>
            {
                dispatcherTimer.Stop();
                callback.Invoke();
            };
            dispatcherTimer.Start();
        }

        public void InvokeDispatchAction(Action action)
        {
            dispatcher.Invoke(action, DispatcherPriority.Background);
        }
    }
}
