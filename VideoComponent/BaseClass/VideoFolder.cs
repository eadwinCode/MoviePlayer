using Common.Interfaces;
using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace VideoComponent.BaseClass
{
    public class VideoFolder : IFolder, INotifyPropertyChanged,IDisposable
    {
        private int childcount;
        private string filepath;
        private FileInfo fileinfo;
        private ObservableCollection<VideoFolder> childrenfiles;
        public int intChildCount;
        private DirectoryInfo directory;
        private IFolder parentdirectory;
        private bool hasfilechanges = false;
        public object Tag;
        public ObservableCollection<PlayedFiles> LastSeenCollection { get; set; }
        private bool hascompleteload;
        public MediaFileWatcher MediaFileWatcher;
        public virtual event OnFileNameChangedHandler OnFileNameChangedChanged;

        public virtual bool HasCompleteLoad
        {
            get
            {
                return hascompleteload;
            }
            set
            {
                hascompleteload = value;
            }
        }
        
        public FileInfo FileInfo
        {
            get
            {
                if (fileinfo == null)
                {
                    fileinfo = new FileInfo(filepath);
                }
                return fileinfo;
            }
        }

        public virtual string FilePath
        {
            get
            {
                return filepath;
            }
        }

        public DirectoryInfo Directory
        {
            get
            {
                if (directory == null)
                {
                    directory = new DirectoryInfo(FileInfo.FullName);
                }
                return directory;
            }
        }

        public VideoFolder(IFolder ParentDir, string filepath)
            : this(filepath)
        {
            this.parentdirectory = ParentDir;
           
        }

        public VideoFolder(string filepath)
        {
            this.filepath = filepath;
        }

        public VideoFolder(FileInfo fileinfo)
        {
            this.fileinfo = fileinfo;
            this.filepath = fileinfo.FullName;
        }

        public SortType SortedBy { get; set; }

        public virtual bool Exists
        {
            get { return Directory.Exists; }
        }

        public string Name
        {
            get { return Directory.Name; }
        }

        public string CreationDate
        {
            get { return this.FileInfo.CreationTime.Date.ToString("MM/dd/yy"); }
        }

        public ObservableCollection<VideoFolder> OtherFiles
        {
            get { return this.childrenfiles; }
            set
            {
                
                this.childrenfiles = value;
                if (value != null && value.Count > 0)
                    MediaFileWatcher = new MediaFileWatcher(this);

                RefreshFileInfo();
            }
        }

        private bool isloading;

        public bool IsLoading
        {
            get { return isloading; }
            set { isloading = value; RaisePropertyChangedEvent("IsLoading"); }
        }

        public string FolderChildCount
        {
            get
            {
                return ChildrenSize > 1 ? childcount + " Items" : childcount + " Item"; ;
            }
        }

        public int ChildrenSize
        {
            get
            {
                childcount = OtherFiles == null ? 0 : OtherFiles.Count;
                return childcount;
            }
        }

        public virtual FileType FileType
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ParentDirectoryChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            // Exit if no subscribers
            if (PropertyChanged == null) return;

            // Raise event
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        public string FileName
        {
            get
            {
                return this.Name;
            }
        }

        public string FullName
        {
            get
            { return Directory.FullName; }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && (obj is VideoFolder))
            {
                return ((VideoFolder)obj).FullName == this.FullName;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool HasFileChanges
        {
            get
            {
                return hasfilechanges;
            }
        }

        public string FileExtension
        {
            get
            {
                return FileInfo.Extension;
            }
            //set 
            //{ 
            //    fileextension = value.ToLower();
            //   //SetBackGroundColor(value);
            //    RaisePropertyChangedEvent("FileExtension");
            //}
        }
        
        public void RefreshFileInfo()
        {
            RaisePropertyChangedEvent("Name");
            RaisePropertyChangedEvent("FileName");
            RaisePropertyChangedEvent("FullName");
            RaisePropertyChangedEvent("OtherFiles");
            RaisePropertyChangedEvent("ChildrenSize");
            RaisePropertyChangedEvent("FolderChildCount");
        }

        public int Day
        {
            get { return this.Directory.CreationTime.Day; }
        }

        public IFolder ParentDirectory
        {
            get {
                return parentdirectory;
            }
        }
        
        public virtual bool HasThumbnail
        {
            get
            {
                return true;
            }
        }

        public virtual void SetParentDirectory(IFolder folder)
        {
            parentdirectory = folder;
        }

        public void Dispose()
        {
            if (MediaFileWatcher != null)
                MediaFileWatcher.UnloadWatch();
        }

        public void RenameFile(string newFilePath)
        {
            var oldname = this.FullName;
            this.filepath = newFilePath;
            directory = null;
            fileinfo = null;
            if (ParentDirectoryChanged != null)
                ParentDirectoryChanged.Invoke(this, new EventArgs());
            if (OnFileNameChangedChanged != null)
                OnFileNameChangedChanged.Invoke(oldname, this);
        }

        private bool hassubfolders = false;
        private string filesize;

        public bool HasSubFolders
        {
            get { return hassubfolders; }
            set { hassubfolders = value; }
        }

        public virtual string FileSize
        {
            get
            {
                return filesize;
            }
            set
            {
                filesize = value;
                RaisePropertyChangedEvent("FileSize");
            }
        }
    }
}
