using Meta.Vlc.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieHub.MediaPlayerElement.Models
{
    public class VideoAdjustManagement
    {
        VlcPlayer mediaplayer;

        public float Brightness
        {
            get { return mediaplayer.VlcMediaPlayer.Brightness; }
            set { mediaplayer.VlcMediaPlayer.Brightness = value; }
        }
        

        public float Contrast
        {
            get { return mediaplayer.VlcMediaPlayer.Contrast; }
            set { mediaplayer.VlcMediaPlayer.Contrast = value; }
        }
        

        public float Gamma
        {
            get { return mediaplayer.VlcMediaPlayer.Gamma; }
            set { mediaplayer.VlcMediaPlayer.Gamma = value; }
        }
        

        public float Saturation
        {
            get { return mediaplayer.VlcMediaPlayer.Saturation; }
            set { mediaplayer.VlcMediaPlayer.Saturation = value; }
        }

        public bool IsAdjustEnabled
        {
            get { return mediaplayer.VlcMediaPlayer.IsAdjustEnable; }
            set { mediaplayer.VlcMediaPlayer.IsAdjustEnable = value; }
        }

        public float Hue
        {
            get { return mediaplayer.VlcMediaPlayer.Hue; }
            set { mediaplayer.VlcMediaPlayer.Hue = value; }
        }

        public VideoAdjustManagement(VlcPlayer mediaplayer)
        {
            this.mediaplayer = mediaplayer;
        }
    }
}
