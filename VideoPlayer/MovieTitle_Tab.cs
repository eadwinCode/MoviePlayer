using Common.Util;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VideoPlayer.ViewModel;
using WPF.JoshSmith.Controls;

namespace VideoPlayer
{
    public class MovieTitle_Tab:Control,INotifyPropertyChanged
    {
        #region TobOrganisedLater
        //private DragCanvas canvas;
        private static string movietitle = "- Movie Title -";

        public static readonly DependencyProperty IsMouseDownProperty = DependencyProperty.RegisterAttached("IsMouseDown",
       typeof(bool), typeof(MovieTitle_Tab), new FrameworkPropertyMetadata(false));

        

        public static void SetIsMouseDown(UIElement element, bool value)
        {
            element.SetValue(IsMouseDownProperty, value);
        }
        public static bool GetIsMouseDown(UIElement element)
        {
            return (bool)element.GetValue(IsMouseDownProperty);
        }

        public bool IsCanvasDrag
        {
            get { return (bool)GetValue(IsCanvasDragProperty); }
            set
            {
                SetValue(IsCanvasDragProperty, value);
                mediaControllervm.MainControl_MouseLeave(new object(), null);
            }
        }

        // Using a DependencyProperty as the backing store for IsCanvasDrag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCanvasDragProperty =
            DependencyProperty.Register("IsCanvasDrag", typeof(bool), typeof(MovieTitle_Tab), new PropertyMetadata(false));

        public TextBlock MovieTitle
        {
            get
            {
                return MediaControlExtension.WindowsTitleBoard;
            }
        }

        public string MovieTitleText { get { return movietitle; }

            set { movietitle = value;
                MovieTitle.Text = value;
                OnProperChanged("MovieTitleText"); } }

        public string MovieText
        {
            get { return movietext; }
            set { movietext = value; OnProperChanged("MovieText"); OnProperChanged("ShowOtherText"); }
        }
        public Visibility ShowOtherText
        {
            get
            {
                if (MovieText != null)
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        private static MarqueeTextBlock tbmarquee;
        private static TextBlock tbmarqueecheck;
        private static Canvas tbmarqueeCanvas;
        private double _marqueeTimeInSeconds;

        public double MarqueeTimeInSeconds
        {
            get { return _marqueeTimeInSeconds; }
            set { _marqueeTimeInSeconds = value; }
        }

        static MovieTitle_Tab()
        {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(MovieTitle_Tab), 
                    new FrameworkPropertyMetadata(typeof(MovieTitle_Tab)));
        }

        public MovieTitle_Tab()
        {
            // TODO: Complete member initialization
           
            this.Loaded += MovieTitle_Tab_Loaded; this.DataContext = this;
            this.ToolTipOpening += MovieTitle_Tab_ToolTipOpening;
            
        }

        private void MovieTitle_Tab_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            this.ToolTip = MovieTitleText;
        }

        void MovieTitle_Tab_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           // this.MouseDown += MovieTitle_Tab_MouseDown;
            //this.canvas = MediaControllerVM.Current.IVideoPlayer.CanvasEnvironment as DragCanvas;
            this.mediaControllervm = MediaControllerVM.MediaControllerInstance;
           // MarqueeTimeInSeconds = 15;
           // RightToLeftMarquee();

        }


        //private void MovieTitle_Tab_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    IsCanvasDrag = true;
        //    canvas.DragCanvas_OnPreviewMouseLeftButtonDown(sender, e);
        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
             tbmarquee = (MarqueeTextBlock)this.Template.FindName("tbmarquee", this);
            tbmarqueecheck = (TextBlock)this.Template.FindName("tbmarqueecheck", this);
            tbmarqueeCanvas = (Canvas)this.Template.FindName("tbmarqueeCanvas", this);
        }

        private bool CalculateIsTextTrimmed()
        {
            if (mediaControllervm.CanAnimate && this.Name=="scMovieBoard")
            {
                return false;
            }
            return tbmarquee.IsTextTrimmed;
            //Typeface typeface = new Typeface(
            //    tbmarqueecheck.FontFamily,
            //    tbmarqueecheck.FontStyle,
            //    tbmarqueecheck.FontWeight,
            //    tbmarqueecheck.FontStretch);
            //FormattedText formattedText = new FormattedText(
            //    tbmarqueecheck.Text,
            //    System.Threading.Thread.CurrentThread.CurrentCulture,
            //    tbmarqueecheck.FlowDirection,
            //    typeface, 
            //    tbmarqueecheck.FontSize,
            //    tbmarqueecheck.Foreground);
            //formattedText.MaxTextWidth = tbmarqueecheck.ActualWidth;
            //double width = tbmarqueecheck.ActualWidth;
            //tbmarqueecheck.Measure(new Size(double.MaxValue, double.MaxValue));
            //double totalWidth = tbmarqueecheck.DesiredSize.Width;
            //return width < totalWidth;
            //return formattedText.Height >tbmarqueecheck.ActualHeight||formattedText.MinWidth >formattedText.MaxTextWidth;
        }

        private void RightToLeftMarquee()
        {
            if (!IsLoaded) { return; }
            if (CalculateIsTextTrimmed())
            {
              //  IstbmarqueeVisible = Visibility.Collapsed;
                double height = tbmarqueeCanvas.ActualHeight - tbmarquee.ActualHeight;
                tbmarquee.Margin = new Thickness(0, 0, 0, 0);
                DoubleAnimation doubleAnimation = new DoubleAnimation
                {
                    From = -tbmarquee.ActualWidth,
                    To = tbmarqueeCanvas.ActualWidth,
                    RepeatBehavior = RepeatBehavior.Forever,
                    Duration = new Duration(TimeSpan.FromSeconds(_marqueeTimeInSeconds))
                };
                tbmarquee.BeginAnimation(Canvas.RightProperty, doubleAnimation);
            }
            else
            {
               // IstbmarqueeVisible = Visibility.Visible;
                tbmarquee.BeginAnimation(Canvas.RightProperty, null);
            }

        }
        //private Visibility istbmarqueevisible;

        //public Visibility IstbmarqueeVisible
        //{
        //    get { return istbmarqueevisible; }
        //    set { istbmarqueevisible = value; OnProperChanged("IstbmarqueeVisible"); }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private MediaControllerVM mediaControllervm;
        private static string movietext;
        private bool istrimmed;

        public bool IsTextTrimmed { get { return istrimmed; }
            set {
                istrimmed = value;
               // RightToLeftMarquee();
            } }

        protected void OnProperChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }

}
