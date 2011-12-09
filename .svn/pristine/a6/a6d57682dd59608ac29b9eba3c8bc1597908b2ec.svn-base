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
//
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//-------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Berico.Windows.Controls
{

    /// <summary>
    /// Represents an item in a menu
    /// </summary>
    [TemplatePart(Name = MenuItem.ElementSubMenuIndicatorName, Type = typeof(FrameworkElement))] 
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MenuItem))]
    public class MenuItem : HeaderedItemsControl
    {
        /// <summary> 
        /// SubMenuIndicator element
        /// </summary>
        internal virtual FrameworkElement ElementSubMenuIndicator { get; set; }
        internal const string ElementSubMenuIndicatorName = "SubMenuIndicator";

        #region Member Fields
        
            private bool isFocused = false;
            private ContextMenu subMenu = null;
            private MenuBase parentMenu = null;
            private DispatcherTimer openTimer;
            private DispatcherTimer closeTimer;
            private Duration hideDelay = Duration.Forever;
            private Duration showDelay = Duration.Forever;
 
        #endregion

        /// <summary>
        /// Initializes a default instance of Berico.Windows.Controls.MenuItem.
        /// The ID of the new MenuItem will be -99 by default.
        /// </summary>
        public MenuItem() : this(-99) { }

        /// <summary>
        /// Initializes a new instance of Berico.Windows.Controls.MenuItem
        /// using the provided ID
        /// </summary>
        public MenuItem(int id)
        {
            DefaultStyleKey = typeof(MenuItem);

            ID = id;

            // Setup the internal timers
            openTimer = new DispatcherTimer();
            openTimer.Tick += new EventHandler(HandleOpenTimerTick);
            closeTimer = new DispatcherTimer();
            closeTimer.Tick += new EventHandler(HandleCloseTimerTick);

            IsEnabledChanged += new DependencyPropertyChangedEventHandler(IsEnabledChangedHandler);

            UpdateIsEnabled();
        }

        #region Properties

            #region Command

                /// <summary>
                /// Defines the Command dependency property.
                /// </summary>
                public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
                    "Command",
                    typeof(ICommand),
                    typeof(MenuItem),
                    new PropertyMetadata(null, OnCommandChanged));

                /// <summary>
                /// Gets or sets the command associated with the menu item.
                /// </summary>
                public ICommand Command
                {
                    get { return (ICommand)GetValue(CommandProperty); }
                    set { SetValue(CommandProperty, value); }
                }

                /// <summary>
                /// Handles changes to the Command DependencyProperty.
                /// </summary>
                /// <param name="o">DependencyObject that changed.</param>
                /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
                private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    MenuItem menuItem = d as MenuItem;
                    menuItem.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
                }

                /// <summary>
                /// Handles changes to the Command property.
                /// </summary>
                /// <param name="oldValue">Old value.</param>
                /// <param name="newValue">New value.</param>
                private void OnCommandChanged(ICommand oldValue, ICommand newValue)
                {
                    if (null != oldValue)
                    {
                        oldValue.CanExecuteChanged -= new EventHandler(CanExecuteChangedHandler);
                    }
                    if (null != newValue)
                    {
                        newValue.CanExecuteChanged += new EventHandler(CanExecuteChangedHandler);
                    }
                    UpdateIsEnabled();
                }

            #endregion

            #region CommandParameter

                /// <summary>
                /// Defines the CommandParameter dependency property.
                /// </summary>
                public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
                    "CommandParameter",
                    typeof(object),
                    typeof(MenuItem),
                    new PropertyMetadata(null, OnCommandParameterChanged));

                /// <summary>
                /// Gets or sets the parameter to pass to the Command property of a MenuItem.
                /// </summary>
                public object CommandParameter
                {
                    get { return (object)GetValue(CommandParameterProperty); }
                    set { SetValue(CommandParameterProperty, value); }
                }

                /// <summary>
                /// Handles changes to the CommandParameter DependencyProperty.
                /// </summary>
                /// <param name="o">DependencyObject that changed.</param>
                /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
                private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    MenuItem menuItem = d as MenuItem;
                    menuItem.OnCommandParameterChanged(e.OldValue, e.NewValue);
                }

                /// <summary>
                /// Handles changes to the CommandParameter property.
                /// </summary>
                /// <param name="oldValue">Old value.</param>
                /// <param name="newValue">New value.</param>
                private void OnCommandParameterChanged(object oldValue, object newValue)
                {
                    UpdateIsEnabled();
                }

            #endregion

            #region Icon

                /// <summary>
                /// Defines the Icon dependency property
                /// </summary>
                public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
                    "Icon",
                    typeof(UIElement),
                    typeof(MenuItem),
                    new PropertyMetadata(null));


                /// <summary>
                /// Gets or sets the icon that appears in a MenuItem
                /// </summary>
                public UIElement Icon
                {
                    get { return (UIElement)GetValue(IconProperty); }
                    set { SetValue(IconProperty, value); }
                }

            #endregion

            #region IsCheckable

                /// <summary>
                /// Defines the IsCheckable dependency property
                /// </summary>
                public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register(
                    "IsCheckable",
                    typeof(bool),
                    typeof(MenuItem),
                    new PropertyMetadata(false));


                /// <summary>
                /// Gets or sets whether or not this item can be checked
                /// </summary>
                public bool IsCheckable
                {
                    get { return (bool)GetValue(IsCheckableProperty); }
                    set { SetValue(IsCheckableProperty, value); }
                }

            #endregion

            #region IsChecked

                /// <summary>
                /// Defines the IsChecked dependency property
                /// </summary>
                public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
                    "IsChecked",
                    typeof(bool),
                    typeof(MenuItem),
                    new PropertyMetadata(false, OnIsCheckedChanged));

                /// <summary>
                /// Gets or sets whether or not this item is checked
                /// </summary>
                public bool IsChecked
                {
                    get { return (bool)GetValue(IsCheckedProperty); }
                    set { SetValue(IsCheckedProperty, value); }
                }

                /// <summary>
                /// Handles changes to the Command DependencyProperty
                /// </summary>
                /// <param name="o">DependencyObject that changed</param>
                /// <param name="e">Event data for the DependencyPropertyChangedEvent</param>
                private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    MenuItem menuItem = d as MenuItem;
                    menuItem.OnIsCheckedChanged((bool)e.OldValue, (bool)e.NewValue);
                }

                /// <summary>
                /// Handles changes to the Command property
                /// </summary>
                /// <param name="oldValue">Old value</param>
                /// <param name="newValue">New value</param>
                private void OnIsCheckedChanged(bool oldValue, bool newValue)
                {
                    if (newValue)
                        OnChecked();
                    else
                        OnUnchecked();

                    UpdateVisualState();
                }

            #endregion

            #region IsHighlighted

                /// <summary>
                /// Defines the IsHighlighted dependency property
                /// </summary>
                public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(
                    "IsHighlighted",
                    typeof(bool),
                    typeof(MenuItem),
                    new PropertyMetadata(false));


                /// <summary>
                /// Gets or sets whether or not this item is highlighted
                /// </summary>
                public bool IsHighlighted
                {
                    get { return (bool)GetValue(IsHighlightedProperty); }
                    set { SetValue(IsHighlightedProperty, value); }
                }

            #endregion

            /// <summary>
            /// Gets whether the control currently has focus
            /// </summary>
            public bool IsFocused
            {
                get { return this.isFocused; }
            }

            /// <summary>
            /// Gets whether this control has any items
            /// </summary>
            public bool HasItems
            {
                get { return (Items != null && Items.Count > 0) ? true : false; }
            }

            /// <summary>
            /// Gets or sets the parent menu base for this item
            /// </summary>
            internal MenuBase ParentMenu {
                get
                {
                    return this.parentMenu;
                }
                set
                {
                    this.parentMenu = value;
                    (this.parentMenu as ContextMenu).Closed += new RoutedEventHandler(ParentMenuClosedHandler);

                    openTimer.Interval = parentMenu.ShowDelay.TimeSpan;
                    closeTimer.Interval = parentMenu.HideDelay.TimeSpan;
                }
            }

            /// <summary>
            /// Gets or sets a non-unique ID for this menu item
            /// </summary>
            public int ID { get; private set; }

        #endregion

        #region Events and EventHandlers

            /// <summary>
            /// Occurs when this item is clicked
            /// </summary>
            public event RoutedEventHandler Clicked;

            /// <summary>
            /// Occurs when this item is checked
            /// </summary>
            public event RoutedEventHandler Checked;

            /// <summary>
            /// Occurs when this item is unchecked
            /// </summary>
            public event RoutedEventHandler Unchecked;

            /// <summary>
            /// Occurs when this item's submenu is opened
            /// </summary>
            public event RoutedEventHandler SubmenuOpened;

            /// <summary>
            /// Occurs when this item's submenu is closed
            /// </summary>
            public event RoutedEventHandler SubmenuClosed;

            /// <summary>
            /// Handles the MenuItem's IsEnabledChanged event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void IsEnabledChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
            {
                UpdateVisualState();
            }

            /// <summary>
            /// Handles the CanExecuteChanged event of the Command property.
            /// </summary>
            /// <param name="sender">Source of the event.</param>
            /// <param name="e">Event arguments.</param>
            private void CanExecuteChangedHandler(object sender, EventArgs e)
            {
                UpdateIsEnabled();
            }

            /// <summary>
            /// Updates the IsEnabled property.
            /// </summary>
            /// <remarks>
            /// WPF overrides the local value of IsEnabled according to ICommand, so Silverlight does, too.
            /// </remarks>
            private void UpdateIsEnabled()
            {
                IsEnabled = (null == Command) || Command.CanExecute(CommandParameter);
                UpdateVisualState();
            }

            /// <summary>
            /// Called when the template (visual tree) is generated
            /// </summary>
            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();

                ElementSubMenuIndicator = GetTemplateChild(ElementSubMenuIndicatorName) as FrameworkElement;

                if (ElementSubMenuIndicator != null)
                {
                    if (HasItems)
                        ElementSubMenuIndicator.Visibility = Visibility.Visible;
                    else
                        ElementSubMenuIndicator.Visibility = Visibility.Collapsed;
                }

                ChangeVisualState(false);
            }

            /// <summary>
            /// Occurs when the control is receiving focus
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnGotFocus(RoutedEventArgs e)
            {
                base.OnGotFocus(e);
                this.isFocused = true;
                UpdateVisualState();
            }

            /// <summary>
            /// Occurs when the control is losing focus
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnLostFocus(RoutedEventArgs e)
            {
                base.OnLostFocus(e);
                this.isFocused = false;
                UpdateVisualState();
            }

            /// <summary>
            /// Occurs when the mouse enters the menu item
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnMouseEnter(MouseEventArgs e)
            {
                base.OnMouseEnter(e);
                Focus();

                if (HasItems)
                {
                    if (subMenu == null)
                    {
                        subMenu = new ContextMenu();
                        subMenu.MouseEnter += new MouseEventHandler(SubMenuMouseEnterHandler);
                        subMenu.Closed += new RoutedEventHandler(SubMenuClosedHandler);
                        subMenu.HideDelay = ParentMenu.HideDelay;
                        subMenu.ShowDelay = ParentMenu.ShowDelay;
                        subMenu.ParentElement = this;
                        subMenu.PrimaryOverlay = (ParentMenu as ContextMenu).PrimaryOverlay;

                        DataTemplate itemTemplate = ItemTemplate;
                        Style itemContainerStyle = ItemContainerStyle;

                        if (itemTemplate != null)
                            subMenu.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);

                        if (itemContainerStyle != null && subMenu.ReadLocalValue(HeaderedItemsControl.ItemContainerStyleProperty) == DependencyProperty.UnsetValue)
                            subMenu.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, itemContainerStyle);

                        subMenu.ItemsSource = this.Items;
                    }

                    openTimer.Start();
                }

                UpdateVisualState();
            }

            /// <summary>
            /// Occurs when the mouse leaves the menu item
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnMouseLeave(MouseEventArgs e)
            {
                base.OnMouseLeave(e);

                if (ParentMenu != null)
                    ParentMenu.Focus();

                if (subMenu != null)
                {
                    openTimer.Stop();
                    if (subMenu.IsOpen)
                    {
                        closeTimer.Start();

                    }
                }

                UpdateVisualState();
            }

            /// <summary>
            /// Occurs when the left mouse button is pressed, 
            /// on this item
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
            {
                // Ensure that the event has not been handled
                if (!e.Handled)
                {
                    OnClicked();
                    e.Handled = true;
                }

                base.OnMouseLeftButtonDown(e);
            }

            /// <summary>
            /// Occurs when the right mouse button is pressed, 
            /// on this item
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
            {
                // Ensure that the event has not been handled
                if (!e.Handled)
                {
                    OnClicked();
                    e.Handled = true;
                }

                base.OnMouseRightButtonDown(e);
            }

            /// <summary>
            /// Occurs when a key is pressed while this
            /// control has focus
            /// </summary>
            /// <param name="e">>Arguments for the event</param>
            protected override void OnKeyDown(KeyEventArgs e)
            {
                // Ensure that the event has not been handled
                // and that the Enter key was pressed
                if (!e.Handled && (e.Key == Key.Enter))
                {
                    OnClicked();
                    e.Handled = true;
                }

                base.OnKeyDown(e);
            }

            /// <summary>
            /// Occurs when this item is clicked
            /// </summary>
            protected virtual void OnClicked()
            {
                ContextMenu contextMenu = ParentMenu as ContextMenu;
                if (contextMenu != null)
                {
                    closeTimer.Stop();

                    // Make sure to close the sub menu, if we have one
                    if (subMenu != null && subMenu.IsOpen)
                        CloseSubMenu();

                    // Inform this item's parent menu that this
                    // item was clicked
                    contextMenu.MenuItemClicked();
                }
                
                // Handle checking the item (if applicable)
                if (IsCheckable && !IsChecked)
                    IsChecked = true;
                else if (IsCheckable && IsChecked)
                    IsChecked = false;

                // Fire the Clicked event
                RoutedEventHandler handler = Clicked;
                if (handler != null)
                    handler(this, new RoutedEventArgs());

                // Execute the associated command, if there is one
                if ((Command != null) && Command.CanExecute(CommandParameter))
                    Command.Execute(CommandParameter);
            }

            /// <summary>
            /// Occurs when the item is checked
            /// </summary>
            protected virtual void OnChecked()
            {
                // Fire the Checked event
                RoutedEventHandler handler = Checked;
                if (handler != null)
                    handler(this, new RoutedEventArgs());
            }

            /// <summary>
            /// Occurs when the item is unchecked
            /// </summary>
            protected virtual void OnUnchecked()
            {
                // Fire the Unchecked event
                RoutedEventHandler handler = Unchecked;
                if (handler != null)
                    handler(this, new RoutedEventArgs());
            }

            /// <summary>
            /// Occurs when the sub menu is closed
            /// </summary>
            protected virtual void OnSubmenuClosed()
            {
                // Fire the SubmenuClosed event
                RoutedEventHandler handler = SubmenuClosed;
                if (handler != null)
                    handler(this, new RoutedEventArgs());
            }

            /// <summary>
            /// Occurs when the sub menu is openend
            /// </summary>
            protected virtual void OnSubmenuOpened()
            {
                // Fire the SubmenuOpened event
                RoutedEventHandler handler = SubmenuOpened;
                if (handler != null)
                    handler(this, new RoutedEventArgs());
            }

            /// <summary>
            /// Occurs when the item's ParentMenu is closed
            /// </summary>
            private void ParentMenuClosedHandler(object sender, RoutedEventArgs e)
            {
                if (subMenu != null)
                {
                    subMenu.IsOpen = false;
                    subMenu = null;
                }
            }

            /// <summary>
            /// Occurs when the item's sub menu is closed
            /// </summary>
            private void SubMenuClosedHandler(object sender, RoutedEventArgs e)
            {
                (ParentMenu as ContextMenu).IsOpen = false;
            }

            /// <summary>
            /// Occurs when the mouse cursor enters a sub menu
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void SubMenuMouseEnterHandler(object sender, MouseEventArgs e)
            {
                closeTimer.Stop();
            }

            /// <summary>
            /// Handles the Tick event of the OpenTimer
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void HandleOpenTimerTick(object sender, EventArgs e)
            {
                closeTimer.Stop();
                openTimer.Stop();

                OpenSubMenu();
            }

            /// <summary>
            /// Handles the Tick event of the CloseTimer
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void HandleCloseTimerTick(object sender, EventArgs e)
            {
                closeTimer.Stop();
                CloseSubMenu();
            }

        #endregion

        #region Internal Methods
        
            /// Update the current visual state of the slider. 
            /// </summary> 
            internal void UpdateVisualState()
            {
                ChangeVisualState(true);
            }

            /// <summary>
            /// Change to the correct visual state for the Slider.
            /// </summary> 
            /// <param name="useTransitions"> 
            /// True to use transitions when updating the visual state, False to
            /// snap directly to the new visual state. 
            /// </param>
            internal void ChangeVisualState(bool useTransitions)
            {
                if (!IsEnabled)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
                }
                else
                {
                    if (IsChecked)
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateChecked, VisualStates.StateUnchecked);
                    else
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateUnchecked);
                }

                if (IsFocused && IsEnabled)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
                }
                else
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
                }

            }

            /// <summary>
            /// Opens the sub menu
            /// </summary>
            private void OpenSubMenu()
            {
                // Open the menu
                subMenu.IsSubMenu = true;
                subMenu.IsOpen = true;
                Focus();

                // Fire the SubmenuOpened event
                OnSubmenuOpened();
            }

            /// <summary>
            /// Closes the sub menu
            /// </summary>
            private void CloseSubMenu()
            {
                // Ensure that we have a sub menu to close
                if (subMenu != null)
                {
                    // Unwire events handled on the sub menu
                    subMenu.MouseEnter -= new MouseEventHandler(SubMenuMouseEnterHandler);
                    subMenu.Closed -= new RoutedEventHandler(SubMenuClosedHandler);

                    // Close the menu and clear it
                    subMenu.IsOpen = false;
                    subMenu = null;

                    // Fire the SubmenuClosed event
                    OnSubmenuClosed();
                }
            }
        }

    #endregion
}