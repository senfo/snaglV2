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
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;

namespace Berico.Windows.Controls
{

    /// <summary>
    /// The slider control lets the user select from a value range by moving a thumb
    /// along a track.  This slider provides enhanced functionality, allowing to to
    /// select an actual range (with an upper and lower bound) rather than just a
    /// single value.
    /// </summary>
    [TemplatePart(Name = Slider.ElementValueTipPopupName, Type = typeof(Popup))]
    [TemplatePart(Name = Slider.ElementValueTipPopupRootName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementValueTipTextBlockName, Type = typeof(TextBlock))]
    [TemplatePart(Name = Slider.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementHorizontalTickPanelName, Type = typeof(Canvas))]
    [TemplatePart(Name = Slider.ElementHorizontalDecreaseHandleName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementHorizontalIncreaseHandleName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementHorizontalSingleThumbTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementHorizontalSingleThumbLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementHorizontalSingleThumbLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementHorizontalThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementHorizontalRangeTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementHorizontalRangeLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementHorizontalRangeLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementHorizontalLowerThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementHorizontalRangeThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementHorizontalUpperThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementVerticalDecreaseHandleName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementVerticalIncreaseHandleName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementVerticalSingleThumbTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementVerticalSingleThumbLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementVerticalSingleThumbLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementVerticalThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementVerticalRangeTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = Slider.ElementVerticalRangeLargeIncreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementVerticalRangeLargeDecreaseName, Type = typeof(RepeatButton))]
    [TemplatePart(Name = Slider.ElementVerticalLowerThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementVerticalRangeThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = Slider.ElementVerticalUpperThumbName, Type = typeof(Thumb))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)] 
    public class Slider : DualRangeBase
    {

        #region Local Members

            /// <summary> 
            /// Whether the mouse is currently over the control
            /// </summary> 
            internal bool IsMouseOver { get; set; }
            
            /// <summary>
            /// The three values below are used for accumulating drag offsets incase the mouse
            /// cursor leaves the track.
            /// </summary>
            private double currentDragValue;
            private double currentLowerDragValue;
            private double currentUpperDragValue;
        
        #endregion

        #region Template Parts

            /// <summary> 
            /// Horizontal template root
            /// </summary>
            internal virtual FrameworkElement ElementHorizontalTemplate { get; set; } 
            internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

            /// <summary> 
            /// Horizontal tick mark panel
            /// </summary>
            internal virtual Canvas ElementHorizontalTickPanel { get; set; }
            internal const string ElementHorizontalTickPanelName = "HorizontalTickPanel";
            
            /// <summary> 
            /// Horizontal template root (for single thumb slider)
            /// </summary>
            internal virtual FrameworkElement ElementHorizontalSingleThumbTemplate { get; set; }
            internal const string ElementHorizontalSingleThumbTemplateName = "HorizontalSingleThumbTemplate";

            /// <summary> 
            /// Horizontal template root (for range slider)
            /// </summary>
            internal virtual FrameworkElement ElementHorizontalRangeTemplate { get; set; }
            internal const string ElementHorizontalRangeTemplateName = "HorizontalRangeTemplate";

            /// <summary> 
            /// Decrease handle
            /// </summary>
            internal virtual RepeatButton ElementHorizontalDecreaseHandle { get; set; }
            internal const string ElementHorizontalDecreaseHandleName = "HorizontalDecreaseHandle";

            /// <summary> 
            /// Increase handle
            /// </summary>
            internal virtual RepeatButton ElementHorizontalIncreaseHandle { get; set; }
            internal const string ElementHorizontalIncreaseHandleName = "HorizontalIncreaseHandle";

            /// <summary> 
            /// Large increase repeat button (for single thumb slider)
            /// </summary>
            internal virtual RepeatButton ElementHorizontalSingleThumbLargeIncrease { get; set; }
            internal const string ElementHorizontalSingleThumbLargeIncreaseName = "HorizontalSingleThumbLargeChangeIncreaseRepeatButton";

            /// <summary> 
            /// Large increase repeat button (for range slider)
            /// </summary>
            internal virtual RepeatButton ElementHorizontalRangeLargeIncrease { get; set; }
            internal const string ElementHorizontalRangeLargeIncreaseName = "HorizontalRangeLargeChangeIncreaseRepeatButton";

            /// <summary> 
            /// Large decrease repeat button  (for single thumb slider)
            /// </summary>
            internal virtual RepeatButton ElementHorizontalSingleThumbLargeDecrease { get; set; }
            internal const string ElementHorizontalSingleThumbLargeDecreaseName = "HorizontalSingleThumbLargeChangeDecreaseRepeatButton";

            /// <summary> 
            /// Large decrease repeat button  (for range slider)
            /// </summary>
            internal virtual RepeatButton ElementHorizontalRangeLargeDecrease { get; set; }
            internal const string ElementHorizontalRangeLargeDecreaseName = "HorizontalRangeLargeChangeDecreaseRepeatButton"; 

            /// <summary> 
            /// Thumb for dragging track (for single thumb slider)
            /// </summary>
            internal virtual Thumb ElementHorizontalThumb { get; set; }
            internal const string ElementHorizontalThumbName = "HorizontalThumb";

            /// <summary> 
            /// Thumb for dragging track (for range slider)
            /// </summary>
            internal virtual Thumb ElementHorizontalLowerThumb { get; set; }
            internal const string ElementHorizontalLowerThumbName = "HorizontalLowerThumb";

            /// <summary> 
            /// Thumb for dragging track (for range slider)
            /// </summary>
            internal virtual Thumb ElementHorizontalRangeThumb { get; set; }
            internal const string ElementHorizontalRangeThumbName = "HorizontalRangeThumb";

            /// <summary> 
            /// Thumb for dragging track (for range slider)
            /// </summary>
            internal virtual Thumb ElementHorizontalUpperThumb { get; set; }
            internal const string ElementHorizontalUpperThumbName = "HorizontalUpperThumb";

            /// <summary>
            /// Popup used to display current value(s) over the Thumb(s)
            /// when they are dragged.
            /// </summary>
            internal Popup ElementValueTipPopup { get; set; }
            internal const string ElementValueTipPopupName = "ValueTipPopup";

            /// <summary>
            /// The root Grid element of the popup.
            /// </summary>
            internal FrameworkElement ElementValueTipPopupRoot { get; set; }
            internal const string ElementValueTipPopupRootName = "ValueTipPopupRoot";

            /// <summary>
            /// The TextBlock that actually displays the appropriate value(s).
            /// </summary>
            internal TextBlock ElementValueTipTextBlock { get; set; }
            internal const string ElementValueTipTextBlockName = "ValueTipTextBlock";

            /// <summary> 
            /// Vertical template root
            /// </summary>
            internal virtual FrameworkElement ElementVerticalTemplate { get; set; }
            internal const string ElementVerticalTemplateName = "VerticalTemplate";

            /// <summary> 
            /// Vertical template root (for single thumb slider)
            /// </summary>
            internal virtual FrameworkElement ElementVerticalSingleThumbTemplate { get; set; }
            internal const string ElementVerticalSingleThumbTemplateName = "VerticalSingleThumbTemplate";

            /// <summary> 
            /// Vertical template root (for range slider)
            /// </summary>
            internal virtual FrameworkElement ElementVerticalRangeTemplate { get; set; }
            internal const string ElementVerticalRangeTemplateName = "VerticalRangeTemplate";

            /// <summary> 
            /// Decrease handle
            /// </summary>
            internal virtual RepeatButton ElementVerticalDecreaseHandle { get; set; }
            internal const string ElementVerticalDecreaseHandleName = "VerticalDecreaseHandle";

            /// <summary> 
            /// Increase handle
            /// </summary>
            internal virtual RepeatButton ElementVerticalIncreaseHandle { get; set; }
            internal const string ElementVerticalIncreaseHandleName = "VerticalIncreaseHandle";

            /// <summary> 
            /// Large increase repeat button (for single thumb slider)
            /// </summary>
            internal virtual RepeatButton ElementVerticalSingleThumbLargeIncrease { get; set; }
            internal const string ElementVerticalSingleThumbLargeIncreaseName = "VerticalSingleThumbLargeChangeIncreaseRepeatButton";

            /// <summary> 
            /// Large increase repeat button (for range slider)
            /// </summary>
            internal virtual RepeatButton ElementVerticalRangeLargeIncrease { get; set; }
            internal const string ElementVerticalRangeLargeIncreaseName = "VerticalRangeLargeChangeIncreaseRepeatButton";

            /// <summary> 
            /// Large decrease repeat button  (for single thumb slider)
            /// </summary>
            internal virtual RepeatButton ElementVerticalSingleThumbLargeDecrease { get; set; }
            internal const string ElementVerticalSingleThumbLargeDecreaseName = "VerticalSingleThumbLargeChangeDecreaseRepeatButton";

            /// <summary> 
            /// Large decrease repeat button  (for range slider)
            /// </summary>
            internal virtual RepeatButton ElementVerticalRangeLargeDecrease { get; set; }
            internal const string ElementVerticalRangeLargeDecreaseName = "VerticalRangeLargeChangeDecreaseRepeatButton";

            /// <summary> 
            /// Thumb for dragging track (for single thumb slider)
            /// </summary>
            internal virtual Thumb ElementVerticalThumb { get; set; }
            internal const string ElementVerticalThumbName = "VerticalThumb";

            /// <summary> 
            /// Thumb for dragging track (for range slider)
            /// </summary>
            internal virtual Thumb ElementVerticalLowerThumb { get; set; }
            internal const string ElementVerticalLowerThumbName = "VerticalLowerThumb";

            /// <summary> 
            /// Thumb for dragging track (for range slider)
            /// </summary>
            internal virtual Thumb ElementVerticalRangeThumb { get; set; }
            internal const string ElementVerticalRangeThumbName = "VerticalRangeThumb";

            /// <summary> 
            /// Thumb for dragging track (for range slider)
            /// </summary>
            internal virtual Thumb ElementVerticalUpperThumb { get; set; }
            internal const string ElementVerticalUpperThumbName = "VerticalUpperThumb";

        #endregion Template Parts

        #region Control Instantiation

            /// <summary>
            /// Initializes a new instance of the Slider class,
            /// which inherits from Berico.Windows.Controls.DualRangeBase.
            /// </summary>
            public Slider()
            {
                SizeChanged += delegate { UpdateTrackLayout(); SetupTickMarks(); };
                
                DefaultStyleKey = typeof(Slider);
                IsEnabledChanged += OnIsEnabledChanged;

            }

            /// <summary>
            /// This method is called when the template should be applied to the control.  It
            /// maps local properties to the XAML controls and provides initialization and 
            /// event hooking.
            /// </summary>
            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();

                // Get the horizontal template parts
                ElementHorizontalTemplate = GetTemplateChild(ElementHorizontalTemplateName) as FrameworkElement;
                ElementHorizontalTickPanel = GetTemplateChild(ElementHorizontalTickPanelName) as Canvas;
                ElementHorizontalSingleThumbTemplate = GetTemplateChild(ElementHorizontalSingleThumbTemplateName) as FrameworkElement;
                ElementHorizontalRangeTemplate = GetTemplateChild(ElementHorizontalRangeTemplateName) as FrameworkElement;
                ElementHorizontalDecreaseHandle = GetTemplateChild(ElementHorizontalDecreaseHandleName) as RepeatButton;
                ElementHorizontalIncreaseHandle = GetTemplateChild(ElementHorizontalIncreaseHandleName) as RepeatButton;
                ElementHorizontalSingleThumbLargeIncrease = GetTemplateChild(ElementHorizontalSingleThumbLargeIncreaseName) as RepeatButton;
                ElementHorizontalRangeLargeIncrease = GetTemplateChild(ElementHorizontalRangeLargeIncreaseName) as RepeatButton;
                ElementHorizontalSingleThumbLargeDecrease = GetTemplateChild(ElementHorizontalSingleThumbLargeDecreaseName) as RepeatButton;
                ElementHorizontalRangeLargeDecrease = GetTemplateChild(ElementHorizontalRangeLargeDecreaseName) as RepeatButton;
                ElementHorizontalThumb = GetTemplateChild(ElementHorizontalThumbName) as Thumb;
                ElementHorizontalLowerThumb = GetTemplateChild(ElementHorizontalLowerThumbName) as Thumb;
                ElementHorizontalRangeThumb = GetTemplateChild(ElementHorizontalRangeThumbName) as Thumb;
                ElementHorizontalUpperThumb = GetTemplateChild(ElementHorizontalUpperThumbName) as Thumb;

                // Get the vertical template parts
                ElementVerticalTemplate = GetTemplateChild(ElementVerticalTemplateName) as FrameworkElement;
                ElementVerticalSingleThumbTemplate = GetTemplateChild(ElementVerticalSingleThumbTemplateName) as FrameworkElement;
                ElementVerticalRangeTemplate = GetTemplateChild(ElementVerticalRangeTemplateName) as FrameworkElement;
                ElementVerticalDecreaseHandle = GetTemplateChild(ElementVerticalDecreaseHandleName) as RepeatButton;
                ElementVerticalIncreaseHandle = GetTemplateChild(ElementVerticalIncreaseHandleName) as RepeatButton;
                ElementVerticalSingleThumbLargeIncrease = GetTemplateChild(ElementVerticalSingleThumbLargeIncreaseName) as RepeatButton;
                ElementVerticalRangeLargeIncrease = GetTemplateChild(ElementVerticalRangeLargeIncreaseName) as RepeatButton;
                ElementVerticalSingleThumbLargeDecrease = GetTemplateChild(ElementVerticalSingleThumbLargeDecreaseName) as RepeatButton;
                ElementVerticalRangeLargeDecrease = GetTemplateChild(ElementVerticalRangeLargeDecreaseName) as RepeatButton;
                ElementVerticalThumb = GetTemplateChild(ElementVerticalThumbName) as Thumb;
                ElementVerticalLowerThumb = GetTemplateChild(ElementVerticalLowerThumbName) as Thumb;
                ElementVerticalRangeThumb = GetTemplateChild(ElementVerticalRangeThumbName) as Thumb;
                ElementVerticalUpperThumb = GetTemplateChild(ElementVerticalUpperThumbName) as Thumb;

                // Get the value tool tip parts
                ElementValueTipPopup = GetTemplateChild(ElementValueTipPopupName) as Popup;
                ElementValueTipPopupRoot = GetTemplateChild(ElementValueTipPopupRootName) as FrameworkElement;
                ElementValueTipTextBlock = GetTemplateChild(ElementValueTipTextBlockName) as TextBlock;

                if (ElementHorizontalThumb != null)
                {
                    ElementHorizontalThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnThumbDragStarted(); };
                    ElementHorizontalThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
                    ElementHorizontalThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementHorizontalLowerThumb != null)
                {
                    ElementHorizontalLowerThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnLowerThumbDragStarted(); };
                    ElementHorizontalLowerThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnLowerThumbDragDelta(e); };
                    ElementHorizontalLowerThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementHorizontalUpperThumb != null)
                {
                    ElementHorizontalUpperThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnUpperThumbDragStarted(); };
                    ElementHorizontalUpperThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnUpperThumbDragDelta(e); };
                    ElementHorizontalUpperThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementHorizontalRangeThumb != null)
                {
                    ElementHorizontalRangeThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnRangeThumbDragStarted(); };
                    ElementHorizontalRangeThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnRangeThumbDragDelta(e); };
                    ElementHorizontalRangeThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementHorizontalSingleThumbLargeDecrease != null)
                {
                    // Decrease the Value by LargeChange
                    ElementHorizontalSingleThumbLargeDecrease.Click += delegate { this.Focus(); Value -= LargeChange; };
                }
                if (ElementHorizontalSingleThumbLargeIncrease != null)
                {
                    // Increase the Value by LargeChange
                    ElementHorizontalSingleThumbLargeIncrease.Click += delegate { this.Focus(); Value += LargeChange; };
                }
                if (ElementHorizontalRangeLargeDecrease != null)
                {
                    // Decrease the LowerRangeValue by LargeChange
                    ElementHorizontalRangeLargeDecrease.Click += delegate { this.Focus(); LowerRangeValue -= LargeChange; };
                }
                if (ElementHorizontalRangeLargeIncrease != null)
                {
                    // Increase the UpperRangeValue by LargeChange
                    ElementHorizontalRangeLargeIncrease.Click += delegate { this.Focus(); UpperRangeValue += LargeChange; };
                }

                if (ElementHorizontalDecreaseHandle != null)
                {
                    ElementHorizontalDecreaseHandle.ClickMode = ClickMode.Press;
                    ElementHorizontalDecreaseHandle.Click += new RoutedEventHandler(OnDecreaseHandleClick);
                }
                if (ElementHorizontalIncreaseHandle != null)
                {
                    ElementHorizontalIncreaseHandle.ClickMode = ClickMode.Press;
                    ElementHorizontalIncreaseHandle.Click += new RoutedEventHandler(OnIncreaseHandleClick);
                }

                if (ElementVerticalThumb != null)
                {
                    ElementVerticalThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnThumbDragStarted(); };
                    ElementVerticalThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnThumbDragDelta(e); };
                    ElementVerticalThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementVerticalLowerThumb != null)
                {
                    ElementVerticalLowerThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnLowerThumbDragStarted(); };
                    ElementVerticalLowerThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnLowerThumbDragDelta(e); };
                    ElementVerticalLowerThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementVerticalUpperThumb != null)
                {
                    ElementVerticalUpperThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnUpperThumbDragStarted(); };
                    ElementVerticalUpperThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnUpperThumbDragDelta(e); };
                    ElementVerticalUpperThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementVerticalRangeThumb != null)
                {
                    ElementVerticalRangeThumb.DragStarted += delegate(object sender, DragStartedEventArgs e) { this.Focus(); OnRangeThumbDragStarted(); };
                    ElementVerticalRangeThumb.DragDelta += delegate(object sender, DragDeltaEventArgs e) { OnRangeThumbDragDelta(e); };
                    ElementVerticalRangeThumb.DragCompleted += delegate(object sender, DragCompletedEventArgs e) { OnThumbDragCompleted(); };
                }
                if (ElementVerticalSingleThumbLargeDecrease != null)
                {
                    // Decrease the Value by LargeChange
                    ElementVerticalSingleThumbLargeDecrease.Click += delegate { this.Focus(); Value -= LargeChange; };
                }
                if (ElementVerticalSingleThumbLargeIncrease != null)
                {
                    // Increase the Value by LargeChange
                    ElementVerticalSingleThumbLargeIncrease.Click += delegate { this.Focus(); Value += LargeChange; };
                }
                if (ElementVerticalRangeLargeDecrease != null)
                {
                    // Decrease the LowerRangeValue by LargeChange
                    ElementVerticalRangeLargeDecrease.Click += delegate { this.Focus(); LowerRangeValue -= LargeChange; };
                }
                if (ElementVerticalRangeLargeIncrease != null)
                {
                    // Increase the UpperRangeValue by LargeChange
                    ElementVerticalRangeLargeIncrease.Click += delegate { this.Focus(); UpperRangeValue += LargeChange; };
                }

                if (ElementVerticalDecreaseHandle != null)
                {
                    ElementVerticalDecreaseHandle.ClickMode = ClickMode.Press;
                    ElementVerticalDecreaseHandle.Click += new RoutedEventHandler(OnDecreaseHandleClick);
                }
                if (ElementVerticalIncreaseHandle != null)
                {
                    ElementVerticalIncreaseHandle.ClickMode = ClickMode.Press;
                    ElementVerticalIncreaseHandle.Click += new RoutedEventHandler(OnIncreaseHandleClick);
                }

                // Updating states for parts where properties might have been updated through 
                // XAML before the template was loaded. 
                OnOrientationChanged();
                OnIsRangeEnabledChanged();
                OnShowHandlesChanged(ShowHandles);
                OnTickLocationChanged(TickLocation);
                ChangeVisualState(false);
            }

        #endregion

        #region Properties

            #region Orientation

                /// <summary>
                /// Gets whether the Slider has an orientation of vertical or horizontal. 
                /// </summary> 
                public Orientation Orientation
                {
                    get { return (Orientation)GetValue(OrientationProperty); }
                    set { SetValue(OrientationProperty, value); }
                }

                /// <summary>
                /// Identifies the Orientation dependency property. 
                /// </summary> 
                public static readonly DependencyProperty OrientationProperty =
                    DependencyProperty.Register(
                        "Orientation",
                        typeof(Orientation),
                        typeof(Slider),
                        new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

                /// <summary> 
                /// OrientationProperty property changed handler. 
                /// </summary>
                /// <param name="d">Slider that changed Orientation.</param> 
                /// <param name="e">DependencyPropertyChangedEventArgs.</param>
                private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.OnOrientationChanged();
                }

                /// <summary>
                /// This code will run whenever Orientation changes, to change the template 
                /// being used to display this control.
                /// </summary>
                internal virtual void OnOrientationChanged()
                {
                    if (ElementHorizontalTemplate != null)
                    {
                        ElementHorizontalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed);
                    }
                    if (ElementVerticalTemplate != null)
                    {
                        ElementVerticalTemplate.Visibility = (Orientation == Orientation.Horizontal ? Visibility.Collapsed : Visibility.Visible);
                    }
                    UpdateTrackLayout();
                }

            #endregion Orientation

            #region IsFocused

                /// <summary>
                /// Gets a value that determines whether this element has logical focus.
                /// </summary> 
                public bool IsFocused
                {
                    get { return (bool)GetValue(IsFocusedProperty); }
                    internal set { SetValue(IsFocusedProperty, value); }
                }

                /// <summary>
                /// Identifies the IsFocused dependency property.
                /// </summary> 
                public static readonly DependencyProperty IsFocusedProperty =
                    DependencyProperty.Register(
                        "IsFocused",
                        typeof(bool),
                        typeof(Slider),
                        new PropertyMetadata(OnIsFocusedPropertyChanged));

                /// <summary>
                /// IsFocusedProperty property changed handler. 
                /// </summary> 
                /// <param name="d">Slider that changed IsFocused.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
                private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.OnIsFocusChanged(e);
                }

                /// <summary> 
                /// Called when the IsFocused property changes.
                /// </summary>
                /// <param name="e"> 
                /// The data for DependencyPropertyChangedEventArgs.
                /// </param>
                internal virtual void OnIsFocusChanged(DependencyPropertyChangedEventArgs e)
                {
                    UpdateVisualState();
                }

            #endregion IsFocused 

            #region ShowHandles

                // Defines the ShowHandles dependency property.
                public static readonly DependencyProperty ShowHandlesProperty = DependencyProperty.Register("ShowHandles", typeof(bool), typeof(Slider), new PropertyMetadata(true, OnShowHandlesPropertyChanged));

                // Gets or sets the ShowHandles value.
                public bool ShowHandles
                {
                    get { return (bool)GetValue(ShowHandlesProperty); }
                    set { SetValue(ShowHandlesProperty, value); }
                }

                /// <summary>
                /// The PropertyChangedCallback that is called when the ShowHandlesProperty is changed.
                /// </summary>
                /// <param name="d">An instance of the DualRangeBase object whose ShowHandles property was changed.</param>
                /// <param name="e">An instanced of the DependencyPropertyChangedEventArgs class.</param>
                private static void OnShowHandlesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    if (slider != null)
                    {
                        bool value = (bool)e.NewValue;
                        slider.OnShowHandlesChanged(value);
                    }
                }

                /// <summary>
                /// Called when the ShowHandles property changes.
                /// </summary>
                /// <param name="isEnabled">The new value of the ShowHandles property.</param>
                internal virtual void OnShowHandlesChanged(bool showHandles)
                {

                    if (Orientation == Orientation.Horizontal)
                    {
                        if (ElementHorizontalDecreaseHandle != null)
                            ElementHorizontalDecreaseHandle.Visibility = showHandles ? Visibility.Visible : Visibility.Collapsed;

                        if (ElementHorizontalIncreaseHandle != null)
                            ElementHorizontalIncreaseHandle.Visibility = showHandles ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        if (ElementVerticalDecreaseHandle != null)
                            ElementVerticalDecreaseHandle.Visibility = showHandles ? Visibility.Visible : Visibility.Collapsed;

                        if (ElementVerticalIncreaseHandle != null)
                            ElementVerticalIncreaseHandle.Visibility = showHandles ? Visibility.Visible : Visibility.Collapsed;
                    }
                }

            #endregion

            #region TickFrequency

                // Defines the TickFrequency dependency property.
                public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), typeof(Slider), new PropertyMetadata(2.0, OnTickFrequencyPropertyChanged));

                // Gets or sets the TickFrequency value.
                public double TickFrequency
                {
                    get { return (double)GetValue(TickFrequencyProperty); }
                    set { SetValue(TickFrequencyProperty, value); }
                }

                /// <summary>
                /// The PropertyChangedCallback that is called when the TickFrequencyProperty is changed.
                /// </summary>
                /// <param name="d">An instance of the DualRangeBase object whose ShowTickMarks property was changed.</param>
                /// <param name="e">An instance of the DependencyPropertyChangedEventArgs class.</param>
                private static void OnTickFrequencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    if (slider != null)
                    {
                        slider.OnTickFrequencyChanged();
                    }
                }

                /// <summary>
                /// Called when the TickFrequency property changes.
                /// </summary>
                internal virtual void OnTickFrequencyChanged()
                {
                    CoerceTickFrequency();
                    SetupTickMarks();
                }

            #endregion

            #region TickLocation

                /// <summary>
                /// Gets or sets the TickLocation.  The default is None, which
                /// indicates that tick marks are not shown
                /// </summary> 
                public TickLocation TickLocation
                {
                    get { return (TickLocation)GetValue(TickLocationProperty); }
                    set { SetValue(TickLocationProperty, value); }
                }

                /// <summary>
                /// Identifies the TickLocation dependency property. 
                /// </summary> 
                public static readonly DependencyProperty TickLocationProperty =
                    DependencyProperty.Register(
                        "TickLocation",
                        typeof(TickLocation),
                        typeof(Slider),
                        new PropertyMetadata(TickLocation.None, OnTickLocationPropertyChanged));

                /// <summary> 
                /// TickLocationProperty changed handler.
                /// </summary>
                /// <param name="d">Slider that changed TickLocation.</param> 
                /// <param name="e">DependencyPropertyChangedEventArgs.</param>
                private static void OnTickLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.OnTickLocationChanged((TickLocation)e.NewValue);
                }

                /// <summary>
                /// This code will run whenever TickLocation changes
                /// </summary>
                internal virtual void OnTickLocationChanged(TickLocation location)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (ElementHorizontalTickPanel != null)
                        {
                            if (location == TickLocation.None)
                                ElementHorizontalTickPanel.Visibility = Visibility.Collapsed;
                            else
                            {
                                ElementHorizontalTickPanel.Visibility = Visibility.Visible;
                                SetupTickMarks();
                            }
                        }

                    }
                    else
                    {
                        //TODO:  IMPLEMENT FOR VERTICAL TEMPLATE
                    }
                }

            #endregion Orientation

            #region TickTemplate

                /// <summary>
                /// Gets or sets the DataTemplate for tick marks
                /// </summary> 
                public DataTemplate TickTemplate
                {
                    get { return (DataTemplate)GetValue(TickTemplateProperty); }
                    set { SetValue(TickTemplateProperty, value); }
                }

                /// <summary>
                /// Identifies the TickTemplate dependency property. 
                /// </summary> 
                public static readonly DependencyProperty TickTemplateProperty =
                    DependencyProperty.Register(
                        "TickTemplate",
                        typeof(DataTemplate),
                        typeof(Slider),
                        null);

                /// <summary> 
                /// TickTemplateProperty changed handler.
                /// </summary>
                /// <param name="d">Slider that changed TickLocation.</param> 
                /// <param name="e">DependencyPropertyChangedEventArgs.</param>
                private static void OnTickTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.OnTickTemplateChanged();
                }

                /// <summary>
                /// This code will run whenever TickTemplate changes
                /// </summary>
                internal virtual void OnTickTemplateChanged()
                {
                    SetupTickMarks();
                }

            #endregion Orientation

            #region IsDirectionReversed
                /// <summary> 
                /// Gets a value that determines whether the direction is reversed. 
                /// </summary>
                public bool IsDirectionReversed
                {
                    get { return (bool)GetValue(IsDirectionReversedProperty); }
                    set { SetValue(IsDirectionReversedProperty, value); }
                }

                /// <summary> 
                /// Identifies the IsDirectionReversed dependency property. 
                /// </summary>
                public static readonly DependencyProperty IsDirectionReversedProperty =
                    DependencyProperty.Register(
                        "IsDirectionReversed",
                        typeof(bool),
                        typeof(Slider),
                        new PropertyMetadata(OnIsDirectionReversedChanged));

                /// <summary> 
                /// IsDirectionReversedProperty property changed handler.
                /// </summary> 
                /// <param name="d">Slider that changed IsDirectionReversed.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param>
                private static void OnIsDirectionReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.UpdateTrackLayout();

                }

            #endregion IsDirectionReversed 

            #region IsRangeEnabled

                /// <summary> 
                /// Gets a value that determines whether the slider is a regular
                /// slider or a range slider.
                /// </summary>
                public bool IsRangeEnabled
                {
                    get { return (bool)GetValue(IsRangeEnabledProperty); }
                    set { SetValue(IsRangeEnabledProperty, value); }
                }

                /// <summary> 
                /// Identifies the IsRangeEnabled dependency property. 
                /// </summary>
                public static readonly DependencyProperty IsRangeEnabledProperty =
                    DependencyProperty.Register(
                        "IsRangeEnabled",
                        typeof(bool),
                        typeof(Slider),
                        new PropertyMetadata(OnIsRangeEnabledPropertyChanged));

                /// <summary> 
                /// IsRangeEnabledProperty property changed handler.
                /// </summary> 
                /// <param name="d">Slider that changed IsRangeEnabled.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param>
                private static void OnIsRangeEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.OnIsRangeEnabledChanged();

                }

                /// <summary>
                /// This code will run whenever IsRangeEnabled changes, to change the template 
                /// being used to display this control.
                /// </summary>
                internal virtual void OnIsRangeEnabledChanged()
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (ElementHorizontalTemplate != null && ElementHorizontalSingleThumbTemplate != null && ElementHorizontalRangeTemplate != null)
                        {
                            if (IsRangeEnabled)
                            {
                                ElementHorizontalSingleThumbTemplate.Visibility = Visibility.Collapsed;
                                ElementHorizontalRangeTemplate.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ElementHorizontalSingleThumbTemplate.Visibility = Visibility.Visible;
                                ElementHorizontalRangeTemplate.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    else
                    {
                        if (ElementVerticalTemplate != null && ElementVerticalSingleThumbTemplate != null && ElementVerticalRangeTemplate != null)
                        {
                            if (IsRangeEnabled)
                            {
                                ElementVerticalSingleThumbTemplate.Visibility = Visibility.Collapsed;
                                ElementVerticalRangeTemplate.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ElementVerticalSingleThumbTemplate.Visibility = Visibility.Visible;
                                ElementVerticalRangeTemplate.Visibility = Visibility.Collapsed;
                            }
                        }
                    }

                    UpdateTrackLayout();
                }

            #endregion IsDirectionReversed 

            #region IsSnapToTickEnabled

                /// <summary> 
                /// Gets a value that determines whether the thumb will
                /// snap to tick marks or not
                /// </summary>
                public bool IsSnapToTickEnabled
                {
                    get { return (bool)GetValue(IsSnapToTickEnabledProperty); }
                    set { SetValue(IsSnapToTickEnabledProperty, value); }
                }

                /// <summary> 
                /// Identifies the OnIsSnapToTick dependency property. 
                /// </summary>
                public static readonly DependencyProperty IsSnapToTickEnabledProperty =
                    DependencyProperty.Register(
                        "IsSnapToTick",
                        typeof(bool),
                        typeof(Slider),
                        new PropertyMetadata(false, OnIsSnapToTickEnabledPropertyChanged));

                /// <summary> 
                /// IsRangeEnabledProperty property changed handler.
                /// </summary> 
                /// <param name="d">Slider that changed IsRangeEnabled.</param>
                /// <param name="e">DependencyPropertyChangedEventArgs.</param>
                private static void OnIsSnapToTickEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    Slider slider = d as Slider;

                    slider.OnIsSnapToTickChanged();
                }

                /// <summary>
                /// This code will run whenever IsRangeEnabled changes, to change the template 
                /// being used to display this control.
                /// </summary>
                internal virtual void OnIsSnapToTickChanged()
                {
                    // Snap to the nearest TickLocation
                    //Value = SnapToNearestTick(Value);
                }

            #endregion IsDirectionReversed 

            #region ShowValueTips

                // Defines the ShowValueTips dependency property.
                public static readonly DependencyProperty ShowValueTipsProperty = DependencyProperty.Register("ShowValueTips", typeof(bool), typeof(Slider), new PropertyMetadata(true));

                // Gets or sets the ShowValueTips value.
                public bool ShowValueTips
                {
                    get { return (bool)GetValue(ShowValueTipsProperty); }
                    set { SetValue(ShowValueTipsProperty, value); }
                }

            #endregion

            #region ValueTipsText

                // Defines the ValueTipsText dependency property.
                public static readonly DependencyProperty ValueTipsTextProperty = DependencyProperty.Register("ValueTipsText", typeof(string), typeof(Slider), new PropertyMetadata(""));

                // Gets or sets the ValueTipsText value.
                public string ValueTipsText
                {
                    get { return (string)GetValue(ValueTipsTextProperty); }
                    set { SetValue(ValueTipsTextProperty, value); }
                }
                
            #endregion

        #endregion

        #region Event Handlers

            #region Focus Events
                    /// <summary>
                    /// Appropriately sets the local IsFocused variable.
                    /// </summary>
                    /// <param name="e">An instance of RoutedEventArgs.</param>
                    protected override void OnGotFocus(RoutedEventArgs e)
                    {
                        base.OnGotFocus(e);
                        IsFocused = true;
                    }

                    /// <summary>
                    /// Appropriately sets the local IsFocused variable.
                    /// </summary>
                    /// <param name="e">An instance of RoutedEventArgs.</param>
                    protected override void OnLostFocus(RoutedEventArgs e)
                    {
                        base.OnLostFocus(e);
                        IsFocused = false;
                    }
                #endregion

            #region Drag Events

                /// <summary>
                /// When the thumb is dragged, this event handles updating the
                /// current value based on the thumbs delta.
                /// </summary>
                /// <param name="e">An instance of the DragDeltaEventArgs class.</param>
                private void OnThumbDragDelta(DragDeltaEventArgs e)
                {
                    double offset = 0;

                    if (Orientation == Orientation.Horizontal && ElementHorizontalThumb != null)
                    {
                        offset = e.HorizontalChange / (ElementHorizontalSingleThumbTemplate.ActualWidth - ElementHorizontalThumb.ActualWidth) * (Maximum - Minimum);
                    }
                    else if (Orientation == Orientation.Vertical && ElementVerticalThumb != null)
                    {
                        offset = -e.VerticalChange / (ElementVerticalSingleThumbTemplate.ActualHeight - ElementVerticalThumb.ActualHeight) * (Maximum - Minimum);
                    }

                    if (!double.IsNaN(offset) && !double.IsInfinity(offset))
                    {
                        currentDragValue += IsDirectionReversed ? -offset : offset;

                        double newValue = Math.Min(Maximum, Math.Max(Minimum, currentDragValue));
                        double currentValue = Value;

                        if (newValue != Value)
                        {
                            if (IsSnapToTickEnabled)
                                Value = SnapToNearestTick(newValue, Value);
                            else
                                Value = newValue;
                        }
                    }

                    if (Orientation == Orientation.Horizontal)
                        PositionToolTipHorizontally(ElementHorizontalThumb);
                    else
                        PositionToolTipVertically(ElementVerticalThumb);
                }

                /// <summary>
                /// Called whenever the Thumb drag operation is started
                /// </summary>
                private void OnThumbDragStarted()
                {
                    this.currentDragValue = this.Value;

                    ChangeValueTip();

                    Dispatcher.BeginInvoke(delegate
                    {
                        if (Orientation == Orientation.Horizontal)
                            PositionToolTipHorizontally(ElementHorizontalThumb);
                        else
                            PositionToolTipVertically(ElementVerticalThumb);
                    });

                    OpenTips();
                }

                /// <summary>
                /// Called whenver the Thumb drag operation has completed
                /// </summary>
                private void OnThumbDragCompleted()
                {
                    CloseTips();
                }

                /// <summary>
                /// Called whenever the LowerRangeThumb is being dragged to update
                /// the value based on the thumbs drag delta value.
                /// </summary>
                /// <param name="e">An instance of the DragDeltaEventArgs class.</param>
                private void OnLowerThumbDragDelta(DragDeltaEventArgs e)
                {
                    double offset = 0;

                    if (Orientation == Orientation.Horizontal && ElementHorizontalLowerThumb != null)
                    {
                        // Calculate the offset value 
                        offset = e.HorizontalChange / (ElementHorizontalRangeTemplate.ActualWidth - ElementHorizontalLowerThumb.ActualWidth) * (Maximum - Minimum);
                    }
                    else if (Orientation == Orientation.Vertical && ElementVerticalLowerThumb != null)
                    {
                        offset = -e.VerticalChange / (ElementVerticalRangeTemplate.ActualHeight - ElementVerticalLowerThumb.ActualHeight) * (Maximum - Minimum);
                    }

                    if (!double.IsNaN(offset) && !double.IsInfinity(offset))
                    {
                        // Increase the current value by the offset
                        currentLowerDragValue += IsDirectionReversed ? -offset : offset;

                        double newValue = Math.Min(Maximum, Math.Max(Minimum, currentLowerDragValue));

                        // Set the LowerRangeValue accordingly
                        if (newValue != LowerRangeValue)
                            if (IsSnapToTickEnabled)
                                LowerRangeValue = SnapToNearestTick(newValue, LowerRangeValue);
                            else
                                LowerRangeValue = newValue;
                    }

                    if (Orientation == Orientation.Horizontal)
                        PositionToolTipHorizontally(ElementHorizontalRangeThumb);
                    else
                        PositionToolTipVertically(ElementVerticalRangeThumb);
                }

                /// <summary>
                /// Called whenever the LowerRangeThumb drag operation is started
                /// </summary>
                private void OnLowerThumbDragStarted()
                {
                    this.currentLowerDragValue = this.LowerRangeValue;

                    ChangeValueTip();

                    Dispatcher.BeginInvoke(delegate
                    {
                        if (Orientation == Orientation.Horizontal)
                            PositionToolTipHorizontally(ElementHorizontalRangeThumb);
                        else
                            PositionToolTipVertically(ElementVerticalRangeThumb);
                    });

                    OpenTips();
                }

                /// <summary>
                /// Called whenever the UpperRangeThumb is being dragged to update
                /// the value based on the thumbs drag delta value.
                /// </summary>
                /// <param name="e">An instance of the DragDeltaEventArgs class.</param>
                private void OnUpperThumbDragDelta(DragDeltaEventArgs e)
                {
                    double offset = 0;

                    if (Orientation == Orientation.Horizontal && ElementHorizontalUpperThumb != null)
                    {
                        // Calculate the offset value 
                        offset = e.HorizontalChange / (ElementHorizontalRangeTemplate.ActualWidth - ElementHorizontalUpperThumb.ActualWidth) * (Maximum - Minimum);
                    }
                    else if (Orientation == Orientation.Vertical && ElementVerticalUpperThumb != null)
                    {
                        offset = -e.VerticalChange / (ElementVerticalRangeTemplate.ActualHeight - ElementVerticalUpperThumb.ActualHeight) * (Maximum - Minimum);
                    }

                    if (!double.IsNaN(offset) && !double.IsInfinity(offset))
                    {
                        // Increase the current value by the offset
                        currentUpperDragValue += IsDirectionReversed ? -offset : offset;

                        double newValue = Math.Min(Maximum, Math.Max(Minimum, currentUpperDragValue));

                        // Set the UpperRangeValue accordingly
                        if (newValue != UpperRangeValue)
                            if (IsSnapToTickEnabled)
                                UpperRangeValue = SnapToNearestTick(newValue, UpperRangeValue);
                            else
                                UpperRangeValue = newValue;
                    }

                    if (Orientation == Orientation.Horizontal)
                        PositionToolTipHorizontally(ElementHorizontalRangeThumb);
                    else
                        PositionToolTipVertically(ElementVerticalRangeThumb);
                }

                /// <summary>
                /// Called whenever the UpperRangeThumb drag operation is started
                /// </summary>
                private void OnUpperThumbDragStarted()
                {
                    this.currentUpperDragValue = this.UpperRangeValue;

                    ChangeValueTip();

                    Dispatcher.BeginInvoke(delegate
                    {
                        if (Orientation == Orientation.Horizontal)
                            PositionToolTipHorizontally(ElementHorizontalRangeThumb);
                        else
                            PositionToolTipVertically(ElementVerticalRangeThumb);
                    });

                    OpenTips();
                }

                /// <summary>
                /// Called whenever the actual range thumb is being dragged to update
                /// the value based on the thumbs drag delta value.
                /// </summary>
                /// <param name="e">An instance of the DragDeltaEventArgs class.</param>
                private void OnRangeThumbDragDelta(DragDeltaEventArgs e)
                {
                    //TODO:  NEED TO TAKE ORIENTATION INTO ACCOUNT
                    double offset = 0;

                    if (Orientation == Orientation.Horizontal && ElementHorizontalRangeThumb != null)
                    {
                        // Calculate the offset value 
                        offset = e.HorizontalChange / (ElementHorizontalRangeTemplate.ActualWidth - ElementHorizontalRangeThumb.ActualWidth) * (Maximum - Minimum);
                    }
                    else if (Orientation == Orientation.Vertical && ElementVerticalRangeThumb != null)
                    {
                        offset = -e.VerticalChange / (ElementVerticalRangeTemplate.ActualHeight - ElementVerticalRangeThumb.ActualHeight) * (Maximum - Minimum);
                    }

                    if (!double.IsNaN(offset) && !double.IsInfinity(offset))
                    {
                        // Increase the current lower value by the offset
                        currentLowerDragValue += IsDirectionReversed ? -offset : offset;

                        double newValue = Math.Min(Maximum, Math.Max(Minimum, currentLowerDragValue));

                        // Set the LowerRangeValue accordingly
                        if (newValue != LowerRangeValue)
                            if (IsSnapToTickEnabled)
                                LowerRangeValue = SnapToNearestTick(newValue, LowerRangeValue);
                            else
                                LowerRangeValue = newValue;

                        // Increase the current upper value by the offset
                        currentUpperDragValue += IsDirectionReversed ? -offset : offset;
                        newValue = Math.Min(Maximum, Math.Max(Minimum, currentUpperDragValue));

                        // Set the UpperRangeValue accordingly
                        if (newValue != UpperRangeValue)
                            if (IsSnapToTickEnabled)
                                UpperRangeValue = SnapToNearestTick(newValue, UpperRangeValue);
                            else
                                UpperRangeValue = newValue;
                    }

                    if (Orientation == Orientation.Horizontal)
                        PositionToolTipHorizontally(ElementHorizontalRangeThumb);
                    else
                        PositionToolTipVertically(ElementVerticalRangeThumb);
                }

                /// <summary>
                /// Called whenever the RangeThumb drag operation is started
                /// </summary>
                private void OnRangeThumbDragStarted()
                {
                    // Record the current lower and upper values to local variables.
                    currentLowerDragValue = this.LowerRangeValue;
                    currentUpperDragValue = this.UpperRangeValue;

                    ChangeValueTip();

                    Dispatcher.BeginInvoke(delegate
                    {
                        if (Orientation == Orientation.Horizontal)
                            PositionToolTipHorizontally(ElementHorizontalRangeThumb);
                        else
                            PositionToolTipVertically(ElementVerticalRangeThumb);
                    });
                   
                    OpenTips();

                }

            #endregion

            #region Mouse Events

                /// <summary>
                /// Responds to the MouseEnter event.
                /// </summary>
                /// <param name="e">The event data for the MouseEnter event.</param>
                protected override void OnMouseEnter(MouseEventArgs e)
                {
                    base.OnMouseEnter(e);

                    IsMouseOver = true;

                    if (IsRangeEnabled)
                    {
                        if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                            (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
                        {
                            UpdateVisualState();
                        }
                    }
                    else
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (ElementHorizontalLowerThumb != null && !ElementHorizontalThumb.IsDragging && ElementHorizontalLowerThumb != null
                                && !ElementHorizontalUpperThumb.IsDragging && ElementHorizontalRangeThumb != null && !ElementHorizontalRangeThumb.IsDragging)
                            {
                                UpdateVisualState();
                            }
                        }
                        else
                        {
                            if (ElementVerticalLowerThumb != null && !ElementVerticalThumb.IsDragging && ElementVerticalLowerThumb != null
                                && !ElementVerticalUpperThumb.IsDragging && ElementVerticalRangeThumb != null && !ElementVerticalRangeThumb.IsDragging)
                            {
                                UpdateVisualState();
                            }
                        }
                    }
                }

                /// <summary>
                /// Responds to the MouseLeave event.
                /// </summary>
                /// <param name="e">The event data for the MouseLeave event.</param>
                protected override void OnMouseLeave(MouseEventArgs e)
                {
                    base.OnMouseLeave(e);

                    IsMouseOver = false;

                    if (IsRangeEnabled)
                    {
                        if ((Orientation == Orientation.Horizontal && ElementHorizontalThumb != null && !ElementHorizontalThumb.IsDragging) ||
                            (Orientation == Orientation.Vertical && ElementVerticalThumb != null && !ElementVerticalThumb.IsDragging))
                        {
                            UpdateVisualState();
                        }
                    }
                    else
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (ElementHorizontalLowerThumb != null && !ElementHorizontalThumb.IsDragging && ElementHorizontalLowerThumb != null
                                && !ElementHorizontalUpperThumb.IsDragging && ElementHorizontalRangeThumb != null && !ElementHorizontalRangeThumb.IsDragging)
                            {
                                UpdateVisualState();
                            }
                        }
                        else
                        {
                            if (ElementVerticalLowerThumb != null && !ElementVerticalThumb.IsDragging && ElementVerticalLowerThumb != null
                                && !ElementVerticalUpperThumb.IsDragging && ElementVerticalRangeThumb != null && !ElementVerticalRangeThumb.IsDragging)
                            {
                                UpdateVisualState();
                            }
                        }
                    }
                }

                /// <summary>
                /// Responds to the MouseLeftButtonDown event.
                /// </summary>
                /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
                protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
                {
                    base.OnMouseLeftButtonDown(e);

                    if (e.Handled)
                    {
                        return;
                    }
                    e.Handled = true;
                    Focus();
                    CaptureMouse();
                }

                /// <summary>
                /// Responds to the MouseLeftButtonUp event.
                /// </summary>
                /// <param name="e">The event data for the MouseLeftButtonUp event.</param>
                protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
                {
                    base.OnMouseLeftButtonUp(e);

                    if (e.Handled)
                    {
                        return;
                    }
                    e.Handled = true;
                    ReleaseMouseCapture();
                    UpdateVisualState();
                }

            #endregion

            #region Value Events

                /// <summary>
                /// Called when the Value property changes.
                /// </summary>
                /// <param name="oldValue">The old value of the Value property.</param>
                /// <param name="newValue">The new value of the Value property.</param>
                protected override void OnValueChanged(double oldValue, double newValue)
                {
                    base.OnValueChanged(oldValue, newValue);
                    UpdateTrackLayout();
                    ChangeValueTip();
                }

                /// <summary>
                /// Called when the Minimum property changes.
                /// </summary>
                /// <param name="oldMinimum">The old value of the Minimum property.</param>
                /// <param name="newMinimum">The new value of the Minimum property.</param>
                protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
                {
                    base.OnMinimumChanged(oldMinimum, newMinimum);
                    UpdateTrackLayout();
                    SetupTickMarks();
                }

                /// <summary>
                /// Called when the Maximum property changes.
                /// </summary>
                /// <param name="oldMaximum">The old value of the Maximum property.</param>
                /// <param name="newMaximum">The new value of the Maximum property.</param>
                protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
                {
                    base.OnMaximumChanged(oldMaximum, newMaximum);
                    UpdateTrackLayout();
                    SetupTickMarks();
                }

                //TODO:  Can the two methods below be combined in to one using OnRangeChanged?

                /// <summary>
                /// Called when the LowerRangeValue property changes.
                /// </summary>
                /// <param name="oldValue">The old value of the LowerRangeValue property.</param>
                /// <param name="newValue">The new value of the LowerRangeValue property.</param>
                protected override void OnLowerRangeValueChanged(double oldValue, double newValue)
                {
                    base.OnLowerRangeValueChanged(oldValue, newValue);
                    UpdateTrackLayout();
                    ChangeValueTip();
                }

                /// <summary>
                /// Called when the UpperRangeValue property changes.
                /// </summary>
                /// <param name="oldValue">The old value of the UpperRangeValue property.</param>
                /// <param name="newValue">The new value of the UpperRangeValue property.</param>
                protected override void OnUpperRangeValueChanged(double oldValue, double newValue)
                {
                    base.OnUpperRangeValueChanged(oldValue, newValue);
                    UpdateTrackLayout();
                    ChangeValueTip();
                }

            #endregion

            #region Handle Events

                /// <summary>
                /// Increases the Value (or UpperRangeValue) when clicked.
                /// </summary>
                /// <param name="sender">An instance of the button that fired the event.</param>
                /// <param name="e">The event data for the Click event.</param>
                private void OnIncreaseHandleClick(object sender, RoutedEventArgs e)
                {
                    if (IsRangeEnabled)
                    {
                        UpperRangeValue += SmallChange;
                    }
                    else
                    {
                        Value += SmallChange;
                    }

                }

                /// <summary>
                /// Decreases the Value (or LowerRangeValue) when clicked.
                /// </summary>
                /// <param name="sender">An instance of the button that fired the event.</param>
                /// <param name="e">The event data for the Click event.</param>
                private void OnDecreaseHandleClick(object sender, RoutedEventArgs e)
                {
                    if (IsRangeEnabled)
                    {
                        LowerRangeValue -= SmallChange;
                    }
                    else
                    {
                        Value -= SmallChange;
                    }
                }

            #endregion

            /// <summary> 
            /// Called when the IsEnabled property changes.
            /// </summary> 
            /// <param name="e">Property changed args</param>
            private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                if (!IsEnabled)
                {
                    IsMouseOver = false;
                }

                UpdateVisualState();
            }

            

        #endregion

        #region Change State

            /// <summary>
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
                else if (IsMouseOver)
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
                }
                else
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
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

        #endregion Change State

        #region Updating Appearance

            /// <summary>
            /// This method will take the current min, max, and value to
            /// calculate and layout the current control measurements. 
            /// </summary>
            internal virtual void UpdateTrackLayout()
            {
                double maximum = Maximum;
                double minimum = Minimum;
                double value = Value;
                double multiplier = 1 - (maximum - value) / (maximum - minimum);

                Grid templateGrid = null;

                if (Orientation == Orientation.Horizontal)
                {
                    templateGrid = ElementHorizontalTemplate as Grid;

                    if (templateGrid != null && templateGrid.ColumnDefinitions != null && templateGrid.ColumnDefinitions.Count == 3)
                    {
                        if (ElementHorizontalDecreaseHandle != null)
                        {
                            ElementHorizontalDecreaseHandle.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 2 : 0);
                        }
                        if (ElementHorizontalIncreaseHandle != null)
                        {
                            ElementHorizontalIncreaseHandle.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 0 : 2);
                        }
                    }

                    if (IsRangeEnabled)
                    {
                        templateGrid = ElementHorizontalRangeTemplate as Grid;

                        if (templateGrid != null && templateGrid.ColumnDefinitions != null && templateGrid.ColumnDefinitions.Count == 5)
                        {
                            templateGrid.ColumnDefinitions[0].Width = new GridLength(1, IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);
                            templateGrid.ColumnDefinitions[4].Width = new GridLength(1, IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);

                            if (ElementHorizontalRangeLargeDecrease != null)
                            {
                                ElementHorizontalRangeLargeDecrease.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 4 : 0);
                            }
                            if (ElementHorizontalRangeLargeIncrease != null)
                            {
                                ElementHorizontalRangeLargeIncrease.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 0 : 4);
                            }
                            if (ElementHorizontalLowerThumb != null)
                            {
                                ElementHorizontalLowerThumb.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 3 : 1);
                            }
                            if (ElementHorizontalUpperThumb != null)
                            {
                                ElementHorizontalUpperThumb.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 1 : 3);
                            }
                        }
                    }
                    else
                    {
                        templateGrid = ElementHorizontalSingleThumbTemplate as Grid;

                        if (templateGrid != null && templateGrid.ColumnDefinitions != null && templateGrid.ColumnDefinitions.Count == 3)
                        {
                            templateGrid.ColumnDefinitions[0].Width = new GridLength(1, IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);
                            templateGrid.ColumnDefinitions[2].Width = new GridLength(1, IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);

                            if (ElementHorizontalSingleThumbLargeDecrease != null)
                            {
                                ElementHorizontalSingleThumbLargeDecrease.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 2 : 0);
                            }
                            if (ElementHorizontalSingleThumbLargeIncrease != null)
                            {
                                ElementHorizontalSingleThumbLargeIncrease.SetValue(Grid.ColumnProperty, IsDirectionReversed ? 0 : 2);
                            }
                        }
                    }


                    if (IsRangeEnabled && ElementHorizontalRangeTemplate != null)
                    {
                        if (ElementHorizontalSingleThumbLargeDecrease != null && ElementHorizontalLowerThumb != null && ElementHorizontalUpperThumb != null)
                        {
                            ElementHorizontalRangeLargeDecrease.Width = (LowerRangeValue - minimum) * (ElementHorizontalRangeTemplate.ActualWidth - ElementHorizontalUpperThumb.ActualWidth - ElementHorizontalLowerThumb.ActualWidth) / (maximum - minimum);
                            ElementHorizontalRangeThumb.Width = (UpperRangeValue - LowerRangeValue) * (ElementHorizontalRangeTemplate.ActualWidth - ElementHorizontalUpperThumb.ActualWidth - ElementHorizontalLowerThumb.ActualWidth) / (maximum - minimum);
                            ElementHorizontalRangeLargeIncrease.Width = (maximum - UpperRangeValue) * (ElementHorizontalRangeTemplate.ActualWidth - ElementHorizontalUpperThumb.ActualWidth - ElementHorizontalLowerThumb.ActualWidth) / (maximum - minimum);
                        }
                    }
                    else
                    {
                        if (ElementHorizontalSingleThumbLargeDecrease != null && ElementHorizontalThumb != null)
                        {
                            ElementHorizontalSingleThumbLargeDecrease.Width = Math.Max(0, multiplier * (ElementHorizontalSingleThumbTemplate.ActualWidth - ElementHorizontalThumb.ActualWidth));
                        }
                    }

                }
                else
                {
                    templateGrid = ElementVerticalTemplate as Grid;

                    if (templateGrid != null && templateGrid.RowDefinitions != null && templateGrid.RowDefinitions.Count == 3)
                    {
                        if (ElementVerticalDecreaseHandle != null)
                        {
                            ElementVerticalDecreaseHandle.SetValue(Grid.RowProperty, IsDirectionReversed ? 0 : 2);
                        }
                        if (ElementVerticalIncreaseHandle != null)
                        {
                            ElementVerticalIncreaseHandle.SetValue(Grid.RowProperty, IsDirectionReversed ? 2 : 0);
                        }
                    }

                    if (IsRangeEnabled)
                    {
                        templateGrid = ElementVerticalRangeTemplate as Grid;

                        if (templateGrid != null && templateGrid.RowDefinitions != null && templateGrid.RowDefinitions.Count == 5)
                        {
                            templateGrid.RowDefinitions[0].Height = new GridLength(1, IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);
                            templateGrid.RowDefinitions[4].Height = new GridLength(1, IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);

                            if (ElementVerticalRangeLargeDecrease != null)
                            {
                                ElementVerticalRangeLargeDecrease.SetValue(Grid.RowProperty, IsDirectionReversed ? 0 : 4);
                            }
                            if (ElementVerticalRangeLargeIncrease != null)
                            {
                                ElementVerticalRangeLargeIncrease.SetValue(Grid.RowProperty, IsDirectionReversed ? 4 : 0);
                            }
                            if (ElementVerticalLowerThumb != null)
                            {
                                ElementVerticalLowerThumb.SetValue(Grid.RowProperty, IsDirectionReversed ? 1 : 3);
                            }
                            if (ElementVerticalUpperThumb != null)
                            {
                                ElementVerticalUpperThumb.SetValue(Grid.RowProperty, IsDirectionReversed ? 3 : 1);
                            }
                        }
                    }
                    else
                    {
                        templateGrid = ElementVerticalSingleThumbTemplate as Grid;

                        if (templateGrid != null && templateGrid.ColumnDefinitions != null && templateGrid.ColumnDefinitions.Count == 3)
                        {
                            templateGrid.RowDefinitions[0].Height = new GridLength(1, IsDirectionReversed ? GridUnitType.Auto : GridUnitType.Star);
                            templateGrid.RowDefinitions[2].Height = new GridLength(1, IsDirectionReversed ? GridUnitType.Star : GridUnitType.Auto);

                            if (ElementVerticalSingleThumbLargeDecrease != null)
                            {
                                ElementVerticalSingleThumbLargeDecrease.SetValue(Grid.RowProperty, IsDirectionReversed ? 0 : 2);
                            }
                            if (ElementVerticalSingleThumbLargeIncrease != null)
                            {
                                ElementVerticalSingleThumbLargeIncrease.SetValue(Grid.RowProperty, IsDirectionReversed ? 2 : 0);
                            }
                        }
                    }


                    if (IsRangeEnabled && ElementVerticalRangeTemplate != null)
                    {
                        if (ElementVerticalSingleThumbLargeDecrease != null && ElementVerticalLowerThumb != null && ElementVerticalUpperThumb != null)
                        {
                            ElementVerticalRangeLargeDecrease.Height = (LowerRangeValue - minimum) * (ElementVerticalRangeTemplate.ActualHeight - ElementVerticalUpperThumb.ActualHeight - ElementVerticalLowerThumb.ActualHeight) / (maximum - minimum);
                            ElementVerticalRangeThumb.Height = (UpperRangeValue - LowerRangeValue) * (ElementVerticalRangeTemplate.ActualHeight - ElementVerticalUpperThumb.ActualHeight - ElementVerticalLowerThumb.ActualHeight) / (maximum - minimum);
                            ElementVerticalRangeLargeIncrease.Height = (maximum - UpperRangeValue) * (ElementVerticalRangeTemplate.ActualHeight - ElementVerticalUpperThumb.ActualHeight - ElementVerticalLowerThumb.ActualHeight) / (maximum - minimum);
                        }
                    }
                    else
                    {
                        if (ElementVerticalSingleThumbLargeDecrease != null && ElementVerticalThumb != null)
                        {
                            ElementVerticalSingleThumbLargeDecrease.Height = Math.Max(0, multiplier * (ElementVerticalSingleThumbTemplate.ActualHeight - ElementVerticalThumb.ActualHeight));
                        }
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            internal virtual void SetupTickMarks()
            {
                if (ElementHorizontalTickPanel != null && TickLocation != TickLocation.None)
                {
                    // clear the tick panel of marks
                    ElementHorizontalTickPanel.Children.Clear();

                    int numberOfTicks = (int)((Maximum - Minimum) / TickFrequency);

                    // Loop over the amount of ticks
                    for (int i = 0; i <= numberOfTicks; i++)
                    {
                        double x1 = 0;

                        if (TickLocation == TickLocation.Top || TickLocation == TickLocation.Both)
                        {
                            // Add top tick marks
                            x1 = ((i) * ((ElementHorizontalTickPanel.ActualWidth) / numberOfTicks));
                            ElementHorizontalTickPanel.Children.Add(CreateTickMark(new Point(x1, 0), new Point(x1, 5), TickLocation.Top));
                        }

                        if (TickLocation == TickLocation.Bottom || TickLocation == TickLocation.Both)
                        {
                            x1 = ((i) * ((ElementHorizontalTickPanel.ActualWidth) / numberOfTicks));

                            // Saftey measure to handle low values
                            if (double.IsNaN(x1)) x1 = Maximum;

                            ElementHorizontalTickPanel.Children.Add(CreateTickMark(new Point(x1, ElementHorizontalTickPanel.ActualHeight), new Point(x1, ElementHorizontalTickPanel.ActualHeight - 5), TickLocation.Bottom));
                        }
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <returns></returns>
            FrameworkElement CreateTickMark(Point start, Point end, TickLocation location)
            {
                // If no template was specified, use the default
                if (TickTemplate == null)
                {
                    System.Windows.Shapes.Line ln = new System.Windows.Shapes.Line();
                    ln.Stroke = new SolidColorBrush(Colors.Black);
                    ln.StrokeThickness = 1.0;
                    ln.X1 = start.X;
                    ln.Y1 = start.Y;
                    ln.X2 = end.X;
                    ln.Y2 = end.Y;

                    return ln;
                }
                else
                {
                    ContentPresenter cp = new ContentPresenter();
                    //cp.Content = "a";
                    cp.ContentTemplate = TickTemplate;

                    if (location == TickLocation.Top)
                    {
                        cp.SetValue(Canvas.TopProperty, start.Y - cp.ActualHeight);
                        cp.SetValue(Canvas.LeftProperty, start.X - (cp.ActualWidth / 2));
                    }
                    else
                    {
                        cp.SetValue(Canvas.TopProperty, end.Y);
                        cp.SetValue(Canvas.LeftProperty, start.X - (cp.ActualWidth / 2));
                    }

                    return cp;
                }
            }

            private void CoerceTickFrequency()
            {
                double min = Minimum;
                double max = Maximum;
                double frequency = TickFrequency;

                if (frequency < min)
                {
                    SetValue(TickFrequencyProperty, min);
                    return;
                }

                if (frequency > max)
                {
                    SetValue(TickFrequencyProperty, max);
                    return;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="newValue"></param>
            /// <param name="currentValue"></param>
            /// <returns></returns>
            private double SnapToNearestTick(double newValue, double currentValue)
            {
                if (newValue >= currentValue + (TickFrequency / 2))
                    return currentValue + TickFrequency;
                else if (newValue <= currentValue - (TickFrequency / 2))
                   return currentValue - TickFrequency;
                else
                    return currentValue;
            }
        #endregion

        #region Display Value ToolTips

            /// <summary>
            /// Opens the ValueTipPopup popup.
            /// </summary>
            private void OpenTips()
            {
                // Display the tooltip popup
                if (!ElementValueTipPopup.IsOpen && ShowValueTips)
                    ElementValueTipPopup.IsOpen = true;
            }

            /// <summary>
            /// Closes the ValueTipPopup popup.
            /// </summary>
            private void CloseTips()
            {

                // Hide the tooltip popup
                if (ElementValueTipPopup.IsOpen)
                    ElementValueTipPopup.IsOpen = false;

            }

            /// <summary>
            /// Position the tool tip (Horizontally)
            /// </summary>
            /// <param name="element">The Thumb that the tooltip will be possitioned above.</param>
            private void PositionToolTipHorizontally(Thumb element)
            {
                double popupWidth = ElementValueTipPopupRoot.ActualWidth;
                double thumbWidth = element.ActualWidth;

                System.Windows.Media.GeneralTransform gt = element.TransformToVisual(this as UIElement);

                // Check if the popup will display too far left and adjust
                double farLeft = gt.Transform(new Point((-popupWidth / 2) + (thumbWidth / 2), 0)).X;
                double offset = Math.Abs(Math.Min(0 + farLeft, 0));
                //TODO:  MAKE SURE '0' WORKS IF SLIDER IS NOT AGAINST THE SIDE

                if (offset == 0)
                {
                    // Check if the popup will display too far right and adjust
                    double farRight = gt.Transform(new Point(((-popupWidth / 2) + (thumbWidth / 2)) + popupWidth, 0)).X;
                    offset = Math.Min(ActualWidth - farRight, 0);
                }

                Point tipOffset = new Point();

                if (IsRangeEnabled)
                {
                    tipOffset = gt.Transform(new Point(((-popupWidth / 2) + (thumbWidth / 2)) + offset, -(ElementValueTipPopupRoot.ActualHeight + (ElementHorizontalLowerThumb.ActualHeight / 2)))); ;
                }
                else
                {
                    tipOffset = gt.Transform(new Point(((-popupWidth / 2) + (thumbWidth / 2)) + offset, -ElementValueTipPopupRoot.ActualHeight)); ;
                }

                ElementValueTipPopup.HorizontalOffset = tipOffset.X;
                ElementValueTipPopup.VerticalOffset = tipOffset.Y;

            }

            /// <summary>
            /// Position the tool tip (Vertically)
            /// </summary>
            /// <param name="element">The Thumb that the tooltip will be possitioned next to.</param>
            private void PositionToolTipVertically(Thumb element)
            {
                // Turn the tool tip to the side for a better appearance
                ElementValueTipPopupRoot.RenderTransform = new RotateTransform { Angle = -90 };

                double popupWidth = ElementValueTipPopupRoot.ActualWidth;
                double thumbHeight = element.ActualHeight;

                System.Windows.Media.GeneralTransform gt = element.TransformToVisual(this as UIElement);

                //// Check if the popup will display too far up or down
                double top = gt.Transform(new Point(0, (-(popupWidth / 2) + (thumbHeight / 2)))).Y;
                double offset = Math.Abs(Math.Min(top, 0));

                if (offset == 0)
                {
                    // Check if the popup will display too far right and adjust
                    double bottom = gt.Transform(new Point(0, (-(popupWidth / 2) + (thumbHeight / 2)) + popupWidth)).Y;
                    offset = Math.Min(ActualHeight - bottom, 0);
                }

                Point tipOffset = new Point();

                if (IsRangeEnabled)
                {
                    tipOffset = gt.Transform(new Point(-(ElementValueTipPopupRoot.ActualHeight + (ElementHorizontalLowerThumb.Height / 2)), ((popupWidth / 2) + (thumbHeight / 2)) + offset));
                }
                else
                {
                    tipOffset = gt.Transform(new Point(-ElementValueTipPopupRoot.ActualHeight, ((popupWidth / 2) + (thumbHeight / 2)) + offset));
                }

                ElementValueTipPopup.HorizontalOffset = tipOffset.X;
                ElementValueTipPopup.VerticalOffset = tipOffset.Y;

            }

            /// <summary>
            /// Changes the text to be displayed on the popup tooltip.
            /// </summary>
            private void ChangeValueTip()
            {

                if (ElementValueTipPopup != null)
                {
                    if (ValueTipsText == null || ValueTipsText == string.Empty)
                    {
                        if (!IsRangeEnabled)
                        {
                            ElementValueTipTextBlock.Text = string.Format("{0}", Value < 1 ? "0" : Value.ToString("###"));
                        }
                        else
                        {
                            ElementValueTipTextBlock.Text = string.Format(IsDirectionReversed ? "{1} - {0}" : "{0} - {1}", LowerRangeValue < 1 ? "0" : LowerRangeValue.ToString("###"), UpperRangeValue < 1 ? "0" : UpperRangeValue.ToString("###"));
                        }
                    }
                    else
                    {
                        ElementValueTipTextBlock.Text = ValueTipsText;
                    }
                }

            }

        #endregion

    }

}