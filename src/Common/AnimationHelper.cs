using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace Common
{
    public static class AnimationHelper
    {
        /// <summary>
        /// Starts an animation to a particular value on the specified dependency property.
        /// </summary>
        public static void StartAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, double toValue, double animationDurationSeconds)
        {
            StartAnimation(animatableElement, dependencyProperty, toValue, animationDurationSeconds, null);
        }

        /// <summary>
        /// Starts an animation to a particular value on the specified dependency property.
        /// </summary>
        public static void StartAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, Visibility toValue, double animationDurationSeconds)
        {
            StartAnimation(animatableElement, dependencyProperty, toValue, animationDurationSeconds, null);
        }

        /// <summary>
        /// Starts an animation to a particular value on the specified dependency property.
        /// You can pass in an event handler to call when the animation has completed.
        /// </summary>
        public static void StartAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, Visibility toValue, double animationDurationSeconds, EventHandler completedEvent)
        {
            ObjectAnimationUsingKeyFrames objanimation = new ObjectAnimationUsingKeyFrames();
            DiscreteObjectKeyFrame dobjf = new DiscreteObjectKeyFrame(toValue,KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
            objanimation.BeginTime = TimeSpan.FromMilliseconds(0);
            objanimation.Duration = TimeSpan.FromMilliseconds(animationDurationSeconds);
            objanimation.KeyFrames = new ObjectKeyFrameCollection() { dobjf };

            objanimation.Completed += delegate (object sender, EventArgs e)
            {
                //
                // When the animation has completed bake final value of the animation
                // into the property.
                //
                animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
                CancelAnimation(animatableElement, dependencyProperty);

                if (completedEvent != null)
                {
                    completedEvent(sender, e);
                }
            };

            objanimation.Freeze();

            objanimation.BeginAnimation(dependencyProperty, objanimation);
        }


        /// <summary>
        /// Starts an animation to a particular value on the specified dependency property.
        /// You can pass in an event handler to call when the animation has completed.
        /// </summary>
        public static void StartAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, double toValue, double animationDurationSeconds, EventHandler completedEvent)
        {
            double fromValue = (double)animatableElement.GetValue(dependencyProperty);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = fromValue;
            animation.To = toValue;
            animation.Duration = TimeSpan.FromSeconds(animationDurationSeconds);

            animation.Completed += delegate (object sender, EventArgs e)
            {
                //
                // When the animation has completed bake final value of the animation
                // into the property.
                //
                animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
                CancelAnimation(animatableElement, dependencyProperty);

                if (completedEvent != null)
                {
                    completedEvent(sender, e);
                }
            };

            animation.Freeze();

            animatableElement.BeginAnimation(dependencyProperty, animation);
        }

        /// <summary>
        /// Cancel any animations that are running on the specified dependency property.
        /// </summary>
        public static void CancelAnimation(UIElement animatableElement, DependencyProperty dependencyProperty)
        {
            animatableElement.BeginAnimation(dependencyProperty, null);
        }
    }
}
