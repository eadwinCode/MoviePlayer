using Delimon.Win32.IO;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Movies.MoviePlaylistManager.Views;
using PresentationExtension.InterFaces;
using PresentationExtension.CommonEvent;
using System.Windows.Controls;

namespace Movies.MoviePlaylistManager.ViewModel
{
    public partial class PlaylistManagerViewModel
    {
        private bool isloading;

        public bool IsLoading
        {
            get { return isloading; }
            set { isloading = value; RaisePropertyChanged(() => this.IsLoading); }
        }

        private string sortedname;
        public string SortedName
        {
            get { return sortedname; }
            set { sortedname = value; RaisePropertyChanged(() => this.SortedName); }
        }

        private DelegateCommand nameCommand;
        public DelegateCommand NameSortCommand
        {
            get
            {
                if (nameCommand == null)
                    nameCommand = new DelegateCommand(() => {
                        SortFunction(SortType.Name);
                        (MediaControllerViewModel.IVideoElement as Window).Focus();
                    });
                return nameCommand;
            }
        }

        private DelegateCommand datesortcommand;
        public DelegateCommand DateSortCommand
        {
            get
            {
                if (datesortcommand == null)
                    datesortcommand = new DelegateCommand(() => {
                        SortFunction(SortType.Date);
                        (MediaControllerViewModel.IVideoElement as Window).Focus();
                    });
                return datesortcommand;
            }
        }

        private DelegateCommand extsortcommand;
        public DelegateCommand ExtSortCommand
        {
            get
            {
                if (extsortcommand == null)
                    extsortcommand = new DelegateCommand(() => {
                        SortFunction(SortType.Extension);
                        (MediaControllerViewModel.IVideoElement as Window).Focus();
                    });
                return extsortcommand;
            }
        }

        private DelegateCommand okcommand;
        public DelegateCommand OkCommand
        {
            get
            {
                if (okcommand == null)
                    okcommand = new DelegateCommand(() => {
                        SavePlaylistAction();
                        MediaControllerViewModel.IVideoElement.ContentDockRegion.Content = null;
                        (MediaControllerViewModel.IVideoElement.IVideoPlayerController as UserControl).Focus();
                    },CanOkAction);
                return okcommand;
            }
        }

        private DelegateCommand cancelcommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (cancelcommand == null)
                    cancelcommand = new DelegateCommand(() => {
                        TempPlaylistName = string.Empty;
                        MediaControllerViewModel.IVideoElement.ContentDockRegion.Content = null;
                        (MediaControllerViewModel.IVideoElement.IVideoPlayerController as UserControl).Focus();
                    });
                return cancelcommand;
            }
        }


        private bool CanOkAction()
        {
            return TempPlaylistName != string.Empty;
        }


        private void SortFunction(SortType sortType)
        {
            SortedName = sortType.ToString();
            this.PlayListCollection = FileLoader.SortList(sortType, PlayListCollection);
        }

        IFileExplorerCommonHelper FileExplorerCommonHelper
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileExplorerCommonHelper>();
            }
        }
        public IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        IBackgroundService BackgroundService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IBackgroundService>();
            }
        }
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

        IDispatcherService DispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        IMediaControllerViewModel MediaControllerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>().MediaControllerViewModel;
            }
        }

        IEventManager IEventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }


    }

}
