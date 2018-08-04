using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.FileHelper
{
    public class SavedLastSeenCollection
    {
        private IDictionary<string, PlayedFiles> lastseencollection;
        public IDictionary<string, PlayedFiles> LastSeenCollection
        {
            get { return lastseencollection; }
            private set { lastseencollection = value; }
        }
        public SavedLastSeenCollection()
        {
            LastSeenCollection = new Dictionary<string, PlayedFiles>();
        }
        public void Add(PlayedFiles playedFiles)
        {
            if(!LastSeenCollection.ContainsKey(playedFiles.FileName))
                this.LastSeenCollection.Add(playedFiles.FileName, playedFiles);
        }

        public void Remove(PlayedFiles playedFiles)
        {
            if (LastSeenCollection.ContainsKey(playedFiles.FileName))
                this.LastSeenCollection.Remove(playedFiles.FileName);
        }

        public PlayedFiles GetLastSeen(string fileName)
        {
            PlayedFiles playedFiles = null;
            LastSeenCollection.TryGetValue(fileName, out playedFiles);
            return playedFiles;
        }

        public bool HasItem(PlayedFiles playedFiles)
        {
            return LastSeenCollection.ContainsKey(playedFiles.FileName);
        }
    }
}
