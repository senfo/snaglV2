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
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Berico.Windows.Controls
{

    #region RangeChangedEventArgs

        /// <summary>
        /// Provides data for the RangeChanged event.
        /// </summary>
        public class RangeChangedEventArgs : EventArgs
        {
            private double lowerValue;
            private double upperValue;

            /// <summary>
            /// Initializes a new instance of the Berico.LinkAnalysis.Web.Controls.RangeChnagedEventArgs class.
            /// </summary>
            /// <param name="lowVal">A double representing the lower value of range that changed.</param>
            /// <param name="highVal">A double representing the upper value of the range that changed.</param>
            public RangeChangedEventArgs(double lowVal, double highVal)
            {
                lowerValue = lowVal;
                upperValue = highVal;
            }

            /// <summary>
            /// Gets the lower value of the range that changed.
            /// </summary>
            public double LowerValue
            {
                get { return lowerValue; }
            }

            /// <summary>
            /// Gets the upper value of the range that changed.
            /// </summary>
            public double UpperValue
            {
                get { return upperValue; }
            }

        }

    #endregion

    /// <summary>
    /// Base class that represents a range with an upper and lower bound.
    /// </summary>
    public abstract class DualRangeBase : RangeBase
    {
        internal double requestedUpperRangeValue, requestedLowerRangeValue;
        internal double preCoersionUpperRangeValue, preCoersionLowerRangeValue;
        internal double initialUpperRangeValue, initialLowerRangeValue;
        internal int levelsFromLowerRootCall;
        internal int levelsFromUpperRootCall; 

        //TODO:  See if we can use just one "levelsFromXXXRootCall" variable

        public event RoutedPropertyChangedEventHandler<double> LowerRangeValueChanged;
        public event RoutedPropertyChangedEventHandler<double> UpperRangeValueChanged;
        public event RoutedPropertyChangedEventHandler<RangeChangedEventArgs> RangeChanged;

        #region Properties

            #region LowerRangeValue

                // Defines the LowerRangeValue dependency property.
                public static readonly DependencyProperty LowerRangeValueProperty = DependencyProperty.Register("LowerRangeValue", typeof(double), typeof(DualRangeBase), new PropertyMetadata(0.2d, OnLowerRangeValuePropertyChanged));

                // Gets or sets the LowerRangeValue of the range.
                public double LowerRangeValue
                {
                    get { return (double)GetValue(LowerRangeValueProperty); }
                    set { SetValue(LowerRangeValueProperty, value); }
                }

                /// <summary>
                /// The PropertyChangedCallback that is called when the LowerRangeValueProperty is changed.
                /// </summary>
                /// <param name="d">An instance of the DualRangeBase object whose LowerRangeValue was changed.</param>
                /// <param name="e">An instance of the DependencyPropertyChangedEventArgs class.</param>
                private static void OnLowerRangeValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    DualRangeBase dualRangeBase = d as DualRangeBase;

                    // Validate the provided LowerRangeValue.
                    if (!IsValidDoubleValue(e.NewValue))
                    {
                        throw new ArgumentException("Invalid double value", LowerRangeValueProperty.ToString());
                    }

                    // The code that follows is borrowed from the Microsoft code in RangeBase
                    // that performs the same actions on the Value property.  The trick here
                    // is to hold calls to the property changed methods until after all
                    // coercion has completed.
                    if (dualRangeBase.levelsFromLowerRootCall == 0)
                    {
                        dualRangeBase.requestedLowerRangeValue = (double)e.NewValue;
                        dualRangeBase.preCoersionLowerRangeValue = (double)e.NewValue;
                        dualRangeBase.initialLowerRangeValue = (double)e.OldValue;
                    }
                    dualRangeBase.levelsFromLowerRootCall++;

                    // Coerce values
                    dualRangeBase.CoerceLowerValue();
                    //dualRangeBase.CoerceValues();

                    // This portion of the borrowed Microsoft code finally fires
                    // the change events once all coercion is confirmed complete.
                    dualRangeBase.levelsFromLowerRootCall--;
                    if (dualRangeBase.levelsFromLowerRootCall == 0)
                    {
                        double value = dualRangeBase.LowerRangeValue;
                        if (dualRangeBase.initialLowerRangeValue != value)
                        {
                            dualRangeBase.OnLowerRangeValueChanged(dualRangeBase.initialLowerRangeValue, value);
                            dualRangeBase.OnRangeChanged(dualRangeBase.initialLowerRangeValue, value, dualRangeBase.initialUpperRangeValue, dualRangeBase.UpperRangeValue);
                        }
                    }
                    // End coercion code borrowed from Microsoft.

                }

                /// <summary>
                /// Called when the LowerRangeValue property changes.  Raises the LowerRangeValueChanged
                /// event.
                /// </summary>
                /// <param name="oldMinimum">Old value of the LowerRangeValue property.</param>
                /// <param name="newMinimum">New value of the LowerRangeValue property.</param>
                protected virtual void OnLowerRangeValueChanged(double oldValue, double newValue)
                {
                    RoutedPropertyChangedEventHandler<double> handler = LowerRangeValueChanged;
                    if (handler != null)
                    {
                        handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
                    }
                }

            #endregion

            #region UpperRangeValue

                // Defines the UpperRangeValue dependency property.
                public static readonly DependencyProperty UpperRangeValueProperty = DependencyProperty.Register("UpperRangeValue", typeof(double), typeof(DualRangeBase), new PropertyMetadata(0.8d, OnUpperRangeValuePropertyChanged));

                // Gets or sets the UpperRangeValue of the range.
                public double UpperRangeValue
                {
                    get { return (double)GetValue(UpperRangeValueProperty); }
                    set { SetValue(UpperRangeValueProperty, value); }
                }

                /// <summary>
                /// The PropertyChangedCallback that is called when the UpperRangeValueProperty is changed.
                /// </summary>
                /// <param name="d">An instance of the DualRangeBase object whose UpperRangeValue was changed.</param>
                /// <param name="e">An instanced of the DependencyPropertyChangedEventArgs class.</param>
                private static void OnUpperRangeValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                    DualRangeBase dualRangeBase = d as DualRangeBase;

                    // Validate the provided UpperRangeValue.
                    if (!IsValidDoubleValue(e.NewValue))
                    {
                        throw new ArgumentException("Invalid double value", UpperRangeValueProperty.ToString());
                    }

                    // The code that follows is borrowed from the Microsoft code in RangeBase
                    // that performs the same actions on the Value property.  The trick here
                    // is to hold calls to the property changed methods until after all
                    // coercion has completed.
                    if (dualRangeBase.levelsFromUpperRootCall == 0)
                    {
                        dualRangeBase.requestedUpperRangeValue = (double)e.NewValue;
                        dualRangeBase.preCoersionUpperRangeValue = (double)e.NewValue;
                        dualRangeBase.initialUpperRangeValue = (double)e.OldValue;
                    }
                    dualRangeBase.levelsFromUpperRootCall++;

                    // Coerce values
                    dualRangeBase.CoerceUpperValue();
                    //dualRangeBase.CoerceValues();

                    // This portion of the borrowed Microsoft code finally fires
                    // the change events once all coercion is confirmed complete.
                    dualRangeBase.levelsFromUpperRootCall--;
                    if (dualRangeBase.levelsFromUpperRootCall == 0)
                    {
                        double value = dualRangeBase.UpperRangeValue;
                        if (dualRangeBase.initialUpperRangeValue != value)
                        {
                            dualRangeBase.OnUpperRangeValueChanged(dualRangeBase.initialUpperRangeValue, value);
                            dualRangeBase.OnRangeChanged(dualRangeBase.initialLowerRangeValue, dualRangeBase.LowerRangeValue, dualRangeBase.initialUpperRangeValue, value);
                        }
                    }
                    // End coercion code borrowed from Microsoft.

                }

                /// <summary>
                /// Called when the UpperRangeValue property changes.  Raises the
                /// UpperRangeValueChanged event.
                /// </summary>
                /// <param name="oldMinimum">Old value of the UpperRangeValue property.</param>
                /// <param name="newMinimum">New value of the UpperRangeValue property.</param>
                protected virtual void OnUpperRangeValueChanged(double oldValue, double newValue)
                {
                    RoutedPropertyChangedEventHandler<double> handler = UpperRangeValueChanged;
                    if (handler != null)
                    {
                        handler(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
                    }
                }

            #endregion

            /// <summary>
            /// Gets the size of the current range based on the difference
            /// between the lower and upper range values.
            /// </summary>
            public double RangeValue
            {
                get { return UpperRangeValue - LowerRangeValue; }
            }

        #endregion

        #region Constructors

            /// <summary>
            /// Initializes a new instance of the Berico.LinkAnalysis.Web.Controls.DualRangeBase.
            /// </summary>
            protected DualRangeBase()
                : base()
            {
                // Minimum (0), Maximum (1), SmallChange (0.1) and LargeChange (1) defaults
                // are set in RangeBase.  The LowerRangeValue (0.2) and UpperRangeValue (0.8) 
                // defaults are set in their respective property code.
            }

        #endregion

        #region Internal Methods

            private void CoerceValues()
            {

                double min = Minimum;
                double max = Maximum;
                double lowerVal = LowerRangeValue;
                double upperVal = UpperRangeValue;

                if (upperVal < min)
                {
                    SetValue(UpperRangeValueProperty, min);
                    return;
                }

                if (upperVal > max)
                {
                    SetValue(UpperRangeValueProperty, max);
                    return;
                }

                if (lowerVal < min)
                {
                    SetValue(LowerRangeValueProperty, min);
                    return;
                }

                if (lowerVal > max)
                {
                    SetValue(LowerRangeValueProperty, max);
                    return;
                }

                if (requestedUpperRangeValue < lowerVal)
                    requestedUpperRangeValue = lowerVal;

                if (requestedUpperRangeValue > max)
                    requestedUpperRangeValue = max;

                if (requestedUpperRangeValue < min)
                    requestedUpperRangeValue = min;

                if (requestedUpperRangeValue != upperVal)
                    SetValue(UpperRangeValueProperty, requestedUpperRangeValue);

                if (requestedLowerRangeValue > upperVal)
                    requestedLowerRangeValue = upperVal;

                if (requestedLowerRangeValue > max)
                    requestedLowerRangeValue = max;

                if (requestedLowerRangeValue < min)
                    requestedLowerRangeValue = min;

                if (requestedLowerRangeValue != lowerVal)
                    SetValue(LowerRangeValueProperty, requestedLowerRangeValue);

            }

            /// <summary>
            /// Make sure that the lower and upper range values fall between the
            /// Minimum and Maximum values.
            /// </summary>
            private void CoerceUpperValue()
            {

                double min = Minimum;
                double max = Maximum;
                double lowerVal = LowerRangeValue;
                double upperVal = UpperRangeValue;

                if (requestedUpperRangeValue != upperVal && requestedUpperRangeValue >= min && requestedUpperRangeValue <= max && requestedUpperRangeValue >= lowerVal)
                {
                    SetValue(UpperRangeValueProperty, requestedUpperRangeValue);
                }
                else
                {
                    if (upperVal < min)
                    {
                        SetValue(UpperRangeValueProperty, min);
                        return;
                    }
                    if (upperVal > max)
                    {
                        SetValue(UpperRangeValueProperty, max);
                        return;
                    }
                    if (upperVal < lowerVal)
                    {
                        SetValue(UpperRangeValueProperty, lowerVal);
                    }
                    if (preCoersionLowerRangeValue != lowerVal && preCoersionLowerRangeValue >= min && preCoersionLowerRangeValue <= max && preCoersionLowerRangeValue <= upperVal)
                    {
                        SetValue(LowerRangeValueProperty, preCoersionLowerRangeValue);
                    }
                    return;
                }

            }

            private void CoerceLowerValue()
            {

                double min = Minimum;
                double max = Maximum;
                double lowerVal = LowerRangeValue;
                double upperVal = UpperRangeValue;

                if (requestedLowerRangeValue != lowerVal && requestedLowerRangeValue >= min && requestedLowerRangeValue <= max && requestedLowerRangeValue <= upperVal)
                {
                    SetValue(LowerRangeValueProperty, requestedLowerRangeValue);
                }
                else
                {
                    if (lowerVal < min)
                    {
                        SetValue(LowerRangeValueProperty, min);
                        return;
                    }
                    if (lowerVal > max)
                    {
                        SetValue(LowerRangeValueProperty, max);
                        return;
                    }
                    if (lowerVal > upperVal)
                    {
                        SetValue(LowerRangeValueProperty, upperVal);
                    }
                    if (preCoersionUpperRangeValue != upperVal && preCoersionUpperRangeValue >= min && preCoersionUpperRangeValue <= max && preCoersionUpperRangeValue >= lowerVal)
                    {
                        SetValue(UpperRangeValueProperty, preCoersionUpperRangeValue);
                    }
                    return;
                }
            }

            /// <summary>
            /// Checks if the provided value is a valid double.
            /// </summary>
            /// <param name="value">The value to be checked.</param>
            /// <returns>true if the vlaue is a double; otherwise false.</returns>
            private static bool IsValidDoubleValue(object value)
            {
                double number = (double)value;
                return !double.IsNaN(number) && !double.IsInfinity(number);
            }

            /// <summary>
            /// Raises the RangeChanged event
            /// </summary>
            /// <param name="oldLowerVal">The previous lower value.</param>
            /// <param name="newLowerVal">The new lower value.</param>
            /// <param name="oldUpperVal">The previous upper value.</param>
            /// <param name="newUpperVal">The new upper value.</param>
            protected virtual void OnRangeChanged(double oldLowerVal, double newLowerVal, double oldUpperVal, double newUpperVal)
            {
                RoutedPropertyChangedEventHandler<RangeChangedEventArgs> handler = RangeChanged;
                if (handler != null)
                {
                    RangeChangedEventArgs oldValues = new RangeChangedEventArgs(oldLowerVal, oldUpperVal);
                    RangeChangedEventArgs newValues = new RangeChangedEventArgs(newLowerVal, newUpperVal);

                    handler(this, new RoutedPropertyChangedEventArgs<RangeChangedEventArgs>(oldValues, newValues));
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="oldMinimum"></param>
            /// <param name="newMinimum"></param>
            protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
            {
                base.OnMinimumChanged(oldMinimum, newMinimum);
                
                requestedLowerRangeValue = LowerRangeValue;
                requestedUpperRangeValue = UpperRangeValue;

                if (newMinimum > requestedUpperRangeValue)
                    SetValue(UpperRangeValueProperty, newMinimum);

                if (newMinimum > requestedLowerRangeValue)
                    SetValue(LowerRangeValueProperty, newMinimum);

                if (requestedUpperRangeValue != preCoersionUpperRangeValue)
                    SetValue(UpperRangeValueProperty, preCoersionUpperRangeValue);
                else
                    this.CoerceUpperValue();

                if (requestedLowerRangeValue != preCoersionLowerRangeValue)
                    SetValue(LowerRangeValueProperty, preCoersionLowerRangeValue);
                else
                    this.CoerceLowerValue();

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="oldMaximum"></param>
            /// <param name="newMaximum"></param>
            protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
            {
                base.OnMaximumChanged(oldMaximum, newMaximum);

                requestedLowerRangeValue = LowerRangeValue;
                requestedUpperRangeValue = UpperRangeValue;

                if (newMaximum < requestedLowerRangeValue)
                    SetValue(LowerRangeValueProperty, newMaximum);

                if (newMaximum < requestedUpperRangeValue)
                    SetValue(UpperRangeValueProperty, newMaximum);

                if (requestedLowerRangeValue != preCoersionLowerRangeValue)
                    SetValue(LowerRangeValueProperty, preCoersionLowerRangeValue);
                else
                    this.CoerceLowerValue();

                if (requestedUpperRangeValue != preCoersionUpperRangeValue)
                    SetValue(UpperRangeValueProperty, preCoersionUpperRangeValue);
                else
                    this.CoerceUpperValue();

            }

        #endregion

        #region External Methods

            /// <summary>
            /// Provides an appropriately formatted string representing the main properties
            /// of the DualRangeBase class.
            /// </summary>
            /// <returns>Returns a string representation of a DualRangeBase object.</returns>
            public override string ToString()
            {
                return string.Format("{0} Minimum:{1} Maximum:{2} Lower Value:{3} Upper Value:{4}", base.ToString(), Minimum, Maximum, LowerRangeValue, UpperRangeValue);
            }

        #endregion

    }
}