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
using System.Windows.Controls;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// This class represents an attached property that lets you specify
    /// the current state of a user control.  An attached property is
    /// used because it allows for adherence to the MVVM pattern.
    /// </summary>
    public static class VisualStates
    {
        /// <summary>
        /// Identifies the Berico.LinkAnalysis.SnagLAssets.VisualStates.CurrentStateProperty
        /// property.  This indicates the current state of the control.
        /// </summary>
        public static readonly DependencyProperty CurrentStateProperty = DependencyProperty.RegisterAttached("CurrentState", typeof(string), typeof(VisualStates), new PropertyMetadata(TransitionToState));

        /// <summary>
        /// Returns the value of the CurrentState property
        /// </summary>
        /// <param name="depObj">The control that the property is attached too</param>
        /// <returns>a string identifying the value of the CurrentState property</returns>
        public static string GetCurrentState(DependencyObject depObj)
        {
            return (string)depObj.GetValue(CurrentStateProperty);
        }

        /// <summary>
        /// Sets the CurrentState property with the specified value
        /// </summary>
        /// <param name="depObj">The control that the property is attached too</param>
        /// <param name="value">The value of the CurrentState property</param>
        public static void SetCurrentState(DependencyObject depObj, string value)
        {
            depObj.SetValue(CurrentStateProperty, value);
        }

        /// <summary>
        /// Changes the visual state on the attached control anytime
        /// the CurrentState property changes.
        /// </summary>
        /// <param name="sender">The control whose CurrentState property changed</param>
        /// <param name="args">The arguments for the event</param>
        private static void TransitionToState(object sender, DependencyPropertyChangedEventArgs args)
        {
            Control c = sender as Control;

            if (c != null)
                VisualStateManager.GoToState(c, (string)args.NewValue, true);
            else
                throw new ArgumentException("CurrentState is only supported on a Control class");
        }
    }
}