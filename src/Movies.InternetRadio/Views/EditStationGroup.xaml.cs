using Movies.InternetRadio.Interfaces;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for AddGroup.xaml
    /// </summary>
    public partial class EditStationGroup : MovieBase, IEditStation
    {
        private IMoviesRadio currentradiostation;

        public IMoviesRadio CurrentRadioStation
        {
            get { return currentradiostation; }
            set { currentradiostation = value; this.RaisePropertyChanged(() => this.CurrentRadioStation); }
        }

        public EditStationGroup()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
