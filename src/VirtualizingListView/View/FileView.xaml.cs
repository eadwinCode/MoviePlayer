using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension;
using System.Windows;
using System.Windows.Controls;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for FileView.xaml
    /// </summary>
    public partial class FileView : UserControl, IPageNavigatorHost
    {
        public INavigatorService PageNavigator { get { return  this.pagenavigator; } }
        
        public ContentControl DockControl
        {
            get
            {
                return this.DialogDock;
            }
        }
        public FileView(FileViewViewModel fileViewViewModel)
        {
            InitializeComponent();
            this.DataContext = fileViewViewModel;
            fileViewViewModel.SetPageHost(this);
        }
       

        private void WindowCommandButton_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
