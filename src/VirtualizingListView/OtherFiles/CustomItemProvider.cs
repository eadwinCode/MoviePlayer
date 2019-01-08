using Common.Util;
using Delimon.Win32.IO;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VirtualizingListView.Model;
using VirtualizingListView.Pages.ViewModel;

namespace VirtualizingListView.OtherFiles
{
    public class CustomItemProvider :Control, IItemsProvider<MediaFolder>
    {
        //int startindex, Endindex;
        private ObservableCollection<MediaFolder> _videofolderchild
        { get { return FilePageViewModel.CurrentVideoFolder.OtherFiles; } }  
        private readonly int _count;
        private readonly int _fetchDelay;
        private BackgroundWorker BGW;

        private object SaveObject;
        readonly ICollectionViewModel CVM;
        IEnumerable<FileInfo> files;

        public IFilePageViewModel FilePageViewModel
        {
            get
            {
                return null;
                //return (CollectionViewModel)CVM.GetCollectionVM;
            }
        }

        public CustomItemProvider(int _count,int _fetchDelay)
        {
            this._count = _count;
            this._fetchDelay = _fetchDelay;
        }

        //public CustomItemProvider(CollectionViewModel cvm)
        //    :this(cvm.CurrentVideoFolder.ChildrenSize,1000)
        //{
        //    //CVM = cvm;
        //    cvm.ViewChanged += ViewChangeExecuted;
        //    BGW = new BackgroundWorker();
        //    BGW.DoWork += BGW_DoWork;
        //    BGW.ProgressChanged += BGW_ProgressChanged;
        //    BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
        //    BGW.WorkerSupportsCancellation = true;
        //    BGW.WorkerReportsProgress = true;
        //}

        private void ViewChangeExecuted(object sender)
        {
            if ((ViewType)sender == ViewType.Large)
            {
                object view = VideoItemViewCollection<MediaFolder>.GetCurrentViewItem();
               this.CompleteLoad(view);
            }
        }


        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            //  System.Windows.MessageBox.Show("Finished");
            //if (SortonLoad)
            //{
            //    //Completeload();
            //    SortonLoad = false;
            //}
            if (e.Cancelled)
            {
                BGW.RunWorkerAsync(SaveObject);
                Trace.WriteLine("Backgroundworking started again... ");
                return;
            }
            Trace.WriteLine("Backgroundworking Completed... ");
            BGW.Dispose();
            FilePageViewModel.IsLoading = false;
            //this.Cursor = Cursors.Arrow;
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
            var list = (ObservableCollection<MediaFolder>)e.Argument;

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

        private void SearchSubtitleFile(MediaFolder item)
        {
            if (item.FileType == GroupCatergory.Grouped) return;
            if ((item as MediaFile).HasSearchSubtitleFile) return;

            Dispatcher.BeginInvoke(new Action(delegate
            {
                var itemchild = (MediaFile)item;
                if (files == null)
                    //files = FileExplorerCommonHelper.GetSubtitleFiles(FilePageViewModel.CurrentVideoFolder.Directory);

                itemchild.SubPath = GetSubtitlePath(item);
            }),DispatcherPriority.Background);
        }

        private MediaFolder GetItems(MediaFolder item)
        {
            //var s = FileLoader.FileLoaderInstance.LoadParentFiles(item, item.SortedBy);
            //foreach (var vfile in item.OtherFiles)
            //{
            //    var temp = vfile;
            //    if(vfile.FileType == FileType.Folder)
            //        temp = FileLoader.FileLoaderInstance.LoadParentFiles(vfile, vfile.SortedBy);
            //}
           
            return null;
        }

        private void LoadOtherFileDetails(MediaFolder item)
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
                    var itemchild = (MediaFile)item;
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
                    if (FilePageViewModel.ActiveViewType == ViewType.Large)
                        imageSource = shell.Thumbnail.LargeBitmapSource;
                }
            }),DispatcherPriority.Background,null);
            return imageSource;
        }

        private ObservableCollection<string> GetSubtitlePath(MediaFolder item)
        {
            //return FileExplorerCommonHelper.MatchSubToMedia(item.Name, files);
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
        public IList<MediaFolder> FetchRange(int startIndex, int count)
        {
            Trace.WriteLine("FetchRange: " + startIndex + "," + count);
            count = GetPossibleNextItemsRange(startIndex, count);
            //Thread.Sleep(_fetchDelay);

            ObservableCollection<MediaFolder> list = new ObservableCollection<MediaFolder>();

            for (int i = startIndex; i < startIndex + count; i++)
            {
                list.Add(FilePageViewModel.CurrentVideoFolder.OtherFiles[i]);
            }

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
