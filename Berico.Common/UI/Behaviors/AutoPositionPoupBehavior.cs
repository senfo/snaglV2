//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;

namespace Berico.Common.UI.Behaviors
{
    /// <summary>
    /// A custom behavior that automatically positions a popup that it is
    /// associated with, based on its properties
    /// </summary>
    public class AutoPositionPoupBehavior : Behavior<UIElement>
    {
        private Popup internalPopup = null;
        private Point currentMousePosition = new Point(0, 0);

        /// <summary>
        /// Handles when the behavior is attached to the associated object 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // Check if the AssociatedObject is a Popup
            if (!(AssociatedObject is Popup))
            {
                // Since the AssociatedObject is not a Popup, we need to 
                // search through the VisualTree until we find a Popup
                this.internalPopup = AssociatedObject.GetChildObject<Popup>(null);
            }
            else
            {
                // Since the AssociatedObject is a Popup, we will setup
                // our internal reference to it directly
                this.internalPopup = AssociatedObject as Popup;
            }

            // Validate that the internalPopup is not null and that
            // it is a popup
            if (this.internalPopup != null && this.internalPopup is Popup)
            {
                // Start listening to the Popup.Opened event
                this.internalPopup.Opened += new EventHandler(internalPopup_Opened);
            }
        }

        /// <summary>
        /// Handles the event that the target popup has been opened.  This is where
        /// the calculation is performed to correctly set the position
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The arguments for the event</param>
        private void internalPopup_Opened(object sender, EventArgs e)
        {
            double hostContentWidth = (Application.Current.RootVisual as FrameworkElement).ActualWidth;
            double hostContentHeight = (Application.Current.RootVisual as FrameworkElement).ActualHeight;

            // Make sure that the RelativeTarget is not empty
            if (RelativeTarget != null)
            {
                // Save the Height and Width of the area that contains the popup
                hostContentWidth = this.RelativeTarget.ActualWidth;
                hostContentHeight = this.RelativeTarget.ActualHeight;
            }

            this.internalPopup.UpdateLayout();

            // Create a rectangle that defines the bounds of the AssociatedObject
            //Rect popupRect = new Rect(currentMousePosition, new Point(currentMousePosition.X + (AssociatedObject as FrameworkElement).ActualWidth,
            //currentMousePosition.Y + (AssociatedObject as FrameworkElement).ActualHeight));
            Rect popupRect = new Rect(currentMousePosition, new Point(currentMousePosition.X + (this.internalPopup.Child as FrameworkElement).ActualWidth,
                currentMousePosition.Y + (this.internalPopup.Child as FrameworkElement).ActualHeight));

            // Set the popup's Horizontal and Vertical offset based on the
            // current mouse position
            this.internalPopup.HorizontalOffset = CalculateXPositionForPopup(popupRect, hostContentWidth);
            this.internalPopup.VerticalOffset = CalculateYPositionForPopup(popupRect, hostContentHeight);
            //this.internalPopup.HorizontalOffset = currentMousePosition.X;
            //this.internalPopup.VerticalOffset = currentMousePosition.Y;

        }

        /// <summary>
        /// Handles when the behavior is detached from the associated object
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Stop listening for the Popup.Opened event
            this.internalPopup.Opened -= new EventHandler(internalPopup_Opened);

            // Check if we have a Relative Target
            if (this.RelativeTarget != null)
            {
                // stop listening to the FrameworkElement.MouseMove event
                this.RelativeTarget.MouseMove -= new MouseEventHandler(RelativeTarget_MouseMove);
            }
        }

        /// <summary>
        /// Returns the calculated horizontal offset for the popup.  This ensures
        /// that the popup remains within the bounding control (RelativeTarget) area.
        /// </summary>
        /// <param name="currentRect">The Rect of the popup</param>
        /// <param name="hostContentWidth">The width of the bounding area</param>
        /// <returns>the calulated horizontal offset</returns>
        private double CalculateXPositionForPopup(Rect currentRect, double hostContentWidth)
        {
            double result = currentRect.Left;

            if (currentRect.Left + currentRect.Width > hostContentWidth)
                result = hostContentWidth - currentRect.Width;
            else
                result = Math.Max(result, 0);

            return result;
        }

        /// <summary>
        /// Returns the calculated vertical offset for the popup  This ensures
        /// that the popup remains within the bounding control (RelativeTarget)
        /// area.
        /// </summary>
        /// <param name="currentRect">The Rect of the popup</param>
        /// <param name="hostContentHeight">The height of the bounding area</param>
        /// <returns>the calculated vertical offset</returns>
        private double CalculateYPositionForPopup(Rect currentRect, double hostContentHeight)
        {
            double result = currentRect.Top;

            if (currentRect.Top + currentRect.Height > hostContentHeight)
                result = hostContentHeight - currentRect.Height;
            else
                result = Math.Max(result, 0);

            return result;
        }

        #region Properties

            #region RelativeTarget

                /// <summary>
                /// 
                /// </summary>
                public static readonly DependencyProperty RelativeTargetProperty = DependencyProperty.Register("RelativeTarget", typeof(FrameworkElement), typeof(AutoPositionPoupBehavior), new PropertyMetadata(OnRelativeTargetPropertyChanged));

                /// <summary>
                /// Handles when the RelativeTarget property changes
                /// </summary>
                /// <param name="sender">The object that fired the event</param>
                /// <param name="args">The arguments for the event</param>
                private static void OnRelativeTargetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    AutoPositionPoupBehavior autoPositionPopup = sender as AutoPositionPoupBehavior;

                    // Call the instance version of the event handler
                    // (passing the event arguments)
                    autoPositionPopup.OnRelativeTargetChanged(args);
                }

                /// <summary>
                /// Handles when the RelativeTarget changes
                /// </summary>
                protected virtual void OnRelativeTargetChanged(DependencyPropertyChangedEventArgs args)
                {
                    // Check if we had a previous RelativeObject
                    if (args.OldValue != null)
                    {
                        // Stop listening to the FrameworkElement.MouseMove event
                        (args.OldValue as FrameworkElement).MouseMove -= new MouseEventHandler(RelativeTarget_MouseMove);
                    }

                    // Stop listening to the FrameworkElement.MouseMove event
                    this.RelativeTarget.MouseMove += new MouseEventHandler(RelativeTarget_MouseMove);
                }

                /// <summary>
                /// Handles the FrameworkElement.MouseMove event for the RelativeTarget
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void RelativeTarget_MouseMove(object sender, MouseEventArgs e)
                {
                    // Save the current mouse position relative to the sender (which
                    // should be the RealtiveTarget)
                    this.currentMousePosition = e.GetPosition(sender as FrameworkElement);
                }

                /// <summary>
                /// Gets or sets the RealtiveTarget
                /// </summary>
                public FrameworkElement RelativeTarget
                {
                    get { return (FrameworkElement)GetValue(RelativeTargetProperty); }
                    set { SetValue(RelativeTargetProperty, value); }
                }

            #endregion

        #endregion

    }
}