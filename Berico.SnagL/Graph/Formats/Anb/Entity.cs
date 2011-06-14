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
    public class Entity
    {
        [DataMember]
        [XmlAttribute("EntityId")]
        public string attrEntityId;

        [DataMember]
        [XmlAttribute("Identity")]
        public string attrIdentity;

        [DataMember]
        [XmlElement("Icon")]
        public Icon icon;

        [DataMember]
        [XmlElement("CardCollection")]
        public CardCollection cardCollection;
    }
}
