using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using RemovableStorageFiles.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RemovableStorageFiles.ViewModels
{
    public class UsbDriveViewModel:NotificationObject
    {
        private IMainPages PageOwner;
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

        IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        IDispatcherService DispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        IBackgroundService BackgroundService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IBackgroundService>();
            }
        }

        private IEventManager IEventManager
        {
            get
            {

                return EventAggregatorService.IEventManager;
            }
        }

        private ObservableCollection<VideoFolder> usbdrives;
        public ObservableCollection<VideoFolder> UsbDrives
        {
            get { return usbdrives; }
            set { usbdrives = value; RaisePropertyChanged(() => this.UsbDrives); }
        }
        public DelegateCommand<object> OpenFolderCommand { get; private set; }
        public DelegateCommand ReloadCommand { get; private set; }

        public UsbDriveViewModel(IMainPages pageowner)
        {
            this.PageOwner = pageowner;

            OpenFolderCommand = new DelegateCommand<object>(OpenFolderCommandAction);
            ReloadCommand = new DelegateCommand(()=> { LoadExternalDrives(); });
            LoadExternalDrives();
        }

        public void OnLoaded()
        {
            LoadExternalDrives();
        }

        private void LoadExternalDrives()
        {
            var drives = DriveInfo.GetDrives();
            ObservableCollection<VideoFolder> folderlist = new ObservableCollection<VideoFolder>();

            foreach (var drive in drives)
            {
                if (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Fixed)
                {
                    if(drive.VolumeLabel != "OS")
                        folderlist.Add(new VideoFolder(drive.RootDirectory.FullName));
                }
            }

            MergeChanges(folderlist);
        }

        private void OpenFolderCommandAction(object obj)
        {
            if ((obj as VideoFolder).Directory.Exists)
            {
                IEventManager.GetEvent<NavigateFolderItemToken>().Publish(obj);
                //IEventAggregator.GetEvent<NavigateObject>().Publish(obj);
                // this.PageOwner.NavigationService.Navigate(new FilePageView(this.PageOwner.NavigationService), obj);
            }
            else
                LoadExternalDrives();
        }

        private void RemoveFolders(ref ObservableCollection<MovieFolderModel> removedFolder)
        {
            if (removedFolder != null && removedFolder.Count == 0) return;
            foreach (var item in removedFolder)
            {
                VideoFolder videoFolder = null;
            }
        }

        private void MergeChanges(ObservableCollection<VideoFolder> folderlist)
        {
            if (UsbDrives == null || folderlist.Count == 0)
                UsbDrives = new ObservableCollection<VideoFolder>();

            foreach (var item in folderlist)
            {
                VideoFolder videoFolder = item;
                var dir = new System.IO.DirectoryInfo(item.FullName);
                
                if (UsbDrives.Contains(videoFolder)) continue;
                UsbDrives.Add(videoFolder);
                //videoFolder.IsLoading = true;
            }
            if (UsbDrives.Count == 0)
            {
                HasNoFolderAction();
                return;
            }

            //StartFileLoading();
        }

        private void HasNoFolderAction()
        {
            UsbNoFolder UsbNoFolder = new UsbNoFolder(PageOwner.Docker);
            UsbNoFolder.OnFinished += UsbNoFolder_OnFinished; 
            UsbNoFolder.Show();
        }

        private void UsbNoFolder_OnFinished(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() 
                => DispatcherService.BeginInvokeDispatchAction(new Action(()=> LoadExternalDrives())));
        }

        private void StartFileLoading()
        {
            if (UsbDrives != null && UsbDrives.Count == 0) return;
            
            var task = BackgroundService.Execute(() =>
            {
                LoaderCompletion.FinishCollectionLoadProcess(UsbDrives, true);
                FileLoader.InitGetAllFiles(UsbDrives);

            }, "Loading your movie folders!.", () =>
            {
                CommandManager.InvalidateRequerySuggested();
                RaisePropertyChanged(() => this.UsbDrives);
            });
        }

    }
}
