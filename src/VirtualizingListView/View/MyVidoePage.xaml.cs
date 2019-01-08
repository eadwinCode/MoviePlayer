using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            CurrentVideoFolder = new MediaFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
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
