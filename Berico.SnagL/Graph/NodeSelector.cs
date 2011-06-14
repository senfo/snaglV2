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
using System.Collections.ObjectModel;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Model;
using Berico.SnagL.UI;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Responsible for managing and mainting the selection
    /// of nodes on the graph.  Each GraphComponents instance
    /// will have its own NodeSelector.
    /// </summary>
    public class NodeSelector
    {

        //TODO: MAYBE JUST QUERY FOR SELECTED NODES RATHER THAN STORING THEM

        private ObservableCollection<NodeViewModelBase> selectedNodes = new ObservableCollection<NodeViewModelBase>();

        /// <summary>
        /// Gets a collection of all the selected node view models
        /// </summary>
        public Collection<NodeViewModelBase> SelectedNodes
        {
            get { return selectedNodes; }
        }

        /// <summary>
        /// Gets the currently selected node.  If more thatn one node
        /// is selected, than null is returned.
        /// </summary>
        public NodeViewModelBase SelectedNode
        {
            get { return selectedNodes.Count == 1 ? selectedNodes[0] : null; }
        }

        /// <summary>
        /// Gets whether or not any nodes are currently selected
        /// </summary>
        public bool AreAnyNodesSelected
        {
            get { return selectedNodes.Count > 0 ? true : false; }
        }

        /// <summary>
        /// Gets whether or not more than one node is selected
        /// </summary>
        public bool AreMultipleNodesSelected
        {
            get { return selectedNodes.Count > 1 ? true : false; }
        }

        /// <summary>
        /// Create new instance of the Berico.LinkAnalysis.SnagL.Data.NodeSelector class
        /// </summary>
        public NodeSelector() { }

        /// <summary>
        /// Selects the provided node
        /// </summary>
        /// <param name="node">The node to be selected</param>
        public void Select(Node node)
        {
            // Get the view model for the provided node
            Select(GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModel(node) as NodeViewModelBase);
        }

        /// <summary>
        /// Selects all of the provided nodes
        /// </summary>
        /// <param name="node">The list of nodes that should be selected</param>
        public void Select(List<Node> nodes)
        {
            // Validate the node list
            if (nodes == null)
                return;

            // Loop over the nodes
            foreach (Node currentNode in nodes)
            {
                // Select the node
                Select(currentNode);
            }

        }

        /// <summary>
        /// Selects the provided node.  If the node is already
        /// selected, it is unselectedf
        /// </summary>
        /// <param name="nodeVM">The view model of the node to be selected</param>
        public void Select(NodeViewModelBase nodeVM)
        {

            // Only select the node if it isn't already selected
            if (!this.selectedNodes.Contains(nodeVM))
            {
                // Keep track of the selected node viewmodels
                selectedNodes.Add(nodeVM);

                // Set the state of the node view model to Selected
                nodeVM.CurrentState = NodeStates.Selected;
            }
            else
                Unselect(nodeVM);

        }

        /// <summary>
        /// Selects all of the provided nodes
        /// </summary>
        /// <param name="nodeVMs">The list of node view models</param>
        public void Select(List<NodeViewModelBase> nodeVMs)
        {
            // Validate the node list
            if (nodeVMs == null)
                return;

            // Loop over the node view models provided
            foreach (NodeViewModelBase currentNodeVM in nodeVMs)
            {
                // Select the node
                Select(currentNodeVM);
            }
        }

        /// <summary>
        /// Unselect the provided node
        /// </summary>
        /// <param name="node">The node that should be unselected</param>
        public void Unselect(Node node)
        {
            // Get the view model for the provided node
            Unselect(GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModel(node) as NodeViewModelBase);
        }

        /// <summary>
        /// Unselect the provided node
        /// </summary>
        /// <param name="nodeVM">The view model of the node to be selected</param>
        public void Unselect(NodeViewModelBase nodeVM)
        {
            // Remove the view model from our selected collection
            selectedNodes.Remove(nodeVM);

            // Set the state of the node view model to Selected
            nodeVM.CurrentState = NodeStates.Unselected;
        }

        /// <summary>
        /// Unselects all currently selected nodes
        /// </summary>
        public void UnselectAll()
        {

            // Loop over all the currently selected nodes
            foreach (NodeViewModelBase currentNodeVM in this.selectedNodes)
            {
                // Set the state of the node view model to Selected
                currentNodeVM.CurrentState = NodeStates.Unselected;
            }

            // Remove all nodes from the selected nodes list
            this.selectedNodes.Clear();
        }

        /// <summary>
        /// Unselects all nodes that are currently selected
        /// </summary>
        public void TurnOffSelection()
        {
            UnselectAll();
        }

        /// <summary>
        /// Unselects all selected nodes and
        /// selects all unselected nodes
        /// </summary>
        public void InvertSelection()
        {
            // Loop over all the node view models
            foreach (INodeShape nodeVM in GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModels())
            {
                // Select this node view model
                this.Select(nodeVM as NodeViewModelBase);
            }
        }

    }
}