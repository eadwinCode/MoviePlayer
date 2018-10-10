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
        
        public PageNavigatorViewModel(INavigatorService navigatorService)
        {
            this.navigatorService = navigatorService;
            ((PageNavigator)navigatorService).Loaded += PageNavigatorViewModel_Loaded;
            WindowCommandButton.OnWindowsCommandActivated += WindowCommandButton_OnWindowsCommandActivated;

          
        }

        private void WindowCommandButton_OnWindowsCommandActivated(object sender, System.Windows.RoutedEventArgs e)
        {
            //HamburgerMenuIconItem hamburgerMenuIconItem = (HamburgerMenuIconItem)(sender as Button).DataContext;
            //navigatorService.NavigationService.Navigate(hamburgerMenuIconItem.Tag);
        }

        private void PageNavigatorViewModel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           
        }
    }
}