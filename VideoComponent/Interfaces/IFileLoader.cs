using Common.Interfaces;
using Common.Util;
using Delimon.Win32.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace VideoComponent.Interfaces
{
    public interface IFileLoader
    {
        IDictionary<string, VideoFolder> DataSource { get; set; }
        bool HasDataSource { get; }

        VideoFolder GetFolderItems(VideoFolder item);
        void GetRootDetails(SortType sorttype, ref VideoFolder ParentDir);
        VideoFolder LoadChildrenFiles(DirectoryInfo directoryInfo, bool newpath = false);
        ObservableCollection<VideoFolder> LoadChildrenFiles(IFolder Parentdir, bool newpath = false);
        VideoFolder LoadParentFiles(VideoFolder ParentDir, SortType sorttype);
        VideoFolder SortList(SortType sorttype, VideoFolder parent);
        VideoFolder LoadParentFiles(IFolder Parentdir,IList<DirectoryInfo> SubDirectory, SortType sorttype);
        VideoFolder LoadParentFiles(IFolder Parentdir,IList<DirectoryInfo> SubDirectory,IList<FileInfo>SubFiles, SortType sorttype);
        VideoFolder LoadParentFiles(IFolder Parentdir,IList<FileInfo>SubFiles, SortType sorttype);
        void RemoveFromDataSource(VideoFolder existingVideoFolder);
    }
}
