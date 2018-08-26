
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PresentationExtension
{
    public partial class ContextButton:Button
    {
        public ContextMenu ShowContextOnClick
        {
            get { return (ContextMenu)GetValue(ShowContextOnClickProperty); }
            set { SetValue(ShowContextOnClickProperty, value); }
        }

        public static readonly DependencyProperty ShowContextOnClickProperty =
            DependencyProperty.Register("ShowContextOnClick", typeof(ContextMenu), typeof(ContextButton), new PropertyMetadata(null));



        public PlacementMode ContextMentPlacement
        {
            get { return (PlacementMode)GetValue(ContextMentPlacementProperty); }
            set { SetValue(ContextMentPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMentPlacementProperty =
            DependencyProperty.Register("ContextMentPlacement", typeof(PlacementMode), typeof(ContextButton), new FrameworkPropertyMetadata() {DefaultValue = PlacementMode.Top });



        private bool IsContextMenuOpen = false;
    }

    public partial class ContextButton
    {
        protected override void OnClick()
        {
            if (ShowContextOnClick == null)
                throw new NotImplementedException("ShowContextMenuOnClick Cannot be null");

            if (!IsContextMenuOpen)
            {
                ShowContextOnClick.Placement = ContextMentPlacement;
                ShowContextOnClick.PlacementTarget = this;
                ShowContextOnClick.Closed += Context_Closed;
                // btn.ContextMenu.p
                ShowContextOnClick.Focusable = false;
                ShowContextOnClick.StaysOpen = true;
                ShowContextOnClick.IsOpen = true;
                IsContextMenuOpen = true;
            }
            else
            {
                ShowContextOnClick.IsOpen = false;
                IsContextMenuOpen = false;
            }
        }

        private void Context_Closed(object sender, RoutedEventArgs e)
        {
            IsContextMenuOpen = false;
        }
    }
}
