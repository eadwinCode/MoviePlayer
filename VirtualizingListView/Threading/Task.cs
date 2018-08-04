using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace VirtualizingListView.Threading
{
    internal delegate void TaskStartedEvent();
    internal delegate void TaskCompletedEvent(Task sender);

    public abstract class Task
    {
        internal event TaskStartedEvent Started = delegate { };
        internal event TaskCompletedEvent Completed = delegate { };

        private BackgroundWorker _worker;
        private Action _action;
        private Action _callbackAction = delegate { };
        private string _message;
        private string _id;

        internal void Run()
        {
            _worker.RunWorkerAsync();
            Started.Invoke();
        }

        internal string GetMessage()
        {
            return _message;
        }

        public string GetSubscription()
        {
            return _id;
        }

        internal class TaskImpl : Task
        {
            public TaskImpl(Action action, string message, Action callback = null)
            {
                _id = Guid.NewGuid().ToString();
                _worker = new BackgroundWorker();
                _worker.WorkerSupportsCancellation = true;
                _action = action;
                _message = message;
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
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _action.Invoke();
            if (_worker.CancellationPending == true) e.Cancel = true;
        }

        internal void Stop()
        {
            if (!_worker.IsBusy) return;
            _worker.CancelAsync();
        }

        public bool IsCancelled
        {
            get { return _worker.CancellationPending; }
        }

        internal bool IsBusy { get { return _worker.IsBusy; } }
    }
}

