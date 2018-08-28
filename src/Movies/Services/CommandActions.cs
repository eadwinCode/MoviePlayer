using Common.ApplicationCommands;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Movies.Services
{
    public class CommandActions
    {
        IApplicationService ApplicationService;
        private bool IsPlaylistManagerBusy = false;

        public CommandActions(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            IEventManager.GetEvent<IsPlaylistManagerBusy>().Subscribe((value) =>
            {
                IsPlaylistManagerBusy = value;
            });
        }

        public void RegisterCommands()
        {
            var commandbings = IShell.CommandBindings;
            commandbings.Add(new CommandBinding(VideoPlayerCommands.Play, Play_executed, CanExecute));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.AddtoPlayList, AddtoPlayList_executed,CanExecute));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.WMPPlay, WMPPlay_executed, CanExecute));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.AddTo, AddTo_executed, CanExecute));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.NewPlaylist, NewPlaylist_executed, CanExecute));
            commandbings.Add(new CommandBinding(VideoPlayerCommands.RemoveFromLastSeen,
                RemoveFromLS_executed, RemoveFromLS_enabled));
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsPlaylistManagerBusy;
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
            if (e.Parameter is VideoFolderChild)
            {
                HomePlaylistService.CreateNewPlayList(vfc.FullName);
            }
            else
            {
                List<string> listpath = GetListPath(vfc);
                if (listpath.Count > 0)
                {
                    PlaylistModel playlistModel = new PlaylistModel();
                    foreach (var item in listpath)
                    {
                        playlistModel.Add(new Pathlist(item));
                    }
                    HomePlaylistService.CreateNewPlayList(playlistModel);
                }
            }
        }

        private List<string> GetListPath(VideoFolder vfc)
        {
            var listpath = new List<string>();
            var padlock = new object();
            Parallel.ForEach(vfc.OtherFiles.Where(s => s is VideoFolderChild), (s) => {
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

        private IHomePlaylist HomePlaylistService
        {
            get { return ServiceLocator.Current.GetInstance<IHomePlaylist>(); }
        }

        IEventManager IEventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }
    }
}
