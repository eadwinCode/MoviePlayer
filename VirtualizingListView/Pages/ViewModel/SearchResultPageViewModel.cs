using Common.FileHelper;
using Common.Interfaces;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.Model;
using VirtualizingListView.Pages.Util;
using VirtualizingListView.Util;

namespace VirtualizingListView.Pages.ViewModel
{
    internal class SearchResultPageViewModel:NotificationObject
    {
        public Style ListViewStyle
        {
            get { return listviewstyle; }
            set { listviewstyle = value; RaisePropertyChanged(() => this.ListViewStyle); }
        }
        public DataTemplateSelector MyTemplateChange
        {
            get { return mytemplatechange; }
            set
            {
                mytemplatechange = value;
                this.RaisePropertyChanged(() => this.MyTemplateChange);
            }
        }
        private ObservableCollection<VideoFolder> searchresults;

        public ObservableCollection<VideoFolder> SearchResults
        {
            get { return searchresults; }
            set { searchresults = value; RaisePropertyChanged(() => this.SearchResults); }
        }

        private string resultText;
        private NavigationService navigationService;
        private DataTemplateSelector mytemplatechange;
        private Style listviewstyle;

        private string resulttextindetail;

        public string ResultTextInDetail
        {
            get { return resulttextindetail; }
            set { resulttextindetail = "Found in your movie collection ( " +value+ " )"; RaisePropertyChanged(() => this.ResultTextInDetail); }
        }

        public string ResultText
        {
            get { return resultText; }
            set { resultText = "Results for " +"\""+value+"\""; RaisePropertyChanged(() => this.ResultText); }
        }
        public DelegateCommand<object> OpenFolderCommand { get; private set; }
        public bool IsLoading { get; set; }
        public ViewType ActiveViewType { get; set; }

        public SearchResultPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.navigationService.LoadCompleted += NavigationService_LoadCompleted;
            this.UpdateView(ApplicationService.AppSettings.ViewType);
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            SearchModel searchmodel = (SearchModel)e.ExtraData;
            SearchResults = (ObservableCollection<VideoFolder>)searchmodel.Results;
            ResultText = searchmodel.SearchQuery;
            ResultTextInDetail = SearchResults.Count.ToString();
            navigationService.LoadCompleted -= NavigationService_LoadCompleted;
            OpenFolderCommand = new DelegateCommand<object>(OpenFolderCommandAction);
            UpdateViewCollection();
        }

        private void UpdateViewCollection()
        {
            FileLoaderCompletion fileCompletionLoader = new FileLoaderCompletion();
            var task = FileLoaderCompletion.CurrentTaskExecutor.CreateTask(() => {
                //this.IsLoading = true;
                fileCompletionLoader.FinishCollectionLoadProcess(SearchResults);
            }, "");

            FileLoaderCompletion.CurrentTaskExecutor.Execute(task);
        }
       
        private void UpdateView(ViewType value)
        {
            if (value == ViewType.Large)
            {
                MyTemplateChange = new MoreTemplateSelector();
                ListViewStyle = (Style)Application.Current.FindResource("lvStyle1");
            }
            else
            {
                MyTemplateChange = new ItemListSelector();
                ListViewStyle = (Style)Application.Current.FindResource("listViewControl");
            }
        }

        private void OpenFolderCommandAction(object obj)
        {
            VideoFolder videoFolder = (VideoFolder)obj;
            if (videoFolder.FileType == FileType.Folder)
            {
                this.navigationService.Navigate(new FilePageView(this.navigationService), obj);
            }
            else
                OpenFileCall.Open(videoFolder as IVideoData);
        }

    }
}