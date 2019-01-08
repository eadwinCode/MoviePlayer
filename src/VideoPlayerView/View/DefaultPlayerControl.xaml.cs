using MovieHub.MediaPlayerElement;
using Movies.Models.Interfaces;
using Movies.Models.Model;
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
using VideoPlayerView.interfaces;

namespace VideoPlayerView.View
{
    /// <summary>
    /// Interaction logic for DefaultPlayerControl.xaml
    /// </summary>
    public partial class DefaultPlayerControl : MovieBase, IHomeControl
    {
        public DefaultPlayerControl()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
        }

        public IMovieControl MovieControl { get { return this.controller; } }
    }
}
