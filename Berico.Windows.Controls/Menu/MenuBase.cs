//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//
// Some aspects of this class were borrowed from the Silverlight
// toolkit class of the same name.
//-------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;

namespace Berico.Windows.Controls
{

    /// <summary>
    /// Represents a group of items displayed to the user for
    /// selection.
    /// </summary>
    [StyleTypedProperty(Property="ItemContainerStyle", StyleTargetType=typeof(MenuItem))]
    public class MenuBase : ItemsControl
    {
        #region Member Fields

            private bool isFocused = false;

        #endregion

        /// <summary>
        /// Occurs when an item is clicked
        /// </summary>
        public event RoutedEventHandler ItemClicked;

        /// <summary>
        /// Initializes a new instance of the MenuBase class with
        /// default values.
        /// </summary>
        public MenuBase()
            : base()
        {
            OpenOnClick = false;
            HideDelay = new Duration(new TimeSpan(TimeSpan.TicksPerSecond));
            ShowDelay = new Duration(new TimeSpan(TimeSpan.TicksPerSecond));
        }

        #region Properties

            #region OpenOnClick

                /// <summary>
                /// Defines the OpenOnClick dependency property.
                /// </summary>
                public static readonly DependencyProperty OpenOnClickProperty = DependencyProperty.Register(
                    "OpenOnClick",
                    typeof(bool),
                    typeof(MenuBase),
                    null);

                /// <summary>
                /// Gets or sets whether a submenu opens automatically or
                /// waits to be clicked.
                /// </summary>
                public bool OpenOnClick
                {
                    get { return (bool)GetValue(OpenOnClickProperty); }
                    set { SetValue(OpenOnClickProperty, value); }
                }

            #endregion

            #region HideDelay

                /// <summary>
                /// Defines the HideDelay dependency property.
                /// </summary>
                public static readonly DependencyProperty HideDelayProperty = DependencyProperty.Register(
                    "HideDelay",
                    typeof(Duration),
                    typeof(MenuBase),
                    null);

                /// <summary>
                /// Gets or sets the delay before a menu is closed after
                /// the cursor leaves a submenu item.
                /// </summary>
                public Duration HideDelay
                {
                    get { return (Duration)GetValue(HideDelayProperty); }
                    set { SetValue(HideDelayProperty, value); }
                }

            #endregion

            #region ShowDelay

                /// <summary>
                /// Defines the ShowDelay dependency property.
                /// </summary>
                public static readonly DependencyProperty ShowDelayProperty = DependencyProperty.Register(
                    "ShowDelay",
                    typeof(Duration),
                    typeof(MenuBase),
                    null);

                /// Gets or sets the delay before a menu is open after
                /// the cursor enters a submenu item.
                public Duration ShowDelay
                {
                    get { return (Duration)GetValue(ShowDelayProperty); }
                    set { SetValue(ShowDelayProperty, value); }
                }

            #endregion

            #region ItemContainerStyle

                /// <summary>
                /// Identifies the ItemContainerStyle dependency property.
                /// </summary>
                public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(
                    "ItemContainerStyle",
                    typeof(Style),
                    typeof(MenuBase),
                    null);

                /// <summary>
                /// Gets or sets the Style that is applied to the container element generated for each item.
                /// </summary>
                public Style ItemContainerStyle
                {
                    get { return (Style)GetValue(ItemContainerStyleProperty); }
                    set { SetValue(ItemContainerStyleProperty, value); }
                }

            #endregion

            /// <summary>
            /// Gets whether the control currently has focus
            /// </summary>
            public bool IsFocused
            {
                get { return this.isFocused; }
            }

        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// Occurs when the control is receiving focus
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnGotFocus(RoutedEventArgs e)
            {
                base.OnGotFocus(e);
                this.isFocused = true;
                //UpdateVisualState();
            }

            /// <summary>
            /// Occurs when the control is losing focus
            /// </summary>
            /// <param name="e">Arguments for the event</param>
            protected override void OnLostFocus(RoutedEventArgs e)
            {
                base.OnLostFocus(e);
                this.isFocused = false;
                //UpdateVisualState();
            }

        #endregion

        /// <summary>
        /// Determines whether the specified item is, or is eligible to be, its own item container.
        /// </summary>
        /// <param name="item">The item to check whether it is an item container.</param>
        /// <returns>True if the item is a MenuItem or a Separator; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is MenuItem);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item
        /// </summary>
        /// <returns>A MenuItem, used to display the given item</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            MenuItem menuItem = element as MenuItem;

            // Ensure that the menu item is not null
            if (menuItem != null)
            {
                // Set this MenuItem's parent 
                menuItem.ParentMenu = this;

                // Ensure that the menu item (the element) is not the item
                if (menuItem != item)
                {
                    // Copy the ItemsControl properties from parent (the
                    // context menu) to the child (the menu item)
                    DataTemplate itemTemplate = ItemTemplate;
                    Style itemContainerStyle = ItemContainerStyle;

                    if (itemTemplate != null)
                        menuItem.SetValue(HeaderedItemsControl.ItemTemplateProperty, itemTemplate);

                    if (itemContainerStyle != null && HasDefaultValue(menuItem, HeaderedItemsControl.ItemContainerStyleProperty))
                        menuItem.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, itemContainerStyle);

                    // Copy the header properties from parent (the
                    // context menu) to the child (the menu item)
                    if (HasDefaultValue(menuItem, HeaderedItemsControl.HeaderProperty))
                        menuItem.Header = item;

                    if (ItemTemplate != null)
                        menuItem.SetValue(HeaderedItemsControl.HeaderProperty, itemTemplate);

                    if (itemContainerStyle != null)
                        menuItem.SetValue(HeaderedItemsControl.StyleProperty, itemContainerStyle);
                }
            }
        }

        /// <summary>
        /// Checks whether a control has the default value for a property
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <param name="property">The property to check</param>
        /// <returns>true if the property has the default value; false otherwise</returns>
        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }

    }
}