//-------------------------------------------------------------
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
    using Berico.SnagL.Infrastructure.Data.Attributes;

    public class AttributeMapData
    {
        public string Name { get; private set; }
        public string Value { get; set; }

        public bool IsHidden { get; set; }

        public SemanticType SemanticType { get; set; }
        public string SimilarityMeasure { get; set; }

        public AttributeMapData(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
