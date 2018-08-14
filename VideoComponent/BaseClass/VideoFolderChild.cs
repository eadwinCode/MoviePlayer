using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoComponent.BaseClass
{
    public delegate void OnFileNameChangedHandler(string oldname, VideoFolder videoItem);

    public class VideoFolderChild : VideoFolder, IVideoData, IComparable<VideoFolderChild>, IEquatable<VideoFolderChild>
    {
        private string duration;
        private ObservableCollection<SubtitleFilesModel> subpath;
        private uint frameheight;
        private uint framewidth; 
        private string resolution;
        private string size;
        private int? maxprogress;
        public override event OnFileNameChangedHandler OnFileNameChangedChanged;

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
            ParentDir.ParentDirectoryChanged += ParentDir_ParentDirectoryChanged;
        }

        private void ParentDir_ParentDirectoryChanged(object sender, EventArgs e)
        {
            var oldname = this.FullName;
            var newname = ParentDirectory.FileInfo.FullName + "\\" + this.Name;
            base.RenameFile(newname);
            UpdateProperties();
            if (OnFileNameChangedChanged != null)
                OnFileNameChangedChanged.Invoke(oldname, this);
        }

        public VideoFolderChild(IFolder ParentDir, FileInfo fileinfo)
            : this(ParentDir, fileinfo.FullName)
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
            get {
                if (!HasProgress)
                    return this.Duration;
                return progressasstring;
            }
            set {
                // progressasstring = CommonHelper.SetDuration(progress);
                progressasstring = progress + "% Complete";
                RaisePropertyChangedEvent("PlayedVisible");
                RaisePropertyChangedEvent("ProgressAsString");
            }
        }

        public bool HasLastSeen
        {
            get
            {
                return LastPlayedPoisition.Exist;
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


        public bool HasSearchSubtitleFile { get; set; }

        public override bool HasThumbnail
        {
            get
            {
                return thumbnail != null ? true:false;
            }
        }
        
        public ObservableCollection<SubtitleFilesModel> SubPath
        {
            get { return subpath; }
            set { subpath = value; RaisePropertyChangedEvent("SubVisible");
                RaisePropertyChangedEvent("SubPath"); }
        }

        public ImageSource Thumbnail
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
        private ImageSource thumbnail;

        public override string ToString()
        {
            return this.MediaTitle;
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
        public bool IsActive
        {
            get { return isactive; }
            set { isactive = value; RaisePropertyChangedEvent("IsActive"); }
        }
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
            set { duration = value;
                RaisePropertyChangedEvent("Duration"); 
                RaisePropertyChangedEvent("CreationDate"); 
                RaisePropertyChangedEvent("TooltipMessage"); }
        }

        public int? MaxiProgress
        {
            get { return maxprogress; }
            set
            {
                if (value != null)
                {
                    maxprogress = value;
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
            return this.MediaTitle.Equals(other.MediaTitle);
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
        public override string FileSize
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
                if (ApplicationService.AppSettings.ViewType == ViewType.Small &&
                    MaxiProgress == null || !HasSearchSubtitleFile)
                {
                    return false;
                }
                return true;
            }
        }

        public void PlayCompletely(){
            LastPlayedPoisition.RemoveLastSeen();
            LastPlayedPoisition = new PlayedFiles(this.FileName);
        }

        public bool HasProgress
        {
            get { return Progress > 0; }
        }

        public void UpdateProperties()
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
            RaisePropertyChangedEvent("Duration");
        }

        public string TooltipMessage{
            get
            {
                return string.Format("FileName:- {0} \nDuration:- {1}", Name, Duration);
            }
        }

        private string mediatitle;
        public string MediaTitle
        {
            get {
                if (mediatitle == null) return this.Name;
                return mediatitle;
            }
            set { mediatitle = value; RaisePropertyChangedEvent("MediaTitle"); }
        }
        
        private ILastSeen lastplayedpoisition;
        private double progress;

        public ILastSeen LastPlayedPoisition
        {
            get { return lastplayedpoisition; }
            set
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
