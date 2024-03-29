﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Mapping
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[DataContract]
	public class GraphMapData
	{
		[DataMember]
		private IDictionary<string, NodeMapData> nodes;

		[DataMember]
		private IDictionary<string, EdgeMapData> edges;

		public IDictionary<string, NodeMapData> Nodes
		{
			get
			{
				return nodes;
			}
		}

		public IDictionary<string, EdgeMapData> Edges
		{
			get
			{
				return edges;
			}
		}

		public GraphMapData()
		{
			nodes = new Dictionary<string, NodeMapData>();
			edges = new Dictionary<string, EdgeMapData>();
		}

		public void Add(NodeMapData node)
		{
			nodes.Add(node.Id, node);
		}

		public void Add(EdgeMapData edge)
		{
			string edgeKey = GetKey(edge);
			if (!edges.ContainsKey(edgeKey))
			{
				edges.Add(edgeKey, edge);
			}
		}

		public bool TryGetNode(string id, out NodeMapData node)
		{
			bool result = nodes.TryGetValue(id, out node);
			return result;
		}

		public ICollection<NodeMapData> GetNodes()
		{
			return nodes.Values;
		}

		public ICollection<EdgeMapData> GetEdges()
		{
			return edges.Values;
		}

		private static string GetKey(EdgeMapData edge)
		{
			string edgeKey = edge.Source + edge.Target;
			return edgeKey;
		}
	}
}
