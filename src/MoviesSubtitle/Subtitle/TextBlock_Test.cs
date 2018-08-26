using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using System.Windows.Markup;
using System.Text.RegularExpressions;

namespace HTMLConverter
{
    public class TextBlock_Test : TextBlock
    {
    //    public static DependencyProperty TextProperty =  DependencyProperty.Register("Text", typeof(string),
    //        typeof(TextBlock_Test), new UIPropertyMetadata("Text", new PropertyChangedCallback(OnHtmlChanged)));

    //    public string Text
    //    {
    //        get { return (string)GetValue(TextProperty); }
    //        set { SetValue(TextProperty, value); }
    //    }

    //    public static void OnHtmlChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
    //    {
    //        TextBlock_Test sender = (TextBlock_Test)s;
    //        sender.Parse((string)e.NewValue);
    //    }

    //    private void Parse(string p)
    //    {
    //        if (p != " ")
    //        {
    //            Inlines.Clear();
    //            RichTextBox rtb = new RichTextBox();
    //            var flowoc = Joint_FlowDocument(p);
    //            rtb.Document = flowoc;
    //            this.Inlines.AddRange(FlowDocumentEx.GetInlines(rtb.Document));
    //        }
    //    }

    //    private FlowDocument Joint_FlowDocument(string flowDocument1)
    //    {
    //        return XamlReader.Parse(flowDocument1) as FlowDocument;
    //    }

    //    private void UpdateText(object p)
    //    {
    //        //Text.in
    //    }


    //}

    //public static class FlowDocumentEx
    //{
    //    public static ICollection<Inline> GetInlines(FlowDocument doc)
    //    {
    //        return GetInlines(doc.Blocks);
    //    }

    //    public static ICollection<Inline> GetInlines(TextElementCollection<Block> blocks)
    //    {
    //        var inlines = new List<Inline>();

    //        foreach (var block in blocks)
    //        {
    //            if (block is Paragraph)
    //            {
    //                inlines.AddRange(((Paragraph)block).Inlines);
    //            }
    //            else if (block is Section)
    //            {
    //                inlines.AddRange(GetInlines(((Section)block).Blocks));
    //            }
    //        }
            
    //        return inlines;
    //    }


    }
}
