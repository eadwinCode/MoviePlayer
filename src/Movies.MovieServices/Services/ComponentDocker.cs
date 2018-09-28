using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Movies.MovieServices.Services
{
    public class ComponentDocker : Control
    {
        /// <summary>
        /// 
        /// </summary>
        static ComponentDocker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComponentDocker), 
                new FrameworkPropertyMetadata(typeof(ComponentDocker)));
        }
        

        /// <summary>
        /// Raised when any button is toggled
        /// </summary>
        public event RoutedEventHandler OnFinished;

        IPageNavigatorHost PageNavigatorHost
        {
            get { return ServiceLocator.Current.GetInstance<IPageNavigatorHost>(); }
        }
        public static RoutedCommand OkCommand { get; private set; }
        public static RoutedCommand CancelCommand { get; private set; }



        public string OKButtonText
        {
            get { return (string)GetValue(OKButtonTextProperty); }
            set { SetValue(OKButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OKButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OKButtonTextProperty =
            DependencyProperty.Register("OKButtonText", typeof(string), typeof(ComponentDocker), new PropertyMetadata("Ok"));



        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ComponentDocker), new PropertyMetadata(null));



        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set { SetValue(CancelButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonTextProperty =
            DependencyProperty.Register("CancelButtonText", typeof(string), typeof(ComponentDocker), new PropertyMetadata("Close"));



        public double DialogWidth
        {
            get { return (double)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register("DialogWidth", typeof(double), typeof(ComponentDocker), new PropertyMetadata(400.0));



        public double DialogHeight
        {
            get { return (double)GetValue(DialogHeightProperty); }
            set { SetValue(DialogHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogHeightProperty =
            DependencyProperty.Register("DialogHeight", typeof(double), typeof(ComponentDocker), new PropertyMetadata(200.0));



        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DialogTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(string), typeof(ComponentDocker), new PropertyMetadata("No Title"));

        public ComponentDocker()
        {
            OkCommand = new RoutedCommand();
            CancelCommand = new RoutedCommand();

            this.CommandBindings.Add(new CommandBinding(OkCommand, OkCommandExecuted));
            this.CommandBindings.Add(new CommandBinding(CancelCommand, CancelCommandExecuted));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void CancelCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnFinished!= null)
                this.OnFinished.Invoke(null,new RoutedEventArgs());
            PageNavigatorHost.RemoveView(DialogTitle);
        }

        private void OkCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (OnFinished != null)
                this.OnFinished.Invoke(Content, new RoutedEventArgs());
            PageNavigatorHost.RemoveView(DialogTitle);

        }

        public void ShowDialog()
        {
            PageNavigatorHost.AddView(this,DialogTitle);
        }
        
    }
}
