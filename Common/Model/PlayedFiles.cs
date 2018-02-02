using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Model
{
    public class PlayedFiles : ILastSeen
    {
        private string filename;
        private bool incollection;
        private double progressLastSeen;
        public string FileName { get => filename; set => filename = value; }
        public double ProgressLastSeen { get => progressLastSeen; set => progressLastSeen = value; }
        public bool InCollection { get => incollection; }

        
        public PlayedFiles(string FileName, double duration)
        {
            this.filename = FileName;
            progressLastSeen = duration;
        }
        public PlayedFiles(string FileName)
        {
            this.filename = FileName;
        }
        public PlayedFiles()
        {
            incollection = true;
        }

        public void SetInCollection()
        {
            incollection = true;
        }

        public override string ToString()
        {
            return FileName + ", Duration: " + ProgressLastSeen;
        }
    }
}
