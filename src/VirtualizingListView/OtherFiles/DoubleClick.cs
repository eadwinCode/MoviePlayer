﻿using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using VirtualizingListView.View;

namespace VirtualizingListView
{
    public class DoubleClick : ICommand
    {
        public DoubleClick()
        {

        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var videofolder = parameter as VideoFolder;
            if (videofolder.Directory.Exists)
            {
                //CollectionViewModel.Instance.OnVideoItemSelected(videofolder);
            }
            else
            {
                Iplayfile.PlayFileInit(parameter as IVideoData);
            }
        }

        private IPlayFile Iplayfile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }
    }
}
