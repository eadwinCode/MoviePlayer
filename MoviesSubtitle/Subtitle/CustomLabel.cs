using OutlineText;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace OutlineText
{
    class CustomLabel : Label
    {
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill",
            typeof(Brush),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke",
            typeof(Brush),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness",
            typeof(double),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(
        //    typeof(CustomLabel),
        //    new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        //public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(
        //    typeof(CustomLabel),
        //    new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        //public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(
        //    typeof(CustomLabel),
        //    new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        //public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(
        //    typeof(CustomLabel),
        //    new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        //public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(
        //    typeof(CustomLabel),
        //    new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(null, OnFormattedTextInvalidated));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            "TextAlignment",
            typeof(TextAlignment),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
            "TextDecorations",
            typeof(TextDecorationCollection),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
            "TextTrimming",
            typeof(TextTrimming),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            "TextWrapping",
            typeof(TextWrapping),
            typeof(CustomLabel),
            new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnFormattedTextUpdated));

        private FormattedText FormatedText
        {
            get
            { return formattedText; }
            set
            {
                formattedText = value;
            }
        }
        public Geometry textGeometry;
        private RichTextBox rtb;
        public static FormattedText formattedText;
        //public Brush Colord;



        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        //public FontFamily FontFamily
        //{
        //    get { return (FontFamily)GetValue(FontFamilyProperty); }
        //    set { SetValue(FontFamilyProperty, value); }
        //}

        //[TypeConverter(typeof(FontSizeConverter))]
        //public double FontSize
        //{
        //    get { return (double)GetValue(FontSizeProperty); }
        //    set { SetValue(FontSizeProperty, value); }
        //}

        //public FontStretch FontStretch
        //{
        //    get { return (FontStretch)GetValue(FontStretchProperty); }
        //    set { SetValue(FontStretchProperty, value); }
        //}

        //public FontStyle FontStyle
        //{
        //    get { return (FontStyle)GetValue(FontStyleProperty); }
        //    set { SetValue(FontStyleProperty, value); }
        //}

        //public FontWeight FontWeight
        //{
        //    get { return (FontWeight)GetValue(FontWeightProperty); }
        //    set { SetValue(FontWeightProperty, value); }
        //}

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)this.GetValue(TextDecorationsProperty); }
            set { this.SetValue(TextDecorationsProperty, value); }
        }

        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //System.Drawing.Rectangle rect = new Rectangle();
            //rect.Height = (int)RenderSize.Height;
            //rect.Width = (int)RenderSize.Width;
            //using (GraphicsPath gp = new GraphicsPath())
            //using (System.Drawing.Pen outline = new System.Drawing.Pen(Stroke, 4) { LineJoin = LineJoin.Round})
            //using (StringFormat sf = new StringFormat())
            //using (System.Drawing.Brush foreBrush = new SolidBrush(System.Drawing.Color.Orange))
            //{
            //    gp.AddString(Text, this.FontFamily, (int)this.FontStyle,
            //        (float)this.FontSize,rect, sf);
            //    //e.Graphics.ScaleTransform(1.3f, 1.35f);
            //    //e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            //    //e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    //e.Graphics.DrawPath(outline, gp);
            //    //e.Graphics.FillPath(foreBrush, gp);
            //}
            this.EnsureGeometry();
            drawingContext.DrawText(this.FormatedText, new Point(0,0));
            //formattedText.SetForegroundBrush(Brushes.Black);
            //drawingContext.DrawText(formattedText, new Point(1, 1));
            drawingContext.DrawGeometry(null, new Pen(Brushes.Black, this.StrokeThickness) { LineJoin = PenLineJoin.Round }, this.textGeometry);

        }

        protected override Size MeasureOverride(Size constraint)
        {
            EnsureFormattedText();
            if (constraint.Width != 0 && constraint.Height != 0)
            {
                this.FormatedText.MaxTextWidth = Math.Min(3579139, constraint.Width);
                this.FormatedText.MaxTextHeight = Math.Max(0.0001d, constraint.Height);
                // return the desired size
                return new Size(this.FormatedText.Width, this.FormatedText.Height);
            }

            return base.MeasureOverride(constraint);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            EnsureFormattedText();
            // update the formatted text with the final size
            if (finalSize.Height != 0)
            {
                this.FormatedText.MaxTextWidth = finalSize.Width;
                this.FormatedText.MaxTextHeight = finalSize.Height;

                // need to re-generate the geometry now that the dimensions have changed
                this.textGeometry = null;

                return finalSize;
            }

            return base.ArrangeOverride(finalSize);
        }

        private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (CustomLabel)dependencyObject;
            outlinedTextBlock.FormatedText = null;
            outlinedTextBlock.textGeometry = null;

            outlinedTextBlock.Parse((string)e.NewValue);
        }


        private void Parse(string p)
        {
            int error = Regex.Matches(p, @"[a-zA-Z]").Count;
            if (error > 0)
            {
                rtb = new RichTextBox();
                var flowdoc = Joint_FlowDocument(p);
                rtb.Document = flowdoc;
                FlowDocumentExtensions.GetFormattedText(this.rtb.Document, this);
            }


        }

        private FlowDocument Joint_FlowDocument(string flowDocument1)
        {
            return XamlReader.Parse(flowDocument1) as FlowDocument;
        }

        private static void OnFormattedTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (CustomLabel)dependencyObject;
            //  outlinedTextBlock.UpdateFormattedText();

        }

        private void EnsureFormattedText()
        {
            if (this.FormatedText != null || this.Text == null)
            {

                return;
            }

            this.FormatedText = new FormattedText(
                this.Text,
                CultureInfo.CurrentUICulture,
                this.FlowDirection,
                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, FontStretches.Normal),
                this.FontSize,
                Brushes.Black);

            this.UpdateFormattedText();
        }

        private void UpdateFormattedText()
        {
            if (this.FormatedText == null)
            {

                return;
            }

            //  this.formattedText.MaxLineCount = this.TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            //  this.formattedText.TextAlignment = this.TextAlignment;
            //  this.formattedText.Trimming = this.TextTrimming;

            //  this.formattedText.SetFontSize(this.FontSize);
            // // this.formattedText.SetFontStyle(this.FontStyle);
            ////  this.formattedText.SetFontWeight(this.FontWeight);
            // // this.formattedText.SetFontFamily(this.FontFamily);
            //  this.formattedText.SetFontStretch(this.FontStretch);
            //  this.formattedText.SetTextDecorations(this.TextDecorations);
        }

        private void EnsureGeometry()
        {
            if (this.textGeometry != null)
            {
                return;
            }

            this.EnsureFormattedText();
            this.textGeometry = this.FormatedText.BuildGeometry(new System.Windows.Point(0, 0));

            //  this.FormatedText.BuildGeometry(new Point(0, 0)).GetOutlinedPathGeometry().GetPointAtFractionLength(0, out PointOfOrigin, out PointofOriginTangent);
        }
        //Point PointOfOrigin, PointofOriginTangent;
    }
}
