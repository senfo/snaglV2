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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Input;

namespace Berico.Windows.Controls
{
    /// <summary>
    /// The SplitButton control is a button that consists of two separate buttons.  One,
    /// displays a drop down menu that displays a list of available actions, while the other
    /// performs the action that was selected in the menu.
    /// </summary>

    [TemplatePart(Name = SplitButton.ElementActionButtonName, Type = typeof(Button))]
    [TemplatePart(Name = SplitButton.ElementPopupButtonName, Type = typeof(Button))]
    [TemplatePart(Name = SplitButton.ElementDropDownPopupName, Type = typeof(Popup))]
    [TemplatePart(Name = SplitButton.ElementDropDownPopupBorderName, Type = typeof(Grid))]
    [TemplatePart(Name = SplitButton.ElementDropDownPopupContentName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = SplitButton.ElementDropDownPopupBorderName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    public class SplitButton : ContentControl
    {

        #region Template Elements

            internal FrameworkElement ElementRoot { get; set; }
            internal const string ElementRootName = "Root";

            internal virtual Button ElementActionButton { get; set; }
            internal const string ElementActionButtonName = "ActionButton";

            internal virtual Button ElementPopupButton { get; set; }
            internal const string ElementPopupButtonName = "PopupButton";

            internal virtual Popup ElementDropDownPopup { get; set; }
            internal const string ElementDropDownPopupName = "DropDownPopup";

            internal FrameworkElement ElementDropDownPopupBorder { get; set; }
            internal const string ElementDropDownPopupBorderName = "DropDownPopupBorder";

            internal ContentPresenter ElementDropDownPopupContent { get; set; }
            internal const string ElementDropDownPopupContentName = "DropDownPopupContent";

        #endregion

        public event RoutedEventHandler Click;
        public event EventHandler<EventArgs> DropDownOpen;
        public event EventHandler<EventArgs> DropDownOpening;
        public event EventHandler<EventArgs> DropDownClosed;

        private bool isFocused;
        private Point initialOffset;

        /// <summary>
        /// Initializes a new instance of the Berico.Windows.Controls.SplitButton
        /// class with default values.
        /// </summary>
        public SplitButton()
        {

            DefaultStyleKey = typeof(SplitButton);

            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(SplitButton_IsEnabledChanged);

            UpdateIsEnabled();

        }

        #region Properties
        
            #region DropDownContent

                /// <summary>
                /// Gets a value that indicates the content of the drop down
                /// </summary> 
                public object DropDownContent
                {
                    get { return (object)GetValue(DropDownContentProperty); }
                    set { SetValue(DropDownContentProperty, value); }
                }

                /// <summary>
                /// Identifies the Berico.Windows.Controls.SplitButton.DropDownContent 
                /// dependency property.  This specifies the contents of the drop down.
                /// </summary> 
                public static readonly DependencyProperty DropDownContentProperty =
                    DependencyProperty.Register(
                        "DropDownContent",
                        typeof(object),
                        typeof(SplitButton),
                        new PropertyMetadata(OnDropDownContentPropertyChanged));

                /// <summary>
                /// DropDownContentProperty property changed handler.
                /// </summary> 
                /// <param name="d">Slider that changed DropDownContent.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
                private static void OnDropDownContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    SplitButton splitButton = d as SplitButton;

                    splitButton.OnDropDownContentChanged(e);
                }

                /// <summary> 
                /// Called when the DropDownContent property changes
                /// </summary>
                /// <param name="e"> 
                /// The data for DependencyPropertyChangedEventArgs.
                /// </param>
                internal virtual void OnDropDownContentChanged(DependencyPropertyChangedEventArgs e)
                {
                    if (this.ElementDropDownPopupContent != null)
                        ElementDropDownPopupContent.Content=this.DropDownContent;
                }

            #endregion 

            #region IsOpen

                /// <summary>
                /// Gets a value that determines whether the drop down is open or not
                /// </summary> 
                public bool IsOpen
                {
                    get { return (bool)GetValue(IsOpenProperty); }
                    set { SetValue(IsOpenProperty, value); }
                }

                /// <summary>
                /// Identifies the Berico.Windows.Controls.SplitButton.IsOpen 
                /// dependency property.  This indicates whether the drop down
                /// is open or not.
                /// </summary> 
                public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(SplitButton), new PropertyMetadata(false, OnIsOpenPropertyChanged));

                /// <summary>
                /// IsOpenProperty property changed handler. 
                /// </summary> 
                /// <param name="d">SplitButton that changed IsOpen.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
                private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    SplitButton splitButton = d as SplitButton;

                    splitButton.OnIsOpenChanged(e);
                }

                /// <summary> 
                /// Called when the IsOpen property changes.
                /// </summary>
                /// <param name="e"> 
                /// The data for DependencyPropertyChangedEventArgs.
                /// </param>
                internal virtual void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
                {
                    if (this.ElementDropDownPopup != null)
                    {
                        // Ensure that the new value is different then the current value
                        if (ElementDropDownPopup.IsOpen != (bool)e.NewValue)
                        {
                            // Change the IsOpen property of the Popup
                            ElementDropDownPopup.IsOpen = (bool)e.NewValue;
                        }
                    }
                }

            #endregion 

            #region Command

                /// <summary>
                /// Identifies the Berico.Windows.Controls.SplitButton.Command dependency
                /// property.  This indicates the name of the command that should be called.
                /// </summary>
                public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton), new PropertyMetadata(null, OnCommandPropertyChanged));

                /// <summary>
                /// Gets or sets the Command property
                /// </summary>
                public ICommand Command
                {
                    get { return (ICommand)GetValue(CommandProperty); }
                    set { SetValue(CommandProperty, value); }
                }


                /// <summary>
                /// Command property changed handler
                /// </summary> 
                /// <param name="d">SplitButton that changed IsOpen.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
                private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    SplitButton splitButton = d as SplitButton;

                    splitButton.OnCommandChanged(e);
                }

                /// <summary> 
                /// Called when the Command property changes.
                /// </summary>
                /// <param name="e"> 
                /// The data for DependencyPropertyChangedEventArgs.
                /// </param>
                internal virtual void OnCommandChanged(DependencyPropertyChangedEventArgs e)
                {
                    ICommand oldCommand = (ICommand)e.OldValue;
                    ICommand newCommand = (ICommand)e.NewValue;

                    if (oldCommand != null)
                        oldCommand.CanExecuteChanged += new EventHandler(HandleCanExecuteChanged);

                    if (newCommand != null)
                        newCommand.CanExecuteChanged += new EventHandler(HandleCanExecuteChanged);

                    UpdateIsEnabled();

                }

            #endregion

            #region CommandParameter

                /// <summary>
                /// Identifies the Berico.Windows.Controls.SplitButton.CommandParameter dependency
                /// property.  This indicates the name of the command that should be called.
                /// </summary>
                public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton), new PropertyMetadata(null, OnCommandParameterPropertyChanged));

                /// <summary>
                /// Gets or sets the CommandParameter property
                /// </summary>
                public object CommandParameter
                {
                    get { return (object)GetValue(CommandProperty); }
                    set { SetValue(CommandProperty, value); }
                }


                /// <summary>
                /// CommandParameter property changed handler
                /// </summary> 
                /// <param name="d">SplitButton that changed IsOpen.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
                private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    SplitButton splitButton = d as SplitButton;

                    splitButton.OnCommandParameterChanged(e);
                }

                /// <summary> 
                /// Called when the IsOpen property changes.
                /// </summary>
                /// <param name="e"> 
                /// The data for DependencyPropertyChangedEventArgs.
                /// </param>
                internal virtual void OnCommandParameterChanged(DependencyPropertyChangedEventArgs e)
                {
                    UpdateIsEnabled();
                }

        #endregion

        #endregion

        #region Events

            /// <summary>
            /// Handles the IsEnabledChanged event, which is fired when the IsEnabled property
            /// is changed.
            /// </summary>
            /// <param name="sender">An instance of the SplitButton control that
            /// raised the event.</param>
            /// <param name="e">An instance of DependencyProperyChangedEventArgs that contains
            /// event specific data.</param>
            private void SplitButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {

                // Enable or disable the PopupButton
                if (ElementPopupButton != null)
                    ElementPopupButton.IsEnabled = (bool)e.NewValue;

                // Enable or disable the ActionButton
                if (ElementActionButton != null)
                    ElementActionButton.IsEnabled = (bool)e.NewValue;

            }

            /// <summary>
            /// Handles the Click event, which is fired when the ActionButton is
            /// clicked.
            /// </summary>
            /// <param name="sender">An instance of the button that raised the event.</param>
            /// <param name="e">An instance of RoutedEventArgs that contains event 
            /// specific data.</param>
            private void ElementActionButton_Click(object sender, RoutedEventArgs e)
            {
                // Raise the Click event.
                OnClick();
            }

            /// <summary>
            /// Handles the Opened event, which is fired when the IsOpen property of
            /// a Popup control is set to true.  In this case, this is fired when the
            /// PopupButton is clicked.
            /// </summary>
            /// <param name="sender">An instance of the Popup control that raised the Opened event.</param>
            /// <param name="e">An instance of EventArgs that contains event
            /// specific data.</param>
            private void ElementDropDown_Opened(object sender, EventArgs e)
            {
                IsOpen = true; 
                OnDropDownOpen();

                initialOffset = ElementDropDownPopup.TransformToVisual(null).Transform(new Point());

                UpdateDropDownPopupOffset();

                //LayoutUpdated += new EventHandler(SplitButton_LayoutUpdated);
            }

            /// <summary>
            /// Handles the LayoutUpdated event
            /// </summary>
            /// <param name="sender">The event source</param>
            /// <param name="e">The event arguments</param>
            private void SplitButton_LayoutUpdated(object sender, EventArgs e)
            {
                UpdateDropDownPopupOffset();
            }

            /// <summary>
            /// Handles the Click event, which is fired when the PopupButton
            /// is clicked.
            /// </summary>
            /// <param name="sender">An instance of the button that fired the Click event.</param>
            /// <param name="e">An instance of RoutedEventArgs that contains event
            /// specific data.</param>
            private void ElementPopupButton_Click(object sender, RoutedEventArgs e)
            {

                // Check if the DropDown popup is already open.
                if (!ElementDropDownPopup.IsOpen)
                {
                    OnDropDownOpening();

                    // Open the DropDown popup.
                    ElementDropDownPopup.IsOpen = true;
                }
                else
                    //Close the DropDown popup.
                    ElementDropDownPopup.IsOpen = false;

            }

            /// <summary>
            /// Handles the Closed event of the DropDown and fires the
            /// DropDownClosed event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            private void ElementDropDownPopup_Closed(object sender, EventArgs e)
            {
                IsOpen = false; 
                OnDropDownClosed();

                //LayoutUpdated -= new EventHandler(SplitButton_LayoutUpdated);
                Focus();
            }

            /// <summary>
            /// Handles the LostFocus event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected override void OnLostFocus(RoutedEventArgs e)
            {
                base.OnLostFocus(e);

                // Make sure the drop down menu is closed when the control
                // loses focus.
                if (ElementDropDownPopup.IsOpen)
                    ElementDropDownPopup.IsOpen = false;

                this.isFocused = false;
                ChangeVisualState(true);
            }

            /// <summary>
            /// Handles the GotFocus event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected override void OnGotFocus(RoutedEventArgs e)
            {
                base.OnGotFocus(e);

                this.isFocused = true;
                ChangeVisualState(true);
            }

            /// <summary>
            /// Handles the MouseEneter event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected override void OnMouseEnter(MouseEventArgs e)
            {
                base.OnMouseEnter(e);
                ChangeVisualState(true);
            }

            /// <summary>
            /// Handles the MouseLeave event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected override void OnMouseLeave(MouseEventArgs e)
            {
                base.OnMouseLeave(e);
                ChangeVisualState(true);
            }

            /// <summary>
            /// Fires the Click event and executes the Command
            /// </summary>
            protected virtual void OnClick()
            {
                // Fire the Click event
                if (Click != null)
                {
                    Click(this, new RoutedEventArgs());
                }

                // Check if we have a command set and that it can execute
                if ((Command != null) && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }

            /// <summary>
            /// Handles the CanExecuteChanged event of the Command property
            /// </summary>
            /// <param name="sender">The source of the event</param>
            /// <param name="e">The arguments for the event</param>
            private void HandleCanExecuteChanged(object sender, EventArgs e)
            {
                UpdateIsEnabled();
            }

            /// <summary>
            /// Fires the DropDownOpening event
            /// </summary>
            protected virtual void OnDropDownOpening()
            {
                if (DropDownOpening != null)
                {
                    DropDownOpening(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Fires the DropDownOpen event
            /// </summary>
            protected virtual void OnDropDownOpen()
            {
                if (DropDownOpen != null)
                {
                    DropDownOpen(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Fires the DropDownClosed event
            /// </summary>
            protected virtual void OnDropDownClosed()
            {
                if (DropDownClosed != null)
                {
                    DropDownClosed(this, EventArgs.Empty);
                }
            }

        #endregion

        /// <summary>
        /// Overrides the base class OnApplyTemplate and provides access to the
        /// underlying templates various parts.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementRoot = GetTemplateChild(ElementRootName) as FrameworkElement;
            ElementPopupButton = GetTemplateChild(ElementPopupButtonName) as Button;
            ElementActionButton = GetTemplateChild(ElementActionButtonName) as Button;
            ElementDropDownPopup = GetTemplateChild(ElementDropDownPopupName) as Popup;
            ElementDropDownPopupBorder = GetTemplateChild(ElementDropDownPopupBorderName) as FrameworkElement;

            if (ElementPopupButton != null)
            {
                ElementPopupButton.IsEnabled = this.IsEnabled;
                ElementPopupButton.Click += new RoutedEventHandler(ElementPopupButton_Click);
            }

            if (ElementActionButton != null)
            {
                ElementActionButton.IsEnabled = this.IsEnabled;
                ElementActionButton.Click += new RoutedEventHandler(ElementActionButton_Click);
            }

            if (ElementDropDownPopup != null)
            {
                ElementDropDownPopup.Opened += new EventHandler(ElementDropDown_Opened);
                ElementDropDownPopup.Closed += new EventHandler(ElementDropDownPopup_Closed);
            }

            if (ElementDropDownPopupContent != null)
            {
                ElementDropDownPopupContent.Content = this.DropDownContent;
            }

            ChangeVisualState(false);

        }

        /// <summary>
        /// Updates the Horizontal and Vertical offset values 
        /// for the DropDown popup
        /// </summary>
        private void UpdateDropDownPopupOffset()
        {
            Point currentOffset = this.initialOffset;
            Point desiredOffset = TransformToVisual(Application.Current.RootVisual).Transform(new Point(0, ActualHeight));

            ElementDropDownPopup.HorizontalOffset = desiredOffset.X - currentOffset.X;
            ElementDropDownPopup.VerticalOffset = (desiredOffset.Y - currentOffset.Y) - 1;

        }

        /// <summary>
        /// Updates the IsEnabled property taking any assigned Command
        /// in to account
        /// </summary>
        private void UpdateIsEnabled()
        {
            IsEnabled = (Command == null) || Command.CanExecute(CommandParameter);
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the SplitButton
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
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            if (isFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }
        }

    }
}