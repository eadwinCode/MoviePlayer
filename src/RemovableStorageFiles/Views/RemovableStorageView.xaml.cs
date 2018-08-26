using RemovableStorageFiles.ViewModels;
using System.Windows.Controls;

namespace RemovableStorageFiles.Views
{
    /// <summary>
    /// Interaction logic for RemovableStorageView.xaml
    /// </summary>
    public partial class RemovableStorageView : UserControl
    {
        public RemovableStorageView()
        {
            InitializeComponent();
            this.DataContext = new RemovableStorageViewModel();
        }
    }
}
