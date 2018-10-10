using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemovableStorageFiles.Views
{
    /// <summary>
    /// Interaction logic for VideoFolderMenu.xaml
    /// </summary>
    public partial class RemovableStorageFilesMenu
    {
        public RemovableStorageFilesMenu()
        {
            InitializeComponent();
            this.DataContext = new RemovableStorageFilesMenuViewModel();
        }
    }

    public class RemovableStorageFilesMenuViewModel
    {
        private INavigatorService INavigatorService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<INavigatorService>();
            }
        }

        private DelegateCommand loadpageCommand;
        private UsbDrivePage RemovableStorageFilesPage;
        public DelegateCommand LoadPageCommand
        {
            get
            {
                if(loadpageCommand == null)
                {
                    loadpageCommand = new DelegateCommand(LoadPageAction);
                }
                return loadpageCommand;
            }
        }

        public object Icon { get; private set; }

        public RemovableStorageFilesMenuViewModel()
        {
            Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Usb };
            RemovableStorageFilesPage = new UsbDrivePage();
        }

        private void LoadPageAction()
        {
            INavigatorService.NavigateMainPage(RemovableStorageFilesPage);
        }
    }
}
