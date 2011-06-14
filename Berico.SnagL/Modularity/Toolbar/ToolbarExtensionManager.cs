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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Berico.SnagL.Infrastructure.Controls;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    /// <summary>
    /// This class is responsible for managing all MEF toolbar extensions
    /// used in the application.  While the Berico.LinkAnalysis.SnagL.
    /// Modularity.ExtensionManager class is responsible for managing all
    /// extensions
    /// </summary>
    public class ToolbarExtensionManager : IPartImportsSatisfiedNotification
    {
        private readonly SnaglEventAggregator eventAggregator = SnaglEventAggregator.DefaultInstance;

        private static ToolbarExtensionManager instance;
        private static object syncRoot = new object();

        private string scope = string.Empty;
        private ToolbarViewModel parentToolbar = null;

        /// <summary>
        /// Gets or sets the provider that represents the actual log
        /// itself and provides the means to write to it.  Only a single
        /// provider is used and MEF is responsible for instantiating it.
        /// </summary>
        [ImportMany(typeof(IToolbarItemViewExtension), AllowRecomposition = true)]
        public List<IToolbarItemViewExtension> Extensions { get; set; }

        /// <summary>
        /// Gets the Toolbar control that belongs to
        /// this extension manager.
        /// </summary>
        public ToolbarViewModel Owner
        {
            get { return parentToolbar; }
        }

        /// <summary>
        /// Performs the initial setup of the ToolbarExtensionManager.  This method
        /// must be called once before the Instance property can be used.
        /// </summary>
        /// <param name="_parentToolbar"></param>
        /// <param name="_scope"></param>
        public static void InitialSetup(ToolbarViewModel _parentToolbar, string _scope)
        {
            // Validate parameters
            if (_parentToolbar == null)
                throw new System.ArgumentNullException("_parentToolPanel", "An invalid ToolPanelViewModel was provided");

            if (string.IsNullOrEmpty(_scope))
                throw new System.ArgumentNullException("_scope", "An invalid scope was provided");

            lock (syncRoot)
            {
                // Ensure that this can only be called once
                if (instance == null)
                {
                    instance = new ToolbarExtensionManager(_parentToolbar, _scope);
                    ExtensionManager.ComposeParts(instance);
                }
            }
        }

        /// <summary>
        /// Gets the instance of the ToolbarExtensionManager class.  The first time
        /// this class is ever accessed, the InitialSetup method must be called.
        /// </summary>
        public static ToolbarExtensionManager Instance
        {
            get
            {
                // Check if the instance is null
                if (instance == null)
                {
                    // If we are here then the Instance property was called
                    // before the InitialSetup method.
                    throw new InvalidOperationException("The ToolbarExtensionManager class must be initialized (using the InitialSetup method) before the Instance property can be used.");
                }

                return instance;
            }
        }

        /// <summary>
        /// Create a new instance of the ToolbarExtensionManager.  This class is
        /// private and will be called internally by the InitialSetup method.
        /// </summary>
        /// <param name="_parentToolbar">The viewmodel for the toolbar being managed</param>
        /// <param name="_scope">The scope of the data that the tool bar items will have
        /// access too</param>
        private ToolbarExtensionManager(ToolbarViewModel _parentToolbar, string _scope)
        {
            this.parentToolbar = _parentToolbar;
            this.scope = _scope;
        }

        #region IPartImportsSatisfiedNotification Members

            public void OnImportsSatisfied()
            {
                //TODO: ANALYZE, SORT AND UPDATE THE TOOLBARVIEWMODEL
                // Clear the toolbar and rebuild
                foreach (IToolbarItemViewExtension extension in parentToolbar.Items.Where(uiElement => uiElement is IToolbarItemViewExtension))
                {
                    extension.ViewModel.ToolbarItemSelected -= new EventHandler<EventArgs>(ViewModel_ToolbarItemSelected);
                }
                
                parentToolbar.Items.Clear();

                // Loop through all the extensions and add their contents to the toolbar
                int previousIndex = -1;
                this.Extensions.Sort(delegate(IToolbarItemViewExtension item1, IToolbarItemViewExtension item2)
                {
                    return item1.ViewModel.Index.CompareTo(item2.ViewModel.Index);
                });

                foreach (IToolbarItemViewExtension toolbarItemViewExtension in this.Extensions)
                {
                    // If the index jumps, we need to add a spacer
                    if (previousIndex != -1 && toolbarItemViewExtension.ViewModel.Index > previousIndex + 1)
                    {
                        //parentToolbar.Items.Insert(toolbarItemViewExtension.ViewModel.Index -1, new ToolbarItemSpacer());
                        parentToolbar.Items.Add(new ToolbarItemSpacer());
                    }
                    
                    //TODO:  ADD TOOLBAR ITEM ADDED EVENT

                    // Now add the new toolbar item
                    //parentToolbar.Items.Insert(toolbarItemViewExtension.ViewModel.Index, toolbarItemViewExtension as UIElement);
                    if (!parentToolbar.Items.Contains(toolbarItemViewExtension as UIElement))
                    {
                        parentToolbar.Items.Add(toolbarItemViewExtension as UIElement);
                        toolbarItemViewExtension.ViewModel.ToolbarItemSelected += new EventHandler<EventArgs>(ViewModel_ToolbarItemSelected);
                        previousIndex = toolbarItemViewExtension.ViewModel.Index;
                    }
                }

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ViewModel_ToolbarItemSelected(object sender, EventArgs e)
            {
                IToolbarItemViewModelExtension toolbarItem = sender as IToolbarItemViewModelExtension;

                OnToolBarItemClicked(new ToolBarItemEventArgs(toolbarItem, this.scope));
            }

            /// <summary>
            /// Raises the ToolBarItemClicked event
            /// </summary>
            /// <param name="e">The event arguments</param>
            protected virtual void OnToolBarItemClicked(ToolBarItemEventArgs args)
            {
                this.eventAggregator.GetEvent<ToolBarItemClickedEvent>().Publish(args);
            }

        #endregion

    }
}