using Common.ApplicationCommands;
using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
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
        IEventManager EventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }
        public MainView(IShellWindowService ishellwindowservice)
        {
            InitializeComponent();
            this.Title = ApplicationConstants.SHELLWINDOWTITLE;
            this.DataContext = ishellwindowservice;
            this.Loaded += (s,e) =>ishellwindowservice.OnWindowsLoaded();
            EventManager.GetEvent<FullScreenNotice>().Subscribe((o) =>
            {
                  statusBorder.Visibility = o ? Visibility.Collapsed:Visibility.Visible;
            });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
    }
    
}
