using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using MovieHub.MediaPlayerElement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Service
{
    public partial class MediaPlayerService
    {
        private void HookUpEvents()
        {
            _vlcPlayer.VlcMediaPlayer.Buffering += VlcMediaPlayer_Buffering;
            _vlcPlayer.VlcMediaPlayer.EncounteredError += VlcMediaPlayer_EncounteredError;
            _vlcPlayer.VlcMediaPlayer.EndReached += VlcMediaPlayer_EndReached;
            _vlcPlayer.VlcMediaPlayer.LengthChanged += VlcMediaPlayer_LengthChanged;
            _vlcPlayer.VlcMediaPlayer.MediaChanged += VlcMediaPlayer_MediaChanged;
            _vlcPlayer.VlcMediaPlayer.Opening += VlcMediaPlayer_Opening;
            _vlcPlayer.VlcMediaPlayer.Paused += VlcMediaPlayer_Paused;
            _vlcPlayer.VlcMediaPlayer.Playing += VlcMediaPlayer_Playing;
            _vlcPlayer.VlcMediaPlayer.Stoped += VlcMediaPlayer_Stoped;
            _vlcPlayer.VlcMediaPlayer.SubItemChanged += VlcMediaPlayer_SubItemChanged;
            _vlcPlayer.TimeChanged += VlcMediaPlayer_TimeChanged;
            _vlcPlayer.MediaOpened += _vlcPlayer_MediaOpened;
            _vlcPlayer.StateChanged += _vlcPlayer_StateChanged;
        }

        private void _vlcPlayer_StateChanged(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            state = GetMediaState();

            RaiseEvent(new RoutedEventArgs(OnStateChangedEvent, this));
        }

        private void _vlcPlayer_MediaOpened(object sender, EventArgs e)
        {
            InitializeComponent();
            RaiseEvent(new RoutedEventArgs(OnMediaOpenedEvent, this));
            
        }
        
        void VlcMediaPlayer_TimeChanged(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnTimeChangedEvent, this));
        }

        void VlcMediaPlayer_SubItemChanged(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Core.Events.MediaSubitemAddedArgs> e)
        {
            RaiseEvent(new RoutedEventArgs(SubItemChangedEvent, this));
        }

        void VlcMediaPlayer_Stoped(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            Dispatcher.Invoke((Action)(() => RaiseEvent(new RoutedEventArgs(OnMediaStoppedEvent, this))));
            state = MovieMediaState.Stopped;
        }

        void VlcMediaPlayer_Playing(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            Dispatcher.Invoke((Action)(() => RaiseEvent(new RoutedEventArgs(OnMediaPlayingEvent, this))));
        }

        void VlcMediaPlayer_Paused(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            Dispatcher.BeginInvoke((Action)(() => RaiseEvent(new RoutedEventArgs(OnMediaPausedEvent, this))));
        }

        void VlcMediaPlayer_Opening(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            Dispatcher.Invoke((Action)(()=>RaiseEvent(new RoutedEventArgs(OnMediaOpeningEvent, this))));
        }

        void VlcMediaPlayer_MediaChanged(object sender, Meta.Vlc.MediaPlayerMediaChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnMediaChangedEvent, this));
        }

        void VlcMediaPlayer_LengthChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action)(() => RaiseEvent(new RoutedEventArgs(OnDurationChangedEvent, this))));
        }

        void VlcMediaPlayer_EndReached(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            Dispatcher.BeginInvoke((Action)(() => 
            {
                RaiseEvent(new RoutedEventArgs(EndReachedEvent, this));
                state = MovieMediaState.Ended;
            }));
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => 
            {
                RaiseEvent(new RoutedEventArgs(EncounteredErrorEvent, this));
                state = MovieMediaState.Error;
            }));
        }

        private void VlcMediaPlayer_Buffering(object sender, Meta.Vlc.MediaPlayerBufferingEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => RaiseEvent(new MediaBufferingEventArgs(BufferingEvent, this, e.NewCache))));
        }

        internal void PublishSubItemAddedEvent()
        {
            RaiseEvent(new RoutedEventArgs(OnSubItemAddedEvent, this));
        }

    }
}
