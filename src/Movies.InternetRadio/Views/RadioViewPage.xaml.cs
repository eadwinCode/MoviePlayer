using Movies.InternetRadio.ViewModels;
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
using System.Windows.Threading;

namespace Movies.InternetRadio.Views
{
    /// <summary>
    /// Interaction logic for RadioViewPage.xaml
    /// </summary>
    public partial class RadioViewPage : Page
    {
        public RadioViewPage()
        {
            InitializeComponent();
        }

        public RadioViewPage(NavigationService navigationService) : this()
        {
            this.DataContext = new RadioViewPageViewModel(navigationService);
        }

        private void Collections_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemSizeChangeHandler.Current.OnSizeChanged(e.NewSize, e.PreviousSize);
        }
        
    }
}
