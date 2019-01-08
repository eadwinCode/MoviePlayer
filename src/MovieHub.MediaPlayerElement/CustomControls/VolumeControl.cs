using Common.Util;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MovieHub.MediaPlayerElement
{
    public enum VolumeControlViewType
    {
        DefaultView,
        SmallView,
    };

    [TemplatePart(Name = "volslider", Type = typeof(Slider))]
    public class VolumeControl : Control
    {
        
        private static RoutedCommand _togglepopUp = new RoutedCommand("TogglepopUp", typeof(VolumeControl));
        public static RoutedCommand TogglepopUp
        {
            get { return _togglepopUp; }
        }

        private static void RegisterDefaultCommands()
        {
            MovieControl.RegisterCommandBings(typeof(VolumeControl), VolumeControl.TogglepopUp, new CommandBinding(VolumeControl.TogglepopUp, TogglepopUp_Executed, TogglepopUp_Enabled));
        }

        private static void TogglepopUp_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            VolumeControl volumecontrol = sender as VolumeControl;
            if (volumecontrol != null && e.Parameter != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private static void TogglepopUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            VolumeControl volumecontrol = sender as VolumeControl;
            if (volumecontrol != null && e.Parameter != null)
            {
                Popup popup = e.Parameter as Popup;
                if(popup!= null)
                {
                    popup.IsOpen = true;
                }
            }
        }

        #region Dependency Property

        public static readonly DependencyProperty VolumeControlStyleProperty =
            DependencyProperty.RegisterAttached("VolumeControlStyle",
       typeof(VolumeControlViewType), typeof(VolumeControl), new FrameworkPropertyMetadata
       {
           DefaultValue = VolumeControlViewType.DefaultView
       });


        public static void SetVolumeControlStyle(UIElement element, VolumeControlViewType value)
        {
            element.SetValue(VolumeControlStyleProperty, value);
        }
        public static VolumeControlViewType GetVolumeControlStyle(UIElement element)
        {
            return (VolumeControlViewType)element.GetValue(VolumeControlStyleProperty);
        }

        public VolumeState VolumeState
        {
            get { return (VolumeState)GetValue(VolumeStateProperty); }
            internal set { SetValue(VolumeStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMuted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeStateProperty =
            DependencyProperty.Register("VolumeState", typeof(VolumeState), typeof(VolumeControl), new FrameworkPropertyMetadata(VolumeState.Active));



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
            DependencyProperty.Register("VolumeLevel", typeof(double), typeof(VolumeControl), new PropertyMetadata(50.0, OnVolumeLevelChanged, OnVolumeLevelCoerceValueCallback));

        private static object OnVolumeLevelCoerceValueCallback(DependencyObject d, object baseValue)
        {
            VolumeControl volumeControl = d as VolumeControl;
            double value = (double)baseValue;
            if (value < 0)
                return 0.0;

            if (value > 200)
                return 200.0;

            return baseValue;
        }

        private static void OnVolumeLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VolumeControl volumeControl = d as VolumeControl;
            if (volumeControl != null)
            {
                RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue)
                {
                    RoutedEvent = VolumeControlValueChangedEvent
                };
                volumeControl.RaiseEvent(args);
            }
        }

        #endregion

        static VolumeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeControl),new FrameworkPropertyMetadata(typeof(VolumeControl)));
            RegisterDefaultCommands();
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
        private VolumeControlViewType controlviewtype;

        public Slider CurrentVolumeSlider
        {
            get { return currentvolumeslider; }
        }

        public VolumeControlViewType ControlViewType
        {
            get { return controlviewtype; }
            set
            {
                controlviewtype = value;
                SetVolumeControlStyle(this, value);
            }
        }
    }
}
