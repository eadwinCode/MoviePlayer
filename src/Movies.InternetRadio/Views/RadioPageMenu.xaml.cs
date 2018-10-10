using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Movies.InternetRadio.StreamManager;
using Movies.MoviesInterfaces;
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
    /// Interaction logic for RadioPageMenu.xaml
    /// </summary>
    public partial class RadioPageMenu
    {
        public RadioPageMenu()
        {
            InitializeComponent();
            this.DataContext = new RadioPageMenuViewModel();
        }
    }

    public class RadioPageMenuViewModel
    {
        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }

        private INavigatorService INavigatorService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<INavigatorService>();
            }
        }

        private DelegateCommand loadpageCommand;
        public DelegateCommand LoadPageCommand
        {
            get
            {
                if (loadpageCommand == null)
                {
                    loadpageCommand = new DelegateCommand(LoadPageAction);
                }
                return loadpageCommand;
            }
        }

        public object Icon { get; private set; }

        public RadioPageMenuViewModel()
        {
            Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Radio };
        }

        private void LoadPageAction()
        {
            INavigatorService.NavigateMainPage((IRadioService as RadioService).RadioHomepage);
        }
    }
}
