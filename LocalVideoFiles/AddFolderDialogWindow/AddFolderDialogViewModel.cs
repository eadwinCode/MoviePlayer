using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.ObjectModel;

namespace LocalVideoFiles.AddFolderDialogWindow
{
    internal class AddFolderDialogViewModel : NotificationObject
    {
        private bool filehaschange = false;
        public bool FileHasChange { get { return filehaschange; } }
        IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }

        public ObservableCollection<MovieFolderModel> MovieFolderList
        {
            get { return ApplicationService.AppSettings.MovieFolders; }
            set { ApplicationService.AppSettings.MovieFolders = value;
                RaisePropertyChanged(() => this.MovieFolderList); }
        }
        public ObservableCollection<MovieFolderModel> RemovedFolders;
        public DelegateCommand AddCommand{ get; private set; }
        public DelegateCommand<object> DeleteCommand { get; private set; }
        public AddFolderDialogViewModel()
        {
            AddCommand = new DelegateCommand(AddCommandAction);
            DeleteCommand = new DelegateCommand<object>(DeleteCommandAction);
            RemovedFolders = new ObservableCollection<MovieFolderModel>();
        }

        private void DeleteCommandAction(object obj)
        {
            MovieFolderModel dir = (MovieFolderModel)obj;

            if (MovieFolderList.Contains(dir))
            {
                var removefiledialog = new RemoveItemDialog(dir);
                removefiledialog.ShowDialog();
                if (removefiledialog.DialogResult == true)
                {
                    MovieFolderList.Remove(dir);
                    filehaschange = true;
                    if (!RemovedFolders.Contains(dir))
                        RemovedFolders.Add(dir);
                }
            }
        }

        private void AddCommandAction()
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = 
                new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string dir = folderDialog.SelectedPath;
                MovieFolderModel moviePathModel = new MovieFolderModel(dir);
                if (!MovieFolderList.Contains(moviePathModel)){
                filehaschange = true;
                    MovieFolderList.Add(moviePathModel);
                }

                if (RemovedFolders.Contains(moviePathModel))
                    RemovedFolders.Remove(moviePathModel);
            }
        }
    }
}