using Movies.Enums;
using Movies.MoviesInterfaces;
using Movies.StatusService.Models;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace Movies.StatusService.Services
{
    public abstract class StatusMessageManager : IStatusMessageManager
    {
        private IDictionary<Guid, IStatusMessage> messageCollection;
        private object padlock = new object();
        private Timer dispatcherTimer;
        int messageCount;
        IStatusMessage defaultstatusmessage = new StatusMessage("Done");
        public IDictionary<Guid, IStatusMessage> MessageCollection { get { return messageCollection; } }

        public IStatusMessage DefaultStatusMessage { get { return defaultstatusmessage; } }

        public event EventHandler OnMessageCollectionChanged;
        IEventManager EventManager;
        public StatusMessageManager(IEventManager eventManager)
        {
            messageCollection = new Dictionary<Guid, IStatusMessage>();
            this.EventManager = eventManager;
           // TestStatusMessage();
        }
        private void TestStatusMessage()
        {
            dispatcherTimer = new Timer
            {
                Interval = 20000
            };
            dispatcherTimer.Elapsed += (s, e) =>
            {
                this.CreateMessage("<--Testing  message at " + DateTime.Now.TimeOfDay + " -->", 50000 );
                if (messageCount > 6)
                    dispatcherTimer.Stop();

                messageCount++;
            };
            dispatcherTimer.Start();
        }
        public void AutomateMessageDestroy(IStatusMessage statusMessage, long miilisecond)
        {
            statusMessage.AutomateMessageDestroy(miilisecond);
        }

        public IStatusMessage CreateMessage(string message)
        {
            lock (padlock)
            {
                StatusMessage statusMessage = new StatusMessage(message, this);
                this.Add(statusMessage);
                return statusMessage;
            }
        }

        public IStatusMessage CreateMessage(string message, long destroyTimeInMilliseconds)
        {
            lock (padlock)
            {
                StatusMessage statusMessage = new StatusMessage(message, this);
                this.Add(statusMessage);
                AutomateMessageDestroy(statusMessage, destroyTimeInMilliseconds);
                return statusMessage;
            }
        }

        private void Add(IStatusMessage statusMessage)
        {
            if (!messageCollection.ContainsKey(statusMessage.Id))
                MessageCollection.Add(statusMessage.Id, statusMessage);

            OnMessagePropertyChange();
        }
        
        public void DestroyMessage(IStatusMessage statusMessage)
        {
            lock (this)
            {
                if (messageCollection.ContainsKey(statusMessage.Id))
                    MessageCollection.Remove(statusMessage.Id);

                OnMessagePropertyChange();
            }
        }

        private void OnMessagePropertyChange()
        {
            if (OnMessageCollectionChanged != null)
                OnMessageCollectionChanged.Invoke(this, EventArgs.Empty);
            EventManager.GetEvent<StatusMessageChangedEventToken>().Publish(true);
        }

        public void DestroyMessage(IStatusMessage statusMessage, DestroyTime UsePredefinedTime)
        {
            statusMessage.AutomateMessageDestroy(UsePredefinedTime);
        }
    }

    public class StatusManager : StatusMessageManager {
        public StatusManager(IEventManager eventManager):base(eventManager)
        {

        }
    }
}
