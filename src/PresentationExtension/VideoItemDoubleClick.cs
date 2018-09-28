﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PresentationExtension
{
    public static class VideoItemDoubleClick
    {
        public static readonly DependencyProperty DoubleClickCommand = DependencyProperty.RegisterAttached("DoubleClickCommand",
          typeof(ICommand), typeof(VideoItemDoubleClick), new UIPropertyMetadata(null, CommandChanged));

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Control;
            control.MouseDoubleClick += control_MouseDoubleClick;
        }
        
        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickCommand);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand command)
        {
            obj.SetValue(DoubleClickCommand, command);
        }

        public static object GetDoubleClickCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(DoubleClickCommandParameter);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, object command)
        {
            obj.SetValue(DoubleClickCommandParameter, command);
        }

        public static readonly DependencyProperty DoubleClickCommandParameter = DependencyProperty.RegisterAttached("DoubleClickCommandParameter",
            typeof(object), typeof(VideoItemDoubleClick));

        private static void OnExecuteCommandChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        static void control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClickAction(sender, e);
        }

        private static void ClickAction(object sender, MouseButtonEventArgs e)
        {
           UIElement control= sender as UIElement;
            if (control !=null)
            {
                var command = control.GetValue(DoubleClickCommand) as ICommand;
                var commandParameter = control.GetValue(DoubleClickCommandParameter);
                
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
            e.Handled = true;
        }

        private static void VideoItemDoubleClick_Click(object sender, RoutedEventArgs e)
        {
            UIElement control = sender as UIElement;
            if (control != null)
            {
                var command = control.GetValue(DoubleClickCommand) as ICommand;
                var commandParameter = control.GetValue(DoubleClickCommandParameter);

                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
            e.Handled = true;
        }

    }
}
