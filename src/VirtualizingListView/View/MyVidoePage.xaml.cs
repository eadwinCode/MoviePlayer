using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using VirtualizingListView.Pages.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for MyVidoePage.xaml
    /// </summary>
    public partial class MyVidoePage : Page ,IMainPage
    {
        public ContentControl Docker { get { return HomePageDock; } }

        public IMenuFlyout FlyoutMenu { get; set; }

        private IWindowsCommandButton WindowCommandButton;

        public MyVidoePage()
        {
            InitializeComponent();
            this.DataContext = new MyVidoePageViewModel(this,this.Dispatcher);
            this.Loaded += MyVidoePage_Loaded;
            //stplaylistcontrol = this.playlistcontrol;
        }

        private void MyVidoePage_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = (MyVidoePageViewModel)this.DataContext;
            datacontext.InitDataSource();
            if (WindowCommandButton != null)
                WindowCommandButton.SetActive(true, false);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        }
    }

    internal class MyVidoePageViewModel : FilePageViewModel
    {
        private IMainPage PageOwner;
        private bool hasloaded;
        public MyVidoePageViewModel(IMainPage owner,Dispatcher dispatcher):base(dispatcher)
        {
            PageOwner = owner;
            CurrentVideoFolder = new VideoFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            FileWatceherSubscription(CurrentVideoFolder);
        }
        
        internal void InitDataSource()
        {
            if (!hasloaded)
            {
                hasloaded = true;
                this.IsLoading = true;
                this.AsynLoadData(CurrentVideoFolder, String.Format("Loading in {0} files", CurrentVideoFolder.Name), () =>
                 {
                     this.IsLoading = false;
                 });
            }
        }
    }
}
