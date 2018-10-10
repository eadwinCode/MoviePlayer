using Movies.InternetRadio.ViewModels;
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

namespace Movies.InternetRadio.Views
{
    /// <summary>
    /// Interaction logic for RadioHomepage.xaml
    /// </summary>
    public partial class RadioHomepage : Page,IMainPage
    {
        ContentControl HomePageDock;

        public RadioHomepage()
        {
            InitializeComponent();
            InitDialogDocker();
            this.DataContext = new RadioHomePageService();
            this.Loaded += (s, e) => { (DataContext as RadioHomePageService).Onloaded(s); };
        }

        public ContentControl Docker { get { return HomePageDock; } }

        public IMenuFlyout FlyoutMenu { get; set; }

        private void InitDialogDocker()
        {
            HomePageDock = new ContentControl();
            this._parent.Children.Add(HomePageDock);
        }

        private void Collections_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(ItemSizeChangeHandler.Current != null)
                ItemSizeChangeHandler.Current.OnSizeChanged(e.NewSize, e.PreviousSize);
        }
        
    }
}
