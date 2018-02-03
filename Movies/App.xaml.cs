using RealMediaControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Movies
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Button normmiaxbtn;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bb = (Button)sender;
            if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                bb.Style = (Style)Application.Current.Resources["normbtn"];
                bb.ToolTip = "Restore Down";
            }
            else
            {
                bb.Style = (Style)Application.Current.Resources["maxbtn"];
                bb.ToolTip = "Maximize";
            }
        }
    }
}
