using System.Windows.Controls;
using System.Windows.Media;

namespace Movies.Views
{
    /// <summary>
    /// Interaction logic for MainShellView.xaml
    /// </summary>
    public partial class MainShellView : UserControl
    {
        public MainShellView()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.Focus();
        }
    }
}
