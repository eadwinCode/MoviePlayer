using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VideoPlayer
{
    public class VolumeControl : Control
    {
        static VolumeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeControl),
                new FrameworkPropertyMetadata(typeof(VolumeControl)));

            CurrentVolumeSlider = VolumeSliderPart.VolumeSlider;
        }


        private static Slider currentvolumeslider;

        public static Slider CurrentVolumeSlider
        {
            get { return currentvolumeslider; }
            set { currentvolumeslider = value; }
        }

    }
}
