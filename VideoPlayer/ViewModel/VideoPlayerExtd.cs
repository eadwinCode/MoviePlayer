using Common.ApplicationCommands;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using VideoComponent.BaseClass;
using VirtualizingListView.ViewModel;

namespace VideoPlayer.ViewModel
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
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Play,
                Play_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.TopMost,
                TopMost_executed));
           

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FullScreen, 
                FullScreen_executed, FullScreen_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Rewind, 
                Rewind_executed, Rewind_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ShiftRewind,
                ShiftRewind_executed, Rewind_enabled));

            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.DisableSubtitle,
                DisableSubtitle_executed, DisableSubtitle_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.SelectedSub, 
                SelectedSub_executed));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.FastForward, 
                FastForward_executed, Rewind_enabled));
            IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.ShiftFastForward,
               ShiftFastForward_executed, Rewind_enabled));
            // IVideoElement.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.MinimizeMediaCtrl, MinimizeMediaCtrl_executed));
        }

        private void ShiftFastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Position += TimeSpan.FromMilliseconds(1500);
        }

        private void ShiftRewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Position -= TimeSpan.FromMilliseconds(1500);
        }

        private void DisableSubtitle_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MenuItem menuItem = e.Parameter as MenuItem;
            var iSubtitle = (this.ISubtitleMediaController.Subtitle as ISubtitle);
            menuItem.IsChecked = iSubtitle.IsDisabled? true:false;
            e.CanExecute = iSubtitle.HasSub || iSubtitle.IsDisabled;
        }

        public void RestoreMediaState()
        {
            IVideoElement.MediaPlayer.ScrubbingEnabled = false;
            if (MediaControllerVM.Current.MediaState == MediaState.Playing)
                IVideoElement.MediaPlayer.Play();

            MediaControllerVM.Current.IsRewindOrFastForward = false;
            if (MediaControllerVM.Current.VolumeState
                == VolumeState.Active)
            {
                IVideoElement.MediaPlayer.IsMuted = false;
            }
        }

        private void SelectedSub_executed(object sender, ExecutedRoutedEventArgs e)
        {
            var currentvideoItem = MediaControllerVM.CurrentVideoItem;
            var subtitlecollection = currentvideoItem.SubPath;
            var SelectedSubtitle = e.Parameter as SubtitleFilesModel;
            var firstItem = subtitlecollection.First();
            if (subtitlecollection.Count == 1){
                SelectedSubtitle.IsSelected = true;
                return;
            }
            else
            {
                foreach (var item in subtitlecollection)
                {
                    if (item == SelectedSubtitle)
                    {
                        continue;
                    }
                    item.IsSelected = false;
                }
            }
            ISubtitleMediaController.Subtitle.LoadSub(SelectedSubtitle);
        }

        private void DisableSubtitle_executed(object sender, ExecutedRoutedEventArgs e)
        {
            var iSubtitle = (this.ISubtitleMediaController.Subtitle as ISubtitle);

            if (iSubtitle.HasSub || iSubtitle.IsDisabled)
            {
                MenuItem menuItem = e.Parameter as MenuItem;
                if (iSubtitle.IsDisabled)
                {
                    iSubtitle.IsDisabled = false;
                }
                else
                {
                    iSubtitle.IsDisabled = true;
                }
            }
        }

        private void TopMost_executed(object sender, ExecutedRoutedEventArgs e)
        {
            IVideoElement.SetTopMost();
        }

        private void FastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Position += TimeSpan.FromMilliseconds(10000);
            if (e.OriginalSource is Button)
            {
                RestoreMediaState();
            }
        }

        private void Rewind_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IVideoElement.MediaPlayer.CanPause;
        }

        private void Rewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            IVideoElement.MediaPlayer.Position -= TimeSpan.FromMilliseconds(10000);
            if (e.OriginalSource is Button)
            {
                RestoreMediaState();
            }
            //IVideoPlayer.MediaPlayer.Play();
        }

        private void ReWindFastForward()
        {
            if (!MediaControllerVM.IsRewindOrFastForward)
            {
                MediaControllerVM.IsRewindOrFastForward = true;
                if (MediaControllerVM.MediaState == MediaState.Playing)
                    IVideoElement.MediaPlayer.Pause();

                IVideoElement.MediaPlayer.ScrubbingEnabled = true;
                if (MediaControllerVM.VolumeState == VolumeState.Active){
                    IVideoElement.MediaPlayer.IsMuted = true;
                }

                ResetVisibilityAnimation();
            }
        }

        public void Loaded()
        {
            Isloaded = true;
            IVideoElement.WindowsTab.MouseEnter += WindowsTab_MouseEnter;
            IVideoElement.WindowsTab.MouseLeave += WindowsTab_MouseLeave;
        }

        private void FullScreen_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IVideoElement.MediaPlayer.HasVideo && !IsFullScreenMode;
        }

        private void MinimizeMediaCtrl_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (screensetting == SCREENSETTINGS.Normal)
            {
                FullScreenSettings();
               IsFullScreenMode = true;
                (ISubtitleMediaController as SubtitleMediaController).
                    OnScreenSettingsCanged(new object[] { null });
            }
            else
            {
                IsFullScreenMode = false;
                NormalScreenSettings();
                (ISubtitleMediaController as SubtitleMediaController).
                    OnScreenSettingsCanged(new object[] { null });
            }
            
        }

        public void FullScreenAction()
        {
            if (!IsFullScreenMode)
            {
                ((SubtitleMediaController)ISubtitleMediaController).OnScreenSettingsCanged(
                                new object[] { SCREENSETTINGS.Fullscreen, SCREENSETTINGS.Fullscreen });
                IsFullScreenMode = true;
                (((SubtitleMediaController)ISubtitleMediaController).DataContext as VideoPlayerVM).FullScreenSettings();
            }
        }

        private void FullScreen_executed(object sender, ExecutedRoutedEventArgs e)
        {
            FullScreenAction();
        }

        private void PausePlay_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IVideoElement.MediaPlayer.HasVideo;
        }

        private void RemovefromPlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                MediaControllerVM.Current.Playlist.Remove(vfc);
            }
        }
        
        private void Vol_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
           e.CanExecute = !IVideoElement.MediaPlayer.IsMuted;
        }

        private void Play_executed(object sender, ExecutedRoutedEventArgs e)
        {
            VideoFolderChild vfc = (VideoFolderChild)e.Parameter;
            if (vfc != null)
            {
                MediaControllerVM.Current.GetVideoItem(vfc, true);
            }
        }

        private void Next_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaControllerVM.CanNext();
        }

        private void Previous_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaControllerVM.CanPrev();
        }

        private void CommandEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaControllerVM.IVideoElement.MediaPlayer.HasVideo;
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

        private void Stop_executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Previous_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            if (MediaControllerVM.Current.DragPositionSlider.Value > 50)
                IVideoElement.MediaPlayer.Position = TimeSpan.FromMilliseconds(0);
            else
                MediaControllerVM.Current.PrevPlayAction();
        }

        private void PausePlay_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            MediaControllerVM.Current.PlayAction();
        }

        private void Next_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            MediaControllerVM.Current.NextPlayAction();
        }

        private void Mute_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
            MediaControllerVM.Current.MuteAction();
        }

        private void PlayList_executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistView plv = IVideoElement.PlayListView as PlaylistView;
            plv.OnPlaylistCloseExecute(this);
        }

        private void FileExplorer_executed(object sender, ExecutedRoutedEventArgs e)
        {
            CollectionViewModel.Instance.CloseFileExplorerAction(this);
        }

        private IVideoElement icommandbindings;
        private IVideoElement IVideoElement
        {
            get
            {
                if (icommandbindings == null )
                {
                    icommandbindings = ServiceLocator.Current.GetInstance<IPlayFile>().VideoElement;
                }
                return  icommandbindings;
            }
        }
    }
}
