using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Events;
using System.IO;
using System.Windows.Input;
using VirtualizingListView.Util;
using Microsoft.Practices.ServiceLocation;
using VideoComponent.BaseClass;
using VideoComponent;
using VideoComponent.Command;
using VirtualizingListView.Model;
using VirtualizingListView.Events;
using VideoComponent.Events;
using System.Collections.ObjectModel;

namespace VirtualizingListView.ViewModel
{
    public class VirtualizingVM : Control
    {
        
        //public static LastSeen LastSeen;

        //protected ItemsProvider ItemProvider;
        //public VideoFolder VideoDataAccess
        //{
        //    get { return videodataaccess; }
        //    set
        //    {
        //        videodataaccess = value;
        //        if (value.OtherFiles != null && ItemProvider == null)
        //        {
        //            //ItemProvider = new VideoComponent.ItemProvider(1000, this);
        //            ItemProvider = new ItemsProvider(value);
        //         //   VideoItemViewCollection = new VideoItemViewCollection<VideoFolder>(ItemProvider, 0, 1000);
        //            _aggregator.GetEvent<LoadViewExecuteCommandEvent>().Publish(ItemProvider);
        //        }
        //        //RaisePropertyChangedEvent("VideoItem");
        //    }
        //}
       
        public VirtualizingVM()
        {
            //  string path = @"C:\\Users\\Eadwin Toochos\\Desktop";

            //InitCombox();

            //IsLoaded = false;
            //PlayMovie = new RelayCommand(PlayMovieAction);
            //ViewType = Util.ViewType.Large;
            //TemplateToggle = new RelayCommand(() => TemplateToggleAction());
          


            //  LastSeen.SetPlayedBefore(new ObservableCollection<ILastSeen>());

            // Next = new DelegateCommand(Next_Action, CanNextExecute);
            // Previous = new DelegateCommand(Previous_Action, CanPreviousExecute);

            //  StartLoadingProcedure(path);

            //this.DoubleCommand = new DelegateCommand<object>((parameter) =>
            //{
            //    Console.WriteLine("DoubleClickExecuted"); 
            //   // RaisePropertyChangedEvent("DoubleCommand");
            //});
           // _aggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            //_aggregator.GetEvent<LoadExecuteCommandEvent>().Subscribe(DoubleclickAction);

            //ItemProvider = new VideoComponent.ItemProvider(1000, this);
            //VideoItem = new VideoItemViewCollection<VideoFolder>(ItemProvider);

        }


       
        //private void PlayMovieAction()
        //{
        //    //throw new NotImplementedException();
        //}
        //private void DoubleCommandAction(object parameter)
        //{

        //}
        //private void TemplateToggleAction()
        //{
        //    if (VideoItemViewCollection != null)
        //    {
        //        DataTemplateSelector sed = MyTemplateChange;
        //        if (sed.GetType() == new itemListSelector().GetType())
        //        {
        //            ViewType = Util.ViewType.Large;
        //        }
        //        else
        //            ViewType = Util.ViewType.Small;
        //    }
        //}

        //public ViewType ViewType
        //{
        //    get
        //    {
        //        return viewtype;
        //    }
        //    set
        //    {
        //        viewtype = value;
        //        UpdateView(value);
        //        // Completeload();
        //    }
        //}
        //private void UpdateView(ViewType value)
        //{
        //    if (value == Util.ViewType.Large)
        //    {
        //        MyTemplateChange = new MoreTemplateSelector();
        //    }
        //    else
        //        MyTemplateChange = new itemListSelector();
        //}


        //public void RefreshAction(VideoData obj)
        //{
        //    //  if (obj.intChildCount < VideoDataAccess.ChildFiles.Count)
        //    //  {
        //    //      VideoDataAccess = obj;
        //    //      VideoDataAccess.ChildFiles = VideoDataAccessor.SortList(SortType, VideoDataAccess);
        //    //      return;
        //    //  }

        //    //  VideoDataAccess.ChildFiles.AddRange(obj.ChildFiles);
        //    ////  VideoDataAccess.ChildCount = obj.ChildCount;
        //    //  VideoDataAccess.ChildFiles = VideoDataAccessor.SortList(SortType, VideoDataAccess);
        //    //  RaisePropertyChangedEvent("VideoItem");
        //}

        //private void InitCombox()
        //{
        //    ComboxString = new List<string>();
        //    ComboxString.Add("Date");
        //    ComboxString.Add("Name");
        //    ComboxString.Add("Ext");

        //    ComboxSelectedItem = ComboxString[0];
        //}

        //public void StartLoadingProcedure(string path = null)
        //{
        //    Dispatcher.Invoke(new Action(delegate
        //    {
        //        Reset();
        //        DirectoryPosition = new DirectoryInfo(path);
        //        this.VideoDataAccess = VideoDataAccessor.LoadParent(new VideoFolder(DirectoryPosition.FullName), SortType);
        //        Navigation.Add(new NavigationModel { Dir = DirectoryPosition, VideoData = VideoDataAccess });
        //        CheckCanExecut();
        //    }));

        //}

        //private void Reset()
        //{
        //    activeindex = 0;
        //    Navigation = new List<NavigationModel>();
        //    ItemProvider = null;
        //}

        //public void VideoComponentViewModel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    this.IsLoaded = true;
        //}

        //private void CheckCanExecut()
        //{
        //    Next = CanNextExecute();
        //    Previous = CanPreviousExecute();
        //    // Next.RaiseCanExecuteChanged();
        //    // Previous.RaiseCanExecuteChanged();
        //}

        //public void Previous_Action()
        //{
        //    int prev = activeindex - 1;
        //    DirectoryPosition = Navigation[prev].Dir;
        //    VideoDataAccess = Navigation[prev].VideoData;
        //    // VideoItem.Clear();
        //    // this.VideoDataAccess = VideoDataAccessor.LoadParent(DirectoryPosition, SortType);
        //    activeindex--;
        //    CheckCanExecut();
        //    //RecheckFiles(DirectoryPosition);
        //    RefreshLink();
        //    //CheckSort();
        //}

        //public void Next_Action()
        //{
        //    int next = activeindex + 1;
        //    DirectoryPosition = Navigation[next].Dir;
        //    VideoDataAccess = Navigation[next].VideoData;
        //    // VideoItem.Clear();
        //    // this.VideoDataAccess = VideoDataAccessor.LoadParent(DirectoryPosition, SortType);
        //    activeindex++;
        //    CheckCanExecut();
        //    //RecheckFiles(DirectoryPosition);
        //    RefreshLink();
        //    //CheckSort();
        //}
        //public bool CanNextExecute()
        //{
        //    if (activeindex == Navigation.Count - 1)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        //public bool CanPreviousExecute()
        //{
        //    if (activeindex - 1 < 0)//|| activeindex > Navigation.Count - 1
        //    {
        //        return false;
        //    }

        //    return true;
        //}
        //public void DoubleclickAction(VideoFolder obj)
        //{
        //    AddToNav(obj);
        //}
        //private void AddToNav(VideoFolder obj)
        //{
        //    TrimNav();
        //    activeindex++;
        //    this.DirectoryPosition = obj.Directory;
        //    this.VideoDataAccess = obj;
        //    Navigation.Add(new NavigationModel { Dir = DirectoryPosition, VideoData = VideoDataAccess });
        //    CheckCanExecut();
        //    //CheckSort();
        //}
        private void RefreshLink()
        {
            //Dispatcher.Invoke(new Action(delegate
            //{
            //   VideoDataAccessor.RefreshPath(VideoDataAccess);
            //}));

        }
        //private void TrimNav()
        //{
        //    if (activeindex == Navigation.Count - 1) return;

        //    for (int i = Navigation.Count - 1; i > activeindex; i--)
        //    {
        //        Navigation.RemoveAt(i);
        //    }
        //}
       

        //protected void RaisePropertyChangedEvent(string propertyname)
        //{
        //    // exit if no subscribers
        //    if (PropertyChanged == null) return;

        //    // raise event
        //    var e = new PropertyChangedEventArgs(propertyname);
        //    PropertyChanged(this, e);
        //}


    }
}
