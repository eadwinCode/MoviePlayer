using Common.Interfaces;
using Common.Model;
using HTMLConverter;
using MediaControl.Subtitles;
using System;
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
           // OutlineTextSub = new CustomLabel();
           // this.OutlineTextSub.AutoSize = true;
           // this.OutlineTextSub.Dock = System.Windows.Forms.DockStyle.Fill;
           // this.OutlineTextSub.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           // this.OutlineTextSub.Location = new System.Drawing.Point(0, 0);
           // this.OutlineTextSub.Name = "customLabel1";
           // this.OutlineTextSub.OutlineForeColor = System.Drawing.Color.Black;
           // this.OutlineTextSub.ForeColor = System.Drawing.Color.White;
           // this.OutlineTextSub.BackColor = System.Drawing.Color.Transparent;
           // this.OutlineTextSub.OutlineWidth = 2F;
           // this.OutlineTextSub.Size = new System.Drawing.Size(700,0);
           //// this.OutlineTextSub.Text = "JDvnisvnisvnsidvnsdnsidNDisnvs\nevwevewvsvsdnmvksnvdvnsjdsnvskvn\nygygyftft";
           // this.OutlineTextSub.TabIndex = 0;
           // WFH.Child = OutlineTextSub;
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
            OutlineTextSub.Text = HtmlToXamlConverter.ConvertHtmlToXaml(SubtitleFileCompiler.GetSubtitle(position), true);
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
                AdjustFontSize(12.5 * (96.0 / 72.0), .9);
                IsDisabled = false;
                subtitlefiles.IsSelected = true;
            }
            catch (Exception )
            {
                
            }
           
        }
    }
}
