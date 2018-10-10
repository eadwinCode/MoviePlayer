using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace VirtualizingListView.View
{
    public class PageNavigatorHost : MovieBase, IPageNavigatorHost
    {
        private bool hasaction;
        private bool isonsearchpage;

        IBackgroundService BackgroundService;
        private object pagenavigatorcontenthost;
        private INavigatorService pagenavigator;
        private IDictionary<string, object> _contents;

        public RoutedUICommand GoBackCommand { get; private set; }
        public RoutedUICommand GoForwardCommand { get; private set; }
        
        public bool IsOnSearchPage
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

        public INavigatorService PageNavigator { get { return pagenavigator; } }

        public object PageNavigatorContentHost
        {
            get { return pagenavigatorcontenthost; }
            private set { pagenavigatorcontenthost = value; RaisePropertyChanged(() => this.PageNavigatorContentHost); }
        }

        public PageNavigatorHost(IBackgroundService backgroundService)
        {
            this.Content = new FileView() { DataContext = this};
            this.BackgroundService = backgroundService;
            pagenavigator = new PageNavigator();
            InitCommands();
            _contents = new Dictionary<string, object>();
        }

        private void SearchControl_OnSearchStarted(object sender)
        {
            HasAction = true;
        }

        private void InitCommands()
        {
            GoBackCommand = new RoutedUICommand();
            GoForwardCommand = new RoutedUICommand();
            this.CommandBindings.Add(new CommandBinding(GoBackCommand, GoBackCommand_Execute, GoBackCommand_CanExecute));
            this.CommandBindings.Add(new CommandBinding(GoForwardCommand, GoForwardCommand_Execute, GoForwardCommand_CanExecute));
        }

        private void GoForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PageNavigator.Host.CanGoForward;
        }

        private void GoForwardCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            PageNavigator.Host.GoForward();
        }

        private void GoBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PageNavigator.Host.CanGoBack;
        }

        private void GoBackCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsOnSearchPage) { return; }
            PageNavigator.Host.GoBack();
            BackgroundService.Shutdown();
        }

        private void UpdateContentPresenter()
        {
            PageNavigatorContentHost = _contents.LastOrDefault().Value;
        }

        public void AddView(object view, string uniqueName)
        {
            if (_contents.ContainsKey(uniqueName))
                throw new Exception("Key Already exist");

            _contents.Add(uniqueName, view);
            UpdateContentPresenter();
        }

        public object GetView(string ViewName)
        {
            object findObject = null;
            _contents.TryGetValue(ViewName,out findObject);
            return findObject;
        }

        public void RemoveView(string ViewName)
        {
            if (_contents.ContainsKey(ViewName))
            {
                _contents.Remove(ViewName);
                UpdateContentPresenter();
            }
        }
    }
}
