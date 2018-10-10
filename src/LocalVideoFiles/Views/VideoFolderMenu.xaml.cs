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

namespace LocalVideoFiles.Views
{
    /// <summary>
    /// Interaction logic for VideoFolderMenu.xaml
    /// </summary>
    public partial class VideoFolderMenu 
    {
        public VideoFolderMenu()
        {
            InitializeComponent();
            this.DataContext = new VideoFolderMenuViewModel();
        }
    }

    public class VideoFolderMenuViewModel
    {
        private INavigatorService INavigatorService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<INavigatorService>();
            }
        }

        private DelegateCommand loadpageCommand;
        private HomePageLocal VideoFolderPage;
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

        public VideoFolderMenuViewModel()
        {
            Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Folder };
            VideoFolderPage = new HomePageLocal();
        }

        private void LoadPageAction()
        {
            INavigatorService.NavigateMainPage(VideoFolderPage);
        }
    }
}
