using LocalVideoFiles.AddFolderDialogWindow;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace LocalVideoFiles.Views
{
    /// <summary>
    /// Interaction logic for HomePageLocalNoFolder.xaml
    /// </summary>
    public partial class HomePageLocalNoFolder : UserControl
    {
        private NewFolderModel newFolderModel;
        private ContentControl ParentControl;
        public event RoutedEventHandler OnFinished;

        public HomePageLocalNoFolder(NewFolderModel movieFolderModel, ContentControl contentControl)
        {
            InitializeComponent();
            this.newFolderModel = movieFolderModel;
            this.ParentControl = contentControl;
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //call add window dialog and refresh hompagelocal.
            AddFolderDialog addFolderDialog = new AddFolderDialog();
            addFolderDialog.OnFinished += AddFolderDialog_OnFinished;
            addFolderDialog.ShowDialog();
            
        }

        private void AddFolderDialog_OnFinished(object sender, EventArgs e)
        {
            var vm = ((sender as AddFolderDialog).DataContext as AddFolderDialogViewModel);
            if (vm.FileHasChange && vm.MovieFolderList.Count > 0)
            {
                newFolderModel.NewFolderCollection = vm.MovieFolderList;
                if(OnFinished != null)
                    OnFinished(this,new RoutedEventArgs(null, this.newFolderModel));

                ParentControl.Content = null;
            }
        }

        public void Show()
        {
            ParentControl.Content = this;
        }
        
    }
}
