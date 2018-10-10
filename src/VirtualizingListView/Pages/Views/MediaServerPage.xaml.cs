using Movies.Models.Interfaces;
using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
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

namespace VirtualizingListView.Pages.Views
{
    /// <summary>
    /// Interaction logic for MediaServerPage.xaml
    /// </summary>
    public partial class MediaServerPage : Page, IMainPage
    {
        private IWindowsCommandButton WindowCommandButton;
        public bool HasController { get { return WindowCommandButton != null; } }
        public ContentControl Docker { get { return null; } }

        public IMenuFlyout FlyoutMenu { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MediaServerPage()
        {
            InitializeComponent();
            this.Loaded += MediaServerPage_Loaded;
        }

        public void SetController(IWindowsCommandButton controller)
        {
            this.WindowCommandButton = controller;
        }

        private void MediaServerPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (WindowCommandButton != null)
                WindowCommandButton.SetActive(true, false);
        }

        
    }
}
