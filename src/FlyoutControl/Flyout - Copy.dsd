using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FlyoutControl
{
    [TemplatePart(Name = "FlyoutPanel", Type = typeof(Panel))]
    public class Flyout : UserControl
    {
        IEventManager EventManager
        {
            get {return ServiceLocator.Current.GetInstance<IEventManager>(); }
        }
        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }
        private StackPanel SubItemPanel;
        private Grid TextItemPanel;
        private bool IsOpen;
        private Button MouseOverButton;

        public static RoutedUICommand SubMenuToggleCommand { get; private set; }


        public Style FlyoutButtonStyle
        {
            get { return (Style)GetValue(FlyoutButtonStyleProperty); }
            set { SetValue(FlyoutButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlyoutButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlyoutButtonStyleProperty =
            DependencyProperty.Register("FlyoutButtonStyle", typeof(Style), typeof(Flyout), new PropertyMetadata(null));
        
        public SubMenuItem SubMenuItem
        {
            get { return (SubMenuItem)GetValue(SubMenuItemProperty); }
            set { SetValue(SubMenuItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubMenuItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubMenuItemProperty =
            DependencyProperty.Register("SubMenuItem", typeof(SubMenuItem), typeof(Flyout), new PropertyMetadata(null));

        public FlyoutState FlyoutState
        {
            get { return (FlyoutState)GetValue(FlyoutStateProperty); }
            set { SetValue(FlyoutStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlyoutStateProperty =
            DependencyProperty.Register("FlyoutState", typeof(FlyoutState), typeof(Flyout), new PropertyMetadata(FlyoutState.None));

        public Flyout()
        {
            InitializeComponent();
            
            this.Loaded += Flyout_Loaded;
            SubMenuItem = new SubMenuItem();

            SubMenuToggleCommand = new RoutedUICommand();
            this.CommandBindings.Add(new CommandBinding(SubMenuToggleCommand, SubMenuToggleCommand_Executed));
        }

        private void SubMenuToggleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowSubMenuHandler();
        }

        private void SubItemPanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ShowSubMenuHandler();
        }

        private void SubItemPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            

        }

        private void Flyout_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void InitializeComponent()
        {

        }

        private void ShowSubMenuHandler()
        {
            EventManager.GetEvent<IsFlyoutBusy>().Publish(true);
            string StoryboardName;
            if(FlyoutState == FlyoutState.Closed)
            {
                TextItemPanel.Width = 0;
                AnimationHelper.StartShowMenuAnimation(SubItemPanel, StackPanel.MarginProperty,
                    new Thickness(0), new Thickness(-255, 0, 0, 0), .5, 
                    (s, e) =>
                    {
                        FlyoutState = FlyoutState.Opened;
                        IsOpen = true;
                        EventManager.GetEvent<IsFlyoutBusy>().Publish(false);

                        //MouseOverButton.Visibility = Visibility.Collapsed;
                    });
                //StoryboardName = "sbShowLeftMenu";
                //FlyoutState = FlyoutState.Opened;
               
                //Storyboard sb = Resources[StoryboardName] as Storyboard;
                //if(sb.IsFrozen)
                //    sb.Freeze();
                //sb.Completed += (s, e) =>
                //{
                   
                //};
                //sb.Begin(SubItemPanel);
                
            }
            else
            {
                AnimationHelper.StartHideMenuAnimation(SubItemPanel, StackPanel.MarginProperty, 
                    new Thickness(-256, 0, 0, 0), new Thickness(0), .5, 
                    (s, e) =>
                    {
                        FlyoutState = FlyoutState.Closed;
                        this.Padding = new Thickness(0);
                        IsOpen = false;
                        TextItemPanel.Width = 15;
                        EventManager.GetEvent<IsFlyoutBusy>().Publish(false);

                    });
                //StoryboardName = "sbHideLeftMenu";
              //  FlyoutState = FlyoutState.Closed;
                //Storyboard sb = Resources[StoryboardName] as Storyboard;
                //if (sb.IsFrozen)
                //    sb.Freeze();
                //sb.Completed += (s, e) =>
                //{
                //    IsOpen = false;
                //    MouseOverButton.Visibility = Visibility.Visible;
                //};
                //sb.Begin(SubItemPanel);
            }
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SubItemPanel = (StackPanel)this.Template.FindName("SubItemPanel", this);
            TextItemPanel = (Grid)this.Template.FindName("Txtbxpanel", this);
            this.Resources = this.Template.Resources;
            this.MouseOverButton = (Button)this.Template.FindName("MouseOverButton", this);

            if (FlyoutState == FlyoutState.None)
                this.FlyoutState = FlyoutState.Closed;
            ShowSubMenuHandler();
        }

        private void SubMenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse left submenu panel");
            ShowSubMenuHandler();
            
        }

        private void MouseOverButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsOpen)
            {
                ShowSubMenuHandler();
                MouseOverButton.Visibility = Visibility.Collapsed;
            }
            Console.WriteLine("Mouse entered button panel");

        }
    }

    public enum FlyoutState
    {
        None,
        Opened,
        Closed
    };


    public class AnimationHelper
    {
        public static void StartShowMenuAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, Thickness toValue, Thickness fromValue, double animationDurationSeconds, EventHandler completedEvent)
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = (fromValue);
            animation.To = (toValue);
            animation.Duration = TimeSpan.FromSeconds(animationDurationSeconds);
            animation.AccelerationRatio = 0.9;
            animation.FillBehavior = FillBehavior.HoldEnd;
            animation.Completed += delegate (object sender, EventArgs e)
            {
                //
                // When the animation has completed bake final value of the animation
                // into the property.
                //
               // animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));

                if (completedEvent != null)
                {
                    completedEvent(sender, e);
                }
            };

            animation.Freeze();
            animatableElement.BeginAnimation(dependencyProperty, animation);
        }

        public static void StartHideMenuAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, Thickness toValue, Thickness fromValue, double animationDurationSeconds, EventHandler completedEvent)
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = (fromValue);
            animation.To = (toValue);
            animation.Duration = TimeSpan.FromSeconds(animationDurationSeconds);
            animation.DecelerationRatio = 0.9;
            animation.FillBehavior = FillBehavior.HoldEnd;

            animation.Completed += delegate (object sender, EventArgs e)
            {
                //
                // When the animation has completed bake final value of the animation
                // into the property.
                //
               // animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));

                if (completedEvent != null)
                {
                    completedEvent(sender, e);
                }
            };

            animation.Freeze();
            animatableElement.BeginAnimation(dependencyProperty, animation);
        }
    }
}
