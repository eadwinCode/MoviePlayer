using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieHub.MediaPlayerElement.Views
{
    /// <summary>
    /// Interaction logic for SavePlaylistDialog.xaml
    /// </summary>
    public partial class SavePlaylistDialog : UserControl
    {
        private MoviesPlaylistManager moviesPlaylistManager;
        public event EventHandler OnDialogFinished;
        public SavePlaylistDialog(MoviesPlaylistManager moviesPlaylistManager)
        {
            InitializeComponent();
            this.moviesPlaylistManager = moviesPlaylistManager;
            this.Loaded += (s, e) => {
                this.NewPlaylistName.Focus();
            };
            this.NewPlaylistName.TextChanged += NewPlaylistName_TextChanged;
        }

        private void NewPlaylistName_TextChanged(object sender, TextChangedEventArgs e)
        {
            okButton.IsEnabled = PlaylistName.Length > 5 ? true : false;
        }

        public string PlaylistName
        {
            get
            {
                return NewPlaylistName.Text;
            }
        }

        public void ShowDialog()
        {
            moviesPlaylistManager.mediaPlayer._contentdockregion.Content = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (OnDialogFinished != null)
                OnDialogFinished.Invoke(this, EventArgs.Empty);

            moviesPlaylistManager.mediaPlayer._contentdockregion.Content = null;

            moviesPlaylistManager.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnDialogFinished != null)
                OnDialogFinished.Invoke(null, EventArgs.Empty);

            moviesPlaylistManager.mediaPlayer._contentdockregion.Content = null;
            moviesPlaylistManager.Focus();

        }
    }
}
