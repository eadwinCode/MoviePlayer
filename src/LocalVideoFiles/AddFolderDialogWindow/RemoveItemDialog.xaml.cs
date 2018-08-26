using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace LocalVideoFiles.AddFolderDialogWindow
{
    /// <summary>
    /// Interaction logic for RemoveItemDialog.xaml
    /// </summary>
    public partial class RemoveItemDialog : Window
    {
        private MovieFolderModel movieFolderModel;
        public RemoveItemDialog()
        {
            InitializeComponent();
            this.Owner = Shell as Window;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public RemoveItemDialog(MovieFolderModel movieFolderModel)
            : this()
        {
            this.movieFolderModel = movieFolderModel;
            this.message.Text = "Do you want to remove \"" + movieFolderModel.Name + "\" folder?" ;
            this.pathmessage.Text = "File Path = \""+movieFolderModel.FullName+"\"";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
            }
        }

        private IShell Shell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>();
            }
        }
    }
}
