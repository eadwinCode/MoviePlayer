using HTMLConverter;
using MediaControl.Subtitles;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RealMediaControlSubtitle.View
{
    /// <summary>
    /// Interaction logic for SubtitleView.xaml
    /// </summary>
    public partial class SubtitleView : UserControl,ISubtitle
    {
        private bool IsSubAvailable;
       // CustomLabel OutlineTextSub;
        public SubtitleView()
        {
            InitializeComponent();
        }
        

        public void AdjustFontSize(double fontsize,double thickness)
        {
            this.OutlineTextSub.FontSize = fontsize;
            this.OutlineTextSub.StrokeThickness = thickness;
        }

        public bool HasSub
        {
            get
            {
                return IsSubAvailable && !IsDisabled;
            }
        }
        private bool isdisabled;
        public bool IsDisabled { get { return isdisabled; }
            set { isdisabled = value;
                OutlineTextSub.Visibility =value? System.Windows.Visibility.Hidden:System.Windows.Visibility.Visible;
            }
        }

        public void SetText(double position)
        {
            //HtmlToXamlConverter.ConvertHtmlToXaml(, true)
            Task.Factory.StartNew(() => GetText(position))
               .ContinueWith(t => OutlineTextSub.Text = t.Result,
               TaskScheduler.FromCurrentSynchronizationContext());
           // OutlineTextSub.Text = HtmlToXamlConverter.ConvertHtmlToXaml(SubtitleFileCompiler.
            //    GetSubtitle(position), true);

        }

        private string GetText(double position)
        {
            return HtmlToXamlConverter.ConvertHtmlToXaml(SubtitleFileCompiler.
                GetSubtitle(position), true);
        }

        public void Clear()
        {
            OutlineTextSub.Visibility = System.Windows.Visibility.Visible;
            IsSubAvailable = false;
        }

        public void LoadSub(ISubtitleFiles subtitlefiles)
        {
            try
            {
                SubtitleFileCompiler.ReadSubtitleFile(subtitlefiles.Directory);
                IsSubAvailable = true;
                AdjustFontSize(11.5 * (96.0 / 72.0), .7);
                IsDisabled = false;
                subtitlefiles.IsSelected = true;
            }
            catch (Exception )
            {
                
            }
           
        }
    }
}
