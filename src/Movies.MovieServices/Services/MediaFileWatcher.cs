using Delimon.Win32.IO;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Movies.MoviesInterfaces;

namespace Movies.MovieServices.Services
{
    public class MediaFileWatcher : System.IO.FileSystemWatcher, IFileSystemWatcher
    {
        private readonly VideoFolder MediaFolder;
        public MediaFileWatcher(VideoFolder mediafolder)
        {
            this.MediaFolder = mediafolder;
            this.Path = MediaFolder.FullName;
            InitWatch();
        }


        private void InitWatch()
        {
            this.NotifyFilter = 
                         System.IO.NotifyFilters.FileName |
                         System.IO.NotifyFilters.DirectoryName;
            this.Filter = "*.*";
            Changed += new System.IO.FileSystemEventHandler(OnChanged);
            Created += new System.IO.FileSystemEventHandler(OnChanged);
            Deleted += new System.IO.FileSystemEventHandler(OnChanged);
            Renamed += new System.IO.RenamedEventHandler(OnRenamed);
            this.EnableRaisingEvents = true;
            this.IncludeSubdirectories = true;
        }

        public void UnloadWatch()
        {
            Changed -= new System.IO.FileSystemEventHandler(OnChanged);
            Created -= new System.IO.FileSystemEventHandler(OnChanged);
            Deleted -= new System.IO.FileSystemEventHandler(OnChanged);
            Renamed -= new System.IO.RenamedEventHandler(OnRenamed);
            this.EnableRaisingEvents = false;
        }

        private static void OnChanged(object source, System.IO.FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            FileChangeHandler(source,e);
        }

        private static void FileChangeHandler(object sender,System.IO.FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Created:
                    Task.Factory.StartNew(() => CreateFile( sender,e.FullPath)).ContinueWith(t => { RefreshMediaFolder(sender); }, TaskScheduler.Current);
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    DeleteFile(sender, e.FullPath);
                    //Task.Factory.StartNew(() => DeleteFile(ref sender, e.FullPath)).ContinueWith(t => {  }, TaskScheduler.Current);
                    break;
                case System.IO.WatcherChangeTypes.Changed:
                    RefreshMediaFolder(sender);
                    break;
                case System.IO.WatcherChangeTypes.Renamed:
                    break;
                case System.IO.WatcherChangeTypes.All:
                    break;
                default:
                    break;
            }
        }

        private static void RefreshMediaFolder(object sender)
        {
            var Mediafolder = ((MediaFileWatcher)sender).MediaFolder;
            Mediafolder.RefreshFileInfo();
        }

        private static void RenameFile( object sender, System.IO.RenamedEventArgs e)
        {
            DispatcherService.InvokeDispatchAction(new Action(() =>
            {
                var Mediafolder = ((MediaFileWatcher)sender).MediaFolder;
                VideoFolder videoFolder = new VideoFolder(e.OldFullPath);
                for (int i = 0; i < Mediafolder.OtherFiles.Count; i++)
                {
                    var oldfolder = Mediafolder.OtherFiles[i];
                    if(oldfolder.Equals(videoFolder))
                    {
                        oldfolder.RenameFile(e.FullPath);
                        Mediafolder.OtherFiles.Remove(oldfolder);
                        Mediafolder.OtherFiles.Insert(i, oldfolder);
                        break;
                    }
                }

                RefreshMediaFolder(sender);
            }));
        }

        private static void DeleteFile(object sender, string fullPath)
        {
            DispatcherService.InvokeDispatchAction(new Action(() =>
            {
                var Mediafolder = ((MediaFileWatcher)sender).MediaFolder;
                VideoFolder videoFolder = new VideoFolder(fullPath);
                var existingVideoFolder = Mediafolder.OtherFiles.Where(x => x.Equals(videoFolder)).FirstOrDefault();
                if (Mediafolder.OtherFiles.Contains(videoFolder))
                    Mediafolder.OtherFiles.Remove(existingVideoFolder);
                FileLoader.RemoveFromDataSource(existingVideoFolder);
                RefreshMediaFolder(sender);
            }));
        }

        private static void CreateFile( object sender,string path)
        {
            DispatcherService.InvokeDispatchAction(new Action(() =>
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path);
                var Mediafolder = ((MediaFileWatcher)sender).MediaFolder;
                if (directoryInfo.Exists)
                {
                    var videoFolder = CreateDirectory(Mediafolder,new DirectoryInfo(path));
                    if (videoFolder != null)
                    {
                        Mediafolder.OtherFiles.Add(videoFolder);
                        FileLoader.SortList(Mediafolder.SortedBy, Mediafolder);
                        return;
                    }
                }
                FileInfo fileInfo = new FileInfo(path);
                if (ApplicationService.Formats.ContainsKey(fileInfo.Extension))
                {
                    VideoFolderChild videoFolderChild = new VideoFolderChild(Mediafolder, fileInfo)
                    {
                        FileType = FileType.File
                    };
                    Mediafolder.OtherFiles.Add(videoFolderChild);
                    FileLoader.SortList(Mediafolder.SortedBy, Mediafolder);
                }
            }));
        }

        private static VideoFolder CreateDirectory(VideoFolder MediaFolder,DirectoryInfo directoryInfo)
        {
            var subdir = Fileexplorercommonhelper.GetParentSubDirectory(directoryInfo,
                   ApplicationService.Formats);
            var files = Fileexplorercommonhelper.GetFilesByExtensions(directoryInfo,
              ApplicationService.Formats);
            VideoFolder videoFolder = null;

            if (subdir.Count > 0 && files.Count > 0)
                videoFolder = FileLoader.LoadParentFiles(MediaFolder,subdir, files, MediaFolder.SortedBy);
            else if (subdir.Count > 0)
                videoFolder = FileLoader.LoadParentFiles(MediaFolder,subdir, MediaFolder.SortedBy);
            else if (files.Count > 0)
                videoFolder = FileLoader.LoadParentFiles(MediaFolder,files, MediaFolder.SortedBy);

            return videoFolder;
        }

        private static void OnRenamed(object source, System.IO.RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            Task.Factory.StartNew(() => RenameFile(source, e)).ContinueWith(t => { }, TaskScheduler.Current);
        }

        public static IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        public static IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }

        public static IFileExplorerCommonHelper Fileexplorercommonhelper
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileExplorerCommonHelper>();
            }
        }

        public static IDispatcherService DispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }
    }
}
