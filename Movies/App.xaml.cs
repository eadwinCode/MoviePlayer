using Common.FileHelper;
using RealMediaControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoPlayerControl;

namespace Movies
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string message = e.Exception.InnerException == null ? e.Exception.Message : e.Exception.InnerException.Message;
            MessageBox.Show(message, "Movie Player", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        public Button normmiaxbtn;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
           
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);
        }
        
    }
}
