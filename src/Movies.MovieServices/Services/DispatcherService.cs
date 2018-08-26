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
        public void InvokeDispatchAction(Action action)
        {
            dispatcher.Invoke(action, DispatcherPriority.Background);
        }

        public void BeginInvokeDispatchAction(Dispatcher customDispatcher,Action action)
        {
            customDispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }
        public void InvokeDispatchAction(Dispatcher customDispatcher, Action action)
        {
            customDispatcher.Invoke(action, DispatcherPriority.Background);
        }
        public void ExecuteTimerAction(Action callback, long millisecond)
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
