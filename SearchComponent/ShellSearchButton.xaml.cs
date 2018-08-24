using SearchComponent.ViewModels;
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

namespace SearchComponent
{
    /// <summary>
    /// Interaction logic for ShellSearchButton.xaml
    /// </summary>
    public partial class ShellSearchButton : UserControl
    {
        public ShellSearchButton(SearchPageBtnViewModel searchPageBtnViewModel)
        {
            InitializeComponent();
            this.DataContext = searchPageBtnViewModel;
            this.CommandBindings.Add(new CommandBinding(searchPageBtnViewModel.SearchCommand, searchPageBtnViewModel.SearchCommand_Execute, searchPageBtnViewModel.SearchCommand_CanExecute));
        }
    }
}
