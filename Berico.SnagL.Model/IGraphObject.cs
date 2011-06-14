//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Defines the contract for an object found on the graph
    /// </summary>
    public interface IGraphObject
    {
        /// <summary>
        /// Specifies the mechanism for which an object was added to the graph
        /// </summary>
        CreationType SourceMechanism
        {
            get;
            set;
        }
    }
}