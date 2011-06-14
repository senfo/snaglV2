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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Windows.Media;
    using System.Xml.Serialization;
    using Berico.Common;
    using Berico.SnagL.Infrastructure.Data.Formats.Anb;
    using Berico.SnagL.Infrastructure.Data.Mapping;

    public class AnbGraphDataFormat : GraphDataFormatBase
    {
        public AnbGraphDataFormat()
        {
            Extension = "xml";
            Description = "XML representation of Analysts Note Book";
        }

        public static GraphMapData AnbToGraph(Chart chart)
        {
            if (chart == null)
            {
                throw new ArgumentNullException();
            }

            GraphMapData graph = new GraphMapData();

            foreach (ChartItem chartItem in chart.chartItemCollection.chartItems)
            {
                if (chartItem.end != null)
                {
                    IconNodeMapData node = new IconNodeMapData(chartItem.end.entity.attrEntityId);
                    graph.Add(node);

                    node.Label = chartItem.attrLabel;

                    SolidColorBrush backgroundColor = Conversion.HexColorToBrush(chartItem.ciStyle.font.attrBackColour);
                    node.BackgroundColor = backgroundColor.Color;

                    foreach (Anb.Attribute attribute in chartItem.attributeCollection.attributes)
                    {
                        AttributeMapData objAttribute = new AttributeMapData(attribute.attrAttributeClass, attribute.attrValue);
                        node.Attributes.Add(objAttribute.Name, objAttribute);
                    }
                }
                else
                {
                    EdgeMapData edge = new EdgeMapData(chartItem.link.attrEnd1Id, chartItem.link.attrEnd2Id);
                    graph.Add(edge);

                    edge.Label = chartItem.link.linkStyle.attrType;
                }
            }

            return graph;
        }

        public static Chart GraphToAnb(GraphMapData graph)
        {
            Chart chart = new Chart();
            chart.chartItemCollection = new ChartItemCollection();
            chart.chartItemCollection.chartItems = new Collection<ChartItem>();

            foreach (IconNodeMapData node in graph.GetNodes())
            {
                ChartItem chartItem = new ChartItem();
                chart.chartItemCollection.chartItems.Add(chartItem);

                chartItem.attrLabel = node.Label;

                string hexBackgroundColor = String.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", node.BackgroundColor.A, node.BackgroundColor.R, node.BackgroundColor.G, node.BackgroundColor.B);
                chartItem.ciStyle = new CIStyle();
                chartItem.ciStyle.font = new Font();
                chartItem.ciStyle.font.attrBackColour = hexBackgroundColor;

                chartItem.end = new End();
                chartItem.end.entity = new Entity();
                chartItem.end.entity.icon = new Icon();
                chartItem.end.entity.icon.iconStyle = new IconStyle();

                chartItem.attributeCollection = new AttributeCollection();
                chartItem.attributeCollection.attributes = new Collection<Anb.Attribute>();
                foreach (KeyValuePair<string, AttributeMapData> kvp in node.Attributes)
                {
                    Anb.Attribute attribute = new Anb.Attribute();
                    chartItem.attributeCollection.attributes.Add(attribute);

                    attribute.attrAttributeClass = kvp.Key;
                    attribute.attrValue = kvp.Value.Value;
                }
            }

            foreach (EdgeMapData edge in graph.GetEdges())
            {
                ChartItem chartItem = new ChartItem();
                chart.chartItemCollection.chartItems.Add(chartItem);

                chartItem.link = new Link();
                chartItem.link.attrEnd1Id = edge.Source;
                chartItem.link.attrEnd2Id = edge.Target;

                chartItem.link.linkStyle = new LinkStyle();
                chartItem.link.linkStyle.attrType = edge.Label;
            }

            return chart;
        }

        public override bool Validate(string anbData)
        {
            bool isValid;

            try
            {
                GetChart(anbData);
                isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        private static Chart GetChart(string anbData)
        {
            byte[] anbBytes = Encoding.UTF8.GetBytes(anbData);
            MemoryStream anbStream = new MemoryStream(anbBytes);

            XmlSerializer anbDeserializer = new XmlSerializer(typeof(Chart));
            object deserializedAnb = anbDeserializer.Deserialize(anbStream);
            Chart chart = (Chart)deserializedAnb;

            return chart;
        }

        internal override GraphMapData ImportData(string anbData)
        {
            Chart chart = GetChart(anbData);
            GraphMapData graph = AnbGraphDataFormat.AnbToGraph(chart);
            return graph;
        }

        internal override string ExportData(GraphMapData graph)
        {
            Chart chart = AnbGraphDataFormat.GraphToAnb(graph);

            XmlSerializer anbSerializer = new XmlSerializer(typeof(Chart));
            MemoryStream anbStream = new MemoryStream();
            anbSerializer.Serialize(anbStream, chart);

            using (TextReader reader = new StreamReader(anbStream, Encoding.UTF8))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
