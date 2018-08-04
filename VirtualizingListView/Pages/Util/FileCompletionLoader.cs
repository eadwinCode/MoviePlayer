using Common.FileHelper;
using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.ViewModel;
using VirtualizingListView.Threading.Interface;
using VirtualizingListView.Util;

namespace VirtualizingListView.Pages.Util
{
    public class FileLoaderCompletion:Control 
    {
        private IEnumerable<FileInfo> files;

        public void FinishCollectionLoadProcess(IList<VideoFolder> itemsSource)
        {
            Parallel.ForEach(itemsSource, (o) =>
            {
                if (!o.HasCompleteLoad)
                {
                    LoadOtherFileDetails(o);
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
            Dispatcher.Invoke(new Action(delegate
            {
                for (int i = 0; i < itemsSource.Count();i++)
                {
                    VideoFolder VideoFolder = itemsSource[i];
                    if (VideoFolder.HasCompleteLoad)
                        continue;

                    if (!VideoFolder.HasCompleteLoad && FileLoader.FileLoaderInstance.HasDataSource)
                    {
                        var videofolder = FileLoader.FileLoaderInstance.GetExistingVideoFolderIfAny(VideoFolder);
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
                            HomePageLocalViewModel.GetExistingCopy((VideoFolder)VideoFolder.ParentDirectory);
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
            }), DispatcherPriority.Background);
        }


        private void MergeSameVideoData(ref VideoFolder videoFolder, VideoFolder videoFoldercopy)
        {
            FileLoader.FileLoaderInstance.DeepCopy(videoFolder, videoFoldercopy);
        }

        private void SearchSubtitleFile(VideoFolder item)
        {
            if (item.FileType == FileType.Folder) return;
            if ((item as VideoFolderChild).HasSearchSubtitleFile) return;

            Dispatcher.BeginInvoke(new Action(delegate
            {
                var itemchild = (VideoFolderChild)item;
                if (files == null)
                    files = FileExplorerCommonHelper.GetSubtitleFiles(item.ParentDirectory.Directory);

                itemchild.SubPath = GetSubtitlePath(item);
            }), DispatcherPriority.Background);
        }

        private VideoFolder GetItems(VideoFolder item)
        {
            var s = FileLoader.FileLoaderInstance.GetFolderItems(item);
            return s;
        }

        private void LoadOtherFileDetails(VideoFolder item)
        {
            if (item.FileType == FileType.Folder)
            {
                if (item.HasCompleteLoad) return;
                var task = Task.Factory.StartNew(() => GetItems(item))
                    .ContinueWith(t => item = t.Result,
                    TaskScheduler.Current).Wait(200);
            }
            else
            {

                var itemchild = (VideoFolderChild)item;
                //var task = Task.Factory.StartNew(() => GetThumbnail(itemchild))
                //    .ContinueWith(t =>{}, TaskScheduler.Current).Wait(200);

                var task = Task.Factory.StartNew(() => {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        itemchild.IsLoading = true;
                        FileLoader.GetShellInfo(itemchild);
                        itemchild.UpdateProperties();
                        itemchild.IsLoading = false;
                    }), DispatcherPriority.Background, null);})
                    .ContinueWith(t => {  }, TaskScheduler.Current).Wait(200);
            }
        }

        private void GetThumbnail(VideoFolderChild file)
        {
            ImageSource imageSource = null;
            Dispatcher.Invoke(new Action(delegate
            {
                file.IsLoading = true;
                ShellObject shell = ShellObject.FromParsingName(file.FullName);
                file.Thumbnail = imageSource = shell.Thumbnail.LargeBitmapSource;
                shell.Dispose();
                file.IsLoading = false;

           }), DispatcherPriority.Background, null);
        }

        private ObservableCollection<SubtitleFilesModel> GetSubtitlePath(VideoFolder item)
        {
            return FileExplorerCommonHelper.MatchSubToMedia(item.Name, files);
        }
        
        public static Threading.Executor CurrentTaskExecutor
        {
            get { return (Threading.Executor)ServiceLocator.Current.GetInstance<IBackgroundService>(); }
        }
        

    }
}
