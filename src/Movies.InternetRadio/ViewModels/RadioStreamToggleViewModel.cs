using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.InternetRadio.StreamManager;
using Movies.InternetRadio.Views;
using Movies.MoviesInterfaces;
using PresentationExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.InternetRadio.ViewModels
{
    public class RadioStreamToggleViewModel : NotificationObject
    {
        public DelegateCommand<WindowCommandButton> radiostreamcommand;
        public DelegateCommand<WindowCommandButton> RadioStreamCommand
        {
            get
            {
                if (radiostreamcommand == null)
                    radiostreamcommand = new DelegateCommand<WindowCommandButton>((data) =>
                    {
                        data.SetActive(true, true);
                    });
                return radiostreamcommand;
            }
        }

        private HamburgerMenuIconItem radiostream;

        public HamburgerMenuIconItem RadioStream
        {
            get { return radiostream; }
            set { radiostream = value; this.RaisePropertyChanged(() => this.RadioStream); }
        }

        internal RadioStreamToggleViewModel()
        {
            InitHamBurgerMenu();
        }

        private void InitHamBurgerMenu()
        {
            RadioStream = new HamburgerMenuIconItem()
            {
                Label = "Internet Radio",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Radioactive },
                Tag = (IRadioService as RadioService).RadioHomepage
            };
        }

        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }
    }
}
