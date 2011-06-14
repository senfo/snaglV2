//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    using System.ComponentModel.Composition;
    using System.Windows.Controls;
    using Berico.SnagL.Infrastructure.Modularity.Contracts;

    [Export(typeof(IToolbarItemViewExtension))]
    public partial class SimpleTreeToolbarItemExtensionView : UserControl, IToolbarItemViewExtension
    {
        public SimpleTreeToolbarItemExtensionView()
        {
            InitializeComponent();
        }

        #region IToolbarItemViewExtension Members

        [Import(typeof(SimpleTreeToolbarItemExtensionViewModel), AllowRecomposition = true)]
        public IToolbarItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as SimpleTreeToolbarItemExtensionViewModel;
            }

            set
            {
                this.DataContext = value;
            }
        }

        #endregion
    }
}
