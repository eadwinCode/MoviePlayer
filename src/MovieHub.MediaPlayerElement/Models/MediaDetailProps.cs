using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
    public class MediaDetailProps:DependencyObject
    {
        public const string DefaultMediaText = "- No Media available -";
        internal string movieBoardInfo = DefaultMediaText;

        public TimeSpan LastSeenTextInfo
        {
            get { return (TimeSpan)GetValue(LastSeenTextInfoProperty); }
            set { SetValue(LastSeenTextInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastSeenTextInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastSeenTextInfoProperty =
            DependencyProperty.Register("LastSeenTextInfo", typeof(TimeSpan), typeof(MediaDetailProps), new PropertyMetadata(null));


        public double MediaDuration
        {
            get { return (double)GetValue(MediaDurationProperty); }
            set { SetValue(MediaDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaDurationProperty =
            DependencyProperty.Register("MediaDuration", typeof(double), typeof(MediaDetailProps),
                new FrameworkPropertyMetadata(0.00000000001));



        public double CurrentMediaTime
        {
            get { return (double)GetValue(CurrentMediaTimeProperty); }
            set { SetValue(CurrentMediaTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMediaTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMediaTimeProperty =
            DependencyProperty.Register("CurrentMediaTime", typeof(double), typeof(MediaDetailProps), new PropertyMetadata(0.0));


        public RepeatMode RepeatMode
        {
            get { return (RepeatMode)GetValue(RepeatModeProperty); }
            set { SetValue(RepeatModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RepeatMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepeatModeProperty =
            DependencyProperty.Register("RepeatMode", typeof(RepeatMode), typeof(MediaDetailProps), new PropertyMetadata(RepeatMode.NoRepeat));


        public bool IsLastSeenEnabled
        {
            get { return (bool)GetValue(IsLastSeenEnabledProperty); }
            internal set { SetValue(IsLastSeenEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLastSeenEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLastSeenEnabledProperty =
            DependencyProperty.Register("IsLastSeenEnabled", typeof(bool), typeof(MediaDetailProps),
                new FrameworkPropertyMetadata(false));

    }
}
