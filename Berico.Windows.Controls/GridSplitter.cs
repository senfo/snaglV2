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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Berico.Windows.Controls
{

    /// <summary>
    /// Specifies different collapse modes of a GridSplitter.
    /// </summary>
    public enum GridSplitterCollapseMode
    {
        /// <summary>
        /// The GridSplitter cannot be collapsed or expanded.
        /// </summary>
        None = 0,
        /// <summary>
        /// The column (or row) to the right (or below) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Next = 1,
        /// <summary>
        /// The column (or row) to the left (or above) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Previous = 2
    }

    /// <summary>
    /// An updated version of the standard GridSplitter control that includes a centered handle
    /// which allows complete collapsing and expanding of the appropriate grid column or row.
    /// </summary>
    [TemplatePart(Name = GridSplitter.ElementHorizontalHandleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = GridSplitter.ElementVerticalHandleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = GridSplitter.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = GridSplitter.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    public class GridSplitter : System.Windows.Controls.GridSplitter
    {

        #region TemplateParts

            internal const string ElementHorizontalHandleName = "HorizontalGridSplitterHandle";
            internal const string ElementVerticalHandleName = "VerticalGridSplitterHandle";
            internal const string ElementHorizontalTemplateName = "HorizontalTemplate";
            internal const string ElementVerticalTemplateName = "VerticalTemplate";
            internal const string ElementGridSplitterBackground = "GridSplitterBackground";

            internal ToggleButton _elementHorizontalGridSplitterHandle;
            internal ToggleButton _elementVerticalGridSplitterHandle;
            internal Rectangle _elementGridSplitterBackground;
            internal FrameworkElement _elementHorizontalTemplate;

        #endregion

        #region Dependency Properties
            
            /// <summary>
            /// Identifies the CollapseMode dependency property
            /// </summary>
            public static readonly DependencyProperty CollapseModeProperty = DependencyProperty.Register("CollapseMode", typeof(GridSplitterCollapseMode), typeof(GridSplitter), new PropertyMetadata(GridSplitterCollapseMode.None, new PropertyChangedCallback(OnCollapseModePropertyChanged)));
            
            /// <summary>
            /// Identifies the HorizontalHandleStyle dependency property
            /// </summary>
            public static readonly DependencyProperty HorizontalHandleStyleProperty = DependencyProperty.Register("HorizontalHandleStyle", typeof(Style), typeof(GridSplitter), new PropertyMetadata(null));
            
            /// <summary>
            /// Identifies the VerticalHandleStyle dependency property
            /// </summary>
            public static readonly DependencyProperty VerticalHandleStyleProperty = DependencyProperty.Register("VerticalHandleStyle", typeof(Style), typeof(GridSplitter), new PropertyMetadata(null));
            
            /// <summary>
            /// Identifies the VerticalHandleStyle dependency property
            /// </summary>
            public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register("IsAnimated", typeof(bool), typeof(GridSplitter), new PropertyMetadata(null));

            /// <summary>
            /// Identifies the IsCollapsed dependency property
            /// </summary>
            public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(GridSplitter), new PropertyMetadata(new PropertyChangedCallback(OnIsCollapsedPropertyChanged)));

            /// <summary>
            /// The IsCollapsed property porperty changed handler.
            /// </summary>
            /// <param name="d">GridSplitter that changed IsCollapsed.</param>
            /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
            private static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                GridSplitter s = d as GridSplitter;

                bool value = (bool)e.NewValue;
                s.OnIsCollapsedChanged(value);
            }

            /// <summary>
            /// The CollapseModeProperty property changed handler.
            /// </summary>
            /// <param name="d">GridSplitter that changed IsCollapsed.</param>
            /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
            private static void OnCollapseModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                GridSplitter s = d as GridSplitter;

                GridSplitterCollapseMode value = (GridSplitterCollapseMode)e.NewValue;
                s.OnCollapseModeChanged(value);
            }

        #endregion

        #region Local Members

            GridCollapseDirection _gridCollapseDirection = GridCollapseDirection.Auto;
            GridLength _savedGridLength;
            double _savedActualValue;

        #endregion

        #region Control Instantiation

            /// <summary>
            /// Initializes a new instance of the GridSplitter class,
            /// which inherits from System.Windows.Controls.GridSplitter.
            /// </summary>
            public GridSplitter()
                : base()
            {
                // Set default values
                DefaultStyleKey = typeof(GridSplitter);
                
                CollapseMode = GridSplitterCollapseMode.None;
                IsAnimated = true;
                this.LayoutUpdated += delegate { _gridCollapseDirection = GetCollapseDirection(); };

                // All GridSplitter visual states are handled by the parent GridSplitter class.
            }

            /// <summary>
            /// This method is called when the tempalte should be applied to the control.
            /// </summary>
            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();

                _elementHorizontalGridSplitterHandle = GetTemplateChild(ElementHorizontalHandleName) as ToggleButton;
                _elementVerticalGridSplitterHandle = GetTemplateChild(ElementVerticalHandleName) as ToggleButton;
                _elementGridSplitterBackground = GetTemplateChild(ElementGridSplitterBackground) as Rectangle;
                _elementHorizontalTemplate = GetTemplateChild(ElementHorizontalTemplateName) as FrameworkElement;

                // Wire up the Checked and Unchecked events of the HorizontalGridSplitterHandle.
                if (!(_elementHorizontalGridSplitterHandle == null))
                {
                    _elementHorizontalGridSplitterHandle.Checked += new RoutedEventHandler(GridSplitterHandle_Checked);
                    _elementHorizontalGridSplitterHandle.Unchecked += new RoutedEventHandler(GridSplitterHandle_Unchecked);
                }

                // Wire up the Checked and Unchecked events of the VerticalGridSplitterHandle.
                if (!(_elementVerticalGridSplitterHandle == null))
                {
                    _elementVerticalGridSplitterHandle.Checked += new RoutedEventHandler(GridSplitterHandle_Checked);
                    _elementVerticalGridSplitterHandle.Unchecked += new RoutedEventHandler(GridSplitterHandle_Unchecked);
                }

                // Set default direction since we don't have all the components layed out yet.
                _gridCollapseDirection = GridCollapseDirection.Auto;

                // Directely call these events so design-time view updates appropriately
                OnCollapseModeChanged(CollapseMode);
                OnIsCollapsedChanged(IsCollapsed);
            }

        #endregion

        #region Public Members

        /// <summary>
            /// Gets or sets a value that indicates if the target column is 
            /// currently collapsed.
            /// </summary>
            public bool IsCollapsed
            {
                get { return (bool)GetValue(IsCollapsedProperty); }
                set { SetValue(IsCollapsedProperty, value); }
            }

            /// <summary>
            /// Gets or sets a value that indicates if the collapse and
            /// expanding actions should be animated.
            /// </summary>
            public bool IsAnimated
            {
                get { return (bool)GetValue(IsAnimatedProperty); }
                set { SetValue(IsAnimatedProperty, value); }
            }

            //<summary>
            //Gets or sets the style that customizes the appearance of the horizontal handle 
            //that is used to expand and collapse the GridSplitter.
            //</summary>
            public Style HorizontalHandleStyle
            {
                get { return (Style)GetValue(HorizontalHandleStyleProperty); }
                set { SetValue(HorizontalHandleStyleProperty, value); }
            }

            //<summary>
            //Gets or sets the style that customizes the appearance of the vertical handle 
            //that is used to expand and collapse the GridSplitter.
            //</summary>
            public Style VerticalHandleStyle
            {
                get { return (Style)GetValue(VerticalHandleStyleProperty); }
                set { SetValue(VerticalHandleStyleProperty, value); }
            }

            /// <summary>
            /// Gets or sets a value that indicates the CollapseMode.
            /// </summary>
            public GridSplitterCollapseMode CollapseMode
            {
                get { return (GridSplitterCollapseMode)GetValue(CollapseModeProperty); }
                set { SetValue(CollapseModeProperty, value); }
            }
        #endregion
        
        #region Protected Members

            /// <summary>
            /// Handles the property change event of the IsCollapsed property.
            /// </summary>
            /// <param name="isCollapsed">The new value for the IsCollapsed property.</param>
            protected virtual void OnIsCollapsedChanged(bool isCollapsed)
            {
                // Determine if we are dealing with a vertical or horizontal splitter.
                if (_gridCollapseDirection == GridCollapseDirection.Rows)
                {
                    if (_elementHorizontalGridSplitterHandle != null)
                    {
                        // Sets the target ToggleButton's IsChecked property equal
                        // to the provided isCollapsed property.
                        _elementHorizontalGridSplitterHandle.IsChecked = isCollapsed;
                    }
                }
                else
                {
                    if (_elementVerticalGridSplitterHandle != null)
                    {
                        // Sets the target ToggleButton's IsChecked property equal
                        // to the provided isCollapsed property.
                        _elementVerticalGridSplitterHandle.IsChecked = isCollapsed;
                    }
                }
            }

            /// <summary>
            /// Handles the property change event of the CollapseMode property.
            /// </summary>
            /// <param name="collapseMode">The new value for the CollapseMode property.</param>
            protected virtual void OnCollapseModeChanged(GridSplitterCollapseMode collapseMode)
            {
                // Hide the handles if the CollapseMode is set to None.
                if (collapseMode == GridSplitterCollapseMode.None)
                {
                    if (_elementHorizontalGridSplitterHandle != null)
                    {
                        _elementHorizontalGridSplitterHandle.Visibility = Visibility.Collapsed;
                    }
                    if (_elementVerticalGridSplitterHandle != null)
                    {
                        _elementVerticalGridSplitterHandle.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    // Ensure the handles are Visible.
                    if (_elementHorizontalGridSplitterHandle != null)
                    {
                        _elementHorizontalGridSplitterHandle.Visibility = Visibility.Visible;
                    }
                    if (_elementVerticalGridSplitterHandle != null)
                    {
                        _elementVerticalGridSplitterHandle.Visibility = Visibility.Visible;
                    }

                    //TODO:  Add in error handling if current tempalte does not include an existing ScaleTransform.

                    // Rotate the direction that the handle is facing depending on the CollapseMode.
                    if (collapseMode == GridSplitterCollapseMode.Previous)
                    {
                        if (_elementHorizontalGridSplitterHandle != null)
                        {
                            _elementHorizontalGridSplitterHandle.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, -1.0);
                        }
                        if (_elementVerticalGridSplitterHandle != null)
                        {
                            _elementVerticalGridSplitterHandle.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, -1.0);
                        }
                    }
                    else if (collapseMode == GridSplitterCollapseMode.Next)
                    {
                        if (_elementHorizontalGridSplitterHandle != null)
                        {
                            _elementHorizontalGridSplitterHandle.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, 1.0);
                        }
                        if (_elementVerticalGridSplitterHandle != null)
                        {
                            _elementVerticalGridSplitterHandle.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, 1.0);
                        }
                    }

                }

            }

        #endregion

        #region Collapse and Expand Methods

            //TODO:  Break out repetitive code in COLLAPSE and EXPAND methods

            /// <summary>
            /// Collapses the target ColumnDefinition or RowDefinition.
            /// </summary>
            private void Collapse()
            {
                Grid parentGrid = base.Parent as Grid;

                int splitterIndex = int.MinValue;

                if (_gridCollapseDirection == GridCollapseDirection.Rows)
                {
                    // Get the index of the row containing the splitter
                    splitterIndex = (int)base.GetValue(Grid.RowProperty);

                    // Determing the curent CollapseMode
                    if (this.CollapseMode == GridSplitterCollapseMode.Next)
                    {
                        // Save the next rows Height information
                        _savedGridLength = parentGrid.RowDefinitions[splitterIndex + 1].Height;
                        _savedActualValue = parentGrid.RowDefinitions[splitterIndex + 1].ActualHeight;
                        
                        // Collapse the next row
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.RowDefinitions[splitterIndex + 1]);
                        else
                            parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty, new GridLength(0));
                    }
                    else
                    {
                        // Save the previous row's Height information
                        _savedGridLength = parentGrid.RowDefinitions[splitterIndex - 1].Height;
                        _savedActualValue = parentGrid.RowDefinitions[splitterIndex - 1].ActualHeight;

                        // Collapse the previous row
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.RowDefinitions[splitterIndex - 1]);
                        else
                            parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty, new GridLength(0));
                    }
                }
                else
                {
                    // Get the index of the column containing the splitter
                    splitterIndex = (int)base.GetValue(Grid.ColumnProperty);

                    // Determing the curent CollapseMode
                    if (this.CollapseMode == GridSplitterCollapseMode.Next)
                    {
                        // Save the next column's Width information
                        _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex + 1].Width;
                        _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex + 1].ActualWidth;

                        // Collapse the next column
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                        else
                            parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty, new GridLength(0));
                    }
                    else
                    {
                        // Save the previous column's Width information
                        _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex - 1].Width;
                        _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex - 1].ActualWidth;

                        // Collapse the previous column
                        if (IsAnimated)
                            AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                        else
                            parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty, new GridLength(0));
                    }
                }

            }

            /// <summary>
            /// Expands the target ColumnDefinition or RowDefinition.
            /// </summary>
            private void Expand()
            {
                 Grid parentGrid = base.Parent as Grid;
                int splitterIndex = int.MinValue;

                if (_gridCollapseDirection == GridCollapseDirection.Rows)
                {
                    // Get the index of the row containing the splitter
                    splitterIndex = (int)this.GetValue(Grid.RowProperty);

                    // Determine the curent CollapseMode
                    if (this.CollapseMode == GridSplitterCollapseMode.Next)
                    {
                        // Expand the next row
                        if (IsAnimated)
                            AnimateExpand(parentGrid.RowDefinitions[splitterIndex + 1]);
                        else
                            parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                    }
                    else
                    {
                        // Expand the previous row
                        if (IsAnimated)
                            AnimateExpand(parentGrid.RowDefinitions[splitterIndex - 1]);
                        else
                            parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                    }
                }
                else
                {
                    // Get the index of the column containing the splitter
                    splitterIndex = (int)this.GetValue(Grid.ColumnProperty);

                    // Determine the curent CollapseMode
                    if (this.CollapseMode == GridSplitterCollapseMode.Next)
                    {
                        // Expand the next column
                        if (IsAnimated)
                            AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                        else
                            parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty, _savedGridLength);
                    }
                    else
                    {
                        // Expand the previous column
                        if (IsAnimated)
                            AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                        else
                            parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty, _savedGridLength);
                    }
                }
            }

            /// <summary>
            /// Determine the collapse direction based on the horizontal and vertical alignments
            /// </summary>
            private GridCollapseDirection GetCollapseDirection()
            {
                if (base.HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    return GridCollapseDirection.Columns;
                }
                if ((base.VerticalAlignment == VerticalAlignment.Stretch) && (base.ActualWidth <= base.ActualHeight))
                {
                    return GridCollapseDirection.Columns;
                }
                return GridCollapseDirection.Rows;
            }

        #endregion

        #region Event Handlers and Throwers

            // Define Collapsed and Expanded evenets
            public event EventHandler<EventArgs> Collapsed;
            public event EventHandler<EventArgs> Expanded;

            /// <summary>
            /// Raises the Collapsed event.
            /// </summary>
            /// <param name="e">Contains event arguments.</param>
            protected virtual void OnCollapsed(EventArgs e)
            {
                EventHandler<EventArgs> handler = this.Collapsed;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Raises the Expanded event.
            /// </summary>
            /// <param name="e">Contains event arguments.</param>
            protected virtual void OnExpanded(EventArgs e)
            {
                EventHandler<EventArgs> handler = this.Expanded;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Handles the Checked event of either the Vertical or Horizontal
            /// GridSplitterHandle ToggleButton.
            /// </summary>
            /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
            /// <param name="e">Contains event arguments for the routed event that fired.</param>
            private void GridSplitterHandle_Checked(object sender, RoutedEventArgs e)
            {
                
                //if (IsCollapsed != true)
                //{
                    // In our case, Checked = Collapsed.  Which means we want everything
                    // ready to be expanded.
                    Collapse();

                    IsCollapsed = true;

                    // Deactivate the background so the splitter can not be dragged.
                    _elementGridSplitterBackground.IsHitTestVisible = false;
                    _elementGridSplitterBackground.Opacity = 0.5;

                    // Raise the Collapsed event.
                    OnCollapsed(EventArgs.Empty);
                //}
            }

            /// <summary>
            /// Handles the Unchecked event of either the Vertical or Horizontal
            /// GridSplitterHandle ToggleButton.
            /// </summary>
            /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
            /// <param name="e">Contains event arguments for the routed event that fired.</param>
            private void GridSplitterHandle_Unchecked(object sender, RoutedEventArgs e)
            {
                //if (IsCollapsed != false)
                //{
                    // In our case, Unchecked = Expanded.  Which means we want everything
                    // ready to be collapsed.
                    Expand();

                    IsCollapsed = false;

                    // Activate the background so the splitter can be dragged again.
                    _elementGridSplitterBackground.IsHitTestVisible = true;
                    _elementGridSplitterBackground.Opacity = 1;

                    // Raise the Expanded event.
                    OnExpanded(EventArgs.Empty);
                //}
            }

        #endregion

        #region Collapse and Expand Animations

            /// <summary>
            /// Uses ObjectAnimatioNUsingKeyFrames and a StoryBoard to animated to expansion
            /// of the specificed ColumnDefinition or RowDefinition.
            /// </summary>
            /// <param name="definition">The RowDefinition or ColumnDefintition that will be collapsed.</param>
            private void AnimateCollapse(object definition)
            {
                double decrement;
                double currentValue;

                // Setup the animation and StoryBoard
                ObjectAnimationUsingKeyFrames gridLengthAnimation = new ObjectAnimationUsingKeyFrames();
                Storyboard sb = new Storyboard();

                // Add the animation to the StoryBoard
                sb.Children.Add(gridLengthAnimation);

                if (_gridCollapseDirection == GridCollapseDirection.Rows)
                {
                    // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                    RowDefinition rowDefinition = (RowDefinition)definition;
                    Storyboard.SetTarget(gridLengthAnimation, rowDefinition);
                    Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("Height"));

                    decrement = rowDefinition.ActualHeight / 5;
                    currentValue = rowDefinition.ActualHeight;
                }
                else
                {
                    // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                    ColumnDefinition colDefinition = (ColumnDefinition)definition;
                    Storyboard.SetTarget(gridLengthAnimation, colDefinition);
                    Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("Width"));

                    decrement = colDefinition.ActualWidth / 5;
                    currentValue = colDefinition.ActualWidth;
                }

                DiscreteObjectKeyFrame keyFrame1 = new DiscreteObjectKeyFrame();
                keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.1));
                currentValue = currentValue - decrement;
                keyFrame1.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame1);

                DiscreteObjectKeyFrame keyFrame2 = new DiscreteObjectKeyFrame();
                keyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.2));
                currentValue = currentValue - decrement;
                keyFrame2.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame2);

                DiscreteObjectKeyFrame keyFrame3 = new DiscreteObjectKeyFrame();
                keyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.3));
                currentValue = currentValue - decrement;
                keyFrame3.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame3);

                DiscreteObjectKeyFrame keyFrame4 = new DiscreteObjectKeyFrame();
                keyFrame4.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.4));
                currentValue = currentValue - decrement;
                keyFrame4.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame4);

                DiscreteObjectKeyFrame keyFrame5 = new DiscreteObjectKeyFrame();
                keyFrame5.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5));
                keyFrame5.Value = new GridLength(0);
                gridLengthAnimation.KeyFrames.Add(keyFrame5);

                // Start the StoryBoard.
                sb.Begin();

            }

            /// <summary>
            /// Uses ObjectAnimatioNUsingKeyFrames and a StoryBoard to animated to expansion
            /// of the specificed ColumnDefinition or RowDefinition.
            /// </summary>
            /// <param name="definition">The RowDefinition or ColumnDefintition that will be expanded.</param>
            private void AnimateExpand(object definition)
            {
                double increment;
                double currentValue;

                // Setup the animation and StoryBoard
                ObjectAnimationUsingKeyFrames gridLengthAnimation = new ObjectAnimationUsingKeyFrames();
                Storyboard sb = new Storyboard();

                // Add the animation to the StoryBoard
                sb.Children.Add(gridLengthAnimation);

                if (_gridCollapseDirection == GridCollapseDirection.Rows)
                {
                    // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                    RowDefinition rowDefinition = (RowDefinition)definition;
                    Storyboard.SetTarget(gridLengthAnimation, rowDefinition);
                    Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("Height"));

                    increment = _savedActualValue / 5;
                    currentValue = rowDefinition.ActualHeight;
                }
                else
                {
                    // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                    ColumnDefinition colDefinition = (ColumnDefinition)definition;
                    Storyboard.SetTarget(gridLengthAnimation, colDefinition);
                    Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("Width"));

                    increment = _savedActualValue / 5;
                    currentValue = colDefinition.ActualWidth;
                }

                // Create frames to incrementally expand the target.
                DiscreteObjectKeyFrame keyFrame1 = new DiscreteObjectKeyFrame();
                keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.1));
                currentValue = currentValue + increment;
                keyFrame1.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame1);

                DiscreteObjectKeyFrame keyFrame2 = new DiscreteObjectKeyFrame();
                keyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.2));
                currentValue = currentValue + increment;
                keyFrame2.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame2);

                DiscreteObjectKeyFrame keyFrame3 = new DiscreteObjectKeyFrame();
                keyFrame3.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.3));
                currentValue = currentValue + increment;
                keyFrame3.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame3);

                DiscreteObjectKeyFrame keyFrame4 = new DiscreteObjectKeyFrame();
                keyFrame4.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.4));
                currentValue = currentValue + increment;
                keyFrame4.Value = new GridLength(currentValue);
                gridLengthAnimation.KeyFrames.Add(keyFrame4);

                DiscreteObjectKeyFrame keyFrame5 = new DiscreteObjectKeyFrame();
                keyFrame5.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5));
                keyFrame5.Value = _savedGridLength;
                gridLengthAnimation.KeyFrames.Add(keyFrame5);

                // Start the StoryBoard.
                sb.Begin();

            }

        #endregion

        /// <summary>
        /// An enumeration that specifies the direction the GridSplitter will
        /// be collapased (Rows or Columns).
        /// </summary>
        internal enum GridCollapseDirection
        {
            Auto,
            Columns,
            Rows
        }
    }
}