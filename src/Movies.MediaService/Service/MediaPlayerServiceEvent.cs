using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MediaService.Service
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
            _vlcPlayer.PreviewMouseMove += _vlcPlayer_PreviewMouseMove;
            _vlcPlayer.StateChanged += _vlcPlayer_StateChanged;
        }

        private void _vlcPlayer_StateChanged(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            if (OnStateChanged != null)
                OnStateChanged.Invoke(this, EventArgs.Empty);
        }

        private void _vlcPlayer_MediaOpened(object sender, EventArgs e)
        {
            InitializeComponent();

            if (OnMediaOpened != null)
                OnMediaOpened.Invoke(this, EventArgs.Empty);
        }

        private void _vlcPlayer_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MouseMove != null)
                MouseMove.Invoke(sender, e);
        }

        void VlcMediaPlayer_TimeChanged(object sender, EventArgs e)
        {
            if (OnTimeChanged != null)
                OnTimeChanged.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_SubItemChanged(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Core.Events.MediaSubitemAddedArgs> e)
        {
            
            if (SubItemChanged != null)
                SubItemChanged.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_Stoped(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            if (OnMediaStopped!= null)
                OnMediaStopped.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_Playing(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            if (OnMediaPlaying != null)
                OnMediaPlaying.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_Paused(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            if (OnMediaPaused != null)
                OnMediaPaused.Invoke(this, EventArgs.Empty); 
        }

        void VlcMediaPlayer_Opening(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            if (OnMediaOpening != null)
                OnMediaOpening.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_MediaChanged(object sender, Meta.Vlc.MediaPlayerMediaChangedEventArgs e)
        {
            if (OnMediaChanged != null)
                OnMediaChanged.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_LengthChanged(object sender, EventArgs e)
        {
            if (OnDurationChanged != null)
                OnDurationChanged.Invoke(this, EventArgs.Empty);
        }

        void VlcMediaPlayer_EndReached(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            if (EndReached != null)
                EndReached.Invoke(this, EventArgs.Empty);
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            if (EncounteredError != null)
                EncounteredError.Invoke(this, EventArgs.Empty);
        }

        private void VlcMediaPlayer_Buffering(object sender, Meta.Vlc.MediaPlayerBufferingEventArgs e)
        {
            if (Buffering != null)
                Buffering.Invoke(this, EventArgs.Empty);
        }
        
    }
}
