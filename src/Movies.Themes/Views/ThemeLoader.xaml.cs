using Movies.Themes.ViewModel;
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

namespace Movies.Themes.Views
{
    /// <summary>
    /// Interaction logic for ThemeLoader.xaml
    /// </summary>
    public partial class ThemeLoader : UserControl
    {
        public ThemeLoader(ThemeLoaderViewModel themeLoaderViewModel)
        {
            InitializeComponent();
            this.DataContext = themeLoaderViewModel;
        }
    }
}
