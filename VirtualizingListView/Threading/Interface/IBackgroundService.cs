using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualizingListView.Threading.Interface
{
    public interface IBackgroundService
    {
        void Shutdown();
        void Shutdown(Task task);
        void Execute();
        void Execute(Task task);
        Task Execute(Action action, string message = null, Action callback = null);
        //Task CreateTask<T> (Action<T> action, string message, Action callback);
    }
}
