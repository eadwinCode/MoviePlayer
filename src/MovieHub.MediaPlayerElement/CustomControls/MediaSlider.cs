using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MovieHub.MediaPlayerElement.CustomControls
{
    public class MediaSlider : Slider
    {
        public event DragStartedEventHandler ThumbDragStarted;
        public event DragDeltaEventHandler ThumbDragDelta;
        public event DragCompletedEventHandler ThumbDragCompleted;
        public event MouseButtonEventHandler MediaSliderOnMouseDown;

        static MediaSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaSlider),
                new FrameworkPropertyMetadata(typeof(MediaSlider)));

            EventManager.RegisterClassHandler(typeof(MediaSlider), Thumb.DragStartedEvent,
                new DragStartedEventHandler(MediaSlider.OnThumbDragStarted));
            EventManager.RegisterClassHandler(typeof(MediaSlider), Thumb.DragDeltaEvent,
                new DragDeltaEventHandler(MediaSlider.OnThumbDragDelta));
            EventManager.RegisterClassHandler(typeof(MediaSlider), Thumb.DragCompletedEvent,
                new DragCompletedEventHandler(MediaSlider.OnThumbDragCompleted));
        }

        private static void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = e.OriginalSource as Thumb;
            if (thumb == null)
                return;
            MediaSlider mediaSlider = sender as MediaSlider;
            if (mediaSlider.ThumbDragStarted != null)
                mediaSlider.ThumbDragStarted.Invoke(sender, e);
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.OriginalSource as Thumb;
            if (thumb == null)
                return;

            MediaSlider mediaSlider = sender as MediaSlider;
            if (mediaSlider.ThumbDragDelta != null)
                mediaSlider.ThumbDragDelta.Invoke(sender, e);
        }
        
        private static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = e.OriginalSource as Thumb;
            if (thumb == null)
                return;

            MediaSlider mediaSlider = sender as MediaSlider;
            if (mediaSlider.ThumbDragCompleted != null)
                mediaSlider.ThumbDragCompleted.Invoke(sender, e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (MediaSliderOnMouseDown != null)
                MediaSliderOnMouseDown.Invoke(this, e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (MediaSliderOnMouseDown != null)
                MediaSliderOnMouseDown.Invoke(this, e);
        }

    }
}
