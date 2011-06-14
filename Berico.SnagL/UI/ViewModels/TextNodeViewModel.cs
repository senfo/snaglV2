//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Model;
using Berico.SnagL.Infrastructure.Graph;

namespace Berico.SnagL.UI
{

    /// <summary>
    /// This class represents a node view model that includes
    /// an icon.  Currently this class provides no additional
    /// functionality beyond it's base class.
    /// </summary>
    public class TextNodeViewModel : NodeViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the NodeViewModel class.
        /// </summary>
        public TextNodeViewModel(Node node, string scope)
            : base(node, scope)
        { }

    }
}