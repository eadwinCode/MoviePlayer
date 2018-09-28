using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Interfaces;
using Movies.MoviesInterfaces;
using PresentationExtension;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using VirtualizingListView.Pages;
using VirtualizingListView.Pages.ViewModel;
using VirtualizingListView.Pages.Views;

namespace VirtualizingListView.ViewModel
{
    public class FileViewViewModel : NotificationObject
    {
        private IPageNavigatorHost pageNavigator;
        public RoutedUICommand GoBackCommand { get; private set; }
        public RoutedUICommand GoForwardCommand { get; private set; }
        private bool hasaction;
        private bool isonsearchpage;

        IBackgroundService BackgroundService;
        IFileLoader FileLoader;

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
        

        public FileViewViewModel(IBackgroundService backgroundService, IFileLoader fileLoader)
        {
            this.BackgroundService = backgroundService;
            this.FileLoader = fileLoader;
        }
        public void SetPageHost(IPageNavigatorHost pageNavigatorHost)
        {
            this.pageNavigator = pageNavigatorHost;
            InitCommands();

            InitSearchControl();
        }

        private void InitSearchControl()
        {
            //pageNavigator.GetSearchControl = new SearchControl<VideoFolder>()
            //{
            //    IsSearchButtonEnabled = true,
            //    SearchPattern = SearchMode.Object,
            //};
            //(pageNavigator.GetSearchControl as ISearchControl<VideoFolder>).OnSearchStarted += SearchControl_OnSearchStarted;
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
        }

        
        //private void SearchCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        //{
        //    //SearchPage searchPage = SearchPage.GetSearchPage();
        //    //searchPage.OnFinished += SearchPage_OnFinished;
        //    //searchPage.ShowDialog();
        //    IsOnSearchPage = true;
        //}

        //private void SearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    //HomePageLocalViewModel.HasSearchData() ||
        //    if ( !FileLoader.HasDataSource)
        //    {
        //        e.CanExecute = false; return;
        //    }
        //    e.CanExecute = !IsOnSearchPage;
        //}

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
            BackgroundService.Shutdown();
        }
    }
}