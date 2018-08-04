using Common.Interfaces;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using SearchComponent;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages;
using VirtualizingListView.Pages.Model;
using VirtualizingListView.Pages.Util;
using VirtualizingListView.Pages.ViewModel;
using VirtualizingListView.Pages.Views;
using VirtualizingListView.Util;
using VirtualizingListView.View;

namespace VirtualizingListView.ViewModel
{
    internal class FileViewViewModel : NotificationObject
    {
        private IPageNavigatorHost pageNavigator;
        public RoutedUICommand GoBackCommand { get; private set; }
        public RoutedUICommand GoForwardCommand { get; private set; }
        public RoutedUICommand SearchCommand { get; private set; }
        private bool hasaction;
        private bool isonsearchpage;

        public  bool IsOnSearchPage
        {
            get { return isonsearchpage; }
            set
            {
                isonsearchpage = value;
                RaisePropertyChanged(() => this.IsOnSearchPage);
            }
        }

        public bool HasAction
        {
            get { return hasaction; }
            set { hasaction = value; RaisePropertyChanged(() => this.HasAction); }
        }

        public DelegateCommand<object> videofoldercommand;
        public DelegateCommand<object> VideoFolderCommand
        {
            get
            {
                if (videofoldercommand == null)
                    videofoldercommand = new DelegateCommand<object>((o) =>
                    {
                        var data = o as WindowCommandButton;
                        data.SetActive(true, true); 
                    });
                return videofoldercommand;
            }
        }

        public DelegateCommand<object> usbdrivecommand;
        public DelegateCommand<object> UsbDriveCommand
        {
            get
            {
                if (usbdrivecommand == null)
                    usbdrivecommand = new DelegateCommand<object>((o) =>
                    {
                        var data = o as WindowCommandButton;
                        data.SetActive(true, true); 
                    });
                return usbdrivecommand;
            }
        }

        //public DelegateCommand<object> mediaservercommand;
        //public DelegateCommand<object> MediaServerCommand
        //{
        //    get
        //    {
        //        if (mediaservercommand == null)
        //            mediaservercommand = new DelegateCommand<object>((o) =>
        //            {
        //                var data = o as WindowCommandButton;
        //                data.SetActive(true, true); ;
        //            });
        //        return mediaservercommand;
        //    }
        //}

        private HamburgerMenuIconItem videoFolder;

        public HamburgerMenuIconItem VideoFolders
        {
            get { return videoFolder; }
            set { videoFolder = value; this.RaisePropertyChanged(() => this.VideoFolders); }
        }

        private HamburgerMenuIconItem usbDrive;

        public HamburgerMenuIconItem UsbDrive
        {
            get { return usbDrive; }
            set { usbDrive = value; this.RaisePropertyChanged(() => this.UsbDrive); }
        }

        private HamburgerMenuIconItem mediaServer;

        public HamburgerMenuIconItem MediaServer
        {
            get { return mediaServer; }
            set { mediaServer = value; this.RaisePropertyChanged(() => this.MediaServer); }
        }



        public FileViewViewModel(IPageNavigatorHost pageNavigator)
        {
            this.pageNavigator = pageNavigator;
            InitCommands();

            InitSearchControl();
            InitHamBurgerMenu();
        }

        private void InitSearchControl()
        {
            pageNavigator.GetSearchControl = new SearchControl<VideoFolder>();
            pageNavigator.GetSearchControl.IsSearchButtonEnabled = true;
            (pageNavigator.GetSearchControl as ISearchControl<VideoFolder>).SearchPattern = SearchMode.Object;
            (pageNavigator.GetSearchControl as ISearchControl<VideoFolder>).OnSearchStarted += SearchControl_OnSearchStarted;
        }

        private void SearchControl_OnSearchStarted(object sender)
        {
            HasAction = true;
        }
        

        private void InitCommands()
        {
            GoBackCommand = new RoutedUICommand();
            GoForwardCommand = new RoutedUICommand();
            ((UserControl)pageNavigator).CommandBindings.Add(new CommandBinding(GoBackCommand, GoBackCommand_Execute, GoBackCommand_CanExecute));
            ((UserControl)pageNavigator).CommandBindings.Add(new CommandBinding(GoForwardCommand, GoForwardCommand_Execute, GoForwardCommand_CanExecute));

            SearchCommand = new RoutedUICommand();
            ((UserControl)pageNavigator).CommandBindings.Add(new CommandBinding(SearchCommand, SearchCommand_Execute, SearchCommand_CanExecute));
        }

        private void SearchPage_OnFinished(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var results = sender as SearchModel;

                this.pageNavigator.PageNavigator.NavigationService.Navigate(new SearchResultPage(this.pageNavigator.PageNavigator.NavigationService), results);
            }
            HasAction = false;
            IsOnSearchPage = false;
        }
        private void SearchCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SearchPage searchPage = SearchPage.GetSearchPage();
            searchPage.OnFinished += SearchPage_OnFinished;
            searchPage.ShowDialog();
            IsOnSearchPage = true;
        }

        private void SearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (HomePageLocalViewModel.HasSearchData() || !FileLoader.FileLoaderInstance.HasDataSource)
            {
                e.CanExecute = false; return;
            }
            e.CanExecute = !IsOnSearchPage;
        }

        private void GoForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = pageNavigator.PageNavigator.Host.CanGoForward;
        }

        private void GoForwardCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            pageNavigator.PageNavigator.Host.GoForward();
        }

        private void GoBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = pageNavigator.PageNavigator.Host.CanGoBack;
        }

        private void GoBackCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsOnSearchPage) { return; }
            pageNavigator.PageNavigator.Host.GoBack();
            FileLoaderCompletion.CurrentTaskExecutor.Shutdown();
        }

        private void InitHamBurgerMenu()
        {
            VideoFolders = new HamburgerMenuIconItem()
            {
                Label = "Video folders",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Folder },
                Tag = new HomePageLocal()
            };

            UsbDrive = new HamburgerMenuIconItem()
            {
                Label = "Removable Storage",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Usb },
                Tag = new UsbDrivePage()
            };

            //MediaServer = new HamburgerMenuIconItem()
            //{
            //    Label = "Media server",
            //    Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.ServerNetwork },
            //    Tag = new MediaServerPage()
            //};
        }
    }
}