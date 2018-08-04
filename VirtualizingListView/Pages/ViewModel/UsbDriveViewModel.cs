using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace VirtualizingListView.Pages.ViewModel
{
    public class UsbDriveViewModel:NotificationObject
    {
        public UsbDriveViewModel()
        {
            LoadExternalDrives();
        }

        private ObservableCollection<VideoFolder> usbdrives;

        public ObservableCollection<VideoFolder> UsbDrives
        {
            get { return usbdrives; }
            set { usbdrives = value; RaisePropertyChanged(() => this.UsbDrives); }
        }

        public void OnLoaded()
        {

        }

        private void LoadExternalDrives()
        {
            var drives = DriveInfo.GetDrives();
                         //where drive.DriveType == DriveType.Removable
                         //select drive;

            foreach (var drive in drives)
            {
                if(drive.DriveType == DriveType.Removable)
                    Console.WriteLine(drive.Name);
            }
        }

    }
}
