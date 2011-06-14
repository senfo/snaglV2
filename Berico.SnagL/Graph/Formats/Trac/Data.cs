//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Formats.Trac
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class Data
    {
        [DataMember(Name = "type")]
        public string type;

        [DataMember(Name = "address")]
        public string address;

        [DataMember(Name = "attr0")]
        public string attr0;

        [DataMember(Name = "attr1")]
        public string attr1;

        [DataMember(Name = "attr2")]
        public string attr2;

        [DataMember(Name = "attr3")]
        public string attr3;

        [DataMember(Name = "attr4")]
        public string attr4;

        [DataMember(Name = "attr5")]
        public string attr5;

        [DataMember(Name = "attr6")]
        public string attr6;

        [DataMember(Name = "attr7")]
        public string attr7;

        [DataMember(Name = "attr8")]
        public string attr8;

        [DataMember(Name = "Contacts")]
        public Collection<Data> contacts;

        public Data()
        {
            contacts = new Collection<Data>();
        }
    }
}
