using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
    public class MovieControlSettings: DependencyObject
    {
        internal MovieControl MovieControl;
        public bool IsMinimizeControlButtonEnabled
        {
            get { return (bool)GetValue(IsMinimizeControlButtonEnabledProperty); }
            set { SetValue(IsMinimizeControlButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMinimizeControlButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMinimizeControlButtonEnabledProperty =
            DependencyProperty.Register("IsMinimizeControlButtonEnabled", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true));



        public bool IsControlMediaCloseButtonEnabled
        {
            get { return (bool)GetValue(IsControlMediaCloseButtonEnabledProperty); }
            set { SetValue(IsControlMediaCloseButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsControlMediaCloseButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsControlMediaCloseButtonEnabledProperty =
            DependencyProperty.Register("IsControlMediaCloseButtonEnabled", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(false));


        public bool ShowTimerSeperator
        {
            get { return (bool)GetValue(HideTimerSeperatorProperty); }
            private set { SetValue(HideTimerSeperatorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideTimerSeperator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideTimerSeperatorProperty =
            DependencyProperty.Register("ShowTimerSeperator", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true));



        public bool MediaDurationDisplayVisible
        {
            get { return (bool)GetValue(MediaDurationDisplayVisibleProperty); }
            set { SetValue(MediaDurationDisplayVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaDurationDisplayVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaDurationDisplayVisibleProperty =
            DependencyProperty.Register("MediaDurationDisplayVisible", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true, OnMediaDurationDisplayVisibleChanged));

        private static void OnMediaDurationDisplayVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovieControlSettings MovieControlSettings = d as MovieControlSettings;
            if (MovieControlSettings != null)
            {
                MovieControlSettings.ShowTimerSeperator = MovieControlSettings.CurrentTimerDisplayVisible && MovieControlSettings.MediaDurationDisplayVisible ? true : false;
            }
        }

        public bool CurrentTimerDisplayVisible
        {
            get { return (bool)GetValue(CurrentTimerDisplayVisibleProperty); }
            set { SetValue(CurrentTimerDisplayVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimerDisplayVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimerDisplayVisibleProperty =
            DependencyProperty.Register("CurrentTimerDisplayVisible", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true, OnMediaDurationDisplayVisibleChanged));



        public bool DisableMovieBoardText
        {
            get { return (bool)GetValue(DisableMovieBoardTextProperty); }
            set { SetValue(DisableMovieBoardTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisableMovieBoardText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableMovieBoardTextProperty =
            DependencyProperty.Register("DisableMovieBoardText", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true, OnDisableMovieBoardTextChanged));

        private static void OnDisableMovieBoardTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovieControlSettings controlsettings = d as MovieControlSettings;
            if (controlsettings != null)
            {
                controlsettings.MovieControl.IntiSliderPosition((bool)e.NewValue);
            }
        }

        public bool IsPreviousButtonEnabled
        {
            get { return (bool)GetValue(IsPreviousButtonEnabledProperty); }
            set { SetValue(IsPreviousButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPreviousButtonEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPreviousButtonEnabledProperty =
            DependencyProperty.Register("IsPreviousButtonEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(true));

        public bool IsMediaSliderEnabled
        {
            get { return (bool)GetValue(IsMediaSliderEnabledProperty); }
            set { SetValue(IsMediaSliderEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaSliderEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaSliderEnabledProperty =
            DependencyProperty.Register("IsMediaSliderEnabled", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true));

        public bool IsMediaOptionToggleEnabled
        {
            get { return (bool)GetValue(IsMediaOptionToggleEnabledProperty); }
            set { SetValue(IsMediaOptionToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMediaOptionToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaOptionToggleEnabledProperty =
            DependencyProperty.Register("IsMediaOptionToggleEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(true));



        public bool IsPlaylistToggleEnabled
        {
            get { return (bool)GetValue(IsPlaylistToggleEnabledProperty); }
            set { SetValue(IsPlaylistToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaylistToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlaylistToggleEnabledProperty =
            DependencyProperty.Register("IsPlaylistToggleEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(true));



        public bool IsRepeatToggleEnabled
        {
            get { return (bool)GetValue(IsRepeatToggleEnabledProperty); }
            set { SetValue(IsRepeatToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRepeatToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepeatToggleEnabledProperty =
            DependencyProperty.Register("IsRepeatToggleEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(false));



        public bool IsFullScreenToggleEnabled
        {
            get { return (bool)GetValue(IsFullScreenToggleEnabledProperty); }
            set { SetValue(IsFullScreenToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFullScreenToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFullScreenToggleEnabledProperty =
            DependencyProperty.Register("IsFullScreenToggleEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(true));
        
        public bool IsShowMenuToggleEnabled
        {
            get { return (bool)GetValue(IsShowMenuToggleEnabledProperty); }
            set { SetValue(IsShowMenuToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowMenuToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowMenuToggleEnabledProperty =
            DependencyProperty.Register("IsShowMenuToggleEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(true));



        public bool IsVolumeControlEnabled
        {
            get { return (bool)GetValue(IsVolumeControlEnabledProperty); }
            set { SetValue(IsVolumeControlEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolumeControlEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVolumeControlEnabledProperty =
            DependencyProperty.Register("IsVolumeControlEnabled", typeof(bool), typeof(MovieControlSettings), new PropertyMetadata(true));



        public bool IsNextButtonEnabled
        {
            get { return (bool)GetValue(IsNextButtonEnabledProperty); }
            set { SetValue(IsNextButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNextButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNextButtonEnabledProperty =
            DependencyProperty.Register("IsNextButtonEnabled", typeof(bool), typeof(MovieControlSettings), new FrameworkPropertyMetadata(true));

    }
}
