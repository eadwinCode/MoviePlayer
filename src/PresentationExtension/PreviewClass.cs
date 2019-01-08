using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Movies.Models.Model;
using Movies.Enums;

namespace PresentationExtension
{
    public class VideoPreviewHandler : Control
    {
        public VideoPreviewHandler()
        {

        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {

            MediaFile videodata = DataContext as MediaFile;
            if (videodata == null)
                return;

            if (ToolTip.ToString() == "Loading.." && videodata.FileType != GroupCatergory.Grouped)
            {

                string path = videodata.FilePath;

                //System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
                MediaElement mediaelemnt = new MediaElement();
                mediaelemnt.MediaOpened += new RoutedEventHandler(MediaElement_Loaded);
                mediaelemnt.Loaded += new RoutedEventHandler(MediaElement_Loaded_1);
                mediaelemnt.MediaEnded += mediaelemnt_MediaEnded;
                mediaelemnt.LoadedBehavior = MediaState.Manual;
                mediaelemnt.Volume = 100;
                mediaelemnt.Source = new Uri(path, UriKind.Relative);
                mediaelemnt.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                mediaelemnt.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                Grid grd = new Grid();
                grd.Height = (double)250;
                grd.Width = 420;

                grd.Children.Add(mediaelemnt);
                this.ToolTip = grd;
            }
        }

        void mediaelemnt_MediaEnded(object sender, RoutedEventArgs e)
        {
            ToolTipService.SetShowDuration(this, 3);
        }

        private void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaElement preview = sender as MediaElement;
                double duration = preview.NaturalDuration.TimeSpan.TotalSeconds;
                duration = duration * .25;
                preview.Position = TimeSpan.FromSeconds(duration);
            }
            catch(Exception ){

            }
        }

        private void MediaElement_Loaded_1(object sender, RoutedEventArgs e)
        {
            MediaElement preview = sender as MediaElement;
            preview.Play();
        }

        protected override void OnToolTipClosing(ToolTipEventArgs e)
        {
            MediaFolder videodata = DataContext as MediaFolder;
            if (videodata == null)
                return;

            if (videodata.FileType != GroupCatergory.Grouped)
            {
                this.ToolTip = "Loading..";
            }
        }
    }
}
