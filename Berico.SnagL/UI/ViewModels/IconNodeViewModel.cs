//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.Common;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// This class represents a node view model that includes
    /// an icon.
    /// </summary>
    public class IconNodeViewModel : NodeViewModelBase
    {
        // Specifies the path for default icon nodes
        private string imageSource = "/Berico.SnagL;component/Resources/Icons/genericNode.png";

        /// <summary>
        /// Initializes a new instance of the IconNodeViewModel class.
        /// </summary>
        public IconNodeViewModel(Node node, string scope)
            : base(node, scope)
        {
            BackgroundColor = Conversion.HexColorToBrush("#00FFFFFF");
        }

        /// <summary>
        /// Gets or sets the source for the node's icon
        /// </summary>
        [ExportableProperty("ImageSource")]
        public string ImageSource
        {
            get { return this.imageSource; }
            set
            {
                this.imageSource = value;
                RaisePropertyChanged("ImageSource");
            }
        }
    }
}