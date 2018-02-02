using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using Microsoft.Practices.Prism.Events;
using System.Runtime.Serialization;

namespace VideoComponent.BaseClass
{
//    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
//    public class VideoData : FileSystemInfo, IDisposable,IFolder, IVideoData, INotifyPropertyChanged, IComparable<VideoData>, IEquatable<VideoData>, ILastSeen, ISerializable
//    {
//        readonly static IEventAggregator _aggregator;
//        private string duration;
       
//        private string filepath;
//        private BitmapSource thumbnail;
//        private string creationdate;
//        public double progress;
//        private string[] subpath;
      
//        private uint frameheight;
//        private uint framewidth;  private string resolution;
//        public SortType SortedBy;
//        private string size;
//        private double? maxprogress;
//        private FileInfo fileinfo;
//        private DirectoryInfo directoryinfo;
//        public bool HasCompleteLoad { get; set; }
//        private FileType filetype;
//        public virtual FileType FileType { get { return filetype; } set { filetype = value; UpdateProperties(); } }
       
//        //static VideoData()
//        //{
//        //   // DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoData), new FrameworkPropertyMetadata(typeof(VideoData)));
//        //    _aggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
//        //}
//        public VideoData(SerializationInfo info, StreamingContext context)
//            :base(info,context)
//        {

//        }
//        public VideoData(string filepath)
//        {
//            this.filepath = filepath;
//        }

//        public VideoData(FileInfo fileinfo)
//        {
//            this.fileinfo = fileinfo;
//            this.filepath = FileInfo.FullName;
            
//        }

//        private void Publish(VideoData videodata)
//        {
//            //DirectoryInfo dir = new DirectoryInfo(videodata.FilePath);
//            //if (dir.Exists)
//            //{
//            //    VideoComponentList.DoubleClickAction(videodata);
//            //}
//            //else
//            //{
//            //    _aggregator.GetEvent<PlayExecuteCommandEvent>().Publish(videodata);
//            //}
//        }

//        public FileInfo FileInfo
//        {
//            get
//            {
//                return fileinfo == null ? fileinfo = new FileInfo(FilePath) : fileinfo;
//            }
//        }

//        public DirectoryInfo DirectoryInfo
//        {
//            get
//            {
//                if (filepath == null) return null;
//                return
//                    directoryinfo == null ? directoryinfo = new DirectoryInfo(FilePath) : directoryinfo;
//            }
//        }
      

//        public string FileExtension
//        {
//            get 
//            {
//                return FileInfo.Extension;
//            }
//            //set 
//            //{ 
//            //    fileextension = value.ToLower();
//            //   //SetBackGroundColor(value);
//            //    RaisePropertyChangedEvent("FileExtension");
//            //}
//        }
        
//        public double Progress
//        {
//            get { return progress; }
//            set 
//            {
//                progress = value; 
//                RaisePropertyChangedEvent("Progress");
//                RaisePropertyChangedEvent("PlayedVisible");
//            }
//        }

//        public bool HasLastSeen
//        {
//            get
//            {
//                return LastSeen.HasSeenItem(this as ILastSeen);
//            }
//        }
//        public string FilePath
//        {
//            get
//            {
//                return filepath;
//            }
//        }
//        public BitmapSource Thumbnail
//        {
//            get
//            {
//                return thumbnail;
//            }
//            set
//            {
//                thumbnail = value;
//                RaisePropertyChangedEvent("Thumbnail");
//            }
//        }
//        public Visibility PlayedVisible
//        {
//            get
//            {
//                return Progress > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
//            }
//        }

//        public string[] SubPath
//        {
//            get { return subpath; }
//            set { subpath = value; RaisePropertyChangedEvent("SubVisible"); }
//        }
        
//        public override string ToString()
//        {
//            return FileName;
//        }
       
      
        
//        public uint FrameWidth
//        {
//            get
//            {
//                return framewidth;
//            }
//            set
//            {
//                framewidth = value;
//                RaisePropertyChangedEvent("FrameWidth");
//            }
//        }

//        public uint FrameHeight
//        {
//            get
//            {
//                return frameheight;
//            }
//            set
//            {
//                frameheight = value;
//                RaisePropertyChangedEvent("FrameHeight");
//            }
//        }

//        public string Resolution
//        {
//            get { return resolution; }
//            set { resolution = FrameWidth + " " + value + " " + FrameHeight; RaisePropertyChangedEvent("Resolution"); }
//        }

//        public string Duration
//        {
//            get { return duration; }
//            set { duration = value; RaisePropertyChangedEvent("Duration"); RaisePropertyChangedEvent("CreationDate"); }
//        }

//        public double? MaxiProgress
//        {
//            get { return maxprogress; }
//            set {
//                if (value != null)
//                {
//                    maxprogress = (ulong)(value / Math.Pow(10, 7));
//                }
//            }
//        }
        

//        public int CompareTo(VideoData other)
//        {
//            if (this.FileType == other.FileType)
//            {
//                return this.Name.CompareTo(other.Name);
//            }
//            else if (this.FileType == FileType.Folder)
//            {
//                return -1;
//            }
//            return 1;
//        }



//        public bool Equals(VideoData other)
//        {
//            return this.Name.Equals(other.Name);
//        }


//        public Visibility SubVisible
//        {
//            get
//            {
//                if (SubPath == null)
//                {
//                    return System.Windows.Visibility.Collapsed;
//                }
//                else
//                    return SubPath.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
//            }
//        }

//        public string FileSize
//        {
//            get
//            {
//                return size;
//            }
//            set
//            {
//                size = value;
//                RaisePropertyChangedEvent("FileSize");
//            }
//        }

//        public bool HasProgress
//        {
//            get { return Progress > 0; }
//        }

//        private void UpdateProperties()
//        {
//            RaisePropertyChangedEvent("CreationDate");
//            RaisePropertyChangedEvent("FolderChildCount");
//            RaisePropertyChangedEvent("ChildCount");
//            RaisePropertyChangedEvent("FileExtension");
//            RaisePropertyChangedEvent("CreationDate");
//        }

//        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
//        {
//            GetObjectData(info, context);
//        }

//        public override void Delete()
//        {
//            DirectoryInfo.Delete();
//        }

//        public void FileDelete()
//        {
//            FileInfo.Delete();
//        }

//        public override bool Exists
//        {
//            get { return DirectoryInfo.Exists; }
//        }

//        public bool FileExists
//        {
//            get { return FileInfo.Exists; }
//        }

//        public override string Name
//        {
//            get { return FileInfo.Name; }
//        }

//        public override void GetObjectData(SerializationInfo info, StreamingContext context)
//        {
//            base.GetObjectData(info, context);
//            //info.AddValue("OriginalPath", OriginalPath);
//            //info.AddValue("Label", Label);
//            //info.AddValue("Name", Name);
//            //info.AddValue("FullName", FullName);
//            //info.AddValue("Attributes", Attributes);
//            //info.AddValue("LastWriteTime", LastWriteTime);
//            //info.AddValue("LastAccessTime", LastAccessTime);
//            //info.AddValue("CreationTime", CreationTime);
//        }

//        ~VideoData()
//        {
//           this.Dispose();
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void RaisePropertyChangedEvent(string propertyName)
//        {
//            // Exit if no subscribers
//            if (PropertyChanged == null) return;

//            // Raise event
//            var e = new PropertyChangedEventArgs(propertyName);
//            PropertyChanged(this, e);
//        }

//        public string FileName
//        {
//            get
//            {
//                return this.Name;
//            }
//        }



//        public void Dispose()
//        {

//        }
//    }
  
}
