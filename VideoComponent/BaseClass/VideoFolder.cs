using Common.Interfaces;
using Common.Model;
using Common.Util;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

namespace VideoComponent.BaseClass
{
    public class VideoFolder : FileSystemInfo, IFolder, INotifyPropertyChanged
    {
        private int childcount;
        private string filepath;
        public SortType SortedBy{get;set;}
        private FileInfo fileinfo;
        private ObservableCollection<VideoFolder> childrenfiles;
      //  private string folderchildCount; 
        public int intChildCount;
        private DirectoryInfo directory;
        private IFolder parentdirectory;
        private bool hasfilechanges = false;
        public object Tag;
        public ObservableCollection<PlayedFiles> LastSeenCollection { get; set; }
        public virtual bool HasCompleteLoad
        {
            get
            {
                return OtherFiles == null ? false : true;
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
        public VideoFolder(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {

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
        public override void Delete()
        {
            Directory.Delete();
        }
        public override bool Exists
        {
            get { return Directory.Exists; }
        }
        public override string Name
        {
            get { return FileInfo.Name; }
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
                RaisePropertyChangedEvent("OtherFiles");
                RaisePropertyChangedEvent("ChildrenSize");
                RaisePropertyChangedEvent("FolderChildCount");
            }
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
    }
}
