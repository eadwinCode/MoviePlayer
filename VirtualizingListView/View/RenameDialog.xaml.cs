using Common.Model;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        public static RoutedCommand OkCommand = new RoutedCommand();
        public PlaylistModel PlaylistModel;
        public RenameDialog()
        {
            InitializeComponent();
            this.Closing += RenameDialog_Closing;
            this.Loaded += RenameDialog_Loaded;
            this.CommandBindings.Add(new CommandBinding(OkCommand,
                OkCommand_Execute, OkCommand_Enabled));
        }

        private void RenameDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (PlaylistModel != null)
            {
                this.RenameText.Text = PlaylistModel.PlaylistName;
                this.RenameText.Select(0, PlaylistModel.PlaylistName.Length);
            }
            this.RenameText.Focus();
        }

        private void OkCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.RenameText.Text != string.Empty;
        }

        private void OkCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void RenameDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
