using Common.Util;
using PresentationExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VirtualizingListView;

namespace VideoPlayerControl
{
    [TemplatePart(Name = "volslider", Type = typeof(Slider))]
    public class VolumeControl : Control
    {
        public double VolumeLevel
        {
            get { return (double)GetValue(VolumeLevelProperty); }
            set { SetValue(VolumeLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolumeLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeLevelProperty =
            DependencyProperty.Register("VolumeLevel", typeof(double), typeof(VolumeControl), new PropertyMetadata(100.0));


        static VolumeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeControl),
                new FrameworkPropertyMetadata(typeof(VolumeControl)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            currentvolumeslider = (Slider)this.GetTemplateChild("volslider");
        }
        private static Slider currentvolumeslider;

        public Slider CurrentVolumeSlider
        {
            get { return currentvolumeslider; }
        }

        public static Slider VolumeSlider
        {
            get { return currentvolumeslider; }
        }

    }
}
