//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Mapping.JS
{
    using System.Windows.Browser;

    [ScriptableType()]
    public class ScriptableAttributeMapData
    {
        [ScriptableMember()]
        public string Name { get; set; }

        [ScriptableMember()]
        public string Value { get; set; }

        [ScriptableMember()]
        public bool IsHidden { get; set; }

        [ScriptableMember()]
        public int SemanticType { get; set; }

        [ScriptableMember()]
        public string SimilarityMeasure { get; set; }
    }
}
