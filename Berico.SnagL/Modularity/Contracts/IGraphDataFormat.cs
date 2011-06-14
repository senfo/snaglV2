//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    using Berico.SnagL.Infrastructure.Graph;
    using Berico.SnagL.Model;

    /// <summary>
    /// Provides the template for all graph data format classes
    /// </summary>
    public interface IGraphDataFormat
    {
        /// <summary>
        /// Exports the provided graph data
        /// </summary>
        /// <param name="scope">The scope of the data being exported</param>
        /// <returns>a string containing the exported data to be saved to
        /// a file if the export was successfull; otherwise null</returns>
        string Export(string scope);

        /// <summary>
        /// Imports the provided data
        /// </summary>
        /// <param name="data">The data (in string format) to be imported</param>
        /// <param name="components">The GraphComponents instance</param>
        /// <param name="objectType">Specifies the mechanism for which objects on the graph were imported</param>
        /// <returns>true if the import was successfull (and contained
        /// data); otherwise false</returns>
        bool Import(string data, GraphComponents components, CreationType objectType);

        /// <summary>
        /// Validates that the provided data represents this format
        /// </summary>
        /// <param name="data">The string data to be formatted</param>
        /// <returns>true if the provided data is this format; otherwise
        /// false</returns>
        bool Validate(string data);

        /// <summary>
        /// Gets or sets the priority of this format.  This helps ensure
        /// that that the appropriate format is detected by sorting by
        /// priority when analyzing.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Gets the extension(s) used by files that this format
        /// represents.  This is used when creating the open and
        /// save dialogs.
        /// </summary>
        string Extension { get; set; }

        /// <summary>
        /// Gets a description for this data format.  This is used
        /// when creating the open and save dialogs
        /// </summary>
        string Description { get; set; }
    }
}