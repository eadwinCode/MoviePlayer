using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VideoComponent.BaseClass
{
    public class VideoFolderChild : VideoFolder, IVideoData, IComparable<VideoFolderChild>, IEquatable<VideoFolderChild>
    {
        private string duration;
        private List<SubtitleFilesModel> subpath;
        private uint frameheight;
        private uint framewidth; 
        private string resolution;
        private string size;
        private double? maxprogress;
        public VideoFolderChild(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {

        }

        public VideoFolderChild(string filepath):base(filepath)
        {
        }

        public VideoFolderChild(FileInfo fileinfo)
            : base(fileinfo)
        {
            
        }

        public VideoFolderChild(IFolder ParentDir, string filepath)
            : base(ParentDir, filepath)
        {
        }

        public VideoFolderChild(IFolder ParentDir, FileInfo fileinfo)
            : base(ParentDir, fileinfo.FullName)
        {
        }

        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                ProgressAsString = value.ToString();
                RaisePropertyChangedEvent("Progress");
                RaisePropertyChangedEvent("PlayedVisible");
                
            }
        }
        private string progressasstring;
        public string ProgressAsString
        {
            get { return progressasstring; }
            set { progressasstring = CommonHelper.SetDuration(progress);
                RaisePropertyChangedEvent("ProgressAsString");
            }
        }

        public bool HasLastSeen
        {
            get
            {
                return (LastPlayedPoisition as PlayedFiles).InCollection;
            }
        }
        public override string FilePath
        {
            get
            {
                return base.FilePath;
            }
        }
        public Visibility PlayedVisible
        {
            get
            {
                if (this.FileType == FileType.Folder)
                {
                    return Visibility.Collapsed;  
                }
                return Progress > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        public override bool HasThumbnail
        {
            get
            {
                return thumbnail != null ? true:false;
            }
        }
        
        public List<SubtitleFilesModel> SubPath
        {
            get { return subpath; }
            set { subpath = value; RaisePropertyChangedEvent("SubVisible"); }
        }

        public BitmapSource Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
                RaisePropertyChangedEvent("Thumbnail"); 
                RaisePropertyChangedEvent("HasThumbnail");
            }
        }
        private BitmapSource thumbnail;

        public override string ToString()
        {
            return base.ToString();
        }

        public uint FrameWidth
        {
            get
            {
                return framewidth;
            }
            set
            {
                framewidth = value;
                RaisePropertyChangedEvent("FrameWidth");
            }
        }
        private bool isactive;
        public bool IsActive { get { return isactive; } set { isactive = value; RaisePropertyChangedEvent("IsActive"); } }
        public uint FrameHeight
        {
            get
            {
                return frameheight;
            }
            set
            {
                frameheight = value;
                RaisePropertyChangedEvent("FrameHeight");
            }
        }

        public string Resolution
        {
            get { return resolution; }
            set { resolution = FrameWidth + " " + value + " " + FrameHeight; RaisePropertyChangedEvent("Resolution"); }
        }

        public string Duration
        {
            get { return duration; }
            set { duration = value; RaisePropertyChangedEvent("Duration"); RaisePropertyChangedEvent("CreationDate"); }
        }

        public double? MaxiProgress
        {
            get { return maxprogress; }
            set
            {
                if (value != null)
                {
                    maxprogress = (ulong)(value / Math.Pow(10, 7));
                }

                RaisePropertyChangedEvent("MaxiProgress");
            }
        }

        public int CompareTo(VideoFolderChild other)
        {
            if (this.FileType == other.FileType)
            {
                return this.Name.CompareTo(other.Name);
            }
            else if (this.FileType == FileType.Folder)
            {
                return -1;
            }
            return 1;
        }
        public bool Equals(VideoFolderChild other)
        {
            return this.Name.Equals(other.Name);
        }
        public Visibility SubVisible
        {
            get
            {
                if (SubPath == null)
                {
                    return System.Windows.Visibility.Collapsed;
                }
                else
                    return SubPath.Count > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        public string FileSize
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                RaisePropertyChangedEvent("FileSize");
            }
        }

        public override bool HasCompleteLoad
        {
            get
            {
                if (!HasThumbnail && ApplicationService.AppSettings.ViewType == ViewType.Large)
                {
                    return false;
                }
                if (ApplicationService.AppSettings.ViewType == ViewType.Small && MaxiProgress == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void PlayCompletely(){
            LastSeenHelper.RemoveLastSeen(base.ParentDirectory, lastplayedpoisition);
        }

        public bool HasProgress
        {
            get { return Progress > 0; }
        }

        private void UpdateProperties()
        {
            RaisePropertyChangedEvent("CreationDate");
            RaisePropertyChangedEvent("FolderChildCount");
            RaisePropertyChangedEvent("ChildCount");
            RaisePropertyChangedEvent("FileExtension");
            RaisePropertyChangedEvent("CreationDate");
            RaisePropertyChangedEvent("Progress");
            RaisePropertyChangedEvent("ProgressAsString");
            RaisePropertyChangedEvent("PlayedVisible");
            RaisePropertyChangedEvent("HasProgress");


        }

        public string TooltipMessage{
            get{
                return string.Format("FileName:- {0}\nPath:- {1}\nDuration:- {2}", FileName, FilePath,Duration);
            }
        }

        private ILastSeen lastplayedpoisition;
        private double progress;

        public ILastSeen LastPlayedPoisition
        {
            get { return lastplayedpoisition; } set
            {
                lastplayedpoisition = value;
                Progress = value != null ? value.ProgressLastSeen : 0.0;
                UpdateProperties();
            }
        }
        
        public ObservableCollection<PlaylistModel> PlayListItems
        {
            get { return ApplicationService.AppPlaylist.MoviePlayList; }
        }
    }
}
