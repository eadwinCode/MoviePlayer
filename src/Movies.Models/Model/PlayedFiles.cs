
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class PlayedFiles : ILastSeen, IPlayedFiles
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
        public bool Exist { get { return ApplicationModelService.SavedLastSeenCollection.HasItem(this); } }

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
            ApplicationModelService.SavedLastSeenCollection.Remove(this);
        }

        public void Add()
        {
            ApplicationModelService.SavedLastSeenCollection.Add(this);
        }

        public void Save()
        {
            ApplicationModelService.SaveLastSeenFile();
        }

        public static ILastSeen CreateDummyFile()
        {
            return new PlayedFiles();
        }

        private IApplicationModelService ApplicationModelService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationModelService>();
            }
        }
    }
}
