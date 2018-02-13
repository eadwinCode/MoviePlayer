using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Common.ApplicationCommands
{
    public static class VideoPlayerCommands
    {
        static VideoPlayerCommands()
        {
            PausePlay.InputGestures.AddRange( new List<InputGesture>() { new KeyGesture(Key.Space), new KeyGesture(Key.MediaPlayPause) });
            Stop.InputGestures.Add( new KeyGesture(Key.MediaStop) );
            Next.InputGestures.AddRange(new List<InputGesture>() { new KeyGesture(Key.N, ModifierKeys.Control), new KeyGesture(Key.MediaNextTrack) });
            Previous.InputGestures.AddRange(new List<InputGesture>() { new KeyGesture(Key.P, ModifierKeys.Control), new KeyGesture(Key.MediaPreviousTrack) });
            PlayList.InputGestures.Add(new KeyGesture(Key.P,ModifierKeys.Alt));
           // FileExplorer.InputGestures.Add(new KeyGesture(Key.F,ModifierKeys.Control));
            VolUp.InputGestures.Add(new KeyGesture(Key.Up,ModifierKeys.Control));
            VolDown.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Control));
            Mute.InputGestures.Add(new KeyGesture(Key.M,ModifierKeys.Control));
            FullScreen.InputGestures.Add(new KeyGesture(Key.F,ModifierKeys.Control));

            Rewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Control));
            FastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Control));
            ShiftRewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Shift));
            ShiftFastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Shift));
        }

        private static RoutedCommand _playorpause = new RoutedCommand("PausePlay", typeof(VideoPlayerCommands));
        private static RoutedCommand next = new RoutedCommand("Next", typeof(VideoPlayerCommands));
        private static RoutedCommand previous = new RoutedCommand("Previous", typeof(VideoPlayerCommands));
        private static RoutedCommand playlist = new RoutedCommand("PlayList", typeof(VideoPlayerCommands));
        private static RoutedCommand filexplorer = new RoutedCommand("FileExplorer", typeof(VideoPlayerCommands));
        private static RoutedCommand stop = new RoutedCommand("Stop", typeof(VideoPlayerCommands));
        private static RoutedCommand volup = new RoutedCommand("VolUp", typeof(VideoPlayerCommands));
        private static RoutedCommand voldown = new RoutedCommand("VolDown", typeof(VideoPlayerCommands));
        private static RoutedCommand mute = new RoutedCommand("Mute", typeof(VideoPlayerCommands));
        private static RoutedCommand fullscreen = new RoutedCommand("FullScreen", typeof(VideoPlayerCommands));
        private static RoutedCommand minimizemediactrl = new RoutedCommand("MinimizeMC", typeof(VideoPlayerCommands));
        private static RoutedCommand wmpplay = new RoutedCommand("WMPPlay", typeof(VideoPlayerCommands));

        private static RoutedCommand fforward = new RoutedCommand("FastForward", typeof(VideoPlayerCommands));
        private static RoutedCommand rewind = new RoutedCommand("Rewind", typeof(VideoPlayerCommands));
        private static RoutedCommand shiftforward = new RoutedCommand("ShitFastForward", typeof(VideoPlayerCommands));
        private static RoutedCommand shiftrewind = new RoutedCommand("ShitRewind", typeof(VideoPlayerCommands));

        private static RoutedCommand play = new RoutedCommand("Play", typeof(VideoPlayerCommands));
        private static RoutedCommand addtoplaylist = new RoutedCommand("AddtoPL", typeof(VideoPlayerCommands));
        private static RoutedCommand addto = new RoutedCommand("AddTo", typeof(VideoPlayerCommands));
        private static RoutedCommand newplaylist = new RoutedCommand("NewPlaylist", typeof(VideoPlayerCommands)); 
        private static RoutedCommand removefromplaylist = new RoutedCommand("RemovefrmPL", typeof(VideoPlayerCommands));

        private static RoutedCommand disablesubtitle = new RoutedCommand("DisableSubtitle", typeof(VideoPlayerCommands));
        private static RoutedCommand topmost = new RoutedCommand("TopMost", typeof(VideoPlayerCommands));
        private static RoutedCommand removefromls = new RoutedCommand("RemoveFromLastSeen", typeof(VideoPlayerCommands));
        private static RoutedCommand selectedsub = new RoutedCommand("SelectedSub", typeof(VideoPlayerCommands));
        private static RoutedCommand refreshfiles = new RoutedCommand("RefreshFiles", typeof(VideoPlayerCommands));

        public static RoutedCommand RefreshFiles
        {
            get { return refreshfiles; }
        }

        public static RoutedCommand PausePlay
        {
            get { return _playorpause; }
        }

        public static RoutedCommand RemoveFromLastSeen
        {
            get { return removefromls; }
        }

        public static RoutedCommand TopMost
        {
            get { return topmost; }
        }

        public static RoutedCommand Play
        {
            get { return play; }
        }

        public static RoutedCommand DisableSubtitle
        {
            get { return disablesubtitle; }
        }

        public static RoutedCommand SelectedSub
        {
            get { return selectedsub; }
        }

        public static RoutedCommand Rewind
        {
            get { return rewind; }
        }
        public static RoutedCommand ShiftRewind
        {
            get { return shiftrewind; }
        }
        public static RoutedCommand WMPPlay
        {
            get { return wmpplay; }
        }

        public static RoutedCommand FastForward
        {
            get { return fforward; }
        }
        public static RoutedCommand ShiftFastForward
        {
            get { return shiftforward; }
        }
        public static RoutedCommand MinimizeMediaCtrl
        {
            get { return minimizemediactrl; }
        }

        public static RoutedCommand FullScreen
        {
            get { return fullscreen; }
        }
        
        public static RoutedCommand AddtoPlayList
        {
            get { return addtoplaylist; }
        }

        public static RoutedCommand NewPlaylist
        {
            get { return newplaylist; }
        }

        public static RoutedCommand AddTo
        {
            get { return addto; }
        }
        public static RoutedCommand RemovefromPlayList
        {
            get { return removefromplaylist; }
        }

        public static RoutedCommand Stop
        {
            get { return stop; }
        }
        public static RoutedCommand Next
        {
            get { return next; }
        }
        public static RoutedCommand Previous
        {
            get { return previous; }
        }
        public static RoutedCommand PlayList
        {
            get { return playlist; }
        }
        public static RoutedCommand FileExplorer
        {
            get { return filexplorer; }
        }
        public static RoutedCommand VolUp
        {
            get { return volup; }
        }
        public static RoutedCommand VolDown
        {
            get { return voldown; }
        }
        public static RoutedCommand Mute
        {
            get { return mute; }
        }
    }
}
