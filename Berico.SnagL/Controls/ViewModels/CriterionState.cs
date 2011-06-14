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
    /// Specifies the state of a Criterion
    /// </summary>
    public enum CriterionState
    {
        /// <summary>
        /// Indicates that the criterion is in a 
        /// normal state
        /// </summary>
        Normal,
        /// <summary>
        /// Indicates that the criterion has been
        /// activated
        /// </summary>
        Active,
        /// <summary>
        /// Indicates that the criterion has been 
        /// inactivated
        /// </summary>
        Inactive,
        /// <summary>
        /// Indicates that the criterion has received
        /// focus
        /// </summary>
        Focused,
        /// <summary>
        /// Indicates that the criterion has lost
        /// focus
        /// </summary>
        Unfocused
    }
}