using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.PlaylistCollectionView.RenameDialog;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Movies.PlaylistCollectionView.Views
{
    /// <summary>
    /// Interaction logic for HomePlaylistView.xaml
    /// </summary>
    public partial class HomePlaylistView : Page, IMainPage
    {
        public const string ViewName = "HomePlaylistView";
        IWindowsCommandButton controller;

        public HomePlaylistView()
        {
            InitializeComponent();
            this.Loaded += PlaylistTree_Loaded;
        }

        private void PlaylistTree_Loaded(object sender, RoutedEventArgs e)
        {
            if (controller != null)
                controller.SetActive(true, false);
        }

        public bool HasController { get { return controller != null; } }

        public ContentControl Docker { get { return null; } }

       
        public void SetController(IWindowsCommandButton controller)
        {
            this.controller = controller;
        }
    }
}
