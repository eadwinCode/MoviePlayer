using Movies.Models.Interfaces;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using SearchComponent.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SearchComponent.ViewModels
{
    public class SearchPageBtnViewModel
    {
        public RoutedUICommand SearchCommand { get; private set; }
        IEventManager EventManager;
        IFileLoader fileLoader;
        private bool IsOnSearchPage;

        public SearchPageBtnViewModel(IEventManager ieventmanager, IFileLoader fileLoader)
        {
            this.EventManager = ieventmanager;
            this.fileLoader = fileLoader;
            SearchCommand = new RoutedUICommand();
        }

        public void SearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!fileLoader.HasDataSource)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }

        public void SearchCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsOnSearchPage) return;
            SearchPage searchPage = SearchPage.GetSearchPage();
            searchPage.OnFinished += SearchPage_OnFinished;
            searchPage.Unloaded += SearchPage_Unloaded;
            IsOnSearchPage = true;
            searchPage.ShowDialog();
        }

        private void SearchPage_Unloaded(object sender, RoutedEventArgs e)
        {
            IsOnSearchPage = false;
        }

        private void SearchPage_OnFinished(object sender, EventArgs e)
        {
            if (sender != null)
            {
                EventManager.GetEvent<NavigateSearchResult>().Publish(sender);
            }
            //HasAction = false;
            IsOnSearchPage = false;
        }
    }
}
