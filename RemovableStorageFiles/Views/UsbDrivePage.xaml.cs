using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
using RemovableStorageFiles.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RemovableStorageFiles.Views
{
    /// <summary>
    /// Interaction logic for UsbDrivePage.xaml
    /// </summary>
    public partial class UsbDrivePage : Page, IMainPages
    {
        public bool HasController { get { return WindowCommandButton != null; } }
        public ContentControl Docker { get { return HomePageDock; } }

        private IWindowsCommandButton WindowCommandButton;
        public UsbDrivePage()
        {
            InitializeComponent();
            this.DataContext = new UsbDriveViewModel(this);
            this.Loaded += UsbDrivePage_Loaded;
        }
        
        public void SetController(IWindowsCommandButton controller)
        {
            this.WindowCommandButton = controller;
        }

        private void UsbDrivePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (WindowCommandButton != null)
                WindowCommandButton.SetActive(true, false);

            (this.DataContext as UsbDriveViewModel).OnLoaded();
        }
    }
}
