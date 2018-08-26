using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Common.Util
{

    public class VolumeSliderPart
    {
        public static Slider VolumeSlider;
        public static readonly DependencyProperty VolumeSliderProperty = DependencyProperty.RegisterAttached("VolumeSlider",
             typeof(bool), typeof(VolumeSliderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNotifyVolumeSliderPropertyChanged)));

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(VolumeSliderProperty, value);
        }
        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(VolumeSliderProperty);
        }

        private static void OnNotifyVolumeSliderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            VolumeSlider = (Slider)element;
        }

    }

    public class ProgressSliderPart
    {
        public static Slider ProgressSlider;
        public static readonly DependencyProperty ProgressSliderProperty = DependencyProperty.RegisterAttached("ProgressSlider",
             typeof(bool), typeof(ProgressSliderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNotifyVolumeSliderPropertyChanged)));

        public static void SetProgressSlider(UIElement element, bool value)
        {
            element.SetValue(ProgressSliderProperty, value);
        }
        public static bool GetProgressSlider(UIElement element)
        {
            return (bool)element.GetValue(ProgressSliderProperty);
        }

        private static void OnNotifyVolumeSliderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            ProgressSlider = (Slider)element;
        }

    }

    public class DragProgressSliderPart
    {
        public static Slider ProgressSlider;
        public static readonly DependencyProperty ProgressSliderProperty = DependencyProperty.RegisterAttached("ProgressSlider",
             typeof(bool), typeof(DragProgressSliderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNotifyVolumeSliderPropertyChanged)));

        public static void SetProgressSlider(UIElement element, bool value)
        {
            element.SetValue(ProgressSliderProperty, value);
        }
        public static bool GetProgressSlider(UIElement element)
        {
            return (bool)element.GetValue(ProgressSliderProperty);
        }

        private static void OnNotifyVolumeSliderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            ProgressSlider = (Slider)element;
        }

    }

    public class ProgressSlider:Slider
    {
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            e.Handled = false;
        }
    }

    public class BorderPart
    {
        public static Border Border;
        public static object SCMovieTitle_Tab;
        public static object LCMovieTitle_Tab;
        public static readonly DependencyProperty BorderProperty = DependencyProperty.RegisterAttached("Border",
          typeof(bool), typeof(BorderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNotifyPropertyChanged)));

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(BorderProperty, value);
        }
        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(BorderProperty);
        }

        private static void OnNotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            Border = (Border)element;
        }

        public static readonly DependencyProperty MovieBoardProperty = DependencyProperty.RegisterAttached("MovieBoard",
         typeof(bool), typeof(BorderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));

        public static void SetMovieBoardIsEnabled(UIElement element, bool value)
        {
            element.SetValue(MovieBoardProperty, value);
        }
        public static bool GetMovieBoardIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(MovieBoardProperty);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (SCMovieTitle_Tab == null)
            {
                SCMovieTitle_Tab = d as UIElement;
            }
            else
            {
                LCMovieTitle_Tab = d as UIElement;
            }
        }
    }

    public class MediaControlExtension
    {
        public static bool GetCanAnimateControl(UIElement obj)
        {
            return (bool)obj.GetValue(CanAnimateControlProperty);
        }

        public static void SetCanAnimateControl(UIElement obj, bool value)
        {
            obj.SetValue(CanAnimateControlProperty, value);
        }



        public static readonly DependencyProperty CanAnimateControlProperty = DependencyProperty.RegisterAttached("CanAnimateControl",
            typeof(bool), typeof(MediaControlExtension),
            new FrameworkPropertyMetadata { DefaultValue = false });

        
        public static readonly DependencyProperty IsMouseOverMediaElementProperty = DependencyProperty.RegisterAttached("IsMouseOverMediaElement",
               typeof(bool?), typeof(MediaControlExtension), new FrameworkPropertyMetadata(null));
        

        public static void SetIsMouseOverMediaElement(UIElement element, bool? value)
        {
            element.SetValue(IsMouseOverMediaElementProperty, value);
        }

        public static bool? GetIsMouseOverMediaElement(UIElement element)
        {
            return (bool?)element.GetValue(IsMouseOverMediaElementProperty);
        }

        public static readonly DependencyProperty AnimateWindowsTabProperty = DependencyProperty.RegisterAttached("AnimateWindowsTab",
               typeof(bool?), typeof(MediaControlExtension), new FrameworkPropertyMetadata(null));


        public static void SetAnimateWindowsTab(UIElement element, bool? value)
        {
            element.SetValue(AnimateWindowsTabProperty, value);
        }

        public static bool? GetAnimateWindowsTab(UIElement element)
        {
            return (bool?)element.GetValue(AnimateWindowsTabProperty);
        }

        // Using a DependencyProperty as the backing store for FileexpVisiblityProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileexpVisiblityPropertyProperty =
            DependencyProperty.RegisterAttached("FileexpVisiblity",
        typeof(Visibility),
        typeof(MediaControlExtension),
        new FrameworkPropertyMetadata(Visibility.Visible)
        { DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged });

        public static void SetFileexpVisiblity(UIElement element, Visibility value)
        {
            element.SetValue(FileexpVisiblityPropertyProperty, value);
        }
        public static Visibility GetFileexpVisiblity(UIElement element)
        {
            return (Visibility)element.GetValue(FileexpVisiblityPropertyProperty);
        }

        public static readonly DependencyProperty FileViewVisiblityPropertyProperty =
            DependencyProperty.RegisterAttached("FileViewVisiblity",
        typeof(Visibility),
        typeof(MediaControlExtension),
        new FrameworkPropertyMetadata(Visibility.Visible)
        { DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged });

        public static void SetFileViewVisiblity(UIElement element, Visibility value)
        {
            element.SetValue(FileViewVisiblityPropertyProperty, value);
        }
        public static Visibility GetFileViewVisiblity(UIElement element)
        {
            return (Visibility)element.GetValue(FileViewVisiblityPropertyProperty);
        }

        public static Window Window;
        public static readonly DependencyProperty WindowSliderProperty = DependencyProperty.RegisterAttached("Window",
             typeof(bool), typeof(VolumeSliderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNotifyVolumeSliderPropertyChanged)));

        public static void SetWindow(UIElement element, bool value)
        {
            element.SetValue(WindowSliderProperty, value);
        }
        public static bool GetWindow(UIElement element)
        {
            return (bool)element.GetValue(WindowSliderProperty);
        }

        private static void OnNotifyVolumeSliderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            Window = (Window)element;
        }
        public static Window WindowsTitleBoard;

        public static readonly DependencyProperty WindowsTitleBoardProperty = DependencyProperty.RegisterAttached("WindowsTitleBoard",
             typeof(bool), typeof(VolumeSliderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnWindowsTitleBoardPropertyChanged)));

        public static void SetWindowsTitleBoard(UIElement element, bool value)
        {
            element.SetValue(WindowsTitleBoardProperty, value);
        }
        public static bool GetWindowsTitleBoard(UIElement element)
        {
            return (bool)element.GetValue(WindowsTitleBoardProperty);
        }

        private static void OnWindowsTitleBoardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WindowsTitleBoard = d as Window;
        }

        public static TextBlock TextBlockTitleBoard;
        public static readonly DependencyProperty TextBlockTitleBoardProperty = DependencyProperty.RegisterAttached("TextBlockTitleBoard",
             typeof(bool), typeof(VolumeSliderPart), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnTextBlockTitleBoardPropertyChanged)));

        public static void SetTextBlockTitleBoard(UIElement element, bool value)
        {
            element.SetValue(TextBlockTitleBoardProperty, value);
        }
        public static bool GetTextBlockTitleBoard(UIElement element)
        {
            return (bool)element.GetValue(TextBlockTitleBoardProperty);
        }

        private static void OnTextBlockTitleBoardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlockTitleBoard = d as TextBlock;
        }
    }
}
