using FlyoutControl.PageNagivatorService;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
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
    public class Flyout : ContentControl
    {
        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }
        
        
        public PageNavigatorHost FlyContent
        {
            get { return (PageNavigatorHost)GetValue(FlyContentProperty); }
            internal set { SetValue(FlyContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlyContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlyContentProperty =
            DependencyProperty.Register("FlyContent", typeof(PageNavigatorHost), typeof(Flyout), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));



        public FlyoutMenu FlyoutMenu
        {
            get { return (FlyoutMenu)GetValue(FlyoutMenuProperty); }
            set { SetValue(FlyoutMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubMenuItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlyoutMenuProperty =
            DependencyProperty.Register("FlyoutMenu", typeof(FlyoutMenu), typeof(Flyout), new FrameworkPropertyMetadata(null ,FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public object SubMenuHiddenContent
        {
            get { return (object)GetValue(SubMenuHiddenContentProperty); }
            set { SetValue(SubMenuHiddenContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubMenuHiddenContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubMenuHiddenContentProperty =
            DependencyProperty.Register("SubMenuHiddenContent", typeof(object), typeof(Flyout), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        
        public Flyout()
        {

        }

        public override void BeginInit()
        {
            FlyoutMenu = new FlyoutMenu();
            this.FlyContent = ServiceLocator.Current.GetInstance<IPageNavigatorHost>() as PageNavigatorHost;
            base.BeginInit();
        }
        
        private void ShowSubMenuHandler()
        {
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }

    //public enum FlyoutState
    //{
    //    None,
    //    Opened,
    //    Closed
    //};


    //public class AnimationHelper
    //{
    //    public static void StartShowMenuAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, Thickness toValue, Thickness fromValue, double animationDurationSeconds, EventHandler completedEvent)
    //    {
    //        ThicknessAnimation animation = new ThicknessAnimation();
    //        animation.From = (fromValue);
    //        animation.To = (toValue);
    //        animation.Duration = TimeSpan.FromSeconds(animationDurationSeconds);
    //        animation.AccelerationRatio = 0.9;
    //        animation.FillBehavior = FillBehavior.HoldEnd;
    //        animation.Completed += delegate (object sender, EventArgs e)
    //        {
    //            //
    //            // When the animation has completed bake final value of the animation
    //            // into the property.
    //            //
    //           // animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));

    //            if (completedEvent != null)
    //            {
    //                completedEvent(sender, e);
    //            }
    //        };

    //        animation.Freeze();
    //        animatableElement.BeginAnimation(dependencyProperty, animation);
    //    }

    //    public static void StartHideMenuAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, Thickness toValue, Thickness fromValue, double animationDurationSeconds, EventHandler completedEvent)
    //    {
    //        ThicknessAnimation animation = new ThicknessAnimation();
    //        animation.From = (fromValue);
    //        animation.To = (toValue);
    //        animation.Duration = TimeSpan.FromSeconds(animationDurationSeconds);
    //        animation.DecelerationRatio = 0.9;
    //        animation.FillBehavior = FillBehavior.HoldEnd;

    //        animation.Completed += delegate (object sender, EventArgs e)
    //        {
    //            //
    //            // When the animation has completed bake final value of the animation
    //            // into the property.
    //            //
    //           // animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));

    //            if (completedEvent != null)
    //            {
    //                completedEvent(sender, e);
    //            }
    //        };

    //        animation.Freeze();
    //        animatableElement.BeginAnimation(dependencyProperty, animation);
    //    }
    //}
}
