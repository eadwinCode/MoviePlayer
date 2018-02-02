using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VideoComponent.BaseClass
{
    //public class VideoInfo : VideoData
    //{
    //    private int childcount;
    //    private ObservableCollection<VideoData> childfile;
    //    private string folderchildCount; public int intChildCount;

    //     public VideoInfo(SerializationInfo info, StreamingContext context)
    //        :base(info,context)
    //    {
    //    }
    //    public VideoInfo(string filepath):base(filepath)
    //    {
    //    }

    //    public VideoInfo(FileInfo fileinfo):base(fileinfo)
    //    {
    //    }
    //    public string CreationDate
    //    {
    //        get { return this.FileInfo.CreationTime.Date.ToString("MM/dd/yy"); }
    //    }
    //    public ObservableCollection<VideoData> OtherFiles
    //    {
    //        get { return this.childfile; }
    //        set
    //        {
    //            this.childfile = value;
    //            RaisePropertyChangedEvent("ChildFiles");
    //        }
    //    }
    //    public string FolderChildCount
    //    {
    //        get
    //        {
    //            return ChildCount > 1 ? ChildCount + " Items" : ChildCount + " Item"; ;
    //        }
    //    }

    //    public int ChildCount
    //    {
    //        get
    //        {
    //            childcount = OtherFiles == null ? 0 : OtherFiles.Count;
    //            return childcount;
    //        }
    //    }

    //    public override FileType FileType
    //    {
    //        get
    //        {
    //            return base.FileType;
    //        }
    //        set
    //        {
    //            base.FileType = value;
    //        }
    //    }
    //}
}
