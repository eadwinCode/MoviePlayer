using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MovieHub.MediaPlayerElement.ViewModel;

namespace VideoPlayerView.Util
{
    public static class CheckboxGrouping
    {
        static IDictionary<string, IList<MenuItem>> Grouplist;
        static CheckboxGrouping()
        {
            Grouplist = new Dictionary<string, IList<MenuItem>>();
        }
        public static ICommand GetExecuteCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ExecuteCommand);
        }

        public static void SetExecuteCommand(DependencyObject obj, ICommand command)
        {
            obj.SetValue(ExecuteCommand, command);
        }

        public static readonly DependencyProperty ExecuteCommand = DependencyProperty.RegisterAttached("ExecuteCommand",
            typeof(ICommand), typeof(CheckboxGrouping),
            new UIPropertyMetadata(null, OnExecuteCommandChanged));

        public static object GetExecuteCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ExecuteCommandParameter);
        }

        public static void SetExecuteCommandParameter(DependencyObject obj, ICommand command)
        {
            obj.SetValue(ExecuteCommandParameter, command);
        }

        public static readonly DependencyProperty ExecuteCommandParameter = DependencyProperty.RegisterAttached("ExecuteCommandParameter",
            typeof(object), typeof(CheckboxGrouping));

        private static void OnExecuteCommandChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as MenuItem;
            control.Checked += Control_Checked;
        }

        private static void Control_Checked(object sender, RoutedEventArgs e)
        {
            var control = sender as MenuItem;

            if (control != null)
            {
                var command = control.GetValue(ExecuteCommand) as ICommand;
                var commandParameter = control.GetValue(ExecuteCommandParameter);
                if (control.IsChecked)
                    command.Execute(control.DataContext);
            }
            e.Handled = true;
        }
    }
}
