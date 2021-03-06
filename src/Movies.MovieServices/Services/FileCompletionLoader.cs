﻿using Common.Util;
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

namespace Movies.MovieServices.Services
{
    public class FileLoaderCompletion : IFileLoaderCompletion
    {
        private IEnumerable<FileInfo> files;
        private IFileLoader fileloader;
        private IFileExplorerCommonHelper filecommonhelper;
        private IDispatcherService dispatcherService;
        IDataSource<MediaFolder> MovieDataSource
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataSource<MediaFolder>>();
            }
        }


        public FileLoaderCompletion(IFileLoader fileLoader, IFileExplorerCommonHelper fileExplorerCommonHelper,IDispatcherService dispatcherService)
        {
            this.fileloader = fileLoader;
            this.filecommonhelper = fileExplorerCommonHelper;
            this.dispatcherService = dispatcherService;
        }

        public void FinishCollectionLoadProcess(IList<MediaFolder> itemsSource, Dispatcher dispatcherUnit = null)
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

        public void FinishCollectionLoadProcess(ObservableCollection<MediaFolder> itemsSource,bool IsMovieFolder)
        {
            dispatcherService.InvokeDispatchAction(new Action(delegate
            {
                for (int i = 0; i < itemsSource.Count();i++)
                {
                    MediaFolder VideoFolder = itemsSource[i];
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
                        MediaFolder videoFoldercopy = null;
                        MediaFolder ExistingParent =
                            MovieDataSource.GetExistingCopy((MediaFolder)VideoFolder.ParentDirectory);
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


        private void MergeSameVideoData(ref MediaFolder videoFolder, MediaFolder videoFoldercopy)
        {
            (fileloader as FileLoader).DeepCopy(videoFolder, videoFoldercopy);
        }

        private void SearchSubtitleFile(MediaFolder item)
        {
            if (item.FileType == GroupCatergory.Grouped) return;
            if ((item as MediaFile).HasSearchSubtitleFile) return;
            if (files == null)
                files = filecommonhelper.GetSubtitleFiles(item.ParentDirectory.Directory);
            var subs = GetSubtitlePath(item.Name);
            dispatcherService.BeginInvokeDispatchAction(new Action(delegate
            {
                var itemchild = (MediaFile)item;
                itemchild.SubPath = subs;
            }));
        }

        private MediaFolder GetItems(MediaFolder item)
        {
            var s = fileloader.GetFolderItems(item);
            return s;
        }

        private void LoadOtherFileDetails(MediaFolder item, Dispatcher dispatcherunit)
        {
            if (item.FileType == GroupCatergory.Grouped)
            {
                if (item.HasCompleteLoad) return;
                var task = Task.Factory.StartNew(() => GetItems(item))
                    .ContinueWith(t => item = t.Result,
                    TaskScheduler.Current);
                task.Wait();
            }
            else
            {
                var itemchild = (MediaFile)item;
                Task task = Task.Factory.StartNew(() => {
                    dispatcherService.BeginInvokeDispatchAction(dispatcherunit,new Action(delegate
                    {
                        //itemchild.IsLoading = true;
                        FileLoader.GetShellInfo(itemchild);
                        
                    }));})
                    .ContinueWith(t => 
                    {
                        //itemchild.IsLoading = false;
                        itemchild.RefreshFileInfo();
                    }, TaskScheduler.Current);

                task.Wait();
                itemchild.IsLoading = false;
            }
        }

        private void GetThumbnail(MediaFile file)
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

        private ObservableCollection<string> GetSubtitlePath(string name)
        {
            return filecommonhelper.MatchSubToMedia(name, files);
        }
        
    }
}
