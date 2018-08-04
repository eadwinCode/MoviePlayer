using Common.FileHelper;
using Common.Interfaces;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoComponent.Command;
using VirtualizingListView.Pages;
using VirtualizingListView.Pages.Model;
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
            HamburgerMenuIconItem hamburgerMenuIconItem = (HamburgerMenuIconItem)(sender as Button).DataContext;
            navigatorService.NavigationService.Navigate(hamburgerMenuIconItem.Tag);
        }

        private void PageNavigatorViewModel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           
        }

        
    }
}