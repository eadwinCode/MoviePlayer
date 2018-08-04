using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VideoPlayerControl
{
    //public  class TextBlock
    //{
    //    static TextBlock()
    //    {
    //        // Register for the SizeChanged event on all TextBlocks, even if the event was handled.
    //        EventManager.RegisterClassHandler(typeof(TextBlock),
    //        FrameworkElement.SizeChangedEvent,
    //        new SizeChangedEventHandler(OnTextBlockSizeChanged), true);
    //    }

    //    public static readonly DependencyPropertyKey IsTextTrimmedKey =
    //        DependencyProperty.RegisterAttachedReadOnly(
    //            "IsTextTrimmed",
    //            typeof(bool),
    //            typeof(TextBlock),
    //            new PropertyMetadata(false)
    //        );

    //    public static readonly DependencyProperty IsTextTrimmedProperty =
    //        IsTextTrimmedKey.DependencyProperty;

    //    [AttachedPropertyBrowsableForType(typeof(TextBlock))]
    //    public static Boolean GetIsTextTrimmed(TextBlock target)
    //    {
    //        return (Boolean)target.GetValue(IsTextTrimmedProperty);
    //    }

    //    public static void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e)
    //    {
    //        TextBlock textBlock = sender as TextBlock;
    //        if (null == textBlock)
    //        {
    //            return;
    //        }
    //        textBlock.SetValue(IsTextTrimmedKey, CalculateIsTextTrimmed(textBlock));
    //    }

    //    private static bool CalculateIsTextTrimmed(TextBlock textBlock)
    //    {
    //        double width = textBlock.ActualWidth;
    //        if (textBlock.TextTrimming == TextTrimming.None)
    //        {
    //            return false;
    //        }
    //        if (textBlock.TextWrapping != TextWrapping.NoWrap)
    //        {
    //            return false;
    //        }
    //        textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
    //        double totalWidth = textBlock.DesiredSize.Width;
    //        return width < totalWidth;
    //    }
    //}
}
