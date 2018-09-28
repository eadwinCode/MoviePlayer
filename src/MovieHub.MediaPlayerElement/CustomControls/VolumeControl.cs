using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MovieHub.MediaPlayerElement
{
    [TemplatePart(Name = "volslider", Type = typeof(Slider))]
    public class VolumeControl : Control
    {
        /// <summary>
        /// VolumeControlValueChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent VolumeControlValueChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "VolumeControlValueChanged",
                          RoutingStrategy.Bubble,
                          typeof(RoutedPropertyChangedEventHandler<double>),
                          typeof(VolumeControl));

        /// <summary>
        /// Raised VolumeControl level Changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> VolumeControlValueChanged
        {
            add { AddHandler(VolumeControlValueChangedEvent, value); }
            remove { RemoveHandler(VolumeControlValueChangedEvent, value); }
        }

        public double VolumeLevel
        {
            get { return (double)GetValue(VolumeLevelProperty); }
            internal set { SetValue(VolumeLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolumeLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeLevelProperty =
            DependencyProperty.Register("VolumeLevel", typeof(double), typeof(VolumeControl), new PropertyMetadata(50.0,OnVolumeLevelChanged));

        private static void OnVolumeLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VolumeControl volumeControl = d as VolumeControl;
            if(volumeControl != null)
            {
                RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue)
                {
                    RoutedEvent = VolumeControlValueChangedEvent
                };
                volumeControl.RaiseEvent(args);
            }
        }

        static VolumeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeControl),new FrameworkPropertyMetadata(typeof(VolumeControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            currentvolumeslider = (Slider)this.GetTemplateChild("volslider");
            //currentvolumeslider.ValueChanged += CurrentVolumeSlider_ValueChanged;
            currentvolumeslider.MouseDown += Currentvolumeslider_MouseDown;
            currentvolumeslider.PreviewMouseDown += Currentvolumeslider_MouseDown;
        }

        private void Currentvolumeslider_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Slider slider = (Slider)sender;
            var ratio = CommonHelper.GetMousePointer(slider);
            VolumeLevel = Math.Round(ratio * slider.Maximum, 2);
        }
        
        private Slider currentvolumeslider;

        public Slider CurrentVolumeSlider
        {
            get { return currentvolumeslider; }
        }
        
    }
}
