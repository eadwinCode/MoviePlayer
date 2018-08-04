using Common.Interfaces;
using Common.Themes.Skin;
using RealMediaControl.ViewModel;
using System;
using System.Windows;

namespace Movies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : IShell
    {
        public Skin CurrentSkin { get; set; }
        public IPageNavigatorHost PageNavigatorHost
        {
            get { return this.pagenavigatorhost; }
        }

        public MainView()
        {
            InitializeComponent();
            VideoPlayerVM VM = new VideoPlayerVM();
            this.DataContext = VM;
            this.Loaded += VM.Window_Loaded;
            CurrentSkin = new ReferencedAssemblySkin("Black Skin", 
                new Uri("/Common;component/Themes/Hybrid.xaml", UriKind.Relative));

            this.Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentSkin.Load();
        }

        private void Red_Click(object sender, RoutedEventArgs e)
        {
            CurrentSkin.Unload();
            CurrentSkin = 
                new ReferencedAssemblySkin("Black Skin", 
                new Uri("/Common;component/Themes/BlackSkin.xaml", UriKind.Relative));
            CurrentSkin.Load();
        }

        private void White_Click(object sender, RoutedEventArgs e)
        {
            CurrentSkin.Unload();
            CurrentSkin = 
                new ReferencedAssemblySkin("Black Skin", 
                new Uri("/Common;component/Themes/WhiteSkin.xaml", UriKind.Relative));
            CurrentSkin.Load();
        }

        private void Hybrid_Click(object sender, RoutedEventArgs e)
        {
            CurrentSkin.Unload();
            CurrentSkin = new ReferencedAssemblySkin("Black Skin", 
                new Uri("/Common;component/Themes/hybrid.xaml", UriKind.Relative));
            CurrentSkin.Load();
        }
    }

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };
}
