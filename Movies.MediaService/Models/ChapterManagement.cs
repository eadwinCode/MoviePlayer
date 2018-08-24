using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Vlc.Wpf;

namespace Movies.MediaService.Models
{
    public class ChapterManagement
    {
        private VlcPlayer _vlcPlayer;

        public ChapterManagement(VlcPlayer vlcPlayer)
        {
            _vlcPlayer = vlcPlayer;
        }

        public int Chapters { get{ return _vlcPlayer.VlcMediaPlayer.Chapter; } set { _vlcPlayer.VlcMediaPlayer.Chapter = value; } }

        public int ChapterCount { get { return _vlcPlayer.VlcMediaPlayer.ChapterCount; } }

        public void NextChapter()
        {
            _vlcPlayer.VlcMediaPlayer.NextChapter();
        }

        public void PreviousChapter()
        {
            _vlcPlayer.VlcMediaPlayer.PreviousChapter();
        }
    }
}
