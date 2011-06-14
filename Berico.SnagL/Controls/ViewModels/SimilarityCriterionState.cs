//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------


namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Specifies the state of a AttributeSimilarityCriterion
    /// </summary>
    public enum SimilarityCriterionState
    {
        /// <summary>
        /// Indicates that the criterion is in Simple mode
        /// and has focus
        /// </summary>
        SimpleFocused,
        /// <summary>
        /// Indicates that the criterion is in
        /// Simple mode and has lost focus
        /// </summary>
        SimpleUnfocused,
        /// Indicates that the criterion is in Advanced mode
        /// and has focus
        /// </summary>
        AdvancedFocused,
        /// <summary>
        /// Indicates that the criterion is in
        /// Advanced mode and has lost focus
        /// </summary>
        AdvancedUnfocused
    }
}