using System;
using System.Windows;
using System.Windows.Controls;

namespace MovieHub.MediaPlayerElement
{
    internal class MovieTitleBar : Control
    {
        static MovieTitleBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MovieTitleBar),
                new FrameworkPropertyMetadata(typeof(MovieTitleBar)));
        }
        
        public bool HasSecondText
        {
            get { return (bool)GetValue(HasSecondTextProperty); }
            set { SetValue(HasSecondTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasSecondText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasSecondTextProperty =
            DependencyProperty.Register("HasSecondText", typeof(bool), typeof(MovieTitleBar), new PropertyMetadata(false));
        
        public string MovieTitleText
        {
            get { return (string)GetValue(MovieTitleTextProperty); }
            set { SetValue(MovieTitleTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MovieTitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MovieTitleTextProperty =
            DependencyProperty.Register("MovieTitleText", typeof(string), typeof(MovieTitleBar), new PropertyMetadata("- MovieBoard Text -"));
        
        public string SecondMovieBarText
        {
            get { return (string)GetValue(SecondMovieBarTextProperty); }
            set { SetValue(SecondMovieBarTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondMovieBarText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondMovieBarTextProperty =
            DependencyProperty.Register("SecondMovieBarText", typeof(string), typeof(MovieTitleBar), new PropertyMetadata(null, OnSecondMovieBarTextChanged));

        private static void OnSecondMovieBarTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovieTitleBar movieTitleBar = d as MovieTitleBar;
            if(movieTitleBar != null)
            {
                movieTitleBar.HasSecondText = string.IsNullOrEmpty(movieTitleBar.SecondMovieBarText) ? false : true;
            }
        }

        public MovieTitleBar()
        {
            
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

       
    }

}
