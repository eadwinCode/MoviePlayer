using LocalVideoFiles.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using PresentationExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalVideoFiles.ViewModels
{
    public class VideoFolderViewModel:NotificationObject
    {
        public DelegateCommand<WindowCommandButton> videofoldercommand;
        public DelegateCommand<WindowCommandButton> VideoFolderCommand
        {
            get
            {
                if (videofoldercommand == null)
                    videofoldercommand = new DelegateCommand<WindowCommandButton>((data) =>
                    {
                        data.SetActive(true, true);
                    });
                return videofoldercommand;
            }
        }

        private HamburgerMenuIconItem videoFolder;

        public HamburgerMenuIconItem VideoFolders
        {
            get { return videoFolder; }
            set { videoFolder = value; this.RaisePropertyChanged(() => this.VideoFolders); }
        }

        public VideoFolderViewModel()
        {
            InitHamBurgerMenu();
        }

        private void InitHamBurgerMenu()
        {
            VideoFolders = new HamburgerMenuIconItem()
            {
                Label = "Video folders",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Folder },
                Tag = new HomePageLocal()
            };
        }
    }
}
