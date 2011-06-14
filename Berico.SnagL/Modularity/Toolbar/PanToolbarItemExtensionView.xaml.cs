//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Windows.Controls;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    [Export(typeof(IToolbarItemViewExtension))]
    public partial class PanToolbarItemExtensionView : UserControl, IToolbarItemViewExtension
    {

        public PanToolbarItemExtensionView()
        {
            InitializeComponent();
        }

        #region IToolbarItemViewExtension Members

            [Import(typeof(PanToolbarItemExtensionViewModel), AllowRecomposition = true)]
            public IToolbarItemViewModelExtension ViewModel
            {
                get
                {
                    return this.DataContext as PanToolbarItemExtensionViewModel;
                }
                set
                {
                    this.DataContext = value;
                }
            }

        #endregion

    }
}