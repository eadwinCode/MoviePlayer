using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Threading;

namespace Movies.InternetRadio.Util
{
    public class NetworkConnectionStatus :NotificationObject
    {
        private string networkstatus = "Connected.";
        private DispatcherTimer dispatcherTimer;
        public string NetworkStatus
        {
            get { return networkstatus; }
            set { networkstatus = value; RaisePropertyChanged(() => this.NetworkStatus); }
        }

        public NetworkConnectionStatus()
        {
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            dispatcherTimer.Tick += (s, e) => {
                //if (!CheckForInternetConnection())
                //{
                //    NetworkStatus = "No internet connection";
                //    return;
                //}
                NetworkStatus = "Connected.";
            };

            dispatcherTimer.Start();
        }

        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
