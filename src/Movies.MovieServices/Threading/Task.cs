using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Movies.MovieServices.Threading
{
    internal delegate void TaskStartedEvent();
    internal delegate void TaskCompletedEvent(MovieTask sender);

    public abstract class MovieTask : ITask
    {
        internal event TaskStartedEvent Started = delegate { };
        internal event TaskCompletedEvent Completed = delegate { };

        private BackgroundWorker _worker;
        private Action _action;
        private Action _callbackAction = delegate { };
        private string _message;
        private string _id;
        private IStatusMessage processstatusnotice;

        public void Run()
        {
            _worker.RunWorkerAsync();
            Started.Invoke();
        }

        public string GetMessage()
        {
            if (ProcessStatusNotice == null)
                return null;
            return ProcessStatusNotice.Message;
        }

        public string GetSubscription()
        {
            return _id;
        }

        internal class TaskImpl : MovieTask
        {
            public TaskImpl(Action action, string message, Action callback = null)
            {
                _id = Guid.NewGuid().ToString();
                _worker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _action = action;
                if(!String.IsNullOrEmpty(message))
                    processstatusnotice = IStatusMessageManager.CreateMessage(message);
                _callbackAction = callback;
                _worker.DoWork += _worker_DoWork;
                _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            }
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Completed.Invoke(this);
            if (_callbackAction != null)
                _callbackAction.Invoke();
            if (processstatusnotice != null)
            {
                processstatusnotice.Message += " -- Completed";
                processstatusnotice.AutomateMessageDestroy(DestroyTime.Short);
            }
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _action.Invoke();
            if (_worker.CancellationPending == true) e.Cancel = true;
        }

        public void Stop()
        {
            if (!_worker.IsBusy) return;
            _worker.CancelAsync();
        }

        public bool IsCancelled
        {
            get { return _worker.CancellationPending; }
        }

        public bool IsBusy { get { return _worker.IsBusy; } }

        public IStatusMessage ProcessStatusNotice
        {
            get { return processstatusnotice; }
        }

        private IStatusMessageManager IStatusMessageManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IStatusMessageManager>();
            }
        }
    }
}

