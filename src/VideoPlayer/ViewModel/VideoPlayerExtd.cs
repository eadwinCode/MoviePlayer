using Common.ApplicationCommands;
using Common.Util;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.Enums;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Movies.MoviesInterfaces;
using VideoPlayerControl.View;

namespace VideoPlayerControl.ViewModel
{
    public partial class VideoPlayerVM
    {
        private void RegisterCommands()
        {
           // ICommandBindings.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FileExplorer, FileExplorer_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.PlayList,
                PlayList_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Mute, 
                Mute_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Next, 
                Next_executed,Next_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.PausePlay,
                PausePlay_executed,PausePlay_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Previous,
                Previous_executed, Previous_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Stop, 
                Stop_executed,CommandEnabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.VolDown,
                VolDown_executed, Vol_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.VolUp, 
                VolUp_executed,Vol_enabled));
            // IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.AddtoPlayList, AddtoPlayList_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.RemovefromPlayList,
                RemovefromPlayList_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Play ,Play_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.TopMost,
                TopMost_executed));


            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FullScreen,
                FullScreen_executed));//, FullScreen_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Rewind, 
                Rewind_executed, Rewind_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ShiftRewind,
                ShiftRewind_executed, Rewind_enabled));

            //IMediaController.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.SelectedSub, 
            //    SelectedSub_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FastForward, 
                FastForward_executed, Rewind_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ShiftFastForward,
               ShiftFastForward_executed, Rewind_enabled));

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.AddSubFile, BrowerSubFile));

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ResizeMediaAlways,
              ResizeMediaAlways_executed));

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ToggleMediaMenu,
              ToggleMediaMenu_executed,ToggleMediaMenu_enabled));
            // IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.MinimizeMediaCtrl, MinimizeMediaCtrl_executed));
        }

        private void ToggleMediaMenu_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !FilePlayerManager.MediaPlayerService.HasStopped;
        }

        private void ToggleMediaMenu_executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaMenuViewModel.GetMediaMenuView().ShowDialog();
        }

        private void ResizeMediaAlways_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.AllowAutoResize)
            {
                this.AllowAutoResize = false;
            }
            else
            {
                this.AllowAutoResize = true;
            }
        }

        private void BrowerSubFile(object sender, ExecutedRoutedEventArgs e)
        {
            var openfiles = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All files(*.*)|*.srt;"
            };
            if (openfiles.ShowDialog() == true)
            {
                AddSubtitleFileAction(new string[] { openfiles.FileName });
            }
        }

        private void ShiftFastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer += TimeSpan.FromMilliseconds(1500);
            MediaControllerViewModel.TimeChangeAction();
        }

        private void ShiftRewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer -= TimeSpan.FromMilliseconds(1500);
            MediaControllerViewModel.TimeChangeAction();
        }

        private void DisableSubtitle_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = MediaPlayerService.SubtitleCount != 0;
        }
             
        private void TopMost_executed(object sender, ExecutedRoutedEventArgs e)
        {
            IVideoElement.SetTopMost();
        }

        private void FastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer += TimeSpan.FromMilliseconds(10000);
            MediaControllerViewModel.TimeChangeAction();
            if (e.OriginalSource is Button)
            {
                RestoreMediaState();
            }
        }

        private void Rewind_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!MediaPlayerService.HasVideo)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute =  MediaPlayerService.IsSeekable;
        }

        private void Rewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer -= TimeSpan.FromMilliseconds(10000);
            MediaControllerViewModel.TimeChangeAction();
            if (e.OriginalSource is Button)
            {
                RestoreMediaState();
            }
            //IVideoPlayer.MediaPlayer.Play();
        }
                        
        private void MinimizeMediaCtrl_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (screensetting == SCREENSETTINGS.Normal)
            {
                FullScreenSettings();
               IsFullScreenMode = true;
                (IMediaController as SubtitleMediaController).
                    OnScreenSettingsChanged(new object[] { null });
            }
            else
            {
                IsFullScreenMode = false;
                NormalScreenSettings();
                (IMediaController as SubtitleMediaController).
                    OnScreenSettingsChanged(new object[] { null });
            }
            
        }
               
        private void FullScreen_executed(object sender, ExecutedRoutedEventArgs e)
        {
            FullScreenAction();
        }

        private void FullScreen_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void RemovefromPlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                FilePlayerManager.PlaylistManagerViewModel.Remove(vfc);
            }
        }
        
        private void Play_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
               FilePlayerManager.MediaControllerViewModel.GetVideoItem(vfc, true);
            }
        }
        
        private void CommandEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaPlayerService.Duration != TimeSpan.Zero;
        }

        private void VolUp_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            VolumeControl.CurrentVolumeSlider.Value += 10;
        }

        private void VolDown_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            VolumeControl.CurrentVolumeSlider.Value -= 10;
        }

        private void Vol_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !MediaPlayerService.IsMute;
        }
        
        private void Stop_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerService.Stop();
            ResetVisibilityAnimation();
        }

        private void Previous_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            if (FilePlayerManager.MediaControllerViewModel.DragPositionSlider.Value > 50)
                MediaPlayerService.CurrentTimer = TimeSpan.FromMilliseconds(0);
            else
               FilePlayerManager.MediaControllerViewModel.PrevPlayAction();
        }

        private void Previous_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (FilePlayerManager.MediaControllerViewModel.IsfetchingRepeatItemAsync || !FilePlayerManager.MediaPlayerService.HasLoadedMedia)
                e.CanExecute = false;

            e.CanExecute = MediaControllerViewModel.CanPrev();
        }
        
        private void PausePlay_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MediaPlayerService.State == MovieMediaState.Playing)
                ResetVisibilityAnimation();
            else
                VisibilityAnimation();
            FilePlayerManager.MediaControllerViewModel.PlayAction();
           
        }

        private void PausePlay_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =FilePlayerManager.MediaControllerViewModel.CurrentVideoItem != null || !FilePlayerManager.MediaPlayerService.HasLoadedMedia;
        }
        
        private void Next_executed(object sender, ExecutedRoutedEventArgs e)
        {
           ResetVisibilityAnimation();
           FilePlayerManager.MediaControllerViewModel.NextPlayAction();
        }

        private void Next_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (FilePlayerManager.MediaControllerViewModel.IsfetchingRepeatItemAsync || !FilePlayerManager.MediaPlayerService.HasLoadedMedia)
                e.CanExecute = false;
            else
                e.CanExecute = MediaControllerViewModel.CanNext();
        }
        
        private void Mute_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
           FilePlayerManager.MediaControllerViewModel.MuteAction();
        }

        private void PlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            IPlaylistViewMediaPlayerView plv = IVideoElement.PlayListView;
            plv.OnPlaylistCloseExecute(this);
        }

        private void FileExplorer_executed(object sender, ExecutedRoutedEventArgs e)
        {
           // CollectionViewModel.Instance.CloseFileExplorerAction(this);
        }
          
        private void ReWindFastForward()
        {
            if (!MediaControllerViewModel.IsRewindOrFastForward)
            {
                MediaControllerViewModel.IsRewindOrFastForward = true;
                //if (MediaControllerViewModel.MediaState == MovieMediaState.Playing)
                //    MediaPlayerService.Pause();

                if (MediaControllerViewModel.VolumeState == VolumeState.Active)
                {
                    MediaPlayerService.IsMute = true;
                }

                ResetVisibilityAnimation();
            }
        }

        public void RestoreMediaState()
        {
            //IVideoElement.MediaPlayer.ScrubbingEnabled = false;
            //if (FilePlayerManager.MediaControllerViewModel.MediaState == MovieMediaState.Playing)
            //    MediaPlayerService.Play();

           FilePlayerManager.MediaControllerViewModel.IsRewindOrFastForward = false;
            if (FilePlayerManager.MediaControllerViewModel.VolumeState
                == VolumeState.Active)
            {
                MediaPlayerService.IsMute = false;
            }
        }

        public void FullScreenAction()
        {
            if (!IsFullScreenMode)
            {
                ((SubtitleMediaController)IMediaController).OnScreenSettingsChanged(
                                new object[] { SCREENSETTINGS.Fullscreen, SCREENSETTINGS.Fullscreen });
                IsFullScreenMode = true;
                (((SubtitleMediaController)IMediaController).DataContext as VideoPlayerVM).FullScreenSettings();
            }
            else
                RestoreScreen();
        }

        public void Loaded()
        {
            Isloaded = true;
            //IVideoElement.WindowsTab.MouseEnter += WindowsTab_MouseEnter;
            //IVideoElement.WindowsTab.MouseLeave += WindowsTab_MouseLeave;
        }

    }
}
