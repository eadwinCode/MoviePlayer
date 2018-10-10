using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PresentationExtension
{
    public class CustomButton : Button
    {
        private Point? dragStartPoint = null;
        //private Thumb thumb;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            this.dragStartPoint = new Point?(e.GetPosition(this));
        }
        public CustomButton()
        {
            this.Loaded += CustomButton_Loaded;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.Focus();
        }

        private void CustomButton_Loaded(object sender, RoutedEventArgs e)
        {
            //if (thumb != null)
            //{
            //    thumb.DragDelta += Thumb_DragDelta;
            //}
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DragDrop.DoDragDrop(this, this.DataContext, DragDropEffects.Copy);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
          //  thumb = (Thumb)this.Template.FindName("DragThumb", this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            if (this.dragStartPoint.HasValue)
            {
                Point newPosition = e.GetPosition(this);
                if (((newPosition.X - this.dragStartPoint.Value.X) >= 5 || 
                    (newPosition.X - this.dragStartPoint.Value.X) <= -5) ||
                    (newPosition.Y -this.dragStartPoint.Value.Y >= 5 || 
                    newPosition.Y - this.dragStartPoint.Value.Y <= -5))
                {
                    DragDrop.DoDragDrop(this, this.DataContext, DragDropEffects.Copy);
                    e.Handled = true;
                }
            }
        }


    }
}
