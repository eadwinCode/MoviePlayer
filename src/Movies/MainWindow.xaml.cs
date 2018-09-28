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
    public partial class MainView : IShellWindow
    {
        public MainView(IShellWindowService ishellwindowservice)
        {
            InitializeComponent();
            this.DataContext = ishellwindowservice;
            this.Loaded += (s,e) =>ishellwindowservice.OnWindowsLoaded();
        }
    }
    
}
