using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FlyoutControl.PageNagivatorService
{
    public class PageNavigator : ContentControl, INavigatorService
    {
        IMainPage _currentmainpage;
        private Frame _frame;
        IDictionary<int, IMainPage> pageownerTracker;
        
        public NavigationService NavigationService
        {
            get { return _frame.NavigationService; }
        }

        public Frame Host
        {
           get { return _frame; }
        }

        public PageNavigator()
        {
            _frame = new Frame() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            this.Content = _frame;
            this.NavigationService.LoadCompleted += NavigationService_LoadCompleted;
            pageownerTracker = new Dictionary<int, IMainPage>();
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            UIElement page = e.Content as UIElement;
            if (_currentmainpage.GetHashCode().ToString().Equals(page.Uid))
                return;

            IMainPage mainPage = FindOwner(page.Uid);
            if(mainPage != null && mainPage.FlyoutMenu != null)
            {
                _currentmainpage = mainPage;
                if (!mainPage.FlyoutMenu.IsSelected)
                    mainPage.FlyoutMenu.IsSelected = true;
                return;
            }
        }

        private IMainPage FindOwner(string uid)
        {
            IMainPage mainPage = null;
            pageownerTracker.TryGetValue(int.Parse(uid), out mainPage);
            return mainPage;
        }

        public void NavigatePage(object page, object pageData = null)
        {
            UIElement PagetoLoad = page as UIElement;
            if (_currentmainpage != null )
                PagetoLoad.Uid = _currentmainpage.GetHashCode().ToString();
            NavigationService.Navigate(page, pageData);
        }

        public void NavigateMainPage(IMainPage mainPage, object pageData = null)
        {
            if (mainPage == null)
                return;

            if (FlyoutMenu.FlyoutMenuInstance.CurrentSelection != null && mainPage.FlyoutMenu == null)
                mainPage.FlyoutMenu = FlyoutMenu.FlyoutMenuInstance.CurrentSelection;

            Add(mainPage);
            NavigatePage(mainPage);
        }
        

        private void Add(IMainPage mainPage)
        {
            if (!pageownerTracker.ContainsKey(mainPage.GetHashCode()))
            {
                pageownerTracker.Add(mainPage.GetHashCode(), mainPage);
            }

            _currentmainpage = mainPage;
        }
    }
}
