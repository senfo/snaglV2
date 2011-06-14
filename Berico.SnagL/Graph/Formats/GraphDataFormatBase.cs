//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Formats
{
    using System.ComponentModel.Composition;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Infrastructure.Events;
    using Berico.SnagL.Infrastructure.Graph;
    using Berico.SnagL.Infrastructure.Graph.Events;
    using Berico.SnagL.Infrastructure.Logging;
    using Berico.SnagL.Infrastructure.Modularity.Contracts;
    using Berico.SnagL.Model;

    [Export(typeof(IGraphDataFormat))]
    public abstract class GraphDataFormatBase : IGraphDataFormat
    {
        internal readonly Logger _logger = Logger.GetLogger(typeof(GraphDataFormatBase));

        private int _priority = 0;
        private string _extension = string.Empty;
        private string _description = string.Empty;

        /// <summary>
        /// Creates a new instance of the GraphDataFormatBase class
        /// </summary>
        public GraphDataFormatBase()
        { }

        #region IGraphDataFormat Members

        public abstract bool Validate(string data);
        internal abstract string ExportData(GraphMapData graph);

        /// <summary>
        /// Imports the specified data onto the graph
        /// </summary>
        /// <param name="data">Data to be loaded onto the graph</param>
        /// <returns>true if the import was succesfull; otherwise false</returns>
        internal abstract GraphMapData ImportData(string data);

        /// <summary>
        /// Exports the provided graph data
        /// </summary>
        /// <param name="scope">The scope of the data being exported</param>
        /// <returns>a string containing the exported data to be saved to
        /// a file if the export was successfull; otherwise null</returns>
        public string Export(string scope)
        {
            _logger.WriteLogEntry(LogLevel.DEBUG, "Export started", null, null);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataExportingEvent>().Publish(new DataLoadedEventArgs(scope, CreationType.Exported));

            //GraphMapData graph = MappingExtensions.ExportGraph(scope).ex;
            GraphMapData graph = GraphManager.Instance.GetGraphComponents(scope).ExportGraph();

            // Call the abstract ExportData method
            string graphMLData = ExportData(graph);

            _logger.WriteLogEntry(LogLevel.DEBUG, "Export completed", null, null);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataExportedEvent>().Publish(new DataLoadedEventArgs(scope, CreationType.Exported));

            return graphMLData;
        }

        /// <summary>
        /// Imports the provided data
        /// </summary>
        /// <param name="data">The data (in string format) to be imported</param>
        /// <param name="components">The GraphComponents instance</param>
        /// <param name="sourceMechanism">Specifies the mechanism for which objects on the graph were imported</param>
        /// <returns>true if the import was successfull (and contained
        /// data); otherwise false</returns>
        public bool Import(string data, GraphComponents components, CreationType sourceMechanism)
        {
            _logger.WriteLogEntry(LogLevel.DEBUG, "Import started", null, null);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataImportingEvent>().Publish(new DataLoadedEventArgs(components.Scope, CreationType.Imported));

            // Cal the abstract ImportData method
            GraphMapData graph = ImportData(data);

            // Convert the mapping data to GraphComponents
            graph.ImportGraph(components, sourceMechanism);
            //MappingExtensions.ImportGraph(graph, components, sourceMechanism);

            _logger.WriteLogEntry(LogLevel.DEBUG, "Import completed", null, null);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataImportedEvent>().Publish(new DataLoadedEventArgs(components.Scope, CreationType.Imported));

            return true;
        }

        /// <summary>
        /// Gets or sets the priority of this format.  This helps ensure
        /// that that the appropriate format is detected by sorting by
        /// priority when analyzing.
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// Gets the extension(s) used by files that this format
        /// represents.  This is used when creating the open and
        /// save dialogs.
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        /// <summary>
        /// Gets a description for this data format.  This is used
        /// when creating the open and save dialogs.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        #endregion
    }
}