﻿//-------------------------------------------------------------
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

namespace Berico.Windows.Controls
{ 
    /// <summary> 
    /// Names and helpers for visual states in the controls.  This class
    /// was borrowed from Microsoft's source.  It serves its purpose well
    /// and I saw no reason to not simply reuse it in this case.
    /// </summary> 
    internal static class VisualStates
    {
        #region GroupCommon 
            /// <summary>
            /// Normal state
            /// </summary> 
            public const string StateNormal = "Normal"; 

            /// <summary> 
            /// MouseOver state
            /// </summary>
            public const string StateMouseOver = "MouseOver"; 

            /// <summary>
            /// Pressed state 
            /// </summary> 
            public const string StatePressed = "Pressed";
     
            /// <summary>
            /// Disabled state
            /// </summary> 
            public const string StateDisabled = "Disabled";

            /// <summary> 
            /// Readonly state 
            /// </summary>
            public const string StateReadOnly = "ReadOnly";

            /// <summary>
            /// Checked state
            /// </summary>
            public const string StateChecked = "Checked";

            /// <summary>
            /// Unchecked state
            /// </summary>
            public const string StateUnchecked = "Unchecked";

            /// <summary>
            /// Common state group 
            /// </summary>
            public const string GroupCommon = "CommonStates";
        #endregion GroupCommon 
 
        #region GroupFocus
            /// <summary> 
            /// Unfocused state
            /// </summary>
            public const string StateUnfocused = "Unfocused"; 

            /// <summary>
            /// Focused state 
            /// </summary> 
            public const string StateFocused = "Focused";
     
            /// <summary>
            /// Focused and Dropdown is showing state
            /// </summary> 
            public const string StateFocusedDropDown = "FocusedDropDown";

            /// <summary> 
            /// Focus state group 
            /// </summary>
            public const string GroupFocus = "FocusStates"; 
        #endregion GroupFocus

        #region GroupSelection 
        /// <summary>
        /// Selected state
        /// </summary> 
        public const string StateSelected = "Selected"; 

        /// <summary> 
        /// Selected and unfocused state
        /// </summary>
        public const string StateSelectedUnfocused = "SelectedUnfocused"; 

        /// <summary>
        /// Unselected state 
        /// </summary> 
        public const string StateUnselected = "Unselected";
 
        /// <summary>
        /// Selection state group
        /// </summary> 
        public const string GroupSelection = "SelectionStates";
        #endregion GroupSelection
 
        #region GroupActive 
        /// <summary>
        /// Active state 
        /// </summary>
        public const string StateActive = "Active";
 
        /// <summary>
        /// Inactive state
        /// </summary> 
        public const string StateInactive = "Inactive"; 

        /// <summary> 
        /// Active state group
        /// </summary>
        public const string GroupActive = "ActiveStates"; 
        #endregion GroupActive

        /// <summary> 
        /// Use VisualStateManager to change the visual state of the control. 
        /// </summary>
        /// <param name="control"> 
        /// Control whose visual state is being changed.
        /// </param>
        /// <param name="useTransitions"> 
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param> 
        /// <param name="stateNames"> 
        /// Ordered list of state names and fallback states to transition into.
        /// Only the first state to be found will be used. 
        /// </param>
        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        { 
            if (control == null)
            {
                throw new ArgumentNullException("control"); 
            } 

            if (stateNames == null) 
            {
                return;
            } 

            foreach (string name in stateNames)
            { 
                if (VisualStateManager.GoToState(control, name, useTransitions)) 
                {
                    break; 
                }
            }
        } 
    }
}