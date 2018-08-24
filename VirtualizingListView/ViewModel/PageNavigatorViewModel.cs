using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using PresentationExtension;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VirtualizingListView.Pages;
using VirtualizingListView.View;

namespace VirtualizingListView.ViewModel
{
    internal class PageNavigatorViewModel
    {
        private INavigatorService navigatorService;
        private IEventManager IEventManager
        {
            get
            {

                return EventAggregatorService.IEventManager;
            }
        }
        public PageNavigatorViewModel(INavigatorService navigatorService)
        {
            this.navigatorService = navigatorService;
            ((PageNavigator)navigatorService).Loaded += PageNavigatorViewModel_Loaded;
            WindowCommandButton.OnWindowsCommandActivated += WindowCommandButton_OnWindowsCommandActivated;

            IEventManager.GetEvent<NavigateFolderItemToken>().Subscribe((videofolder) => {
                this.navigatorService.NavigationService.Navigate(new FilePageView(navigatorService.NavigationService), videofolder);
            });
           
            IEventManager.GetEvent<NavigateNewPage>().Subscribe((o) =>
            {
                navigatorService.NavigationService.Navigate(o);
            });

            IEventManager.GetEvent<NavigateSearchResult>().Subscribe((o) =>
            {
                navigatorService.NavigationService.Navigate(new SearchResultPage(navigatorService.NavigationService), o);
            });
        }

        private void WindowCommandButton_OnWindowsCommandActivated(object sender, System.Windows.RoutedEventArgs e)
        {
            HamburgerMenuIconItem hamburgerMenuIconItem = (HamburgerMenuIconItem)(sender as Button).DataContext;
            navigatorService.NavigationService.Navigate(hamburgerMenuIconItem.Tag);
        }

        private void PageNavigatorViewModel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           
        }
    }
}