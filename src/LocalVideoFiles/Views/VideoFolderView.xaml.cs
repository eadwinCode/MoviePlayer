using LocalVideoFiles.ViewModels;
using PresentationExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalVideoFiles.Views
{
    /// <summary>
    /// Interaction logic for VideoFolderView.xaml
    /// </summary>
    public partial class VideoFolderView : UserControl
    {
        public VideoFolderView()
        {
            InitializeComponent();
            this.DataContext =new VideoFolderViewModel();
        }

        private void WindowCommandButton_Loaded(object sender, RoutedEventArgs e)
        {
            WindowCommandButton windowCommandButton = sender as WindowCommandButton;
            windowCommandButton.SetActive(true, true);
        }
    }
}
