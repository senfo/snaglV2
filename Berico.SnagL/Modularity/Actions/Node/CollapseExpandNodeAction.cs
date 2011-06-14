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
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Modularity.Actions.Node
{
    /// <summary>
    /// Represents some action (functionality) that is to be performed
    /// on an object
    /// </summary>
    [Export(typeof(IAction))]
    [PartCreationPolicy(CreationPolicy.Shared)] 
    public class CollapseExpandNodeAction : IAction
    {
        GraphComponents graph;

        private Dictionary<NodeViewModelBase, List<IEdgeViewModel>> savedEdges = new Dictionary<NodeViewModelBase, List<IEdgeViewModel>>();
        private Dictionary<NodeViewModelBase, List<IEdgeViewModel>> addedEdges = new Dictionary<NodeViewModelBase, List<IEdgeViewModel>>();

        /// <summary>
        /// Initializes a new instance of the CollapseExpandNodeAction class
        /// </summary>
        public CollapseExpandNodeAction()
        {
            //SnaglEventAggregator.DefaultInstance.GetEvent<NodeDoubleClickEvent>().Subscribe(NodeDoubleClickEventHandler, false);
        }

        /// <summary>
        /// Handles the NodeDoubleClick event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void NodeDoubleClickEventHandler(NodeViewModelMouseEventArgs<MouseButtonEventArgs> args)
        {
            CollapseOrExpandNode(args.NodeViewModel);
        }

        /// <summary>
        /// Determines which operation (Collapse or Expand) is most appropriate
        /// and executes it
        /// </summary>
        /// <param name="targetNode">The node view model that is being collapsed or expanded</param>
        private void CollapseOrExpandNode(NodeViewModelBase targetNode)
        {

            //TODO: check collection to determine if this node has already been collapsed

            CollapseNode(targetNode);

        }

        /// <summary>
        /// Collapse all children (outgoing nodes) for the target node
        /// </summary>
        /// <param name="targetNode"></param>
        private void CollapseNode(NodeViewModelBase targetNode)
        {
            List<INode> childNodes = new List<INode>();
            List<IEdgeViewModel> edgesToBeRemoved = new List<IEdgeViewModel>();
            List<IEdgeViewModel> edgesToBeAdded = new List<IEdgeViewModel>();
            List<NodeViewModelBase> nodesToBeRemoved = new List<NodeViewModelBase>();

            graph = Data.GraphManager.Instance.GetGraphComponents(targetNode.Scope);
            
            // Get all the chidren for the target node
            childNodes = GetChildren(targetNode.ParentNode);

            // Ensure we have any child nodes before continuing
            if (childNodes.Count > 0)
            {
                foreach (INode childNode in childNodes)
                {
                    NodeViewModelBase nodeVM = graph.GetNodeViewModel(childNode) as NodeViewModelBase;

                    foreach (IEdge edge in graph.GetEdges(childNode))
                    {
                        IEdgeViewModel edgeVM = graph.GetEdgeViewModel(edge);

                        // Determine if this is an incoming edge
                        if (edge.Target == childNode)
                        {
                            // Existing incoming edges need to be removed
                            // and new ones need to be added
                            edgesToBeRemoved.Add(edgeVM);

                            // Determine if this edge's source node is inside
                            // of the child nodes being collapsed
                            if (!childNodes.Contains(edge.Source) && edge.Source != targetNode.ParentNode)
                            {
                                IEdgeViewModel newEdgeVM = (edgeVM as EdgeViewModelBase).Copy(edge.Source, targetNode.ParentNode);

                                edgesToBeAdded.Add(newEdgeVM);
                            }
                        }
                        else // Handle the outgoing edges
                        {
                            // Outgoing edges need to be saved and removed
                            edgesToBeRemoved.Add(edgeVM);
                        }
                    }

                    // Remove (hide) the node
                    //nodeVM.IsHidden = true;
                    nodesToBeRemoved.Add(nodeVM);
                }

                graph.RemoveNodeViewModels(nodesToBeRemoved);

                // Remove (hide) the edges
                RemoveEdges(edgesToBeRemoved, targetNode);

                // Add new edges
                AddEdges(edgesToBeAdded, targetNode);

            }


        }

        private void RemoveEdges(List<IEdgeViewModel> edges, NodeViewModelBase parentNode)
        {
            // Save the edges
            savedEdges.Add(parentNode, edges);

            graph.RemoveEdgeViewModels(edges);
            // Loop over all the edge view models that were provided
            // that need to be removed
            //foreach (IEdgeViewModel edgeVM in edges)
            //{
            //    edgeVM.Visibility = Visibility.Collapsed;
            //}
        }

        private void AddEdges(List<IEdgeViewModel> edges, NodeViewModelBase parentNode)
        {
            // Record the new edges
            addedEdges.Add(parentNode, edges);

            // Add the edges to the graph
            graph.AddEdgeViewModels(edges);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        private List<INode> GetChildren(INode parentNode)
        {

            List<INode> children = new List<INode>();

            // Get all the successors for the parent node
            foreach (INode successor in graph.Data.Successors(parentNode))
            {
                // Add this child (successor)
                children.Add(successor);

                // Get all of this child's children
                children.AddRange(GetChildren(successor));
            }

            return children;
        }

        #region IAction Members

            public string Name
            {
                get { return "COLLAPSE_EXPAND_ACTION"; }
            }

        #endregion
    }
}