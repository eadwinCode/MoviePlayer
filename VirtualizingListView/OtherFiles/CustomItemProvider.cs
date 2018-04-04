using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VirtualizingListView.Model;
using VirtualizingListView.Util;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.OtherFiles
{
    public class CustomItemProvider :Control, IItemsProvider<VideoFolder>
    {
        //int startindex, Endindex;
        private ObservableCollection<VideoFolder> 
            _videofolderchild { get { return CollectionVM.VideoDataAccess.OtherFiles; } }  
        private readonly int _count;
        private readonly int _fetchDelay;
        private BackgroundWorker BGW;

        private object SaveObject;
        readonly ICollectionViewModel CVM;
        IEnumerable<FileInfo> files;
        public CollectionViewModel CollectionVM
        {
            get
            {
                return (CollectionViewModel)CVM.GetCollectionVM;
            }
        }
        public CustomItemProvider(int _count,int _fetchDelay)
        {
            this._count = _count;
            this._fetchDelay = _fetchDelay;
        }

        public CustomItemProvider(CollectionViewModel cvm)
            :this(cvm.VideoDataAccess.ChildrenSize,1000)
        {
            CVM = cvm;
            CollectionVM.ViewChanged += ViewChangeExecuted;
            BGW = new BackgroundWorker();
            BGW.DoWork += BGW_DoWork;
            BGW.ProgressChanged += BGW_ProgressChanged;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;
            BGW.WorkerSupportsCancellation = true;
            BGW.WorkerReportsProgress = true;        }

        private void ViewChangeExecuted(object sender)
        {
            if ((ViewType)sender == ViewType.Large)
            {
                object view = VideoItemViewCollection<VideoFolder>.GetCurrentViewItem();
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
            CollectionVM.IsLoading = false;
            //this.Cursor = Cursors.Arrow;
        }
        
        void BGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CollectionVM.IsLoading = true;
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
                Loadthumbnails(list[i]);
                //Thread.SpinWait(list.Count());
                if (CollectionVM.VideoDataAccess.Tag != null) Thread.Sleep(10);
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
            if (item.FileType == FileType.Folder) return;
            if ((item as VideoFolderChild).HasSearchSubtitleFile) return;

            Dispatcher.BeginInvoke(new Action(delegate
            {
                var itemchild = (VideoFolderChild)item;
                if (files == null)
                    files = FileExplorerCommonHelper.GetSubtitleFiles(CollectionVM.VideoDataAccess.Directory);

                itemchild.SubPath = GetSubtitlePath(item);
            }),DispatcherPriority.Background);
        }

        private VideoFolder GetItems(VideoFolder item)
        {
            var s = FileLoader.LoadParentFiles(item, item.SortedBy,this.CollectionVM);
            return s;
        }

        private void Loadthumbnails(VideoFolder item)
        {

            Dispatcher.Invoke(new Action(delegate
            {
                if (item.FileType == FileType.Folder)
                {
                    Task.Factory.StartNew(() => GetItems(item))
                        .ContinueWith(t => item = t.Result, TaskScheduler.FromCurrentSynchronizationContext());
                    return;
                }
                else
                {    
                    var itemchild = (VideoFolderChild)item;
                    using (ShellObject shell = ShellObject.FromParsingName(itemchild.FilePath))
                    {
                        if (CollectionVM.ViewType == ViewType.Large)
                            itemchild.Thumbnail = shell.Thumbnail.LargeBitmapSource;

                        //IShellProperty prop = shell.Properties.System.Media.Duration;
                        //itemchild.Duration = prop.FormatForDisplay(PropertyDescriptionFormat.ShortTime);
                        //var duration = shell.Properties.System.Media.Duration;
                        //if (duration.Value != null)
                        //    itemchild.MaxiProgress = double.Parse(duration.Value.ToString());

                    }
                }
            }));
        }

        private List<SubtitleFilesModel> GetSubtitlePath(VideoFolder item)
        {
            return FileExplorerCommonHelper.MatchSubToMedia(item.Name, files);
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

            for (int i = startIndex; i < startIndex + count; i++)
            {
                list.Add(CollectionVM.VideoDataAccess.OtherFiles[i]);
            }
             //   var list = _videofolderchild.Where(x => (_videofolderchild.IndexOf(x) >= startindex) && (_videofolderchild.IndexOf(x) <= startindex + count)).ToList<VideoFolder>(); ;
            // var list = (from a in _videofolderchild where (_videofolderchild.IndexOf(a) >=startindex) && (_videofolderchild.IndexOf(a) <=startindex+count) select a).ToList();

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
           
            if (BGW.IsBusy )
            {
                if (CollectionVM.VideoDataAccess.Tag != null) return;
                BGW.CancelAsync();
                SaveObject = s;
                Trace.WriteLine(" Backgroundworking Cancelling... ");
            }
            else
            {
                BGW.RunWorkerAsync(s);
                Trace.WriteLine("Backgroundworking... ");
            }
        }
    }
}
