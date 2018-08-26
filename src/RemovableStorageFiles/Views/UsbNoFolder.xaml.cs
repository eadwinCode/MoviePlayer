using System.Windows;
using System.Windows.Controls;

namespace RemovableStorageFiles.Views
{
    /// <summary>
    /// Interaction logic for UsbNoFOlder.xaml
    /// </summary>
    public partial class UsbNoFolder : UserControl
    {
        ContentControl ParentController;
        public event RoutedEventHandler OnFinished;

        public UsbNoFolder(ContentControl contentControl)
        {
            InitializeComponent();
            ParentController = contentControl;
        }
        
        public void Show()
        {
            ParentController.Content = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnFinished != null)
                OnFinished(this, new RoutedEventArgs());

            ParentController.Content = null;
        }
    }
}
