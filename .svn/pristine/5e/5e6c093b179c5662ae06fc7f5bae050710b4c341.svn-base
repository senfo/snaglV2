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
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Berico.SnagL.Infrastructure.Data.Formats.Trac;
    using Berico.SnagL.Infrastructure.Data.Mapping;

    public class TracGraphDataFormat : GraphDataFormatBase
    {
        public TracGraphDataFormat()
        {
            Extension = "json";
            Description = "JSON representation of a Trac";
        }

        public static GraphMapData JsonToGraph(TracWrapper tracWrapper)
        {
            if (tracWrapper == null)
            {
                throw new ArgumentNullException();
            }

            GraphMapData graph = new GraphMapData();

            IconNodeMapData seedNode = SeedToNode(tracWrapper.trac.result.seed);
            graph.Add(seedNode);

            // by definition Data are guaranteed to not have been seen yet
            foreach (Data data in tracWrapper.trac.result.datas)
            {
                IconNodeMapData dataNode = DataToNode(data);
                graph.Add(dataNode);

                EdgeMapData edgeToSeed = new EdgeMapData(seedNode.Id, dataNode.Id);
                graph.Add(edgeToSeed);

                // by definition Contact may have already been seen
                if (data.contacts != null)
                {
                    foreach (Data contact in data.contacts)
                    {
                        if (!contact.address.Equals(seedNode.Id))
                        {
                            NodeMapData contactNode;
                            bool nodeAlreadyExists = graph.TryGetNode(contact.address, out contactNode);
                            if (!nodeAlreadyExists)
                            {
                                contactNode = DataToNode(contact);
                                graph.Add(contactNode);
                            }

                            EdgeMapData edgeToData = new EdgeMapData(dataNode.Id, contactNode.Id);
                            graph.Add(edgeToData);
                        }
                    }
                }
            }

            return graph;
        }

        private static IconNodeMapData SeedToNode(string seed)
        {
            string[] splitSeed = SplitOnFirstColon(seed);
            string seedType = splitSeed[0];
            string seedAddress = splitSeed[1];

            IconNodeMapData seedNode = new IconNodeMapData(seedAddress);

            return seedNode;
        }

        private static string[] SplitOnFirstColon(string input)
        {
            char[] separator = new char[] { ':' };
            string[] result = input.Split(separator, StringSplitOptions.None);
            return result;
        }

        private static IconNodeMapData DataToNode(Data data)
        {
            IconNodeMapData node = new IconNodeMapData(data.address);

            if (data.attr0 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr0", data.attr0);
                node.Attributes.Add("attr0", objAttribute);
            }

            if (data.attr1 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr1", data.attr1);
                node.Attributes.Add("attr1", objAttribute);
            }

            if (data.attr2 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr2", data.attr2);
                node.Attributes.Add("attr2", objAttribute);
            }

            if (data.attr3 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr3", data.attr3);
                node.Attributes.Add("attr3", objAttribute);
            }

            if (data.attr4 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr4", data.attr4);
                node.Attributes.Add("attr4", objAttribute);
            }

            if (data.attr5 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr5", data.attr5);
                node.Attributes.Add("attr5", objAttribute);
            }

            if (data.attr6 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr6", data.attr6);
                node.Attributes.Add("attr6", objAttribute);
            }

            if (data.attr7 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr7", data.attr7);
                node.Attributes.Add("attr7", objAttribute);
            }

            if (data.attr8 != null)
            {
                AttributeMapData objAttribute = new AttributeMapData("attr8", data.attr8);
                node.Attributes.Add("attr8", objAttribute);
            }

            return node;
        }

        public override bool Validate(string jsonData)
        {
            bool isValid;

            try
            {
                GetTracWrapper(jsonData);
                isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        private static TracWrapper GetTracWrapper(string jsonData)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            using (MemoryStream jsonStream = new MemoryStream(jsonBytes))
            {
                DataContractJsonSerializer jsonDeserializer = new DataContractJsonSerializer(typeof(TracWrapper));
                object deserializedJson = jsonDeserializer.ReadObject(jsonStream);
                TracWrapper tracWrapper = (TracWrapper)deserializedJson;

                return tracWrapper;
            }
        }

        internal override GraphMapData ImportData(string jsonData)
        {
            TracWrapper tracWrapper = GetTracWrapper(jsonData);
            GraphMapData graph = TracGraphDataFormat.JsonToGraph(tracWrapper);
            return graph;
        }

        internal override string ExportData(GraphMapData graph)
        {
            throw new NotSupportedException();
        }
    }
}
