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
    public partial class PrintToolbarItemExtensionView : UserControl, IToolbarItemViewExtension
    {
        public PrintToolbarItemExtensionView()
        {
            InitializeComponent();
        }

        #region IToolbarItemViewExtension Members

        [Import(typeof(PrintToolbarItemExtensionViewModel), AllowRecomposition = true)]
        public IToolbarItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as PrintToolbarItemExtensionViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        #endregion
    }
}
