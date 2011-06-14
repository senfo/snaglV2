//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Berico.SnagL.Infrastructure.Controls;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Modularity.ToolPanel
{
    /// <summary>
    /// This class is responsible for managing all MEF ToolPanel extensions
    /// used in the application.  While the Berico.LinkAnalysis.SnagL.
    /// Modularity.ExtensionManager class is responsible for managing all
    /// extensions
    /// </summary>
    public class ToolPanelExtensionManager : IPartImportsSatisfiedNotification
    {
        private static ToolPanelExtensionManager instance;
        private static object syncRoot = new object();
        private string scope = string.Empty;
        private ToolPanelViewModel parentToolPanel = null;

        /// <summary>
        /// Gets or sets the list of available ToolPanel items
        /// </summary>
        [ImportMany(typeof(IToolPanelItemViewExtension), AllowRecomposition = true)]
        public List<IToolPanelItemViewExtension> Extensions { get; set; }

        /// <summary>
        /// Gets the Toolpanel control that belongs to
        /// this extension manager.
        /// </summary>
        public ToolPanelViewModel Owner
        {
            get { return parentToolPanel; }
        }

        /// <summary>
        /// Performs the initial setup of the ToolPanelExtensionManager.  This method
        /// must be called once before the Instance property can be used.
        /// </summary>
        /// <param name="_parentToolPanel">The view model for the toolpanel being managed</param>
        /// <param name="_scope">The scope of the data that the tool panel items will have
        /// access too</param>
        public static void InitialSetup(ToolPanelViewModel _parentToolPanel, string _scope)
        {
            // Validate parameters
            if (_parentToolPanel == null)
                throw new System.ArgumentNullException("_parentToolPanel","An invalid ToolPanelViewModel was provided");

            if (string.IsNullOrEmpty(_scope))
                throw new System.ArgumentNullException("_scope", "An invalid scope was provided");

            lock (syncRoot)
            {
                // Ensure that this can only be called once
                if (instance == null)
                {
                    instance = new ToolPanelExtensionManager(_parentToolPanel, _scope);
                    ExtensionManager.ComposeParts(instance);
                }
            }
        }

        /// <summary>
        /// Gets the instance of the ToolPanelExtensionManager class.  The first time
        /// this class is ever accessed, the InitialSetup method must be called.
        /// </summary>
        public static ToolPanelExtensionManager Instance
        {
            get
            {
                // Check if the instance is null
                if (instance == null)
                {
                    // If we are here then the Instance property was called
                    // before the InitialSetup method.
                    throw new System.InvalidOperationException("The ToolPanelExtensionManager class must be initialized (using the InitialSetup method) before the Instance property can be used.");
                }

                return instance;
            }
        }

        /// <summary>
        /// Create a new instance of the ToolPanelExtensionManager.  This class is
        /// private and will be called internally by the InitialSetup method.
        /// </summary>
        /// <param name="_parentToolPanel">The view model for the toolpanel being managed</param>
        /// <param name="_scope">The scope of the data that the tool panel items will have
        /// access too</param>
        private ToolPanelExtensionManager(ToolPanelViewModel _parentToolPanel, string _scope)
        {
            this.parentToolPanel = _parentToolPanel;
            this.scope = _scope;
        }

        #region IPartImportsSatisfiedNotification Members

            /// <summary>
            /// Adds all the tool panels to the control, after all the MEF
            /// imports have been satisfied
            /// </summary>
            public void OnImportsSatisfied()
            {
                // Sort the extensions by index
                this.Extensions.Sort(delegate(IToolPanelItemViewExtension item1, IToolPanelItemViewExtension item2)
                {
                    return item1.ViewModel.Index.CompareTo(item2.ViewModel.Index);
                });

                // Loop through all extensions and created an AccordionItem for each
                foreach (IToolPanelItemViewExtension toolPanelItemViewExtension in this.Extensions)
                {
                    AccordionItem newItem;
                    bool alreadyContains = false;

                    // Make sure we don't try to add the item twice
                    // TODO: This is a bit of a hack becaue OnImportsSatisfied() is getting called more than once
                    foreach (AccordionItem item in parentToolPanel.Items)
                    {
                        if ((string)item.Header == toolPanelItemViewExtension.ViewModel.ToolName)
                        {
                            alreadyContains = true;
                            break;
                        }
                    }

                    if (!alreadyContains)
                    {
                        try
                        {
                            // Create the AccordionItem
                            newItem = new AccordionItem();
                            newItem.Header = toolPanelItemViewExtension.ViewModel.ToolName;
                            newItem.Content = toolPanelItemViewExtension;
                        }
                        catch
                        {
                            newItem = null;
                        }

                        // Define binding for IsEnabled property
                        Binding binding = new Binding()
                        {
                            Source = toolPanelItemViewExtension.ViewModel,
                            Path = new PropertyPath("IsEnabled")
                        };

                        newItem.SetBinding(AccordionItem.IsEnabledProperty, binding);

                        //ToolTipService.SetToolTip(newItem, toolPanelItemViewExtension.ViewModel.Description);

                        // Add the AccordionItem to the Accordion
                        parentToolPanel.Items.Add(newItem);
                    }
                }

            }

        #endregion
    }
}