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
            eventManager.GetEvent<StatusMessageChangedEventToken>().Subscribe((o) =>
            {
                RaisePropertyChanged(() => this.Messages);
                RaisePropertyChanged(() => this.IncomingMessage);
                RaisePropertyChanged(() => this.MessageCount);
                RaisePropertyChanged(() => this.MessageCountVisibility);
                ShowPopup.RaiseCanExecuteChanged();
            });

            eventManager.GetEvent<MessageChangedEventToken>().Subscribe((o) =>
            {

            });
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

        public ObservableCollection<IStatusMessage> Messages
        {
            get
            {
                lock (_lock)
                {
                    return new ObservableCollection<IStatusMessage>(StatusMessageManager.MessageCollection.Values);
                }
            }
        }

        public int MessageCount
        {
            get { return Messages.Count; }
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
            DispatcherService.InvokeDispatchAction(() => { statusMessage =  Messages.LastOrDefault(); });
            return statusMessage;
        }
        
    }
}
