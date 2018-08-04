using Common.FileHelper;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Model
{
    public class PlayedFiles : ILastSeen
    {
        private string percentage;
        private string filename;
        private double progressLastSeen;
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }
        public double ProgressLastSeen
        {
            get { return progressLastSeen; }
            set { progressLastSeen = value; }
        }
        public bool Exist { get { return ApplicationService.SavedLastSeenCollection.HasItem(this); } }

        public string Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }

        public PlayedFiles(string FileName, double duration)
        {
            this.filename = FileName;
            progressLastSeen = duration;
        }
        public PlayedFiles()
        {

        }
        public PlayedFiles(string FileName)
        {
            this.filename = FileName;
        }

        public override string ToString()
        {
            return FileName + ", Duration: " + ProgressLastSeen;
        }

        public void RemoveLastSeen()
        {
            ApplicationService.SavedLastSeenCollection.Remove(this);
        }
    }
}
