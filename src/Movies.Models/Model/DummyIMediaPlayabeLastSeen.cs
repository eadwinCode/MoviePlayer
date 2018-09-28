using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class DummyIMediaPlayabeLastSeen : IMediaPlayabeLastSeen
    {
        public double Progress
        {
            get; set;
        }
        public ILastSeen LastPlayedPoisition
        {
            get ;
            set ;
        }

        private DummyIMediaPlayabeLastSeen()
        {

        }

        public static DummyIMediaPlayabeLastSeen CreateDummtObject()
        {
            var dummy = new DummyIMediaPlayabeLastSeen();
            dummy.LastPlayedPoisition = PlayedFiles.CreateDummyFile();
            return dummy;
        }

        public void SetProgress()
        {
            
        }

        public void PlayCompletely()
        {
            
        }
    }
}
