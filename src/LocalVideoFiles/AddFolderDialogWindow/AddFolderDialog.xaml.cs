using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using System;
using System.Windows.Controls;

namespace LocalVideoFiles.AddFolderDialogWindow
{
    /// <summary>
    /// Interaction logic for AddFolderDialog.xaml
    /// </summary>
    public partial class AddFolderDialog : UserControl, IAddFolderDialog
    {
        public event EventHandler OnFinished;
        public AddFolderDialog()
        {
            InitializeComponent();
            this.DataContext = new AddFolderDialogViewModel();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnFinished != null)
                OnFinished.Invoke(this, null);

            IPageNavigatorHost.RemoveView(typeof(AddFolderDialog).Name);
        }

        public void ShowDialog()
        {
            IPageNavigatorHost.AddView(this,typeof(AddFolderDialog).Name);
        }

        private IPageNavigatorHost IPageNavigatorHost
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPageNavigatorHost>();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
