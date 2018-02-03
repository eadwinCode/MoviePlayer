using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using RealMediaControl.ViewModel;
using System.ComponentModel;
using VirtualizingListView.View;
using Microsoft.Practices.Prism.Events;
using VideoPlayer;
using Common;
using Common.ApplicationCommands;
using Common.Interfaces;

namespace Movies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window, IShell
    {
        public MainView()
        {
            InitializeComponent();
            VideoPlayerVM VM = new VideoPlayerVM();
            this.DataContext = VM;
            this.Loaded += VM.Window_Loaded;
        }

        private void MediaElementPlayer_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (mediacontrol.CanAnimate && mediacontrol.IsPlaying)
            //{
            //    this.MousemoveTimer.Start();
            //}
        }

        private void MediaElementPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (!mediacontrol.MovieTitle_Tab.IsCanvasDrag)
            //{
            //    if (mediacontrol.IsMouseControlOver)
            //    {
            //        return;
            //    }
            //   this.MousemoveTimer.Start();
            //}
            
        }
        

        private void ParentGrid_MouseMove(object sender, MouseEventArgs e)
        {
            //if (ScreenSetting == SCREENSETTINGS.Normal)
            //{
            //    return;
            //}

            //if (!mediacontrol.MovieTitle_Tab.IsCanvasDrag && !mediacontrol.IsMouseControlOver)
            //{

            //    this.MousemoveTimer.Stop();
            //    Cursor = Cursors.Arrow;
            //    SetIsMouseOverMediaElement(mediacontrol as UIElement, true);
            //    this.MousemoveTimer.Start();
            //}

        }

        

        public IFileViewer FileView { get { return this.fileView; } }
        

        private void MainView_Closing(object sender, CancelEventArgs e)
        {
            //var ser = SerializeDeserializeHelper.Serialize<LastSeen>(VideoComponentViewModel.LastSeen);
        }
        
    }

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };
}
