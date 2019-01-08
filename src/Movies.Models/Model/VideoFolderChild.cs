                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             using Delimon.Win32.IO;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.Enums;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Common.ApplicationCommands;

namespace Movies.Models.Model
{

    public class MediaFile : MediaFolder, IVideoData,IPlayable, IComparable<MediaFile>,IEquatable<MediaFile>
    {
        private string duration = ApplicationDummyMessage.DurationNotYetLoaded;
        private ObservableCollection<string> subpath;
        private uint frameheight;
        private uint framewidth; 
        private string resolution;
        private string size;
        private int? maxprogress;
        public override event OnFileNameChangedHandler OnFileNameChangedChanged;

        public MediaFile(string filepath):base(filepath)
        {
        }

        public MediaFile(FileInfo fileinfo)
            : base(fileinfo)
        {

        }

        public MediaFile(IFolder ParentDir, string filepath)
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

        public MediaFile(IFolder ParentDir, FileInfo fileinfo)
            : this(ParentDir, fileinfo.FullName)
        {
        }

        public override GroupCatergory FileType
        {
            get { return GroupCatergory.Child; }
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

        public override bool Exists
        {
            get { return FileInfo.Exists; }
        }

        public Visibility PlayedVisible
        {
            get
            {
                if (this.FileType == GroupCatergory.Grouped)
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
        
        public ObservableCollection<string> SubPath
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

        public int CompareTo(MediaFile other)
        {
            if (this.FileType == other.FileType)
            {
                return this.Name.CompareTo(other.Name);
            }
            else if (this.FileType == GroupCatergory.Grouped)
            {
                return -1;
            }
            return 1;
        }

        public bool Equals(MediaFile other)
        {
            return this.MediaTitle.Equals(other.MediaTitle);
        }

        public Visibility SubVisible
        {
            get
            {
                if (SubPath == null)
                {
                    return Visibility.Collapsed;
                }
                else
                    return SubPath.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public override string FileSize
        {
            get { return size; }
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
                if (!HasThumbnail && ApplicationModelService.AppSettings.ViewType == ViewType.Large)
                {
                    return false;
                }
                if (ApplicationModelService.AppSettings.ViewType == ViewType.Small && 
                    Duration == ApplicationDummyMessage.DurationNotYetLoaded)
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

        private void UpdateProperties()
        {
            RaisePropertyChangedEvent("CreationDate");
            RaisePropertyChangedEvent("FileExtension");
            RaisePropertyChangedEvent("CreationDate");
            RaisePropertyChangedEvent("Progress");
            RaisePropertyChangedEvent("ProgressAsString");
            RaisePropertyChangedEvent("PlayedVisible");
            RaisePropertyChangedEvent("HasProgress");
            RaisePropertyChangedEvent("Duration");
        }

        public override void RefreshFileInfo()
        {
            UpdateProperties();
        }

        public void SetProgress()
        {
            this.LastPlayedPoisition.ProgressLastSeen = this.progress;
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

        public Uri Url
        {
            get { return new Uri(FilePath); }
        }
    }
}                                                                                                                                                                                                                                         