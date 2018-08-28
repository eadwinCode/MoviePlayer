using Movies.MoviesInterfaces;
using RealMediaControl.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace Movies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : IShell
    {
        public MainView(VideoPlayerVM videoPlayerVM)
        {
            InitializeComponent();
            this.DataContext = videoPlayerVM;
            this.Loaded += videoPlayerVM.Window_Loaded;
            this.Loaded += MainView_Loaded;
        }
        

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };
}
