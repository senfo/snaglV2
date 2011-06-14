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

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar.Toolbar
{
    [Export(typeof(IToolbarItemViewExtension))]
    public partial class SearchToolbarItemExtensionView : UserControl, IToolbarItemViewExtension
    {

        public SearchToolbarItemExtensionView()
        {
            InitializeComponent();
        }

        #region IToolbarItemViewExtension Members

            [Import(typeof(SearchToolbarItemExtensionViewModel), AllowRecomposition = true)]
            public IToolbarItemViewModelExtension ViewModel
            {
                get
                {
                    return this.DataContext as SearchToolbarItemExtensionViewModel;
                }
                set
                {
                    this.DataContext = value;
                }
            }

        #endregion

    }
}