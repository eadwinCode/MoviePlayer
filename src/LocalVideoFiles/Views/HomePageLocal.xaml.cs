using LocalVideoFiles.ViewModels;
using Movies.Models.Interfaces;
using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LocalVideoFiles.Views
{
    /// <summary>
    /// Interaction logic for HomePageLocal.xaml
    /// </summary>
    public partial class HomePageLocal : Page,IMainPage
    {
        public ContentControl Docker { get { return HomePageDock; } }

        public IMenuFlyout FlyoutMenu { get; set; }

        private IWindowsCommandButton WindowCommandButton;

        public HomePageLocal()
        {
            InitializeComponent();
            this.DataContext = new HomePageLocalViewModel(this);
            this.Loaded += HomePageLocal_Loaded;
            //stplaylistcontrol = this.playlistcontrol;
        }

        private void HomePageLocal_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = (HomePageLocalViewModel)this.DataContext;
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
}
