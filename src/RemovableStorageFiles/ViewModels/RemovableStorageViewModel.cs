using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using PresentationExtension;
using RemovableStorageFiles.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemovableStorageFiles.ViewModels
{
    public class RemovableStorageViewModel : NotificationObject
    {
        public DelegateCommand<WindowCommandButton> usbdrivecommand;
        public DelegateCommand<WindowCommandButton> UsbDriveCommand
        {
            get
            {
                if (usbdrivecommand == null)
                    usbdrivecommand = new DelegateCommand<WindowCommandButton>((o) =>
                    {
                        var data = o;
                        data.SetActive(true, true);
                    });
                return usbdrivecommand;
            }
        }
        
        private HamburgerMenuIconItem usbDrive;

        public HamburgerMenuIconItem UsbDrive
        {
            get { return usbDrive; }
            set { usbDrive = value; this.RaisePropertyChanged(() => this.UsbDrive); }
        }

        public RemovableStorageViewModel()
        {
            InitHamBurgerMenu();
        }

        private void InitHamBurgerMenu()
        {

            UsbDrive = new HamburgerMenuIconItem()
            {
                Label = "Removable Storage",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Usb },
                Tag = new UsbDrivePage()
            };

            //MediaServer = new HamburgerMenuIconItem()
            //{
            //    Label = "Media server",
            //    Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.ServerNetwork },
            //    Tag = new MediaServerPage()
            //};
        }
    }
}
