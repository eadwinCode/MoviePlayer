using Meta.Vlc;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Interfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Models.Model
{

    public class SubtitleFilesModel : NotificationObject, ISubtitleFiles
    {
        public TrackDescription TrackDescription;
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string FileName { get; set; }
        public string Directory { get; set; }
        private bool isselected = false;
        public SubtitleType SubtitleType { get; private set; }
        public bool IsSelected
        {
            get { return isselected; }
            set { isselected = value; RaisePropertyChanged(() => this.IsSelected); }
        }

        public SubtitleFilesModel(SubtitleType SubtitleType, string dir)
        {
            if (dir != null)
            {
                this.Directory = dir;
                this.FileName = Path.GetFileNameWithoutExtension(dir);
            }
            this.SubtitleType = SubtitleType;
        }
        public SubtitleFilesModel(TrackDescription trackDescription,int currentid,string path = null)
        {
            this.Directory = path;
            this.TrackDescription = trackDescription;
            this.Name = trackDescription.Name;
            this.Id = TrackDescription.Id;
            this.SubtitleType = SubtitleType.HardCoded;
            if(trackDescription.Id == currentid)
                this.IsSelected = true;
            
        }

        public override string ToString()
        {
            if (TrackDescription != null)
            {
                return TrackDescription.Name;
            }
            return FileName == null ? SubtitleType.ToString(): FileName;
        }

        public static void Add(IList<SubtitleFilesModel> collection, string filename, SubtitleType SubtitleType)
        {
            SubtitleFilesModel file = new SubtitleFilesModel(SubtitleType,filename);
            if (collection.FirstOrDefault(x => x.FileName == file.FileName) == null)
            {
                collection.Add(file);
            }
        }

        public static void Remove(List<SubtitleFilesModel> subPath, SubtitleFilesModel subtitleFiles)
        {
            if (subPath.Contains(subtitleFiles))
            {
                subPath.Remove(subtitleFiles);
            }
        }
    }
}
