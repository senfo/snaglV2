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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Berico.Common.Modularity;
using Berico.SnagL.Infrastructure.Configuration;

namespace Berico.SnagL.Infrastructure.Modularity
{
    /// <summary>
    /// This class is responsible for managing all MEF extensions
    /// used in the application.  It loads all catalogs and composes
    /// parts.  All parts (which can be considered plugins) are centrally
    /// managed by this class.
    /// </summary>
    public static class ExtensionManager
    {
        private static Logging.Logger logger = Logging.Logger.GetLogger(typeof(ExtensionManager));
        private static AggregateCatalog primaryCatalog = new AggregateCatalog();
        private static object syncRoot = new object();
        private static bool isBusy = false;
        private static bool initialized = false;

        /// <summary>
        /// This method initializes the ExtensionManager.  Its primary job is to setup all
        /// the part catalogs.  It starts by setting up the deployment catalogs.  Since deployment
        /// catalogs download parts asynchronously, the rest of initialization will not occur until
        /// after the downloads are complete.
        /// </summary>
        public static void Initialize()
        {
            List<DeploymentCatalog> catalogs = new List<DeploymentCatalog>();

            if (!initialized)
            {
                // Indicate that the extension manager is busy
                IsBusy = true;

                // Create all of our catalogs.  Catalogs provide the means for discovering
                // available extensions

                try
                {
                    // Gets the part catalogs for external (downloadable) parts
                    foreach (Extension extension in ConfigurationManager.Instance.CurrentConfig.Extensions)
                    {
                        catalogs.Add(GetDeploymentCatalog(extension.Path));
                    }
                }
                catch (ArgumentNullException ex)
                {
                    // Log event but continue 
                    logger.WriteLogEntry(Logging.LogLevel.ERROR, "An error occurred while trying to build a catalog from an external location.", ex, null);
                }

                // Finish up initializing the extension manager
                CompleteInitialization(catalogs);
            }
        }

        /// <summary>
        /// Completes initialization of the ExtensionManager class.  This will be called after
        /// all DeploymentCatalogs have finished downloading parts.
        /// </summary>
        /// <param name="catalogs">The external deployment catalogs to load</param>
        private static void CompleteInitialization(IEnumerable<DeploymentCatalog> catalogs)
        {
            lock (syncRoot)
            {
                AggregateCatalog tempCatalog = new AggregateCatalog();

                // Clear the primary catalog
                primaryCatalog.Catalogs.Clear();

                // Build a catalog containing all Parts in the entire assembly
                AssemblyCatalog assemblyCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());

                foreach (DeploymentCatalog catalog in catalogs)
                {
                    tempCatalog.Catalogs.Add(catalog);
                }

                tempCatalog.Catalogs.Add(assemblyCatalog);

                // Create the two special catalogs for the logger and preference providers
                FilteredCatalog loggerProviderCatalog = BuildLoggerProviderCatalog(tempCatalog);
                FilteredCatalog preferencesProviderCatalog = BuildPreferenceProviderCatalog(tempCatalog);
                FilteredCatalog toolbarExtensionsCatalog = BuildToolbarExtensionsCatalog(tempCatalog);
                FilteredCatalog toolPanelExtensionsCatalog = BuildToolPanelExtensionsCatalog(tempCatalog);
                FilteredCatalog similarityMeasureExtensionsCatalog = BuildSimilarityMeasureExtensionsCatalog(tempCatalog);
                FilteredCatalog rankingCatalog = BuildRankingCatalog(tempCatalog);
                FilteredCatalog graphDataFormatExtensionsCatalog = BuildGraphDataFormatExtensionsCatalog(tempCatalog);
                FilteredCatalog actionCatalog = BuildActionCatalog(tempCatalog);
                FilteredCatalog layoutCatalog = BuildLayoutCatalog(tempCatalog);

                // Add the two filtered catalogs to the master catalog
                primaryCatalog.Catalogs.Add(loggerProviderCatalog);
                primaryCatalog.Catalogs.Add(preferencesProviderCatalog);
                primaryCatalog.Catalogs.Add(toolbarExtensionsCatalog);
                primaryCatalog.Catalogs.Add(toolPanelExtensionsCatalog);
                primaryCatalog.Catalogs.Add(similarityMeasureExtensionsCatalog);
                primaryCatalog.Catalogs.Add(graphDataFormatExtensionsCatalog);
                primaryCatalog.Catalogs.Add(actionCatalog);
                primaryCatalog.Catalogs.Add(layoutCatalog);
                primaryCatalog.Catalogs.Add(rankingCatalog);
            }

            // Reset the flags
            IsBusy = false;
            initialized = true;

            // Satisfy the imports for the logger manager
            CompositionInitializer.SatisfyImports(Logging.LoggerManager.Instance);

            //Logging.LoggerManager.Instance.Level = ConfigurationManager.GetValue(ConfigurationManager.PARAMETER_LOGGER_LEVEL);
        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain a single logger provider part.
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing the 
        /// configured logger provider; otherwise the default logger provider</returns>
        private static FilteredCatalog BuildLoggerProviderCatalog(ComposablePartCatalog parentCatalog)
        {
            string loggerProvider = ConfigurationManager.Instance.CurrentConfig.LoggerProvider.Provider;
            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findSpecifiedLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("ILoggerProvider"))) && (def.Metadata.ContainsKey("ID") && def.Metadata["ID"].ToString().ToLower().Equals(loggerProvider.ToLower()));
            Expression<Func<ComposablePartDefinition, bool>> findDefaultLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("ILoggerProvider"))) && (def.Metadata.ContainsKey("IsDefault") && bool.Parse(def.Metadata["IsDefault"].ToString()) == true);

            // We don't want to search for the specified logger provider if
            // none was specified.
            if (string.IsNullOrEmpty(loggerProvider))
            {
                return new FilteredCatalog(parentCatalog, findDefaultLogger);
            } 

            // If we get here, we need to try and find the logger provider specified
            // by the user
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findSpecifiedLogger);

            // If the catalog is empty, fall to the default
            if ((newFilteredCatalog.Parts == null) || (newFilteredCatalog.Parts.Count() == 0))
            {
                // If we didn't find any parts, we should try again and just get the
                // default provider
                return new FilteredCatalog(parentCatalog, findDefaultLogger);
            }
            else
            {
                return newFilteredCatalog;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the extension manager is
        /// currently busy.  This is used to indicate that the
        /// manager is actively building its catalogs or composing.
        /// </summary>
        public static bool IsBusy
        {
            get { return isBusy; }
            set
            {
                lock (syncRoot)
                {
                    isBusy = value;
                }
            }
        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain a single preference provider part.
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing the 
        /// configured preference provider; otherwise the default perference provider</returns>
        private static FilteredCatalog BuildPreferenceProviderCatalog(ComposablePartCatalog parentCatalog)
        {
            // Retrieve the name of the configrued preference provider
            string preferencesProvider = ConfigurationManager.Instance.CurrentConfig.PreferencesProvider.Provider;
            
            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findSpecifiedLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IPreferencesProvider"))) && (def.Metadata.ContainsKey("ID") && def.Metadata["ID"].ToString().ToLower().Equals(preferencesProvider.ToLower()));
            Expression<Func<ComposablePartDefinition, bool>> findDefaultLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IPreferencesProvider"))) && (def.Metadata.ContainsKey("IsDefault") && bool.Parse(def.Metadata["IsDefault"].ToString()) == true);

            // We don't want to search for the specified logger provider if
            // none was specified.
            if (string.IsNullOrEmpty(preferencesProvider))
            {
                return new FilteredCatalog(parentCatalog, findDefaultLogger);
            }

            // If we get here, we need to try and find the logger provider specified
            // by the user
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findSpecifiedLogger);

            // If the catalog is empty, fall to the default
            if ((newFilteredCatalog.Parts == null) || (newFilteredCatalog.Parts.Count() == 0))
            {
                // If we didn't find any parts, we should try again and just get the
                // default provider
                return new FilteredCatalog(parentCatalog, findDefaultLogger);
            }

            return newFilteredCatalog;
        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain all toolbar extensions
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing all
        /// tool bar extensions</returns>
        private static FilteredCatalog BuildToolbarExtensionsCatalog(ComposablePartCatalog parentCatalog)
        {
            // Retrieve the name of the configrued preference provider
            //string preferencesProvider = ConfigurationManager.GetValue(ConfigurationManager.PARAMETER_PREFERENCES_PROVIDER);

            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findAllToolbarItemExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IToolbarItemViewExtension")) || (def.Metadata.ContainsKey("ID") && def.Metadata["ID"].ToString().ToLower().Equals("toolbaritemviewmodelextension")));
            //Expression<Func<ComposablePartDefinition, bool>> findDefaultLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IPreferencesProvider"))) && (def.Metadata.ContainsKey("IsDefault") && bool.Parse(def.Metadata["IsDefault"].ToString()) == true);

            // If we get here, we need to try and find the logger provider specified
            // by the user
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllToolbarItemExtensions);

            return newFilteredCatalog;

        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain all tool panel extensions
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing all
        /// Tool Panel extensions</returns>
        private static FilteredCatalog BuildToolPanelExtensionsCatalog(ComposablePartCatalog parentCatalog)
        {
            // Retrieve the name of the configrued preference provider
            //string preferencesProvider = ConfigurationManager.GetValue(ConfigurationManager.PARAMETER_PREFERENCES_PROVIDER);

            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findAllToolPanelItemExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IToolPanelItemViewExtension")) || (def.Metadata.ContainsKey("ID") && def.Metadata["ID"].ToString().ToLower().Equals("toolpanelitemviewmodelextension")));

            // If we get here, we need to try and find the logger provider specified
            // by the user
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllToolPanelItemExtensions);

            return newFilteredCatalog;

        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain all similarity measure classes
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing the 
        /// configured preference provider; otherwise the default perference provider</returns>
        private static FilteredCatalog BuildSimilarityMeasureExtensionsCatalog(ComposablePartCatalog parentCatalog)
        {
            // Retrieve the name of the configrued preference provider
            //string preferencesProvider = ConfigurationManager.GetValue(ConfigurationManager.PARAMETER_PREFERENCES_PROVIDER);

            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findAllSimilarityMeasureExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("ISimilarityMeasure")) || (def.Metadata.ContainsKey("ID") && def.Metadata["ID"].ToString().ToLower().Equals("SimilarityMeasureExtension")));
            //Expression<Func<ComposablePartDefinition, bool>> findDefaultLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IPreferencesProvider"))) && (def.Metadata.ContainsKey("IsDefault") && bool.Parse(def.Metadata["IsDefault"].ToString()) == true);

            // If we get here, we need to try and find the logger provider specified
            // by the user
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllSimilarityMeasureExtensions);

            return newFilteredCatalog;

        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain all graph data format classes
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing the 
        /// configured preference provider; otherwise the default perference provider</returns>
        private static FilteredCatalog BuildGraphDataFormatExtensionsCatalog(ComposablePartCatalog parentCatalog)
        {
            // Retrieve the name of the configrued preference provider
            //string preferencesProvider = ConfigurationManager.GetValue(ConfigurationManager.PARAMETER_PREFERENCES_PROVIDER);

            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findAllSimilarityMeasureExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IGraphDataFormat")));
            //Expression<Func<ComposablePartDefinition, bool>> findDefaultLogger = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IPreferencesProvider"))) && (def.Metadata.ContainsKey("IsDefault") && bool.Parse(def.Metadata["IsDefault"].ToString()) == true);

            // If we get here, we need to try and find the logger provider specified
            // by the user
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllSimilarityMeasureExtensions);

            return newFilteredCatalog;

        }

        /// <summary>
        /// Builds and returns a special catalog, based on the application assembly, that
        /// has been filtered to contain all IAction classes
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns>a Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog containing the 
        /// configured preference provider; otherwise the default perference provider</returns>
        private static FilteredCatalog BuildActionCatalog(ComposablePartCatalog parentCatalog)
        {
            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findAllActionExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IAction")));

            newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllActionExtensions);

            return newFilteredCatalog;

        }

        private static FilteredCatalog BuildLayoutCatalog(ComposablePartCatalog parentCatalog)
        {
            Expression<Func<ComposablePartDefinition, bool>> findAllLayoutExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("LayoutBase")));
            FilteredCatalog newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllLayoutExtensions);

            return newFilteredCatalog;
        }

        /// <summary>
        /// </summary>
        /// <param name="parentCatalog"></param>
        /// <returns></returns>
        private static FilteredCatalog BuildRankingCatalog(ComposablePartCatalog parentCatalog)
        {
            FilteredCatalog newFilteredCatalog = null;

            Expression<Func<ComposablePartDefinition, bool>> findAllRankingExtensions = (def) => (def.ExportDefinitions.Any(exportDef => exportDef.ContractName.EndsWith("IRanker")));
            newFilteredCatalog = new FilteredCatalog(parentCatalog, findAllRankingExtensions);

            return newFilteredCatalog;

        }

        /// <summary>
        /// Instructs MEF to satisfy all imports, against the current
        /// collection of catalogs,for the provided part.
        /// </summary>
        /// <param name="part">The object (MEF part) that should be composed</param>
        public static void ComposeParts(Object part)
        {
            try
            {
                // The logger is used before configruation and initialization
                // so we want to handle it in a special way.  We just want to
                // compose it using a single filtered catalog to find the default
                // logger.  Everything will be rebuilt and recomposed after
                // configruation.
                if (part is Logging.LoggerManager)
                {
                    // Create a Berico.LinkAnalysis.
                    FilteredCatalog loggerCatalog = BuildLoggerProviderCatalog(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()));
                    primaryCatalog.Catalogs.Add(loggerCatalog);

                    CompositionHost.Initialize(new CompositionContainer(primaryCatalog));
                }

                // Now we compose and satisfy the imports for the provided Part
                CompositionInitializer.SatisfyImports(part);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLogEntry(Logging.LogLevel.ERROR, "An error occurred trying to compose the parts for the provided object.", ex, null);
                }
                else
                {
                    // This is only for testing.  We don't really want to
                    // do this in the long run.
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Creates a DeploymentCatalog that targets the specified path.  This
        /// method asynchronously downloads the XAP files in the target folder
        /// and will update the catalog as it completes.
        /// </summary>
        /// <param name="path">The path to download XAP files from</param>
        /// <returns>the DeploymentCatalog that was created</returns>
        private static DeploymentCatalog GetDeploymentCatalog(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path", "The provided path was empty");

            DeploymentCatalog catalog = new DeploymentCatalog(path);

            catalog.DownloadCompleted += (s, e) => DownloadCompleted(s, e);
            catalog.DownloadAsync();


            return catalog;
        }

        /// <summary>
        /// Handles the DownloadCompleted event of the DeploymentCatalog
        /// </summary>
        /// <param name="sender">The object that initially fired the event</param>
        /// <param name="e">The event arguments</param>
        private static void DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //TODO:  THIS SHOULD BE TESTED FURTHER AND INCLUDE ERROR TRAPPING

            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
        }
    }
}