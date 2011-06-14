//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Searching
{
    /// <summary>
    /// Specifies the search operation that should
    /// be used during a search
    /// </summary>
    public enum SearchOperator
    {
        /// <summary>
        /// Indicates an 'Equal' search
        /// </summary>
        Equals,
        /// <summary>
        /// Indicates a 'DoesNotEqual' search
        /// </summary>
        DoesNotEqual,
        /// <summary>
        /// Indicates a 'Contains' search
        /// </summary>
        Contains,
        /// <summary>
        /// Indicates a 'DoesNotContain' search
        /// </summary>
        DoesNotContain,
        /// <summary>
        /// Indicates a 'StartsWith' search
        /// </summary>
        StartsWith,
        /// <summary>
        /// Indicates a 'DoesNotStartWith' search
        /// </summary>
        DoesNotStartWith,
        /// <summary>
        /// Indicates an 'EndsWith' search
        /// </summary>
        EndsWith,
        /// <summary>
        /// Indicates a 'DoesNotEndWith' search
        /// </summary>
        DoesNotEndWith
    }
}