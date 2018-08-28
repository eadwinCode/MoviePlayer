using Common.Util;
using Delimon.Win32.IO;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Movies.Enums;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using VideoComponent.BaseClass;

namespace Movies.MovieServices.Services
{
    public class FileLoaderCompletion : IFileLoaderCompletion
    {
        private IEnumerable<FileInfo> files;
        private IFileLoader fileloader;
        private IFileExplorerCommonHelper filecommonhelper;
        private IDispatcherService dispatcherService;
        IDataSource<VideoFolder> MovieDataSource
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataSource<VideoFolder>>();
            }
        }


        public FileLoaderCompletion(IFileLoader fileLoader, IFileExplorerCommonHelper fileExplorerCommonHelper,IDispatcherService dispatcherService)
        {
            this.fileloader = fileLoader;
            this.filecommonhelper = fileExplorerCommonHelper;
            this.dispatcherService = dispatcherService;
        }

        public void FinishCollectionLoadProcess(IList<VideoFolder> itemsSource, Dispatcher dispatcherUnit = null)
        {
            Parallel.ForEach(itemsSource, (o) =>
            {
                if (!o.HasCompleteLoad)
                {
                    LoadOtherFileDetails(o,dispatcherUnit);
                    SearchSubtitleFile(o);
                }
            });
            //for (int i = 0; i < itemsSource.Count; i++)
            //{
            //    if (itemsSource[i].HasCompleteLoad) continue;
            //    LoadOtherFileDetails(itemsSource[i]);
            //    SearchSubtitleFile(itemsSource[i]);
            //}
        }

        public void FinishCollectionLoadProcess(ObservableCollection<VideoFolder> itemsSource,bool IsMovieFolder)
        {
            dispatcherService.InvokeDispatchAction(new Action(delegate
            {
                for (int i = 0; i < itemsSource.Count();i++)
                {
                    VideoFolder VideoFolder = itemsSource[i];
                    if (VideoFolder.HasCompleteLoad)
                        continue;

                    if (!VideoFolder.HasCompleteLoad && fileloader.HasDataSource)
                    {
                        var videofolder = fileloader.GetExistingVideoFolderIfAny(VideoFolder);
                        if (videofolder != null)
                        {
                            itemsSource[i].SetParentDirectory(videofolder.ParentDirectory);
                            itemsSource.Remove(VideoFolder);
                            itemsSource.Insert(i,videofolder);
                            continue;
                        }
                    }

                    if (VideoFolder.ParentDirectory != null)
                    {
                        VideoFolder videoFoldercopy = null;
                        VideoFolder ExistingParent =
                            MovieDataSource.GetExistingCopy((VideoFolder)VideoFolder.ParentDirectory);
                        if (ExistingParent != null && ExistingParent.OtherFiles != null)
                            videoFoldercopy =
                                ExistingParent.OtherFiles.Where(j => j.Equals(VideoFolder)).FirstOrDefault();

                        if (videoFoldercopy == null)
                            VideoFolder.IsLoading = true;
                        else
                        {
                            itemsSource.Remove(VideoFolder);
                            itemsSource.Insert(i,videoFoldercopy);
                            //MergeSameVideoData(ref videoFolder, videoFoldercopy);
                        }
                    }
                 //SearchSubtitleFile(list[i]);
                }
            }));
        }


        private void MergeSameVideoData(ref VideoFolder videoFolder, VideoFolder videoFoldercopy)
        {
            (fileloader as FileLoader).DeepCopy(videoFolder, videoFoldercopy);
        }

        private void SearchSubtitleFile(VideoFolder item)
        {
            if (item.FileType == FileType.Folder) return;
            if ((item as VideoFolderChild).HasSearchSubtitleFile) return;

            dispatcherService.BeginInvokeDispatchAction(new Action(delegate
            {
                var itemchild = (VideoFolderChild)item;
                if (files == null)
                    files = filecommonhelper.GetSubtitleFiles(item.ParentDirectory.Directory);

                itemchild.SubPath = GetSubtitlePath(item);
            }));
        }

        private VideoFolder GetItems(VideoFolder item)
        {
            var s = fileloader.GetFolderItems(item);
            return s;
        }

        private void LoadOtherFileDetails(VideoFolder item, Dispatcher dispatcherunit)
        {
            if (item.FileType == FileType.Folder)
            {
                if (item.HasCompleteLoad) return;
                var task = Task.Factory.StartNew(() => GetItems(item))
                    .ContinueWith(t => item = t.Result,
                    TaskScheduler.Current);
                task.Wait();
            }
            else
            {
                var itemchild = (VideoFolderChild)item;
                Task task = Task.Factory.StartNew(() => {
                    dispatcherService.BeginInvokeDispatchAction(dispatcherunit,new Action(delegate
                    {
                        //itemchild.IsLoading = true;
                        FileLoader.GetShellInfo(itemchild);
                        
                    }));})
                    .ContinueWith(t => 
                    {
                        //itemchild.IsLoading = false;
                        itemchild.UpdateProperties();
                    }, TaskScheduler.Current);

                task.Wait();
                itemchild.IsLoading = false;
            }
        }

        private void GetThumbnail(VideoFolderChild file)
        {
            ImageSource imageSource = null;
            dispatcherService.InvokeDispatchAction(new Action(delegate
            {
                file.IsLoading = true;
                ShellObject shell = ShellObject.FromParsingName(file.FullName);
                file.Thumbnail = imageSource = shell.Thumbnail.LargeBitmapSource;
                shell.Dispose();
                file.IsLoading = false;

           }));
        }

        private ObservableCollection<SubtitleFilesModel> GetSubtitlePath(VideoFolder item)
        {
            return filecommonhelper.MatchSubToMedia(item.Name, files);
        }
        
    }
}
