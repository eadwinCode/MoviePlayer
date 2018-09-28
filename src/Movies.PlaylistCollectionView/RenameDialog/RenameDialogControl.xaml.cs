
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Movies.PlaylistCollectionView.RenameDialog
{
    /// <summary>
    /// Interaction logic for RenameDialogControl.xaml
    /// </summary>
    public partial class RenameDialogControl : UserControl, IAddFolderDialog
    {
        public RenameDialogControl()
        {
            InitializeComponent();
            this.Loaded += RenameDialog_Loaded;
            this.CommandBindings.Add(new CommandBinding(OkCommand,
                OkCommand_Execute, OkCommand_Enabled));
        }

        public PlaylistModel PlaylistModel;
        public static RoutedCommand OkCommand = new RoutedCommand();
        public event EventHandler OnFinished;
        private bool iscancel = false;
        public bool IsCancel { get { return iscancel; } }
        public string ItemPath;

        private void RenameDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (PlaylistModel != null && PlaylistModel.PlaylistName != null)
            {
                this.RenameText.Text = PlaylistModel.PlaylistName;
                this.RenameText.Select(0, PlaylistModel.PlaylistName.Length);
            }
            this.RenameText.Focus();
        }

        private void OkCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.RenameText.Text != string.Empty && this.RenameText.Text.Length > 4;
        }

        private void OkCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnFinished != null)
                OnFinished.Invoke(this, new RoutedEventArgs());

            IPageNavigatorHost.RemoveView(typeof(RenameDialogControl).Name);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iscancel = true;
            if (OnFinished != null)
                OnFinished.Invoke(this, null);
            IPageNavigatorHost.RemoveView(typeof(RenameDialogControl).Name);
        }

        private void RenameDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void ShowDialog()
        {
            IPageNavigatorHost.AddView(this, typeof(RenameDialogControl).Name);
        }

        private IPageNavigatorHost IPageNavigatorHost
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPageNavigatorHost>();
            }
        }

        
    }
}
