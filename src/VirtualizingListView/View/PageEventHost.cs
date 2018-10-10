using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;
using VirtualizingListView.Pages;

namespace VirtualizingListView.View
{
    public class PageEventHost 
    {
        private IEventManager IEventManager
        {
            get
            {
                return EventAggregatorService.IEventManager;
            }
        }

        private INavigatorService NavigatorService
        {
            get { return ServiceLocator.Current.GetInstance<INavigatorService>(); }
        }
        
        public PageEventHost()
        {
            IEventManager.GetEvent<NavigateFolderItemToken>().Subscribe((videofolder) => {
                NavigatePage(new FilePageView(NavigatorService.NavigationService), videofolder);
            });

            IEventManager.GetEvent<NavigateNewPage>().Subscribe((o) =>
            {
                NavigatePage(o);
            });

            IEventManager.GetEvent<NavigateSearchResult>().Subscribe((o) =>
            {
                NavigatePage(new SearchResultPage(NavigatorService.NavigationService), o);
            });
        }

        public void NavigatePage(object page,object pageData = null)
        {
            NavigatorService.NavigatePage(page, pageData);
        }
    }
}
