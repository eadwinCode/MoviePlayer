using Delimon.Win32.IO;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;

namespace Movies.Models.Model
{
    public class MediaFolder :  IFolder, INotifyPropertyChanged, IDisposable
    {
        private int childcount;
        private string filepath;
        private FileInfo fileinfo;
        private bool hassubfolders = false;
        private string filesize;
        private ObservableCollection<MediaFolder> childrenfiles;
        public int intChildCount;
        private DirectoryInfo directory;
        private IFolder parentdirectory;
        private bool hasfilechanges = false;
        public object Tag;
        public ObservableCollection<PlayedFiles> LastSeenCollection { get; set; }
        private bool hascompleteload;
        public IFileSystemWatcher MediaFileWatcher;
        public virtual event OnFileNameChangedHandler OnFileNameChangedChanged;
        public event EventHandler OnFileWatcherUpdate;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ParentDirectoryChanged;
        
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

        public MediaFolder(IFolder ParentDir, string filepath)
            : this(filepath)
        {
            this.parentdirectory = ParentDir;
        }

        public MediaFolder(string filepath)
        {
            this.filepath = filepath;
        }

        public MediaFolder(FileInfo fileinfo)
        {
            this.fileinfo = fileinfo;
            this.filepath = fileinfo.FullName;
        }
        
        public virtual bool Exists
        {
            get { return Directory.Exists; }
        }

        public string Name
        {
            get { return Directory.Name; }
        }

        //public string CreationDate
        //{
        //    get { return this.FileInfo.CreationTime.Date.ToString("MM/dd/yy"); }
        //}

        public DateTime CreationDate
        {
            get { return FileInfo.CreationTime; }
        }

        public ObservableCollection<MediaFolder> OtherFiles
        {
            get { return this.childrenfiles; }
            set
            {
                
                this.childrenfiles = value;

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
                childcount = OtherFiles == null ? 0 : OtherFiles.Count();
                return childcount;
            }
        }

        public virtual GroupCatergory FileType
        {
            get { return GroupCatergory.Grouped; }
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

        public override bool Equals(object obj)
        {
            if (obj != null && (obj is MediaFolder))
            {
                return ((MediaFolder)obj).FullName == this.FullName;
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

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            // Exit if no subscribers
            if (PropertyChanged == null) return;

            // Raise event
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        public virtual void RefreshFileInfo()
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

        SortType sortedby = SortType.Date;
        public SortType SortedBy { get { return sortedby; } set { sortedby = value; } } 

        public virtual void SetParentDirectory(IFolder folder)
        {
            parentdirectory = folder;
        }

        public void Dispose()
        {
            if (MediaFileWatcher != null)
                MediaFileWatcher.UnloadWatch();
        }

        public void AddChild(MediaFolder videoFolder,bool fileWatchChanges =false)
        {
            this.childrenfiles.Add(videoFolder);
            if (fileWatchChanges)
                FileWatcherUpdate();
        }
        public void AddRange(IEnumerable<MediaFolder> videoFolders, bool fileWatchChanges = false)
        {
            foreach (var item in videoFolders)
            {
                AddChild(item,fileWatchChanges);
            }
        }

        public void RemoveChild(MediaFolder videoFolder, bool fileWatchChanges = false)
        {
            this.childrenfiles.Remove(videoFolder);
            if (fileWatchChanges)
                FileWatcherUpdate();
        }

        public void FileWatcherUpdate()
        {
            if (OnFileWatcherUpdate != null)
                OnFileWatcherUpdate.Invoke(this, EventArgs.Empty);
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

        public ObservableCollection<PlaylistModel> PlayListItems
        {
            get { return ApplicationModelService.AppPlaylist.MoviePlayList; }
        }

        protected IApplicationModelService ApplicationModelService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationModelService>();
            }
        }

    }
}
