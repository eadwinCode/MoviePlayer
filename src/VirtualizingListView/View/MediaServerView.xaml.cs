using MahApps.Metro.Controls;
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

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for MediaServerView.xaml
    /// </summary>
    public partial class MediaServerView : UserControl
    {
        public MediaServerView()
        {
            InitializeComponent();
        }


        //public DelegateCommand<object> mediaservercommand;
        //public DelegateCommand<object> MediaServerCommand
        //{
        //    get
        //    {
        //        if (mediaservercommand == null)
        //            mediaservercommand = new DelegateCommand<object>((o) =>
        //            {
        //                var data = o as WindowCommandButton;
        //                data.SetActive(true, true); ;
        //            });
        //        return mediaservercommand;
        //    }
        //}


        private HamburgerMenuIconItem mediaServer;

        public HamburgerMenuIconItem MediaServer
        {
            get { return mediaServer; }
            set { mediaServer = value; }//this.RaisePropertyChanged(() => this.MediaServer); }
        }


    }
}
