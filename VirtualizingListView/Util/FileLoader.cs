using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VideoComponent.Interfaces;
using VirtualizingListView.Pages.ViewModel;

namespace VirtualizingListView.Util
{
    public class FileLoader :Control,  IFileLoader
    {
        static Dispatcher FileLoaderDispatcher
        {
            get
            {
                return FileLoaderInstance.Dispatcher;
            }
        }

        Threading.Executor Executor = new Threading.ExecutorImpl();
        private static object padlock = new object();
        private static FileLoader fileloaderinstance;
        private List<Task> ChildrenTasks = new List<Task>();
        public static FileLoader FileLoaderInstance
        {
            get
            {
                if (fileloaderinstance == null)
                {
                    lock (padlock)
                    {
                        fileloaderinstance = new FileLoader();
                    }
                }
                return fileloaderinstance;
            }
        }

        private  IDictionary<string, VideoFolder> data;
        public  IDictionary<string, VideoFolder> DataSource
        {
            get {
                return data;
            }
            set { data = value; }
        }

        public  bool HasDataSource
        {
            get { return data != null && data.Count > 0; }
        }
       
        internal  VideoFolder GetExistingVideoFolderIfAny(VideoFolder videoFolder)
        {
            VideoFolder folder = null;
            if(data != null)
                data.TryGetValue(videoFolder.FullName,out folder);
            return folder;
        }

        public VideoFolder GetFolderItems(VideoFolder item)
        {
            item.IsLoading = true;
            List<Task> Tasks = new List<Task>();
            object padlock = new object();
            lock (item)
            {
                int taskcount = 0;
                if (item.OtherFiles == null)
                {
                    var s = LoadParentFiles(item, item.SortedBy);
                    if (s.OtherFiles == null)
                        return s;
                }
                try
                {
                    Parallel.ForEach(item.OtherFiles, (s) =>
                    {

                        var temp = s;
                        if (temp.FileType == FileType.Folder)
                        {
                            var task = Task.Factory.StartNew(() =>
                            {
                                temp.IsLoading = true;
                                GetFolderItemsExtended(temp);
                                temp.IsLoading = false;
                            });
                            taskcount++;
                            AddTask(Tasks, task, padlock);
                        }
                        else
                        {
                            var task = Task.Factory.StartNew(() =>
                            {
                                RunShellFunction(temp as VideoFolderChild);
                            });
                            taskcount++;
                            AddTask(Tasks, task, padlock);
                        }
                        if (taskcount % 5 == 0 && taskcount != 0)
                        {
                            Task.WaitAll(Tasks.ToArray());
                            taskcount = 0;
                            Tasks.Clear();
                        }
                    });
                }
                catch (Exception ex)
                {
                }
            }
            item.IsLoading = false;
            return item;
        }

        private void AddTask(List<Task> tasks, Task task, object padlock)
        {
            lock (padlock)
            {
                tasks.Add(task);
            }
        }

        internal  VideoFolder DeepCopy(VideoFolder existing,VideoFolder videoFoldercopy)
        {
            var newcopy = new VideoFolder(videoFoldercopy, videoFoldercopy.FullName)
            {
                OtherFiles = videoFoldercopy.OtherFiles,
                HasCompleteLoad = videoFoldercopy.HasCompleteLoad,
                SortedBy = videoFoldercopy.SortedBy
            };
            return newcopy;
        }

        private  VideoFolder GetFolderItemsExtended(VideoFolder vfile)
        {
            List<Task> Tasks = new List<Task>();
            var s = LoadParentFiles(vfile, vfile.SortedBy);
            if (s.OtherFiles == null )
                return s;
            int taskcount = 0;
            for (int i = 0; i < s.OtherFiles.Count; i++)
            {
                try
                {
                    var temp = s.OtherFiles[i];
                    if (temp.FileType == FileType.Folder)
                    {
                        temp.IsLoading = true;
                        GetSubFolderItems(temp);
                        temp.IsLoading = false;
                    }
                    //else
                    //{
                    //    taskcount++;
                    //    var task = Task.Factory.StartNew(() =>
                    //    {
                    //        RunShellFunction(temp as VideoFolderChild);
                    //    });
                    //    Tasks.Add(task);
                    //}
                    //if (taskcount % 3 == 0)
                    //{
                    //    Task.WaitAll(Tasks.ToArray());
                    //    taskcount = 0;
                    //    Tasks.Clear();
                    //}
                }
                catch (Exception ex)
                {
                }
            }
            return s;
        }

        private VideoFolder GetSubFolderItems(VideoFolder videoFolder)
        {
            List<Task> Tasks = new List<Task>();
            Console.WriteLine("------Starting to Load {0} item-----",videoFolder);

            var s = LoadParentFiles(videoFolder, videoFolder.SortedBy);
            if (s.OtherFiles == null)
                return s;
            int taskcount = 0;

            Parallel.ForEach(s.OtherFiles, (k) =>
            {
                try
                {
                    var temp = k;
                    if (temp.FileType == FileType.Folder)
                    {
                        temp.IsLoading = true;
                        GetSubFolderItems(temp);
                        temp.IsLoading = false;
                    }
                    //else
                    //{
                    //    taskcount++;
                    //    var task = Task.Factory.StartNew(() =>
                    //    {
                    //        RunShellFunction(temp as VideoFolderChild);
                    //    }).ContinueWith(t => { }, TaskScheduler.Default);
                    //    Tasks.Add(task);
                    //}
                    //if (taskcount % 3 == 0)
                    //{
                    //    Task.WaitAll(Tasks.ToArray());
                    //    taskcount = 0;
                    //    Tasks.Clear();
                    //}
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
                var originalcopy = HomePageLocalViewModel.GetExistingCopy(child);
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
                FileExplorerCommonHelper.GetParentSubDirectory(ParentDir.Directory, ApplicationService.formats);

            if (ParentSubDir == null)
            {
                return new VideoFolder(ParentDir, "");
            }

            children = new ObservableCollection<VideoFolder>();
            children = LoadChildrenFiles(ParentDir);

            LoadParentSubDirectories(ParentSubDir,  children, ParentDir);
            
            if (ParentDir.OtherFiles == null || children.Count > ParentDir.OtherFiles.Count)
            {
                ParentDir.OtherFiles = new ObservableCollection<VideoFolder>();
                ParentDir.OtherFiles.AddRange(children);
                GetRootDetails(sorttype, ref ParentDir);
            }
            
            ParentDir.OnFileNameChangedChanged += ParentDir_OnFileNameChangedChanged;
            ParentDir.HasCompleteLoad = true;
            return ParentDir;

        }

        private void ParentDir_OnFileNameChangedChanged(string oldname, VideoFolder videoItem)
        {
            if(data != null && data.ContainsKey(oldname))
            {
                data.Remove(oldname);
                data.Add(videoItem.FullName, videoItem);
            }
        }

        private static VideoFolder LoadDirInfo(IFolder parent, DirectoryInfo directoryInfo)
        {
            VideoFolder vd = new VideoFolder(parent,directoryInfo.FullName)
            {
                FileType = FileType.Folder
            };
            return vd;
        }

        public VideoFolder SortList(SortType sorttype, VideoFolder parent)
        {
            if (parent.OtherFiles == null) return parent;

            ObservableCollection<VideoFolder> asd = new ObservableCollection<VideoFolder>();
            if (sorttype == SortType.Date)
            {
                IEnumerable<VideoFolder> de = (parent.OtherFiles).OrderBy(x => x, new SortByDate());
                asd.AddRange(de);
            }
            else if (sorttype == SortType.Extension)
            {
                IEnumerable<VideoFolder> de = parent.OtherFiles.OrderBy(x => x, new SortByExtension());
                asd.AddRange(de);
            }
            else
            {
                IEnumerable<VideoFolder> de = parent.OtherFiles.OrderBy(x => x, new SortByNames());
               asd.AddRange(de);
            }
            parent.OtherFiles.Clear();
            parent.OtherFiles = asd;
            parent.SortedBy = sorttype;
            return parent;
        }

        public ObservableCollection<VideoFolder> LoadChildrenFiles(IFolder Parentdir, bool newpath = false)
        {
            ObservableCollection<VideoFolder> Toparent = new ObservableCollection<VideoFolder>();
            List<Task> Tasks = new List<Task>();
            List<FileInfo> files = 
                FileExplorerCommonHelper.GetFilesByExtensions(Parentdir.Directory,ApplicationService.formats);

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] == null) continue;
                VideoFolderChild vd;
                PlayedFiles pdf = ApplicationService.SavedLastSeenCollection.GetLastSeen(files[i].Name);
                vd = new VideoFolderChild(Parentdir, files[i])
                {
                    FileSize = FileExplorerCommonHelper.FileSizeConverter(files[i].Length),
                    FileType = FileType.File
                };
                if (pdf != null)
                {
                    vd.LastPlayedPoisition = pdf;
                }
                else
                {
                    vd.LastPlayedPoisition = new PlayedFiles(files[i].Name);
                }
                vd.OnFileNameChangedChanged += ParentDir_OnFileNameChangedChanged;
                if (vd != null)
                    Toparent.Add(vd);
            }
            return Toparent;
        }
        

        private void RunShellFunction( VideoFolderChild vd)
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
                //if (duration.Value != null)
                //    vd.MaxiProgress = (int)(duration.Value / Math.Pow(10, 7));
            }
            catch (Exception ex)
            {

            }

            //MediaInfo.MediaInfoWrapper mediaInfoWrapper = new MediaInfo.MediaInfoWrapper(vd.FullName);
            //string result = null;
            //if (mediaInfoWrapper.HasVideo)
            //{
            //    //Dispatcher.Invoke(new Action(() => {
            //    // mediaInfoWrapper.Open(vd.FullName);
            //    //result = mediaInfoWrapper.Get(MediaInfo.StreamKind.General, 0, 167);
            //    result = (mediaInfoWrapper.Tags as AudioTags).Title;
            //    vd.MaxiProgress = mediaInfoWrapper.Duration;
            //    vd.Duration = mediaInfoWrapper.BestVideoStream.Duration.ToString();
            //}
            //mediaInfoWrapper.Close();
            //}),DispatcherPriority.Background);

            return shell.Properties.System.Title.Value;
        }

        public static void GetShellInfo(VideoFolderChild vd)
        {
            GetMediaInfo(vd);
            //GetMediaInfo(vd);
        }

        private static void GetMediaInfo(VideoFolderChild vd)
        {
            ShellObject shell = ShellObject.FromParsingName(vd.FilePath);
            try
            {
                vd.Thumbnail = shell.Thumbnail.LargeBitmapSource;
                if (string.IsNullOrEmpty(vd.Duration)){
                    var duration = shell.Properties.System.Media.Duration;
                    vd.Duration = duration.FormatForDisplay(PropertyDescriptionFormat.ShortTime);
                }
                //if (duration.Value != null)
                //    vd.MaxiProgress = (int)(duration.Value / Math.Pow(10, 7));
            }
            catch (Exception ex)
            {
            }
            var prop = shell.Properties.System.Title.Value;
            if (!string.IsNullOrEmpty(prop) && !prop.Contains("\""))
                vd.MediaTitle = prop;
        }
        public ObservableCollection<VideoFolder> LoadChildrenFiles(IFolder Parentdir, IList<FileInfo> files, 
            bool newpath = false)
        {
            ObservableCollection<VideoFolder> Toparent = new ObservableCollection<VideoFolder>();
            for (int i = 0; i < files.Count; i++)
            {
                VideoFolderChild vd;
                PlayedFiles pdf = ApplicationService.SavedLastSeenCollection.GetLastSeen(files[i].Name);
                vd = new VideoFolderChild(Parentdir, files[i])
                {
                    FileSize = FileExplorerCommonHelper.FileSizeConverter(files[i].Length),
                    FileType = FileType.File

                };
                if (pdf != null)
                {
                    vd.LastPlayedPoisition = pdf;
                }
                else
                {
                    vd.LastPlayedPoisition = new PlayedFiles(files[i].Name);
                }
                vd.OnFileNameChangedChanged += ParentDir_OnFileNameChangedChanged;
                if(vd != null)
                    Toparent.Add(vd);
            }
            return Toparent;
        }

        public VideoFolder LoadChildrenFiles(DirectoryInfo directoryInfo, bool newpath = false)
        {
            VideoFolder vf = null;
            data.TryGetValue(directoryInfo.FullName, out vf);
            if (vf == null)
            {
                IFolder Parentdir = new VideoFolder(directoryInfo.Parent.FullName);
                FileInfo fileInfo = new FileInfo(directoryInfo.FullName);
                PlayedFiles pdf = ApplicationService.SavedLastSeenCollection.GetLastSeen(fileInfo.Name);
                VideoFolderChild vfc = new VideoFolderChild(Parentdir, fileInfo)
                {
                    FileSize = FileExplorerCommonHelper.FileSizeConverter(fileInfo.Length),
                    FileType = FileType.File
                };
                if (pdf != null)
                {
                    vfc.LastPlayedPoisition = pdf;
                }
                else
                {
                    vfc.LastPlayedPoisition = new PlayedFiles(fileInfo.Name);
                }
                IEnumerable<FileInfo> files = null;
                files = FileExplorerCommonHelper.GetSubtitleFiles(directoryInfo.Parent);
                vfc.SubPath = FileExplorerCommonHelper.MatchSubToMedia(vfc.Name, files);

                RunShellFunction(vfc);
                return vfc;
            }
           
            return vf;
        }

        public void GetRootDetails(SortType sorttype, ref VideoFolder ParentDir)
        {
            ParentDir.FileType = FileType.Folder;
            ParentDir = SortList(sorttype, ParentDir);
        }

        public VideoFolder LoadParentFiles(IFolder Parentdir,IList<DirectoryInfo> SubDirectory, SortType sorttype)
        {
            VideoFolder videoFolder = new VideoFolder(Parentdir, SubDirectory[0].Parent.FullName);
            var children = new ObservableCollection<VideoFolder>();
            LoadParentSubDirectories(SubDirectory, children, videoFolder);
            videoFolder.OtherFiles = children;
            GetFolderItems(videoFolder);
            HomePageLocalViewModel.FileLoaderDelegate.Invoke();
            return videoFolder;
        }

        public VideoFolder LoadParentFiles(IFolder Parentdir,IList<DirectoryInfo> SubDirectory, IList<FileInfo> SubFiles, SortType sorttype)
        {
            VideoFolder videoFolder = new VideoFolder(Parentdir, SubDirectory[0].Parent.FullName);
            var children = new ObservableCollection<VideoFolder>();
            children = LoadChildrenFiles(videoFolder,SubFiles);
            LoadParentSubDirectories(SubDirectory,  children, videoFolder);
            videoFolder.OtherFiles = children;
            GetFolderItems(videoFolder);
            HomePageLocalViewModel.FileLoaderDelegate.Invoke();
            return videoFolder;
        }

        public VideoFolder LoadParentFiles(IFolder Parentdir,IList<FileInfo> SubFiles, SortType sorttype)
        {
            VideoFolder videoFolder = new VideoFolder(Parentdir, SubFiles[0].Directory.FullName);

            var children = new ObservableCollection<VideoFolder>();
            children = LoadChildrenFiles(videoFolder, SubFiles);
            videoFolder.OtherFiles= children;
            videoFolder.HasSubFolders = false;
            GetFolderItems(videoFolder);
            HomePageLocalViewModel.FileLoaderDelegate.Invoke();
            return videoFolder;
        }

        public IDictionary<string, VideoFolder> GetAllFiles(ObservableCollection<VideoFolder> itemsSource)
        {
            IDictionary<string, VideoFolder> allfile = new Dictionary<string, VideoFolder>();
            object padlock = new object();
            List<Task> Tasks = new List<Task>();
            if (itemsSource != null)
            {
                for (int i = 0; i < itemsSource.Count; i++)
                {
                    VideoFolder item = itemsSource[i];
                    if (item.FileType == FileType.Folder && item.ParentDirectory != null)
                        continue;

                    if (item.FileType == FileType.Folder)
                    {
                        var task = Task.Factory.StartNew(() =>
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            Console.WriteLine(item.Name + " started Loading");
                            stopwatch.Start();
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
                            stopwatch.Stop();
                            Console.WriteLine(item.Name + " Loaded in {0} secs", stopwatch.ElapsedMilliseconds * 1.0 / 1000);
                        }).ContinueWith(t => { }, TaskScheduler.Current);
                        Tasks.Add(task);
                    }
                    else
                        allfile.Add(item.FullName, item);

                    if (Tasks.Count % 2 == 0 && Tasks.Count != 0)
                    {
                        Task.WaitAll(Tasks.ToArray());
                        Tasks.Clear();
                    }
                }
            }
            Task.WaitAll(Tasks.ToArray());
            return allfile;
        }

        private IDictionary<string, VideoFolder> GetAllFiles(IList<VideoFolder> itemsSource,VideoFolder videoFolder = null)
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
                    if (item.FileType == FileType.Folder)
                    {
                        IDictionary<string, VideoFolder> items = GetAllFiles(item.OtherFiles, item);
                        foreach (var subitem in items)
                        {
                            allfile.Add(subitem);
                        }
                        allfile.Add(item.FullName, item);
                    }
                    else
                        allfile.Add(item.FullName, item);
                }
            }
            return allfile;
        }

        public void InitGetAllFiles(ObservableCollection<VideoFolder> itemsSource)
        {
            lock (this)
            {
                this.DataSource = GetAllFiles(itemsSource);
            }
        }

        public void RemoveFromDataSource(VideoFolder existingVideoFolder)
        {
            if (existingVideoFolder == null) return;
            switch (existingVideoFolder.FileType)
            {
                case FileType.Folder:
                    HomePageLocalViewModel.FileLoaderDelegate.Invoke();
                    break;
                case FileType.File:
                    if (data.ContainsKey(existingVideoFolder.FullName))
                        data.Remove(existingVideoFolder.FullName);
                    break;
                default:
                    break;
            }
            
        }

        public ObservableCollection<VideoFolder> SortList(SortType sorttype, ObservableCollection<VideoFolder> list)
        {
            if (list == null) return list;

            ObservableCollection<VideoFolder> asd = new ObservableCollection<VideoFolder>();
            if (sorttype == SortType.Date)
            {
                IEnumerable<VideoFolder> de = (list).OrderBy(x => x, new SortByDate());
                asd.AddRange(de);
            }
            else if (sorttype == SortType.Extension)
            {
                IEnumerable<VideoFolder> de = list.OrderBy(x => x, new SortByExtension());
                asd.AddRange(de);
            }
            else
            {
                IEnumerable<VideoFolder> de = list.OrderBy(x => x, new SortByNames());
                asd.AddRange(de);
            }
            return asd;
        }
    }
}
