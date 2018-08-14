using Common.Interfaces;
using Common.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.Util;
using VirtualizingListView.Pages.Views;
using VirtualizingListView.Util;

namespace VirtualizingListView.Pages.ViewModel
{
    public class UsbDriveViewModel:NotificationObject
    {
        private IMainPages PageOwner;
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
            PageOwner = pageowner;
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
                if (drive.DriveType == DriveType.Removable)
                    folderlist.Add(new VideoFolder(drive.RootDirectory.FullName));
            }

            MergeChanges(folderlist);
        }

        private void OpenFolderCommandAction(object obj)
        {
            if ((obj as VideoFolder).Directory.Exists)
                this.PageOwner.NavigationService.Navigate(new FilePageView(this.PageOwner.NavigationService), obj);
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
                videoFolder.IsLoading = true;
            }
            if (UsbDrives.Count == 0)
            {
                HasNoFolderAction();
                return;
            }

            StartFileLoading();
        }

        private void HasNoFolderAction()
        {
            UsbNoFolder UsbNoFolder = new UsbNoFolder(PageOwner.Docker);
            UsbNoFolder.OnFinished += UsbNoFolder_OnFinished; ;
            UsbNoFolder.Show();
        }

        private void UsbNoFolder_OnFinished(object sender, System.Windows.RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>Application.Current.Dispatcher.Invoke(new Action(()=> LoadExternalDrives())));
        }

        private void StartFileLoading()
        {
            if (UsbDrives != null && UsbDrives.Count == 0) return;

            FileLoaderCompletion fileCompletionLoader = new FileLoaderCompletion();
            Threading.Executor executor = new Threading.ExecutorImpl();
            var task = executor.CreateTask(() =>
            {
                fileCompletionLoader.FinishCollectionLoadProcess(UsbDrives, true);
                FileLoader.FileLoaderInstance.InitGetAllFiles(UsbDrives);

            }, "Loading your movie folders!.", () =>
            {
                CommandManager.InvalidateRequerySuggested();
                RaisePropertyChanged(() => this.UsbDrives);
            });

            executor.Execute(task);
        }

    }
}
