﻿using LocalVideoFiles.AddFolderDialogWindow;
using LocalVideoFiles.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace LocalVideoFiles.ViewModels
{
    internal class HomePageLocalViewModel:NotificationObject
    {
        private IMainPage PageOwner;
        private static bool isLoadingData;
        //private static VideoFolder HomePageFolderHost;

        //public bool HasSearchData()
        //{
        //    if(ApplicationService.AppSettings.MovieFolders.Count == 0)
        //    {
        //        return true;
        //    }
        //    return isLoadingData;
        //}
        private IEventManager IEventManager
        {
            get
            {
               
                return EventAggregatorService.IEventManager;
            }
        }

        public ObservableCollection<MediaFolder> AllFoldersList
        {
            get
            {
                if (MovieDataSource == null) return null;
                return MovieDataSource.AllFoldersList;
            }
        }

        public bool IsAllDataLoaded
        {
            get { return isLoadingData; }
            set
            {
                isLoadingData = value;
                RaisePropertyChanged(() => this.IsAllDataLoaded);
            }
        }

        public DelegateCommand AddFolderCommand { get; private set; }
        public DelegateCommand<MediaFolder> OpenFolderCommand { get; private set; }

        public HomePageLocalViewModel(IMainPage owner)
        {
            PageOwner = owner;
            AddFolderCommand = new DelegateCommand(AddFolderCommandAction);
            OpenFolderCommand = new DelegateCommand<MediaFolder>(OpenFolderCommandAction);

            IEventManager.GetEvent<NoFolderNoticeEventToken>().Subscribe((o) => 
            {
                HasNoFolderAction();
            });

            IEventManager.GetEvent<FolderItemChangeEventToken>().Subscribe((o) =>
            {
                RaisePropertyChanged(() => this.AllFoldersList);
            });

        }

        private void Testing(bool obj)
        {
            //throw new NotImplementedException();
        }

        public void InitDataSource()
        {
            if (!IsAllDataLoaded)
            {
                MovieDataSource.InitFileLoading();
                IsAllDataLoaded = true;
            }
            //if(!MovieDataSource.HasDataSource && !MovieDataSource.IsLoadingData)
                
        }

        private void OpenFolderCommandAction(MediaFolder obj)
        {
            IEventManager.GetEvent<NavigateFolderItemToken>().Publish(obj);
        }

        private void AddFolderCommandAction()
        {
            AddFolderDialog addFolderDialog = new AddFolderDialog();
            addFolderDialog.OnFinished += AddFolderDialog_OnFinished;
            addFolderDialog.ShowDialog();
        }

        private void AddFolderDialog_OnFinished(object sender, EventArgs e)
        {
            var addfiledialogVM = ((sender as AddFolderDialog).DataContext as AddFolderDialogViewModel);
            if (addfiledialogVM.FileHasChange)
            {
                ApplicationService.SaveMovieFolders();
                MovieDataSource.LoadAllFolders(addfiledialogVM.RemovedFolders);
            }
            HasNoFolderAction();
        }

        private void HasNoFolderAction()
        {
            var moviefolderlist = ApplicationService.AppSettings.MovieFolders;
            if (moviefolderlist != null && moviefolderlist.Count == 0)
            {
                HomePageLocalNoFolder homePageLocalNoFolder = new HomePageLocalNoFolder(new NewFolderModel(), PageOwner.Docker);
                homePageLocalNoFolder.OnFinished += HomePageLocalNoFolder_OnFinished;
                homePageLocalNoFolder.Show();
            }
        }

        private void HomePageLocalNoFolder_OnFinished(object sender, RoutedEventArgs e)
        {
            if (e == null) return;
            NewFolderModel newFolderModel = e.Source as NewFolderModel;
            if (newFolderModel.NewFolderCollection == null) return;
            MovieDataSource.LoadAllFolders();
            ApplicationService.SaveMovieFolders();
        }

        IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }
        IDataSource<MediaFolder> MovieDataSource
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataSource<MediaFolder>>();
            }
        }
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }
    }
}