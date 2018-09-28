using Common.Util;
using Delimon.Win32.IO;
using Microsoft.WindowsAPICodePack.Shell;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.ViewModel;

namespace VirtualizingListView.Pages.Util
{
    public class MovieItemProvider : Control, IItemsProvider<VideoFolder>
    {
        //int startindex, Endindex;
        private ObservableCollection<VideoFolder>
            _videofolderchild
        { get { return FilePageViewModel.CurrentVideoFolder.OtherFiles; } }
        private readonly int _count;
        private readonly int _fetchDelay;
        private BackgroundWorker BGW;

        private object SaveObject;
        private FilePageViewModel CVM;
        IEnumerable<FileInfo> files;
        

        public IFilePageViewModel FilePageViewModel
        {
            get
            {
                return CVM;
            }
        }

        public MovieItemProvider(int _count, int _fetchDelay)
        {
            this._count = _count;
            this._fetchDelay = _fetchDelay;
        }

        public MovieItemProvider(FilePageViewModel cvm)
            : this(cvm.CurrentVideoFolder.ChildrenSize, 1000)
        {
            CVM = cvm;
            BGW = new BackgroundWorker();
            BGW.DoWork += BGW_DoWork;
            BGW.ProgressChanged += BGW_ProgressChanged;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.WorkerSupportsCancellation = true;
            BGW.WorkerReportsProgress = true;
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                BGW.RunWorkerAsync(SaveObject);
                Trace.WriteLine("Backgroundworking started again... ");
                return;
            }
            Trace.WriteLine("Backgroundworking Completed... ");
            BGW.Dispose();
            FilePageViewModel.IsLoading = false;
        }

        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FilePageViewModel.IsLoading = true;
            // throw new System.NotImplementedException();
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            // Select item wer dey dont ve a thumbail.
            // Refresh dir on click change
            // this.Cursor = Cursors.Wait;
            //if(CollectionVM.ViewType == ViewType.Large)
            //Thread.Sleep(100);
            var list = (ObservableCollection<VideoFolder>)e.Argument;

            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].HasCompleteLoad)
                {
                    SearchSubtitleFile(list[i]);
                    continue;
                }
                SearchSubtitleFile(list[i]);
                LoadOtherFileDetails(list[i]);
                //Thread.SpinWait(list.Count());
                if (FilePageViewModel.CurrentVideoFolder.Tag != null) Thread.Sleep(10);
                Thread.Sleep(5);
            }

            if (BGW.CancellationPending)
            {
                e.Cancel = true;
                Trace.WriteLine("Backgroundworking Canceled... ");
                return;
            }
        }

        private void SearchSubtitleFile(VideoFolder item)
        {
            if (item.FileType == GroupCatergory.Grouped) return;
            if ((item as VideoFolderChild).HasSearchSubtitleFile) return;

            Dispatcher.BeginInvoke(new Action(delegate
            {
                //var itemchild = (VideoFolderChild)item;
                //if (files == null)
                //    files = FileExplorerCommonHelper.GetSubtitleFiles(FilePageViewModel.CurrentVideoFolder.Directory);

                //itemchild.SubPath = GetSubtitlePath(item);
            }), DispatcherPriority.Background);
        }

        private VideoFolder GetItems(VideoFolder item)
        {
            //var s = FileLoader.FileLoaderInstance.LoadParentFiles(item, item.SortedBy);
            //foreach (var vfile in item.OtherFiles)
            //{
            //    var temp = vfile;
            //    if (vfile.FileType == FileType.Folder)
            //        temp = FileLoader.FileLoaderInstance.LoadParentFiles(vfile, vfile.SortedBy);
            //}

            return null;
        }

        private void LoadOtherFileDetails(VideoFolder item)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (item.FileType == GroupCatergory.Grouped)
                {
                    //item = GetItems(item);
                    Task.Factory.StartNew(() => GetItems(item))
                        .ContinueWith(t => item = t.Result, TaskScheduler.FromCurrentSynchronizationContext());
                    return;
                }
                else
                {
                    var itemchild = (VideoFolderChild)item;
                    Task.Factory.StartNew(() => GetThumbnail(item.FilePath))
                        .ContinueWith(t => itemchild.Thumbnail = t.Result, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }), DispatcherPriority.Background, null);
        }

        private ImageSource GetThumbnail(string filepath)
        {
            ImageSource imageSource = null;
            Dispatcher.Invoke(new Action(delegate
            {
                using (ShellObject shell = ShellObject.FromParsingName(filepath))
                {
                    //if (CollectionVM.ViewType == ViewType.Large)
                    imageSource = shell.Thumbnail.LargeBitmapSource;
                }
            }), DispatcherPriority.Background, null);
            return imageSource;
        }

        private ObservableCollection<string> GetSubtitlePath(VideoFolder item)
        {
            // return FileExplorerCommonHelper.MatchSubToMedia(item.Name, files);
            return null;
        }


        /// <summary>
        /// Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            Trace.WriteLine("FetchCount");
            //  Thread.Sleep(_fetchDelay);
            return _count;
        }

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<VideoFolder> FetchRange(int startIndex, int count)
        {
            Trace.WriteLine("FetchRange: " + startIndex + "," + count);
            count = GetPossibleNextItemsRange(startIndex, count);
            //Thread.Sleep(_fetchDelay);

            ObservableCollection<VideoFolder> list = new ObservableCollection<VideoFolder>();

            //for (int i = startIndex; i < startIndex + count; i++)
            //{
            //    list.Add(FilePageViewModel.CurrentVideoFolder.OtherFiles[i]);
            //}

            return list;
        }

        private int GetPossibleNextItemsRange(int startIndex, int count)
        {
            int nextmove = startIndex + count;
            int possiblerange = _count - nextmove;
            if (possiblerange < 0)
            {
                return _count - startIndex;
            }
            else
                return count;
        }


        public int GetItemsCount()
        {
            return _count;
        }

        public void CompleteLoad(object s)
        {
            if (BGW.IsBusy)
            {
                if (FilePageViewModel.CurrentVideoFolder.Tag != null) return;
                BGW.CancelAsync();
                SaveObject = s;
                Trace.WriteLine("Backgroundworking Cancelling...");
            }
            else
            {
                BGW.RunWorkerAsync(s);
                Trace.WriteLine("Backgroundworking... ");
            }

        }
    }
}
