using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Common.ApplicationCommands
{
    public static class VideoPlayerCommands
    {
        //static VideoPlayerCommands()
        //{
        //    PlayList.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Alt));
        //}


        //private static RoutedUICommand play = new RoutedUICommand();
        //private static RoutedUICommand addtoplaylist = new RoutedUICommand();
        //private static RoutedUICommand addto = new RoutedUICommand();
        //private static RoutedUICommand newplaylist = new RoutedUICommand(); 
        //private static RoutedUICommand removefromplaylist = new RoutedUICommand();

        //private static RoutedUICommand filexplorer = new RoutedUICommand();
        //private static RoutedUICommand playlist = new RoutedUICommand();
        //private static RoutedUICommand wmpplay = new RoutedUICommand();
        //private static RoutedUICommand removefromls = new RoutedUICommand();
        ////private static RoutedUICommand selectedsub = new RoutedUICommand(); 
        //private static RoutedUICommand refreshfiles = new RoutedUICommand();
        //private static RoutedUICommand addsubfile = new RoutedUICommand();

        //public static RoutedUICommand PlayList
        //{
        //    get { return playlist; }
        //}

        //public static RoutedUICommand RefreshFiles
        //{
        //    get { return refreshfiles; }
        //}

        //public static RoutedUICommand AddSubFile
        //{
        //    get { return addsubfile; }
        //}


        //public static RoutedUICommand RemoveFromLastSeen
        //{
        //    get { return removefromls; }
        //}

        //public static RoutedUICommand Play
        //{
        //    get { return play; }
        //}

        ////public static RoutedUICommand SelectedSub
        ////{
        ////    get { return selectedsub; }
        ////}

        //public static RoutedUICommand WMPPlay
        //{
        //    get { return wmpplay; }
        //}

        //public static RoutedUICommand AddtoPlayList
        //{
        //    get { return addtoplaylist; }
        //}

        //public static RoutedUICommand NewPlaylist
        //{
        //    get { return newplaylist; }
        //}

        //public static RoutedUICommand AddTo
        //{
        //    get { return addto; }
        //}
        //public static RoutedUICommand RemovefromPlayList
        //{
        //    get { return removefromplaylist; }
        //}

        //public static RoutedUICommand FileExplorer
        //{
        //    get { return filexplorer; }
        //}

        static VideoPlayerCommands()
        {
            PausePlay.InputGestures.AddRange(new List<InputGesture>() { new KeyGesture(Key.Space), new KeyGesture(Key.MediaPlayPause) });
            Stop.InputGestures.Add(new KeyGesture(Key.MediaStop));
            Next.InputGestures.AddRange(new List<InputGesture>() { new KeyGesture(Key.N, ModifierKeys.Control), new KeyGesture(Key.MediaNextTrack) });
            Previous.InputGestures.AddRange(new List<InputGesture>() { new KeyGesture(Key.P, ModifierKeys.Control), new KeyGesture(Key.MediaPreviousTrack) });
            PlayList.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Alt));
            // FileExplorer.InputGestures.Add(new KeyGesture(Key.F,ModifierKeys.Control));
            VolUp.InputGestures.Add(new KeyGesture(Key.Up, ModifierKeys.Control));
            VolDown.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Control));
            Mute.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            FullScreen.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));

            Rewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Control));
            FastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Control));
            ShiftRewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Shift));
            ShiftFastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Shift));
        }

        private static RoutedUICommand _playorpause = new RoutedUICommand();
        private static RoutedUICommand next = new RoutedUICommand();
        private static RoutedUICommand previous = new RoutedUICommand();
        private static RoutedUICommand playlist = new RoutedUICommand();
        private static RoutedUICommand filexplorer = new RoutedUICommand();
        private static RoutedUICommand stop = new RoutedUICommand();
        private static RoutedUICommand volup = new RoutedUICommand();
        private static RoutedUICommand voldown = new RoutedUICommand();
        private static RoutedUICommand mute = new RoutedUICommand();
        private static RoutedUICommand fullscreen = new RoutedUICommand();
        private static RoutedUICommand minimizemediactrl = new RoutedUICommand();
        private static RoutedUICommand wmpplay = new RoutedUICommand();

        private static RoutedUICommand fforward = new RoutedUICommand();
        private static RoutedUICommand rewind = new RoutedUICommand();
        private static RoutedUICommand shiftforward = new RoutedUICommand();
        private static RoutedUICommand shiftrewind = new RoutedUICommand();

        private static RoutedUICommand play = new RoutedUICommand();
        private static RoutedUICommand addtoplaylist = new RoutedUICommand();
        private static RoutedUICommand addto = new RoutedUICommand();
        private static RoutedUICommand newplaylist = new RoutedUICommand();
        private static RoutedUICommand removefromplaylist = new RoutedUICommand();

        private static RoutedUICommand topmost = new RoutedUICommand();
        private static RoutedUICommand removefromls = new RoutedUICommand();
        //private static RoutedUICommand selectedsub = new RoutedUICommand(); 
        private static RoutedUICommand refreshfiles = new RoutedUICommand();
        private static RoutedUICommand addsubfile = new RoutedUICommand();
        private static RoutedUICommand resizemediaalways = new RoutedUICommand();
        private static RoutedUICommand togglemediaoptions = new RoutedUICommand();

        public static RoutedUICommand ToggleMediaMenu
        {
            get { return togglemediaoptions; }
        }
        public static RoutedUICommand ResizeMediaAlways
        {
            get { return resizemediaalways; }
        }
        public static RoutedUICommand RefreshFiles
        {
            get { return refreshfiles; }
        }

        public static RoutedUICommand AddSubFile
        {
            get { return addsubfile; }
        }

        public static RoutedUICommand PausePlay
        {
            get { return _playorpause; }
        }

        public static RoutedUICommand RemoveFromLastSeen
        {
            get { return removefromls; }
        }

        public static RoutedUICommand TopMost
        {
            get { return topmost; }
        }

        public static RoutedUICommand Play
        {
            get { return play; }
        }

        //public static RoutedUICommand SelectedSub
        //{
        //    get { return selectedsub; }
        //}

        public static RoutedUICommand Rewind
        {
            get { return rewind; }
        }
        public static RoutedUICommand ShiftRewind
        {
            get { return shiftrewind; }
        }
        public static RoutedUICommand WMPPlay
        {
            get { return wmpplay; }
        }

        public static RoutedUICommand FastForward
        {
            get { return fforward; }
        }
        public static RoutedUICommand ShiftFastForward
        {
            get { return shiftforward; }
        }
        public static RoutedUICommand MinimizeMediaCtrl
        {
            get { return minimizemediactrl; }
        }

        public static RoutedUICommand FullScreen
        {
            get { return fullscreen; }
        }

        public static RoutedUICommand AddtoPlayList
        {
            get { return addtoplaylist; }
        }

        public static RoutedUICommand NewPlaylist
        {
            get { return newplaylist; }
        }

        public static RoutedUICommand AddTo
        {
            get { return addto; }
        }
        public static RoutedUICommand RemovefromPlayList
        {
            get { return removefromplaylist; }
        }

        public static RoutedUICommand Stop
        {
            get { return stop; }
        }
        public static RoutedUICommand Next
        {
            get { return next; }
        }
        public static RoutedUICommand Previous
        {
            get { return previous; }
        }
        public static RoutedUICommand PlayList
        {
            get { return playlist; }
        }
        public static RoutedUICommand FileExplorer
        {
            get { return filexplorer; }
        }
        public static RoutedUICommand VolUp
        {
            get { return volup; }
        }
        public static RoutedUICommand VolDown
        {
            get { return voldown; }
        }
        public static RoutedUICommand Mute
        {
            get { return mute; }
        }

    }
}
