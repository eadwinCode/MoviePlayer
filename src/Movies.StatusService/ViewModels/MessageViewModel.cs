using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Movies.MoviesInterfaces;
using Movies.StatusService.Models;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Movies.StatusService.ViewModels
{
    public class MessageViewModel : NotificationObject
    {
        IStatusMessageManager StatusMessageManager;
        IDispatcherService DispatcherService;
        public DelegateCommand<Popup> ShowPopup { get; private set; }
        object _lock = new object();
        public MessageViewModel(IStatusMessageManager statusMessageManager,
            IDispatcherService dispatcherService, IEventManager eventManager)
        {
            this.StatusMessageManager = statusMessageManager;
            this.DispatcherService = dispatcherService;
            ShowPopup = new DelegateCommand<Popup>(ShowPopupAction, CanShowPopup);
            _messages = new ObservableCollection<IStatusMessage>();

            eventManager.GetEvent<StatusMessageCreatedEventToken>().Subscribe((o) =>
            {
                dispatcherService.InvokeDispatchAction(() =>
                {
                    lock (_lock)
                    {
                        Messages.Add(o as IStatusMessage);
                    }
                });

                UpdateProperties();
            });

            eventManager.GetEvent<StatusMessageDeletedEventToken>().Subscribe((o) =>
            {
                dispatcherService.InvokeDispatchAction(() =>
                {
                    lock (_lock)
                    {
                        Messages.Remove(o as IStatusMessage);
                    }
                });
                UpdateProperties();
            });
        }

        private void UpdateProperties()
        {
            RaisePropertyChanged(() => this.IncomingMessage);
            RaisePropertyChanged(() => this.MessageCount);
            RaisePropertyChanged(() => this.MessageCountVisibility);
            ShowPopup.RaiseCanExecuteChanged();
        }

        private void ShowPopupAction(Popup obj)
        {
            if (obj.IsOpen)
                obj.IsOpen = false;
            else
                obj.IsOpen = true;
        }

        private bool CanShowPopup(Popup arg)
        {
            return MessageCount > 1;
        }

        ObservableCollection<IStatusMessage> _messages;
        public ObservableCollection<IStatusMessage> Messages
        {
            get
            {
                return _messages;
                //lock (_lock)
                //{
                //    try
                //    {
                //        return new ObservableCollection<IStatusMessage>(StatusMessageManager);
                //    }
                //    catch (InvalidOperationException)
                //    {

                //        return new ObservableCollection<IStatusMessage>(StatusMessageManager);
                //    }
                    
                //}
                //return StatusMessageManager.MessageCollection.Values;
            }
            protected set { _messages = value; RaisePropertyChanged(() => this.Messages); }
        }

        public int MessageCount
        {
            get { return Messages.Count(); }
        }

        public Visibility MessageCountVisibility
        {
            get { return MessageCount > 1 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public IStatusMessage IncomingMessage
        {
            get
            {
                var incomingmesage = GetStatusMessage();
                if (incomingmesage == null)
                    return StatusMessageManager.DefaultStatusMessage;
                return incomingmesage;
            }
        }

        private IStatusMessage GetStatusMessage()
        {
            IStatusMessage statusMessage  = null;
            statusMessage = Messages.LastOrDefault();
            return statusMessage;
        }
        
    }
}
