using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using VideoComponent.BaseClass;

namespace VirtualizingListView.Util
{
    public class FileLoader : Control
    {
        public static double progresslevel;
        
        public static string[] formats = { ".wmv", ".3g2", ".3gp" ,".3gp2", ".3gpp", ".amv", ".asf",  
                 ".avi", ".cue", ".divx", ".dv", ".flv",".gxf",".m1v",
                 ".m2v",".m2t" ,".m2ts",".m4v" ,
                 ".mkv", ".mov", ".mp2", ".mp2v", ".mp4", ".mp4v", ".mpa", ".mpe", ".mpeg",
                 ".mpeg1", ".mpeg2",".mpeg4",".mpg", ".mpv2" ,".mts",".nsv", ".nuv", ".ogg", 
                 ".ogm", ".ogv", ".ogx",".ps", ".rec",".rm", ".rmvb", ".tod", ".ts", ".tts" ,
                 ".vob" ,".vro",".webm"
              };
        public static string[] subtitleformats = 
        { 
          ".aqt ",".vtt",".cvd",".dks" ,".jss",".sub" ,".ttxt" ,".mpl" ,".txt" ,".pjs" ,".psb" 
          ,".rt",".smi" ,".ssf" ,".srt" ,".ssa" ,".svcd",".usf" ,".idx"
        };

        public static VideoFolder LoadParentFiles(VideoFolder ParentDir, SortType sorttype, ICollectionViewModel collectionVM)
        {
            // Dispatcher.Invoke(new Action(delegate {
            ObservableCollection<VideoFolder> children;
            List<DirectoryInfo> ParentSubDir = FileExplorerCommonHelper.GetParentSubDirectory(ParentDir.Directory, formats);
            if (ParentSubDir == null) { collectionVM.IsLoading = false; return new VideoFolder(ParentDir,ParentDir.Directory.Extension); }
            children = new ObservableCollection<VideoFolder>();
            collectionVM.IsLoading = true;
            children = LoadChildrenFiles(ParentDir);
            for (int i = 0; i < ParentSubDir.Count; i++)
            {
                if (ParentSubDir[i].Name == ".movies")
                {
                    ParentSubDir.Remove(ParentSubDir[i]);
                    i -= 1;
                    continue;
                }
                VideoFolder child = LoadDirInfo(ParentDir, ParentSubDir[i]);
                children.Add(child);
            }
            if (ParentDir.OtherFiles == null || children.Count > ParentDir.OtherFiles.Count)
            {
                ParentDir.OtherFiles = new ObservableCollection<VideoFolder>();
                ParentDir.OtherFiles.AddRange(children);
                GetRootDetails(sorttype, ParentDir);
            }
            //   }));
            collectionVM.IsLoading = false;
            return ParentDir;

        }

        private static VideoFolder LoadDirInfo(IFolder parent, DirectoryInfo directoryInfo)
        {
            VideoFolder vd = new VideoFolder(parent, directoryInfo.FullName)
            {
                FileType = FileType.Folder
            };
            return vd;
        }

        public static VideoFolder SortList(SortType sorttype, VideoFolder parent)
        {
            if (parent.OtherFiles == null) return parent;

            ObservableCollection<VideoFolder> asd = new ObservableCollection<VideoFolder>();
            if (sorttype == SortType.Date)
            {
                var de = (parent.OtherFiles).OrderBy(x => x, new SortByDate()).ToList();
                asd.AddRange(de);
               // parent.OtherFiles.Sort(new SortByDate());
            }
            else if (sorttype == SortType.Extension)
            {
                var de = parent.OtherFiles.OrderBy(x => x, new SortByExtension()).ToList();
                //ParentDir.ChildFiles.Sort();

                asd.AddRange(de);
            }
            else
            {
                var de = parent.OtherFiles.OrderBy(x => x, new SortByNames()).ToList();
                //ParentDir.ChildFiles.Sort();

               asd.AddRange(de);
            }
            parent.OtherFiles.Clear();
            parent.OtherFiles = asd;
            parent.SortedBy = sorttype;
            return parent;
        }

        public static ObservableCollection<VideoFolder> LoadChildrenFiles(IFolder Parentdir, bool newpath = false)
        {

            ObservableCollection<VideoFolder> Toparent = new ObservableCollection<VideoFolder>();

            List<FileInfo> files = FileExplorerCommonHelper.GetFilesByExtensions(Parentdir.Directory, formats);
            if (files.Count > 0)
            {
                ApplicationService.CreateLastSeenFolder(Parentdir);
                ApplicationService.LoadLastSeenFile(Parentdir);
            }

            for (int i = 0; i < files.Count; i++)
            {
                VideoFolderChild vd;
                PlayedFiles pdf = (PlayedFiles)LastSeenHelper.GetProgress(Parentdir,files[i].Name);
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
                using (ShellObject shell = ShellObject.FromParsingName(vd.FilePath))
                {
                    IShellProperty prop = shell.Properties.System.Media.Duration;
                    vd.Duration = prop.FormatForDisplay(PropertyDescriptionFormat.ShortTime);
                    var duration = shell.Properties.System.Media.Duration;
                    if (duration.Value != null)
                        vd.MaxiProgress = double.Parse(duration.Value.ToString());
                }

                Toparent.Add(vd);
            }
            return Toparent;
        }

        public static VideoFolder LoadChildrenFiles(DirectoryInfo directoryInfo, bool newpath = false)
        {
            IFolder Parentdir = new VideoFolder(directoryInfo.Parent.FullName);

            ApplicationService.CreateLastSeenFolder(Parentdir);
            ApplicationService.LoadLastSeenFile(Parentdir);
            FileInfo fileInfo = new FileInfo(directoryInfo.FullName);
            VideoFolderChild vd;
            PlayedFiles pdf = (PlayedFiles)LastSeenHelper.GetProgress(Parentdir, fileInfo.Name);
            vd = new VideoFolderChild(Parentdir, fileInfo)
            {
                FileSize = FileExplorerCommonHelper.FileSizeConverter(fileInfo.Length),
                FileType = FileType.File
            };
            if (pdf != null)
            {
                vd.LastPlayedPoisition = pdf;
            }
            else
            {
                vd.LastPlayedPoisition = new PlayedFiles(fileInfo.Name);
            }
            IEnumerable<FileInfo> files = null;
            using (ShellObject shell = ShellObject.FromParsingName(vd.FilePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                vd.Duration = prop.FormatForDisplay(PropertyDescriptionFormat.ShortTime);
                var duration = shell.Properties.System.Media.Duration;
                if (duration.Value != null)
                    vd.MaxiProgress = double.Parse(duration.Value.ToString());

                files = FileExplorerCommonHelper.GetSubtitleFiles(directoryInfo.Parent);
            }
            vd.SubPath = FileExplorerCommonHelper.MatchSubToMedia(vd.Name, files);
            return vd;
        }

        public static void GetRootDetails(SortType sorttype, VideoFolder ParentDir)
        {
            ParentDir.FileType = FileType.Folder;
            ParentDir = SortList(sorttype, ParentDir);
           // return ParentDir;
        }

        public static VideoFolder LoadParent2(DirectoryInfo DirectoryPosition, SortType sorttype)
        {
            VideoFolder ParentDir = null;
            List<VideoFolder> children = new List<VideoFolder>();
            List<DirectoryInfo> ParentSubDir = FileExplorerCommonHelper.GetParentSubDirectory(DirectoryPosition, FileLoader.formats);

           // Dispatcher.Invoke(new Action(delegate
            //{
            ParentDir = new VideoFolder(DirectoryPosition.FullName);
            foreach (var item in ParentSubDir)
            {
                VideoFolder child = LoadParent2(item, sorttype);
                children.Add(child);
            }
            GetRootDetails(sorttype, ParentDir);
           // }));

            return ParentDir;
        }

        //internal static void RefreshPath(VideoData VideoDataAccess)
        //{
           
        //}
    }
}
