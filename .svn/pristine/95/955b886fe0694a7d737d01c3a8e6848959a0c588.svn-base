//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Formats.Anb
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [DataContract]
    [XmlRoot("Chart")]
    public class Chart
    {
        [DataMember]
        [XmlAttribute("IdReferenceLinking")]
        public bool attrIdReferenceLinking;

        [DataMember]
        [XmlAttribute("Rigorous")]
        public bool attrRigorous;

        [DataMember]
        [XmlAttribute("UseLocalTimeZone")]
        public bool attrUseLocalTimeZone;

        [DataMember]
        [XmlElement("Font")]
        public Font font;

        [DataMember]
        [XmlElement("AttributeClassCollection")]
        public AttributeClassCollection attributeClassCollection;

        [DataMember]
        [XmlElement("LinkTypeCollection")]
        public LinkTypeCollection linkTypeCollection;

        [DataMember]
        [XmlElement("CurrentStyleCollection")]
        public CurrentStyleCollection currentStyleCollection;

        [DataMember]
        [XmlElement("ChartItemCollection")]
        public ChartItemCollection chartItemCollection;
    }
}
