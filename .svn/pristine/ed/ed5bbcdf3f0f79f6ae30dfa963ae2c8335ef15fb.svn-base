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
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using Berico.SnagL.Infrastructure.Data;
using System.Linq;

namespace Berico.SnagL.Infrastructure.Graph
{
    public class NodeFilter
    {
        private bool isActive = false;

        private List<NodeViewModelBase> hiddenNodes = new List<NodeViewModelBase>();
        private List<IEdgeViewModel> hiddenEdges = new List<IEdgeViewModel>();

        /// <summary>
        ///
        /// </summary>
        public ReadOnlyCollection<NodeViewModelBase> HiddenNodes
        {
            get { return hiddenNodes.AsReadOnly(); }
        }

        /// <summary>
        ///
        /// </summary>
        public ReadOnlyCollection<IEdgeViewModel> HiddenEdges
        {
            get { return hiddenEdges.AsReadOnly(); }
        }

        /// <summary>
        /// Gets whether or not a filter is active
        /// </summary>
        public bool IsActive
        {
            get { return this.isActive; }
        }

        /// <summary>
        /// Create new instance of the Berico.LinkAnalysis.SnagL.Data.NodeFilter class
        /// </summary>
        public NodeFilter()
        { }

        public void Filter(List<Node> nodes)
        {
            // Validate the node list
            if (nodes == null)
                return;

            List<NodeViewModelBase> foundNodeVMs = new List<NodeViewModelBase>();

            // Loop over the nodes
            foreach (Node currentNode in nodes)
            {
                // Add the node view model to our view model collection
                foundNodeVMs.Add(GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModel(currentNode) as NodeViewModelBase);
            }

            Filter(foundNodeVMs);
        }

        public void Filter(List<NodeViewModelBase> nodeVMs)
        {
            // Validate the node view model list
            if (nodeVMs == null)
                return;

            // Itterate over all node view models
            foreach (NodeViewModelBase currentNodeVM in GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModels())
            {
                // If the current node VM is in the provided list, we can skip over it
                // since this is one of the nodes that we are filtering on
                if (nodeVMs.Contains(currentNodeVM))
                    continue;

                // If we get here, we are dealing with a node that needs
                // to be hidden
                //currentNodeVM.IsHidden = true;

                this.hiddenNodes.Add(currentNodeVM);

                // Get the edges for this node
                this.hiddenEdges.AddRange(GraphManager.Instance.DefaultGraphComponentsInstance.GetEdgeViewModels(currentNodeVM.ParentNode));
            }

            // Now we need to remove the nodes and edges
            // that were effected by the filter
            GraphManager.Instance.DefaultGraphComponentsInstance.RemoveNodeViewModels(this.hiddenNodes);
            GraphManager.Instance.DefaultGraphComponentsInstance.RemoveEdgeViewModels(this.hiddenEdges);

            if (this.hiddenNodes.Count > 0)
                this.isActive = true;
            else
                this.isActive = false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeVM"></param>
        public void Hide(NodeViewModelBase nodeVM)
        {
            // Validate the node view model list
            if (nodeVM == null)
                return;

            if (hiddenNodes.Contains(nodeVM))
                return;

            this.hiddenNodes.Add(nodeVM);

            // Get the edges for this node
            this.hiddenEdges.AddRange(GraphManager.Instance.DefaultGraphComponentsInstance.GetEdgeViewModels(nodeVM.ParentNode));

            // Now we need to remove the nodes and edges
            // that were effected by the filter
            GraphManager.Instance.DefaultGraphComponentsInstance.RemoveNodeViewModels(this.hiddenNodes);
            GraphManager.Instance.DefaultGraphComponentsInstance.RemoveEdgeViewModels(this.hiddenEdges);

            if (this.hiddenNodes.Count > 0)
                this.isActive = true;
            else
                this.isActive = false;
        }

        /// <summary>
        /// Shows all currently hidden nodes and edges
        /// </summary>
        public void ShowAll()
        {
            // Add the hidden node view models
            GraphManager.Instance.DefaultGraphComponentsInstance.AddNodeViewModels(hiddenNodes.Cast<INodeShape>().ToList());
            this.hiddenNodes.Clear();

            // Add the hidden edge view models
            GraphManager.Instance.DefaultGraphComponentsInstance.AddEdgeViewModels(hiddenEdges);
            this.hiddenEdges.Clear();
        }

        /// <summary>
        /// Turns off the currently active filter
        /// </summary>
        public void TurnOffFilter()
        {
            ShowAll();
            this.isActive = false;
        }

    }
}