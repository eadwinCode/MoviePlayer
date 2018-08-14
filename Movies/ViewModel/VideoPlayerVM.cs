using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VideoComponent.BaseClass;
using VideoPlayerControl;
using VideoPlayerControl.ViewModel;
using VirtualizingListView;
using VirtualizingListView.Pages;
using VirtualizingListView.Pages.Views;
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
            
        }
        
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var commandbings = (IShell as Window).CommandBindings;
            commandbings.Add(new CommandBinding(VideoPlayerCommands.Play, Play_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.AddtoPlayList, AddtoPlayList_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.WMPPlay, WMPPlay_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.AddTo, AddTo_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.NewPlaylist, NewPlaylist_executed));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.RefreshFiles, 
                RefreshFiles_executed, RefreshFiles_Enabled));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.RemoveFromLastSeen,
                RemoveFromLS_executed,RemoveFromLS_enabled));

            (IShell as Window).Closing += VideoPlayerVM_Closing;
        }
        
        private void RefreshFiles_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CollectionViewModel.Instance.CanRefresh();
        }

        private void RefreshFiles_executed(object sender, ExecutedRoutedEventArgs e)
        {
            CollectionViewModel.Instance.Refresh_Action();
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
            ApplicationService.SavedLastSeenCollection.Remove((PlayedFiles)vfc.LastPlayedPoisition);
            vfc.LastPlayedPoisition = new PlayedFiles(vfc.FileName);
        }

        private void NewPlaylist_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolder vfc = (VideoFolder)e.Parameter;
            if (e.Parameter is VideoFolderChild){
                HomePageLocal.PlaylistControl.CreateNewPlayList(vfc.FullName);
            }
            else
            {
                if (vfc.OtherFiles.FirstOrDefault(x => x is VideoFolderChild) != null) {
                    List<string> listpath = GetListPath(vfc);
                    PlaylistModel playlistModel = new PlaylistModel();
                    playlistModel.ItemsPaths.AddRange(listpath);
                    HomePageLocal.PlaylistControl.CreateNewPlayList(playlistModel);
                }
            }
        }

        private List<string> GetListPath(VideoFolder vfc)
        {
            var listpath = new List<string>();
            var padlock = new object();
            Parallel.ForEach(vfc.OtherFiles.Where(s=>s is VideoFolderChild), (s) => {
                lock (padlock)
                {
                    listpath.Add(s.FullName);
                }
            });

            return listpath;
        }

        private void AddTo_executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedplaylist = e.Parameter as PlaylistModel;
            VideoFolder vf = ((e.OriginalSource as Button).DataContext as VideoFolder);
            
            if (selectedplaylist.IsActive && vf != null)
            {
                IPlayFile.AddFiletoPlayList(vf);
            }
            else
            {
                selectedplaylist.Add(vf.FullName);
            }
        }

        private void VideoPlayerVM_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationService.SaveFiles();
        }

        private void AddtoPlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is VideoFolderChild)
            {
                IVideoData vfc = (VideoFolderChild)e.Parameter;
                IPlayFile.AddFiletoPlayList(vfc);
                return;
            }

            IFolder videofolder = e.Parameter as VideoFolder;
            IPlayFile.AddFiletoPlayList(videofolder);
        }

        private void Play_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is VideoFolderChild)
            {
                IVideoData vfc = (VideoFolderChild)e.Parameter;
                IPlayFile.PlayFileInit(vfc);
                return;
            }

            IFolder videofolder = e.Parameter as VideoFolder;
            IPlayFile.PlayFileInit(videofolder);
        }

        private void WMPPlay_executed(object sender, ExecutedRoutedEventArgs e)
        {
            IVideoData vfc = (VideoFolderChild)e.Parameter;
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
