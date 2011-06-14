﻿//-------------------------------------------------------------
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
    public class Attribute
    {
        [DataMember]
        [XmlAttribute("AttributeClass")]
        public string attrAttributeClass;

        [DataMember]
        [XmlAttribute("Value")]
        public string attrValue;
    }
}
