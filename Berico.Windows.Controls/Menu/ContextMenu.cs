//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//
// Much of this class was borrowed from the Silverlight
// toolkit class of the same name.

// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//-------------------------------------------------------------

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;

namespace Berico.Windows.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ContextMenu : MenuBase
    {

        #region Member Fields

            private FrameworkElement rootVisual;
            private Point currentMousePosition;
            private UIElement parentElement;
            private Popup popup;
            private Panel overlay;
            private Point popupAlignmentPoint;
            private bool settingIsOpen;

        #endregion

        /// <summary>
        /// Intializes a new instance of Berico.Windows.Controls.ContextMenu
        /// </summary>
        public ContextMenu()
        {
            DefaultStyleKey = typeof(ContextMenu);
            IsSubMenu = false;
            HideDelay = new Duration(new TimeSpan(0));
            ShowDelay = new Duration(new TimeSpan(0));

            // We need to temporarily handle the LayoutUpdated event
            // in order to get the root visual
            LayoutUpdated += new EventHandler(LayoutUpdatedHandler);
        }

        #region Properties

            #region HorizontalOffet

                /// <summary>
                /// Identifies the HorizontalOffset dependency property.
                /// </summary>
                public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
                    "HorizontalOffset",
                    typeof(double),
                    typeof(ContextMenu),
                    new PropertyMetadata(0.0, OnHorizontalVerticalOffsetChanged));

                /// <summary>
                /// Gets or sets the horizontal distance between the target origin and the popup alignment point.
                /// </summary>
                public double HorizontalOffset
                {
                    get { return (double)GetValue(HorizontalOffsetProperty); }
                    set { SetValue(HorizontalOffsetProperty, value); }
                }

            #endregion

            #region VerticalOffet

                /// <summary>
                /// Gets or sets the vertical distance between the target origin and the popup alignment point
                /// </summary>
                public double VerticalOffset
                {
                    get { return (double)GetValue(VerticalOffsetProperty); }
                    set { SetValue(VerticalOffsetProperty, value); }
                }

                /// <summary>
                /// Identifies the VerticalOffset dependency property
                /// </summary>
                public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
                    "VerticalOffset",
                    typeof(double),
                    typeof(ContextMenu),
                    new PropertyMetadata(0.0, OnHorizontalVerticalOffsetChanged));

                /// <summary>
                /// Handles changes to the HorizontalOffset or VerticalOffset DependencyProperty
                /// </summary>
                /// <param name="o">DependencyObject that changed</param>
                /// <param name="e">Event data for the DependencyPropertyChangedEvent</param>
                private static void OnHorizontalVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    ContextMenu contextMenu = d as ContextMenu;
                    contextMenu.UpdateContextMenuPlacement();
                }

            #endregion

            #region IsOpen

                /// <summary>
                /// Gets or sets a value indicating whether the ContextMenu is visible
                /// </summary>
                public bool IsOpen
                {
                    get { return (bool)GetValue(IsOpenProperty); }
                    set { SetValue(IsOpenProperty, value); }
                }

                /// <summary>
                /// Identifies the IsOpen dependency property
                /// </summary>
                public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
                    "IsOpen",
                    typeof(bool),
                    typeof(ContextMenu),
                    new PropertyMetadata(false, OnIsOpenChanged));

                /// <summary>
                /// Handles changes to the IsOpen DependencyProperty
                /// </summary>
                /// <param name="o">DependencyObject that changed</param>
                /// <param name="e">Event data for the DependencyPropertyChangedEvent</param>
                private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    ContextMenu contextMenu = d as ContextMenu;
                    contextMenu.OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue);
                }

                /// <summary>
                /// Handles changes to the IsOpen property
                /// </summary>
                /// <param name="oldValue">Old value</param>
                /// <param name="newValue">New value</param>
                private void OnIsOpenChanged(bool oldValue, bool newValue)
                {
                    // Check if we are setting the IsOpen property
                    // internally (which means we don't want to
                    // respond to the change)
                    if (!settingIsOpen)
                    {
                        if (newValue)
                            if (IsSubMenu)
                            {
                                OpenPopup(new Point((ParentElement as MenuItem).ActualWidth, 0));
                            }
                            else
                                OpenPopup(currentMousePosition);
                        else
                            ClosePopup();
                    }
                }

            #endregion

            #region ParentElement

                /// <summary>
                /// Gets or sets the element that is the parent
                /// (or owner) of this context menu
                /// </summary>
                public UIElement ParentElement
                {
                    get { return this.parentElement; }
                    set
                    {
                        // Check if the parent element is null
                        if (this.parentElement != null)
                        {
                            UIElement parentUIElement = this.parentElement as UIElement;
                            if (parentUIElement != null)
                            {
                                // Unwire the MouseRightBUttonDown event handler
                                parentUIElement.MouseRightButtonDown -= new MouseButtonEventHandler(ParentElementMouseRightButtonDownHandler);
                            }
                        }

                        // Set the parent element
                        this.parentElement = value;

                        // Check if the parent element is not null
                        if (this.parentElement != null)
                        {
                            UIElement parentUIElement = this.parentElement as UIElement;
                            if (parentUIElement != null)
                            {
                                // Wire the MouseRightBUttonDown event handler
                                parentUIElement.MouseRightButtonDown += new MouseButtonEventHandler(ParentElementMouseRightButtonDownHandler);
                            }
                        }
                    }
                }

            #endregion

            /// <summary>
            /// 
            /// </summary>
            public bool IsSubMenu { get; set; }

        #endregion

        #region Events and EventHandlers

            /// <summary>
            /// Occurs when the context menu is opening
            /// </summary>
            public event RoutedEventHandler Opening;

            /// <summary>
            /// Occurs when the context menu is opened
            /// </summary>
            public event RoutedEventHandler Opened;

            /// <summary>
            /// Occurs when the context menu is closed
            /// </summary>
            public event RoutedEventHandler Closed;

            /// <summary>
            /// Fires the Opening event
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected virtual void OnOpening(RoutedEventArgs e)
            {
                // Fire the Opening event
                RoutedEventHandler handler = Opening;
                if (handler != null)
                    handler(this, e);
            }

            /// <summary>
            /// Fires the Opened event
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected virtual void OnOpened(RoutedEventArgs e)
            {
                if (Opened != null)
                    Opened(this, e);
            }

            /// <summary>
            /// Fires the Closed event
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected virtual void OnClosed(RoutedEventArgs e)
            {
                if (Closed != null)
                    Closed(this, e);
            }

            /// <summary>
            /// Occurrs when the mouse left button is pressed
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
            {
                e.Handled = true;
                base.OnMouseLeftButtonDown(e);
            }

            /// <summary>
            /// Occurs when the mouse right button is pressed
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
            {
                e.Handled = true;
                base.OnMouseRightButtonDown(e);
            }

            /// <summary>
            /// Occurs when keys are pressed while the menu
            /// has focus
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                // Check which key was pressed and handle accordingly
                switch (e.Key)
                {
                    case Key.Up:
                        FocusNextItem(false);
                        e.Handled = true;
                        break;
                    case Key.Down:
                        FocusNextItem(true);
                        e.Handled = true;
                        break;
                    case Key.Escape:
                        ClosePopup();
                        e.Handled = true;
                        break;
                }

                base.OnKeyDown(e);
            }

            /// <summary>
            /// Handles the LayoutUpdated event of the Application's RootVisual
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void LayoutUpdatedHandler(object sender, EventArgs e)
            {

                // Ensure that the RootVisual (of the entire application)
                // is not null
                if (Application.Current.RootVisual != null)
                {
                    InitializeRootVisual();

                    // Unwire the LayoutUpdated event handler
                    // since we no longer need it
                    LayoutUpdated -= new EventHandler(LayoutUpdatedHandler);
                }
            }

            /// <summary>
            /// Handles the MouseMove event of the Application's RootVisual
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void RootVisualMouseMoveHandler(object sender, MouseEventArgs e)
            {
                this.currentMousePosition = e.GetPosition(null);
            }

            /// <summary>
            /// Handles the MouseRightButtonDown event of the parent element
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void ParentElementMouseRightButtonDownHandler(object sender, MouseButtonEventArgs e)
            {
                OpenPopup(e.GetPosition(null));
                e.Handled = true;
            }

            /// <summary>
            /// Handles the SizeChanged event of the context menu and
            /// root visual
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void ContextMenuOrRootVisualSizeChangedHandler(object sender, SizeChangedEventArgs e)
            {
                UpdateContextMenuPlacement();
            }

            /// <summary>
            /// Handles the MouseButtonDown event for the overlay control
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void OverlayMouseButtonDownHandler(object sender, MouseButtonEventArgs e)
            {
                ClosePopup();
                e.Handled = true;
            }

        #endregion

        #region Internal Methods

            /// <summary>
            /// Initialize the root visual.  This is reference to the root of
            /// the entire application and is used to control the confines of
            /// the context menu.
            /// </summary>
            private void InitializeRootVisual()
            {
                // Ensure that the root visual has not already been
                // created
                if (rootVisual == null)
                {
                    // Try to capture the Application's RootVisual
                    rootVisual = Application.Current.RootVisual as FrameworkElement;
                    if (null != rootVisual)
                    {
                        // Wire-up the MouseMove event of the RootVisual
                        rootVisual.MouseMove += new MouseEventHandler(RootVisualMouseMoveHandler);
                    }
                }
            }

            /// <summary>
            /// Sets focus to the next (or previous) item in the menu
            /// </summary>
            /// <param name="down">true to move the focus down; false to move it up</param>
            private void FocusNextItem(bool down)
            {
                int count = Items.Count;
                int startingIndex = down ? -1 : count;
                MenuItem focusedMenuItem = FocusManager.GetFocusedElement() as MenuItem;

                // Ensure that we have a valid MenuItem and that this
                // context menu is it's parent
                if ((focusedMenuItem != null) && (this == focusedMenuItem.ParentMenu))
                {
                    startingIndex = ItemContainerGenerator.IndexFromContainer(focusedMenuItem);
                }
                int index = startingIndex;

                // keep moving up or down until we get a valid MenuItem
                do
                {
                    // Set the index to the next (or previous) item
                    index = (index + count + (down ? 1 : -1)) % count;

                    // Get a MenuItem instance for the current item
                    MenuItem container = ItemContainerGenerator.ContainerFromIndex(index) as MenuItem;

                    // Check if the item is valid
                    if (container != null)
                    {
                        // Ensure the item is enabled and that focus can be set
                        if (container.IsEnabled && container.Focus())
                            break;
                    }

                } while (index != startingIndex);
            }

            /// <summary>
            /// Updates the position and size of the contextmenu and overlay
            /// </summary>
            private void UpdateContextMenuPlacement()
            {
                // Start with the current popup alignment point
                double x = popupAlignmentPoint.X;
                double y = popupAlignmentPoint.Y;

                if (IsSubMenu)
                {
                    GeneralTransform transform = ParentElement.TransformToVisual(rootVisual);
                    Point transformedPoint = transform.Transform(new Point(x, y));

                    x = transformedPoint.X;
                    y = transformedPoint.Y;
                }

                // Adjust for offset
                x += HorizontalOffset;
                y += VerticalOffset;

                Rect menuRect = new Rect(new Point(x, y), new Point(x + this.ActualWidth, y + this.ActualHeight));
                if (IsSubMenu && ParentElement is MenuItem)
                {
                    x = CalculateXPositionForPopup(menuRect, rootVisual.ActualWidth, (ParentElement as MenuItem).ActualWidth);
                    y = CalculateYPositionForPopup(menuRect, rootVisual.ActualHeight);
                }
                else
                {
                    x = CalculateXPositionForPopup(menuRect, rootVisual.ActualWidth);
                    y = CalculateYPositionForPopup(menuRect, rootVisual.ActualHeight);
                }

                // Try to keep the popup from overlapping the bottom-right
                //x = Math.Min(x, rootVisual.ActualWidth - ActualWidth);
                //y = Math.Min(y, rootVisual.ActualHeight - ActualHeight);

                // Try to keep the popup from overlapping the top-left
                //x = Math.Max(x, 0);
                //y = Math.Max(y, 0);

                // Set the new location
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);

                // Resize the overlay to match the new container
                if (overlay != null)
                {
                    overlay.Width = rootVisual.ActualWidth;
                    overlay.Height = rootVisual.ActualHeight;
                }
            }

            private double CalculateXPositionForPopup(Rect currentRect, double hostContentWidth)
            {
                return CalculateXPositionForPopup(currentRect, hostContentWidth, double.NaN);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="currentRect"></param>
            /// <param name="hostContentWidth"></param>
            /// <returns></returns>
            private double CalculateXPositionForPopup(Rect currentRect, double hostContentWidth, double offset)
            {
                double result = currentRect.Left;

                if (currentRect.Left + currentRect.Width > hostContentWidth)
                {
                    if (double.IsNaN(offset))
                        result = hostContentWidth - currentRect.Width;
                    else
                        result = currentRect.Left - offset - currentRect.Width;
                }
                else
                    result = Math.Max(result, 0);

                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="currentRect"></param>
            /// <param name="hostContentHeight"></param>
            /// <returns></returns>
            private double CalculateYPositionForPopup(Rect currentRect, double hostContentHeight)
            {
                double result = currentRect.Top;

                if (currentRect.Top + currentRect.Height > hostContentHeight)
                    result = hostContentHeight - currentRect.Height;
                else
                    result = Math.Max(result, 0);

                return result;
            }

            /// <summary>
            /// Opens the context menu (popup)
            /// </summary>
            /// <param name="position">Where to place the context menu (popup)</param>
            public void OpenPopup(Point position)
            {
                OnOpening(new RoutedEventArgs());

                popupAlignmentPoint = position;

                InitializeRootVisual();

                if (!IsSubMenu)
                {
                    // The overlay is invisible and drawn on top of
                    // the entire application.  This is used to
                    // allow the closing of the context menu
                    // when the overlay gets focus or is clicked.
                    overlay = new Canvas { Background = new SolidColorBrush(Colors.Transparent) };
                    overlay.MouseLeftButtonDown += new MouseButtonEventHandler(OverlayMouseButtonDownHandler);
                    overlay.MouseRightButtonDown += new MouseButtonEventHandler(OverlayMouseButtonDownHandler);
                    overlay.Children.Add(this);

                    // Create new popup
                    popup = new Popup { Child = overlay };
                }
                else
                {
                    //popup = new Popup { Child = this };
                    overlay.Children.Add(this);
                }

                // Wire-up the SizeChanged event for the ContextMenu
                SizeChanged += new SizeChangedEventHandler(ContextMenuOrRootVisualSizeChangedHandler);

                // Check if the RootVisual is set
                if (rootVisual != null)
                {
                    // Wire-up the SizeChanged event for the RootVisual
                    rootVisual.SizeChanged += new SizeChangedEventHandler(ContextMenuOrRootVisualSizeChangedHandler);
                }
                UpdateContextMenuPlacement();

                //
                if (ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue)
                {
                    // Set the DataContext for this context menu
                    DependencyObject dataContextSource = ParentElement ?? rootVisual;
                    SetBinding(DataContextProperty, new Binding("DataContext") { Source = dataContextSource });
                }

                if (!IsSubMenu)
                {
                    // Open the popup 
                    popup.IsOpen = true;
                }

                Focus();

                settingIsOpen = true;
                IsOpen = true;
                settingIsOpen = false;

                OnOpened(new RoutedEventArgs());
            }

            /// <summary>
            /// Closes the context menu (popup)
            /// </summary>
            private void ClosePopup()
            {
                if (IsSubMenu)
                    ItemsSource = null;

                // Ensure that the popup is not null
                if (popup != null)
                {
                    // Clear the popup
                    popup.IsOpen = false;
                    popup.Child = null;
                    popup = null;
                }

                if (!IsSubMenu)
                {
                    // Ensure that the overlay is not null
                    if (overlay != null)
                    {
                        // Clear the overlay
                        overlay.Children.Clear();
                        overlay = null;
                    }

                    // Ensure the root visual is not null
                    if (rootVisual != null)
                    {
                        // Unwire the RootVisuals's SizeChanged event handler
                        rootVisual.SizeChanged -= new SizeChangedEventHandler(ContextMenuOrRootVisualSizeChangedHandler);
                    }
                }
                else
                {
                    overlay.Children.Remove(this);
                }

                // Unwire the ContextMenu's SizeChanged event handler
                SizeChanged -= new SizeChangedEventHandler(ContextMenuOrRootVisualSizeChangedHandler);

                // Close the popup
                settingIsOpen = true;
                IsOpen = false;
                settingIsOpen = false;

                OnClosed(new RoutedEventArgs());
            }

            /// <summary>
            /// Indicates that a child MenuItem was clicked
            /// </summary>
            internal void MenuItemClicked()
            {
                ClosePopup();
            }

            internal Panel PrimaryOverlay
            {
                get { return this.overlay; }
                set {
                    if (this.overlay != null)
                    {
                        overlay.MouseLeftButtonDown -= new MouseButtonEventHandler(OverlayMouseButtonDownHandler);
                        overlay.MouseRightButtonDown -= new MouseButtonEventHandler(OverlayMouseButtonDownHandler);
                    }

                    this.overlay = value;

                    overlay.MouseLeftButtonDown += new MouseButtonEventHandler(OverlayMouseButtonDownHandler);
                    overlay.MouseRightButtonDown += new MouseButtonEventHandler(OverlayMouseButtonDownHandler);
                }
            }
        #endregion
    }
}
