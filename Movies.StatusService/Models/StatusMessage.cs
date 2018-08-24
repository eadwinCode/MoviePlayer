using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace Movies.StatusService.Models
{
    public class StatusMessage : NotificationObject, IStatusMessage
    {
        const long DefaultTime = 5000;
        private IStatusMessageManager statusMessageManager;
        private Guid id;
        private string message;
        public string Message {
            get { return message; }
            set { message = value; RaisePropertyChanged(()=>this.Message);
            }
        }

        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public IStatusMessageManager StatusMessageManager
        {
            get
            {
                return statusMessageManager;
            }
        }

        private Timer MessageDestroyTimer;

        public StatusMessage(string message,IStatusMessageManager statusMessageManager)
        {
            id = Guid.NewGuid();
            this.message = message;
            this.statusMessageManager = statusMessageManager;
        }

        public StatusMessage(string message)
        {
            id = Guid.NewGuid();
            this.message = message;
        }


        public void AutomateMessageDestroy(long millisecond)
        {
            MessageDestroyTimer = new Timer
            {
                Interval = millisecond
            };
            MessageDestroyTimer.Elapsed += (s, e) =>
             {
                 MessageDestroyTimer.Stop();
                 statusMessageManager.DestroyMessage(this);
             };
            MessageDestroyTimer.Start();
        }

        public void AutomateMessageDestroy(DestroyTime UsePredefinedTime)
        {
            switch (UsePredefinedTime)
            {
                case DestroyTime.Short:
                    AutomateMessageDestroy(5000);
                    break;
                case DestroyTime.Long:
                    AutomateMessageDestroy(10000);
                    break;
                default:
                    break;
            }
        }
    }
}
