using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VideoComponent.BaseClass;
using VideoPlayer;
using VideoPlayer.ViewModel;
using VirtualizingListView.View;
using VirtualizingListView.ViewModel;

namespace RealMediaControl.ViewModel
{
    public class VideoPlayerVM:NotificationObject
    {
        private Visibility windowscontrol;
        private double windowsheight;
        private Visibility _Sliderthumb;

        public Visibility WindowsControl
        {
            get { return windowscontrol; }
            set { windowscontrol = value; RaisePropertyChanged(() => this.WindowsControl); }
        }
        
        public double WindowsHeight
        {
            get { return windowsheight; }
            set { windowsheight = value; RaisePropertyChanged(() => this.WindowsHeight); }
        }
        
        public Visibility Sliderthumb
        {
            get { return _Sliderthumb; }
            set
            {
                _Sliderthumb = value;
                this.RaisePropertyChanged(() => this.Sliderthumb);
            }
        }

        public string Subtitletext { get; set; }

        public void SetST(string sub)
        {
            Subtitletext = sub;
            this.RaisePropertyChanged(() => this.Subtitletext);
            Sliderthumb = Visibility.Collapsed;
        }
        
        public VideoPlayerVM()
        {
            WindowsControl = Visibility.Visible;
            CollectionViewModel.Instance.CloseFileExporerEvent += new EventHandler(Instance_CloseFileExporerEvent);
        }
       
        private void Instance_CloseFileExporerEvent(object sender, EventArgs e)
        {
            //MainView mv = IShell as MainView;
            //var videoholder = mv._videoContent;
            //if (sender == null)
            //{
            //    MediaControlExtension.SetFileViewVisiblity(IShell.FileView, System.Windows.Visibility.Collapsed);
            //    // IShell.FileView.Visibility= System.Windows.Visibility.Collapsed;
            //    // mv._separator.Visibility = System.Windows.Visibility.Collapsed;
            //    //Grid.SetRowSpan(videoholder, 3);
            //    EndAnimation();
            //}
            //else
            //{
            //    MediaControlExtension.SetFileViewVisiblity(IShell.FileView, System.Windows.Visibility.Visible);
            //    //IShell.FileView.Visibility = System.Windows.Visibility.Visible;
            //    // mv._separator.Visibility = System.Windows.Visibility.Visible;
            //    //Grid.SetRowSpan(videoholder, 1);
            //    StartAnimation();
            //}
        }

        private void EndAnimation()
        {
            //IShell.FileView.BeginAnimation(MediaControlExtension.FileViewVisiblityPropertyProperty, null);
            //DoubleAnimation doubleAnimation = new DoubleAnimation
            //{
            //    //From = ((FrameworkElement)MediaControlExtension.Window.Content).ActualHeight,
            //    To = 0,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(500))
            //};
            //IShell.FileView.BeginAnimation(Grid.HeightProperty, doubleAnimation);
        }

        private void StartAnimation()
        {
            //DoubleAnimation doubleAnimation = new DoubleAnimation
            //{
            //    From = 0,
            //    To = ((FrameworkElement)MediaControlExtension.Window.Content).ActualHeight,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(900))
            //};
            //IShell.FileView.BeginAnimation(Grid.HeightProperty, doubleAnimation);
        }
        
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //IShell.PlayListView.OnPlaylistClose += plv_OnPlaylistClose;
            Instance_CloseFileExporerEvent(this, new EventArgs());
            var commandbings = (IShell as Window).CommandBindings;
            commandbings.Add(new CommandBinding(VideoPlayerCommands.Play, Play_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.AddtoPlayList, AddtoPlayList_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.WMPPlay, WMPPlay_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.AddTo, AddTo_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.NewPlaylist, NewPlaylist_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.RemoveFromLastSeen,
                RemoveFromLS_executed,RemoveFromLS_enabled));

            (IShell as Window).Closing += VideoPlayerVM_Closing;
        }

        private void RemoveFromLS_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            e.CanExecute = vfc.HasLastSeen;
        }

        private void RemoveFromLS_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            IFolder folder = vfc.ParentDirectory;
            vfc.Progress = 0;
            LastSeenHelper.RemoveLastSeen(folder, vfc.LastPlayedPoisition);
            vfc.LastPlayedPoisition = new PlayedFiles(vfc.FileName);
        }

        private void NewPlaylist_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            ((IShell.FileView.TreeViewer as ITreeViewer).MoviesPLaylist as PlaylistTree)
                .CreateNewPlayList(vfc.Directory.FullName);
        }

        private void AddTo_executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedplaylist = e.Parameter as PlaylistModel;
            VideoFolder vf = ((e.Source as IFileViewer).FileExplorer as IFileExplorer)
                .ContextMenuObject as VideoFolder;
            if (selectedplaylist.IsActive)
            {
                IPlayFile.AddFiletoPlayList(vf);
            }
            else
            {
                selectedplaylist.Add(vf.Directory.FullName);
            }
        }

        private void VideoPlayerVM_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CreateHelper.SaveFiles();
        }

        private void AddtoPlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;

            if (vfc != null)
            {
                IPlayFile.AddFiletoPlayList(vfc);
            }
        }

        private void Play_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                IPlayFile.PlayFileInit(vfc);
            }
        }

        private void WMPPlay_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                IPlayFile.WMPPlayFileInit(vfc);
            }
        }
        
        private IShell IShell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>();
            }
        }

        private IPlayFile IPlayFile
        {
            get { return ServiceLocator.Current.GetInstance<IPlayFile>(); }
        }

    }
}
