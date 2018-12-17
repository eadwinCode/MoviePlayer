﻿using Common.ApplicationCommands;
using Delimon.Win32.IO;
using Microsoft.Practices.Prism;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.MovieServices.Services
{
    public class FileLoader : IFileLoader
    {
        private static object padlock = new object();
        private ISortService GetSortService;

        private IFileExplorerCommonHelper filecommonhelper;
        private IApplicationService applicationService;
        private IDispatcherService _dispatcherservice;

        private IDataSource<VideoFolder> MovieDataSource
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataSource<VideoFolder>>();
            }
        }

        private IStatusMessageManager StatusMessageManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IStatusMessageManager>();
            }
        }
        
        public IDispatcherService DispatcherService { get { return _dispatcherservice; } }
        
        private IEventManager IEventManager
        {
            get
            {
                return EventAggregatorService.IEventManager;
            }
        }
        
        public bool HasDataSource
        {
            get { return MovieDataSource.HasDataSource; }
        }
      
        public FileLoader(IFileExplorerCommonHelper fileExplorerCommonHelper, IApplicationService applicationService ,IDispatcherService dispatcherService)
        {
            this.filecommonhelper = fileExplorerCommonHelper;
            this.applicationService = applicationService;
            this._dispatcherservice = dispatcherService;
            GetSortService = new SortService();
        }
       
        public VideoFolder GetExistingVideoFolderIfAny(VideoFolder videoFolder)
        {
            VideoFolder folder = null;
            if(MovieDataSource.DataSource != null)
                MovieDataSource.DataSource.TryGetValue(videoFolder.FullName,out folder);
            return folder;
        }

        public VideoFolder GetFolderItems(VideoFolder item)
        {
            item.IsLoading = true;
            object padlock = new object();
            lock (item)
            {
                if (item.OtherFiles == null)
                {
                    var s = LoadParentFiles(item, item.SortedBy);
                    if (s.OtherFiles == null)
                        return s;
                }
                try
                {
                    foreach (var s in item.OtherFiles)
                    {
                        var temp = s;
                        if (s != null)
                        {
                            if (temp.FileType == GroupCatergory.Grouped)
                            {
                                temp.IsLoading = true;
                                GetFolderItemsExtended(temp);
                                temp.IsLoading = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            item.IsLoading = false;
            return item;
        }
        
        public VideoFolder DeepCopy(VideoFolder existing,VideoFolder videoFoldercopy)
        {
            var newcopy = new VideoFolder(videoFoldercopy, videoFoldercopy.FullName)
            {
                OtherFiles = videoFoldercopy.OtherFiles,
                HasCompleteLoad = videoFoldercopy.HasCompleteLoad,
                SortedBy = videoFoldercopy.SortedBy
            };
            return newcopy;
        }

        private VideoFolder GetFolderItemsExtended(VideoFolder vfile)
        {
            var s = LoadParentFiles(vfile, vfile.SortedBy);
            if (s.OtherFiles == null )
                return s;
            for (int i = 0; i < s.OtherFiles.Count(); i++)
            {
                try
                {
                    var temp = s.OtherFiles[i];
                    if (temp == null)
                        continue;
                    if (temp.FileType == GroupCatergory.Grouped)
                    {
                        temp.IsLoading = true;
                        GetSubFolderItems(temp);
                        temp.IsLoading = false;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return s;
        }

        private VideoFolder GetSubFolderItems(VideoFolder videoFolder)
        {
            Console.WriteLine("------Starting to Load {0} item-----",videoFolder);

            var s = LoadParentFiles(videoFolder, videoFolder.SortedBy);
            if (s.OtherFiles == null)
                return s;

            Parallel.ForEach(s.OtherFiles, (k) =>
            {
                try
                {
                    if (k != null)
                    {
                        var temp = k;
                        if (temp.FileType == GroupCatergory.Grouped)
                        {
                            temp.IsLoading = true;
                            GetSubFolderItems(temp);
                            temp.IsLoading = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
               
            });
            Console.WriteLine("------Finished loading {0} item-----", videoFolder);

            return s;
        }

        private  void LoadParentSubDirectories(IList<DirectoryInfo> parentSubDir, 
             ObservableCollection<VideoFolder> existingchildren, VideoFolder parentdir)
        {
            for (int i = 0; i < parentSubDir.Count; i++)
            {
                if (parentSubDir[i] == null) continue;
                VideoFolder child = LoadDirInfo(parentdir, parentSubDir[i]);
                 var originalcopy = MovieDataSource.GetExistingCopy(child);
                if (originalcopy != null)
                {
                    child = originalcopy;
                    child.SetParentDirectory(parentdir);
                }

                existingchildren.Add(child);
            }
            if (parentSubDir.Count > 0)
                parentdir.HasSubFolders = true;
        }
        
        public VideoFolder LoadParentFiles(VideoFolder ParentDir, SortType sorttype)
        {
            if (ParentDir.HasCompleteLoad == true) return ParentDir;

            ObservableCollection<VideoFolder> children;
            List<DirectoryInfo> ParentSubDir = 
                filecommonhelper.GetParentSubDirectory(ParentDir.Directory, applicationService.Formats);

            if (ParentSubDir == null)
            {
                return new VideoFolder(ParentDir, "");
            }

            ParentDir.MediaFileWatcher = new MediaFileWatcher(ParentDir);

            children = new ObservableCollection<VideoFolder>();
            children = LoadChildrenFiles(ParentDir);

            LoadParentSubDirectories(ParentSubDir,  children, ParentDir);
            
            if (ParentDir.OtherFiles == null || children.Count > ParentDir.OtherFiles.Count)
            {
                ParentDir.OtherFiles = new ObservableCollection<VideoFolder>();
                ParentDir.AddRange(children);
                GetRootDetails(sorttype, ref ParentDir);
            }
            
            ParentDir.OnFileNameChangedChanged += ParentDir_OnFileNameChangedChanged;
            ParentDir.HasCompleteLoad = true;
            return ParentDir;

        }

        private void ParentDir_OnFileNameChangedChanged(string oldname, VideoFolder videoItem)
        {
            if(MovieDataSource.DataSource != null && MovieDataSource.DataSource.ContainsKey(oldname))
            {
                MovieDataSource.DataSource.Remove(oldname);
                MovieDataSource.DataSource.Add(videoItem.FullName, videoItem);
            }
        }

        private static VideoFolder LoadDirInfo(VideoFolder parent, DirectoryInfo directoryInfo)
        {
            return new VideoFolder(parent, directoryInfo.FullName); ;
        }

        public VideoFolder SortList(SortType sorttype, VideoFolder parent)
        {
            return GetSortService.SortList(sorttype, parent);  
        }

        public ObservableCollection<VideoFolder> LoadChildrenFiles(VideoFolder Parentdir, bool newpath = false)
        {
            ObservableCollection<VideoFolder> Toparent = new ObservableCollection<VideoFolder>();
            List<Task> Tasks = new List<Task>();
            List<FileInfo> files = 
                filecommonhelper.GetFilesByExtensions(Parentdir.Directory,applicationService.Formats);

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] == null) continue;
                VideoFolderChild vd = CreateVideoFolderChild(Parentdir, files[i]);
                if (vd != null)
                    Toparent.Add(vd);
            }
            return Toparent;
        }
        
        private void RunShellFunction(VideoFolderChild vd)
        {
            string prop = null;
            prop = GetMediaTitle(vd);
            if (!string.IsNullOrEmpty(prop) && !prop.Contains("\""))
                vd.MediaTitle = prop;
        }

        private string GetMediaTitle(VideoFolderChild vd)
        {
            ShellObject shell = ShellObject.FromParsingName(vd.FilePath);
            try
            {
                var duration = shell.Properties.System.Media.Duration;
                vd.Duration = duration.FormatForDisplay(PropertyDescriptionFormat.ShortTime);
            }
            catch (Exception ex)
            {

            }

            return shell.Properties.System.Title.Value;
        }

        public static void GetShellInfo(VideoFolderChild vd)
        {
            GetMediaInfo(vd);
        }

        private static void GetMediaInfo(VideoFolderChild vd)
        {
            ShellObject shell = ShellObject.FromParsingName(vd.FilePath);
            try
            {
                vd.Thumbnail = shell.Thumbnail.LargeBitmapSource;
                if (string.IsNullOrEmpty(vd.Duration) || vd.Duration == ApplicationDummyMessage.DurationNotYetLoaded)
                {
                    var duration = shell.Properties.System.Media.Duration;
                    vd.Duration = duration.FormatForDisplay(PropertyDescriptionFormat.ShortTime);
                }
            }
            catch (Exception ex)
            {
            }
            var prop = shell.Properties.System.Title.Value;
            if (!string.IsNullOrEmpty(prop) && !prop.Contains("\""))
                vd.MediaTitle = prop;
            vd.RefreshFileInfo();
        }

        public ObservableCollection<VideoFolder> LoadChildrenFiles(VideoFolder Parentdir, IList<FileInfo> files, 
            bool newpath = false)
        {
            ObservableCollection<VideoFolder> Toparent = new ObservableCollection<VideoFolder>();
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] == null) continue;
                VideoFolderChild vd = CreateVideoFolderChild(Parentdir, files[i]);
                if(vd != null)
                    Toparent.Add(vd);
            }
            return Toparent;
        }

        public VideoFolderChild LoadChildrenFiles(DirectoryInfo directoryInfo, bool newpath = false)
        {
            VideoFolder vf = null;
            MovieDataSource.DataSource.TryGetValue(directoryInfo.FullName, out vf);
            if (vf == null)
            {
                VideoFolder Parentdir = new VideoFolder(directoryInfo.Parent.FullName);
                FileInfo fileInfo = new FileInfo(directoryInfo.FullName);
                VideoFolderChild vfc = CreateVideoFolderChild(Parentdir, fileInfo);
                #region Search For Subtitle
                //IEnumerable<FileInfo> files = null;
                //files = filecommonhelper.GetSubtitleFiles(directoryInfo.Parent);
                //vfc.SubPath = filecommonhelper.MatchSubToMedia(vfc.Name, files);

                //RunShellFunction(vfc); 
                #endregion
                return vfc;
            }
           
            return (VideoFolderChild)vf;
        }

        public VideoFolderChild CreateVideoFolderChild(IFolder Parentdir,FileInfo fileInfo)
        {
            VideoFolderChild vfc = new VideoFolderChild(Parentdir, fileInfo)
            {
                FileSize = filecommonhelper.FileSizeConverter(fileInfo.Length)
            };
            vfc.OnFileNameChangedChanged += ParentDir_OnFileNameChangedChanged;
            SetLastSeen(vfc);
            return vfc;
        }

        public void SetLastSeen(VideoFolderChild videoFolderChild)
        {
            PlayedFiles pdf = applicationService.SavedLastSeenCollection.GetLastSeen(videoFolderChild.FileInfo.Name) as PlayedFiles;
            if (pdf != null)
            {
                videoFolderChild.LastPlayedPoisition = pdf;
            }
            else
            {
                videoFolderChild.LastPlayedPoisition = new PlayedFiles(videoFolderChild.FileInfo.Name);
            }
        }

        public void GetRootDetails(SortType sorttype, ref VideoFolder ParentDir)
        {
            ParentDir = SortList(sorttype, ParentDir);
        }

        public VideoFolder LoadParentFiles(VideoFolder Parentdir,IList<DirectoryInfo> SubDirectory, SortType sorttype)
        {
            VideoFolder videoFolder = new VideoFolder(Parentdir, SubDirectory[0].Parent.FullName);
            var children = new ObservableCollection<VideoFolder>();
            LoadParentSubDirectories(SubDirectory, children, videoFolder);
            videoFolder.OtherFiles = children;
            GetFolderItems(videoFolder);
            MovieDataSource.InitFileLoading();
            return videoFolder;
        }

        public VideoFolder LoadParentFiles(VideoFolder Parentdir,IList<DirectoryInfo> SubDirectory, IList<FileInfo> SubFiles, SortType sorttype)
        {
            VideoFolder videoFolder = new VideoFolder(Parentdir, SubDirectory[0].Parent.FullName);
            var children = new ObservableCollection<VideoFolder>();
            children = LoadChildrenFiles(videoFolder,SubFiles);
            LoadParentSubDirectories(SubDirectory,  children, videoFolder);
            videoFolder.OtherFiles = children;
            GetFolderItems(videoFolder);
            MovieDataSource.InitFileLoading();

            return videoFolder;
        }

        public VideoFolder LoadParentFiles(VideoFolder Parentdir,IList<FileInfo> SubFiles, SortType sorttype)
        {
            VideoFolder videoFolder = new VideoFolder(Parentdir, SubFiles[0].Directory.FullName);
            var children = new ObservableCollection<VideoFolder>();
            children = LoadChildrenFiles(videoFolder, SubFiles);
            videoFolder.OtherFiles= children;
            videoFolder.HasSubFolders = false;
            GetFolderItems(videoFolder);
            MovieDataSource.InitFileLoading();

            return videoFolder;
        }

        public IDictionary<string, VideoFolder> GetAllFiles(ObservableCollection<VideoFolder> itemsSource)
        {
            IDictionary<string, VideoFolder> allfile = new Dictionary<string, VideoFolder>();
            object padlock = new object();
            if (itemsSource != null)
            {
                for (int i = 0; i < itemsSource.Count; i++)
                {
                    VideoFolder item = itemsSource[i];
                    if (item == null) continue;

                    if (item.FileType == GroupCatergory.Grouped && item.ParentDirectory != null)
                        continue;

                    if (item.FileType == GroupCatergory.Grouped)
                    {
                        var statusMessage = StatusMessageManager.CreateMessage(item.Name + " Loading");

                        foreach (var subitem in GetAllFiles(item.OtherFiles, item))
                        {
                            lock (padlock)
                            {
                                if (allfile.ContainsKey(subitem.Key)) continue;
                                allfile.Add(subitem);
                            }
                        }
                        if (!allfile.ContainsKey(item.FullName))
                            allfile.Add(item.FullName, item);
                        statusMessage.AutomateMessageDestroy(5000);
                    }
                    else
                        allfile.Add(item.FullName, item);
                }
            }
            return allfile;
        }

        public IDictionary<string, VideoFolder> GetAllFiles(VideoFolder videoFolder)
        {
            IDictionary<string, VideoFolder> allfile = new Dictionary<string, VideoFolder>();
            VideoFolder item = videoFolder;
            if (item == null) return allfile;

            if (item.FileType == GroupCatergory.Grouped && item.ParentDirectory != null)
                return allfile;

            if (item.FileType == GroupCatergory.Grouped)
            {
                var statusMessage = StatusMessageManager.CreateMessage(item.Name + " Loading");

                foreach (var subitem in GetAllFiles(item.OtherFiles, item))
                {
                    if (allfile.ContainsKey(subitem.Key)) continue;
                    allfile.Add(subitem);
                }
                if (!allfile.ContainsKey(item.FullName))
                    allfile.Add(item.FullName, item);

                statusMessage.AutomateMessageDestroy(5000);
            }
            else
                allfile.Add(item.FullName, item);

            return allfile;
        }

        private IDictionary<string, VideoFolder> GetAllFiles(IList<VideoFolder> itemsSource, VideoFolder videoFolder = null)
        {
            IDictionary<string, VideoFolder> allfile = new Dictionary<string, VideoFolder>();
            if (itemsSource == null)
            {
                videoFolder = GetFolderItems(videoFolder);
                itemsSource = videoFolder.OtherFiles;
            }

            if (itemsSource != null)
            {
                for (int i = 0; i < itemsSource.Count; i++)
                {
                    VideoFolder item = itemsSource[i];
                    if (item == null) continue;
                    if (item.FileType == GroupCatergory.Grouped)
                    {
                        IDictionary<string, VideoFolder> items = GetAllFiles(item.OtherFiles, item);
                        foreach (var subitem in items)
                        {
                            if (allfile.ContainsKey(subitem.Key)) continue;
                            allfile.Add(subitem);
                        }
                        if (!allfile.ContainsKey(item.FullName))
                            allfile.Add(item.FullName, item);
                    }
                    else
                    {
                        if (!allfile.ContainsKey(item.FullName))
                            allfile.Add(item.FullName, item);
                    }
                }
            }
            return allfile;
        }

        public void InitGetAllFiles(ObservableCollection<VideoFolder> itemsSource)
        {
            lock (padlock)
            {
                this.MovieDataSource.DataSource = GetAllFiles(itemsSource);
            }
        }

        public VideoFolder InitGetAllFiles(VideoFolder videoFolder)
        {
            var data = GetAllFiles(videoFolder);
            foreach (var item in data)
            {
                if (!this.MovieDataSource.DataSource.ContainsKey(item.Key))
                {
                    lock (padlock)
                    {
                        this.MovieDataSource.DataSource.Add(item);
                    }
                }
            }
            return videoFolder;
        }

        public void RemoveFromDataSource(VideoFolder existingVideoFolder)
        {
            if (existingVideoFolder == null) return;
            switch (existingVideoFolder.FileType)
            {
                case GroupCatergory.Grouped:
                    MovieDataSource.InitFileLoading();

                    break;
                case GroupCatergory.Child:
                    if (MovieDataSource.DataSource.ContainsKey(existingVideoFolder.FullName))
                        MovieDataSource.DataSource.Remove(existingVideoFolder.FullName);
                    break;
                default:
                    break;
            }
            
        }
        
    }
}
