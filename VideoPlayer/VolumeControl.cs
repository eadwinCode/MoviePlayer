using Common.Util;
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
    public class VolumeControl : Control
    {
        static VolumeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeControl),
                new FrameworkPropertyMetadata(typeof(VolumeControl)));
            
        }
        private ContextButton ContextButton;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CurrentVolumeSlider = (Slider)this.Template.FindName("volslider", this);
        }
        private static Slider currentvolumeslider;

        public static Slider CurrentVolumeSlider
        {
            get { return currentvolumeslider; }
            set { currentvolumeslider = value; }
        }

    }
}
