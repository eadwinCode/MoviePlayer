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

namespace Movies.MoviePlaylistManager.Views
{
    /// <summary>
    /// Interaction logic for SavePlaylistDialog.xaml
    /// </summary>
    public partial class SavePlaylistDialog : UserControl
    {
       
        public SavePlaylistDialog()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                this.NewPlaylistName.Focus();
            };
        }

        public void ShowDialog()
        {
          //  FilePlayManager.VideoElement.ContentDockRegion.Content = this;
        }
    }
}
