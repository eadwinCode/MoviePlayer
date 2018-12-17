using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
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

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for MyVideoPageMenu.xaml
    /// </summary>
    public partial class MyVideoPageMenu 
    {
        public MyVideoPageMenu()
        {
            InitializeComponent();
            this.DataContext = new MyVideoPageMenuViewModel();
        }
    }

    class MyVideoPageMenuViewModel
    {
        private INavigatorService INavigatorService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<INavigatorService>();
            }
        }

        private DelegateCommand loadpageCommand;
        private MyVidoePage MyVideoFolderPage;
        public DelegateCommand LoadPageCommand
        {
            get
            {
                if (loadpageCommand == null)
                {
                    loadpageCommand = new DelegateCommand(LoadPageAction);
                }
                return loadpageCommand;
            }
        }

        public object Icon { get; private set; }

        public MyVideoPageMenuViewModel()
        {
            Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Video };
            MyVideoFolderPage = new MyVidoePage();
        }

        private void LoadPageAction()
        {
            INavigatorService.NavigateMainPage(MyVideoFolderPage);
        }
    }
}
