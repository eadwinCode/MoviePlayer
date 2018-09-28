using Common.ApplicationCommands;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace VideoPlayerControl
{
    public partial class MediaPlayerControlService
    {
        private void RegisterCommands()
        {
           
            WindowsBindingCollection.Add(new CommandBinding(MovieControl.PausePlay,
                PausePlay_executed, PausePlay_enabled));
            

            WindowsBindingCollection.Add(new CommandBinding(MovieControl.ToggleMediaMenu,
              ToggleMediaMenu_executed, ToggleMediaMenu_enabled));
        }

        private void ToggleMediaMenu_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !MediaPlayerService.HasStopped;
        }

        private void ToggleMediaMenu_executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaMenuViewModel.GetMediaMenuView().ShowDialog();
        }

       

        private void ShiftFastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer += TimeSpan.FromMilliseconds(1500);
            TimeChangeAction();
        }

        private void ShiftRewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer -= TimeSpan.FromMilliseconds(1500);
            TimeChangeAction();
        }
        
      
        private void FastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer += TimeSpan.FromMilliseconds(10000);
            TimeChangeAction();
            if (e.OriginalSource is Button)
            {
               // RestoreMediaState();
            }
        }

        private void Rewind_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!MediaPlayerService.HasVideo)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = MediaPlayerService.IsSeekable;
        }

        private void Rewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReWindFastForward();
            MediaPlayerService.CurrentTimer -= TimeSpan.FromMilliseconds(10000);
            TimeChangeAction();
            if (e.OriginalSource is Button)
            {
               // RestoreMediaState();
            }
        }
        
        private void CommandEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MediaPlayerService.Duration != TimeSpan.Zero;
        }

       

        private void PausePlay_executed(object sender, ExecutedRoutedEventArgs e)
        {
            //if (MediaPlayerService.State == MovieMediaState.Playing)
            //    ResetVisibilityAnimation();
            //else
            //    VisibilityAnimation();
            //PlayAction();
        }

        private void PausePlay_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentStreamFilePath != null || !MediaPlayerService.HasLoadedMedia;
        }

        private void Mute_executed(object sender, ExecutedRoutedEventArgs e)
        {
            //ResetVisibilityAnimation();
            //MuteAction();
        }

        private void ReWindFastForward()
        {
            //if (!IsRewindOrFastForward)
            //{
            //    IsRewindOrFastForward = true;

            //    if (VolumeState == VolumeState.Active)
            //    {
            //        MediaPlayerService.IsMute = true;
            //    }

            //    //ResetVisibilityAnimation();
            //}
        }
        private void PauseOrResumeHandler()
        {

        }
       
        private void MediaMenuHandler()
        {

        }
        

        public void StopHandler()
        {

        }

        private void RewindHandler()
        {

        }

        private void ShiftRewindHandler()
        {

        }

        private void FastFaorwardHandler()
        {

        }

        private void ShiftFastForwardHandler()
        {

        }

        private void PauseOrResumeAction()
        {
        }



        public void SetSubtitle(string filepath)
        {
            MediaPlayerService.SubtitleManagement.SetSubtitle(filepath);
        }
        
        public bool IsfetchingRepeatItemAsync { get; private set; }
        
    }
}
