using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfStyleableWindow.StyleableWindow
{
    public class WindowMinimizeCommand :ICommand
    {     
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {

            if (parameter is Window window)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
    }
}
