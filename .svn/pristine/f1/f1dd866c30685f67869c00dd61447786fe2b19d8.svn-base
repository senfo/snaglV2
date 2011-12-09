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
    public class AttributeClass
    {
        [DataMember]
        [XmlAttribute("Name")]
        public string attrName;

        [DataMember]
        [XmlAttribute("Type")]
        public string attrType;

        [DataMember]
        [XmlAttribute("ShowValue")]
        public bool attrShowValue;

        [DataMember]
        [XmlAttribute("Visible")]
        public bool attrVisible;

        [DataMember]
        [XmlAttribute("UserCanAdd")]
        public bool attrUserCanAdd;

        [DataMember]
        [XmlAttribute("UserCanRemove")]
        public bool attrUserCanRemove;

        [DataMember]
        [XmlAttribute("Prefix")]
        public string attrPrefix;

        [DataMember]
        [XmlAttribute("ShowPrefix")]
        public bool attrShowPrefix;

        [DataMember]
        [XmlElement("Font")]
        public Font font;
    }
}
