using Common.FileHelper;
using RealMediaControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoPlayer;

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

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);
        }
        
    }
}
