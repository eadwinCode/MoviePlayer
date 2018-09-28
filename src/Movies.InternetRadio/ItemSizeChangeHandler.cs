using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Movies.InternetRadio
{
    public class ItemSizeChangeHandler :UIElement
    {
        private static ItemSizeChangeHandler itemSizeChangeHandler ;
        private bool ExecuteWhenNecessary = false;
        private bool Isbusy = false;
        private Size _newSize;
        private static bool IsAnimating;

        IEventManager EventManager
        {
            get { return ServiceLocator.Current.GetInstance<IEventManager>(); }
        }

        IDispatcherService DispatcherService
        {
            get { return ServiceLocator.Current.GetInstance<IDispatcherService>(); }
        }


        public static ItemSizeChangeHandler Current
        {
            get
            {
                return itemSizeChangeHandler;
            }
        }
        
        public double ItemControlWidth
        {
            get { return (double)GetValue(ItemControlWidthProperty); }
            set { SetValue(ItemControlWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemControlWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemControlWidthProperty =
            DependencyProperty.Register("ItemControlWidth", typeof(double), typeof(ItemSizeChangeHandler), new PropertyMetadata(280.0));
        

        public void OnSizeChanged(Size NewSize, Size PreviousSize)
        {
            _newSize = NewSize;
            if (IsAnimating && !ExecuteWhenNecessary)
            {
                ExecuteWhenNecessary = true;
                DispatcherService.ExecuteTimerAction(()=> DispatchedResizeItems(_newSize, PreviousSize), 850);
                return;
            }
            ResizeItems(_newSize, PreviousSize);
        }

        private void DispatchedResizeItems(Size NewSize, Size PreviousSize)
        {
            var delta = Math.Abs((NewSize.Width - PreviousSize.Width));
            if (delta > 30)
            {
                var numbersInaRow = (int)(NewSize.Width - 35) / 250;
                var remainigspace = (NewSize.Width - 35) % 250;
                var margin = numbersInaRow * 8;
                var actualSpaceRemaing = remainigspace - margin;

                var ItemsWidth = (actualSpaceRemaing / numbersInaRow) + 250;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ItemControlWidth = ItemsWidth;
                }));
            }
            ExecuteWhenNecessary = false;
        }

        private void ResizeItems(Size NewSize, Size PreviousSize)
        {
            if (ExecuteWhenNecessary)
                return;
            var delta = Math.Abs((NewSize.Width - PreviousSize.Width));
            if (delta > 50)
            {
                var numbersInaRow = (int)(NewSize.Width - 35) / 250;
                var remainigspace = (NewSize.Width - 35) % 250;
                var margin = numbersInaRow * 8;
                var actualSpaceRemaing = remainigspace - margin;

                var ItemsWidth = (actualSpaceRemaing / numbersInaRow) + 250;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ItemControlWidth = ItemsWidth;
                }));
            }
        }

        public ItemSizeChangeHandler()
        {
            ItemControlWidth = 300;
            itemSizeChangeHandler = this;
            EventManager.GetEvent<IsFlyoutBusy>().Subscribe(IsFlyoutBusyAction);
        }

        private void IsFlyoutBusyAction(bool obj)
        {
            IsAnimating = obj;
        }
    }
}
