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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.Windows.Controls;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Responsible for managing the ContextMenu control's
    /// used in SnagL
    /// </summary>
    public class ContextMenuManager
    {
        #region Fields

            private static ContextMenuManager instance;
            private static object syncRoot = new object();
            private ObservableCollection<MenuItem> graphMenuItems = new ObservableCollection<MenuItem>();
            private ObservableCollection<MenuItem> nodeMenuItems = new ObservableCollection<MenuItem>();
            private ContextMenu graphContextMenu;
            private ContextMenu nodeContextMenu;
            private NodeViewModelBase targetNodeVM = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the node for which the context menu references
        /// </summary>
        public NodeViewModelBase TargetNodeVM
        {
            get
            {
                return targetNodeVM;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Internal constructor
        /// </summary>
        private ContextMenuManager() { }

        /// <summary>
        /// Initializes the ContextMenuManager class
        /// </summary>
        private void Initialize()
        {
            SnaglEventAggregator.DefaultInstance.GetEvent<GraphMouseRightButtonDownEvent>().Subscribe(GraphMouseRightButtonDownEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseRightButtonDownEvent>().Subscribe(NodeMouseRightButtonDownEventHandler, false);

            graphMenuItems = new ObservableCollection<MenuItem>();
            nodeMenuItems = new ObservableCollection<MenuItem>();
        }

    #endregion

        #region External Methods

            /// <summary>
            /// Gets the instance of the ContextMenuManager class
            /// </summary>
            public static ContextMenuManager Instance
            {
                get
                {
                    if (instance == null)
                    {
                        lock (syncRoot)
                        {
                            if (instance == null)
                            {
                                instance = new ContextMenuManager();
                                instance.Initialize();
                            }
                        }
                    }
                    return instance;
                }
            }

            /// <summary>
            /// Adds the provided item to the Graph's ContextMenu
            /// </summary>
            /// <param name="newMenuItem">The menu item to add to the ContextMenu</param>
            public void AddGraphMenuItem(MenuItem newMenuItem)
            {
                MenuItem existingMenuItem = graphMenuItems.Where(menuitems => menuitems.Header == newMenuItem.Header).FirstOrDefault();

                if (existingMenuItem != null)
                    graphMenuItems.Remove(existingMenuItem);

                graphMenuItems.Add(newMenuItem);
                graphMenuItems.OrderBy(menuItem => menuItem.ID);
            }

            /// <summary>
            /// Adds the provided item to the Node's ContextMenu
            /// </summary>
            /// <param name="newMenuItem">The menu item to add to the ContextMenu</param>
            public void AddNodeMenuItem(MenuItem newMenuItem)
            {
                MenuItem existingMenuItem = nodeMenuItems.Where(menuitems => menuitems.Header == newMenuItem.Header).FirstOrDefault();

                if (existingMenuItem != null)
                    nodeMenuItems.Remove(existingMenuItem);

                nodeMenuItems.Add(newMenuItem);
                nodeMenuItems.OrderBy(menuItem => menuItem.ID);
            }

        #endregion

        #region Events and EventHandlers

            /// <summary>
            /// Indicates the Graph's ContextMenu is opening
            /// </summary>
            public event EventHandler<EventArgs> GraphContextMenuOpening;

            /// <summary>
            /// Indicates the Node's ContextMenu is opening
            /// </summary>
            public event EventHandler<ContextMenuEventArgs> NodeContextMenuOpening;

            /// <summary>
            /// Indicates the Node's ContextMenu has closed
            /// </summary>
            public event EventHandler<EventArgs> NodeContextMenuClosed;

            /// <summary>
            /// Handles the MouseRightButtonDown event for the Graph
            /// </summary>
            /// <param name="args">Arguments for the event</param>
            public void GraphMouseRightButtonDownEventHandler(MouseButtonEventArgs args)
            {
                // Ensure that the context menu has not already been initialized
                if (graphContextMenu == null)
                {
                    graphContextMenu = new ContextMenu();
                    graphContextMenu.ItemsSource = graphMenuItems;
                    graphContextMenu.Opening += new RoutedEventHandler(GraphContextMenuOpeningHandler);
                }

                // Open the Graph's ContextMenu
                graphContextMenu.OpenPopup(args.GetPosition(null));
            }

            /// <summary>
            /// Handles the MouseRightButtonDown event for the Node
            /// </summary>
            /// <param name="args">Arguments for the event</param>
            public void NodeMouseRightButtonDownEventHandler(NodeViewModelMouseEventArgs<MouseButtonEventArgs> args)
            {
                // Ensure that the context menu has not already been initialized
                if (nodeContextMenu == null)
                {
                    nodeContextMenu = new ContextMenu();
                    nodeContextMenu.ItemsSource = nodeMenuItems;
                    nodeContextMenu.Opening += new RoutedEventHandler(NodeContextMenuOpeningHandler);
                    nodeContextMenu.Closed += new RoutedEventHandler(NodeContextMenuClosedHandler);
                }

                // Save the NodeViewModel that the right click was on
                targetNodeVM = args.NodeViewModel;

                // Open the Node's ContextMenu
                nodeContextMenu.OpenPopup(args.MouseArgs.GetPosition(null));
            }

            /// <summary>
            /// Handles the Opening event fired by the ContextMenu control
            /// </summary>
            /// <param name="sender">The ContextMenu that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void GraphContextMenuOpeningHandler(object sender, System.Windows.RoutedEventArgs e)
            {
                OnGraphContextMenuOpening(EventArgs.Empty);
            }

            /// <summary>
            /// Handles the Opening event fired by the ContextMenu control
            /// </summary>
            /// <param name="sender">The ContextMenu that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void NodeContextMenuOpeningHandler(object sender, System.Windows.RoutedEventArgs e)
            {
                OnNodeContextMenuOpening(new ContextMenuEventArgs(targetNodeVM));
            }

            /// <summary>
            /// Handles the Closed event fired by the ContextMenu control
            /// </summary>
            /// <param name="sender">The ContextMenu that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void NodeContextMenuClosedHandler(object sender, System.Windows.RoutedEventArgs e)
            {
                OnNodeContextMenuClosed(EventArgs.Empty);
            }

            /// <summary>
            /// Fires the GraphContextMenuOpening event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected virtual void OnGraphContextMenuOpening(EventArgs e)
            {
                if (GraphContextMenuOpening != null)
                    GraphContextMenuOpening(this, e);
            }

            /// <summary>
            /// Fires the NodeContextMenuOpening event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected virtual void OnNodeContextMenuOpening(ContextMenuEventArgs e)
            {
                if (NodeContextMenuOpening != null)
                    NodeContextMenuOpening(this, e);
            }

            /// <summary>
            /// Fires the NodeContextMenuClosed event
            /// </summary>
            /// <param name="e">The arguments for the event</param>
            protected virtual void OnNodeContextMenuClosed(EventArgs e)
            {
                if (NodeContextMenuClosed != null)
                    NodeContextMenuClosed(this, e);
            }

            public Delegate[] GetContextMenuOpeningInvocationList()
            {
                return GraphContextMenuOpening.GetInvocationList();
            }

        #endregion
    }
}
