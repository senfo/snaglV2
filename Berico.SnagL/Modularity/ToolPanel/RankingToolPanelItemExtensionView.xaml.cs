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

namespace Berico.SnagL.Infrastructure.Modularity.ToolPanel
{
    [Export(typeof(IToolPanelItemViewExtension))]
    public partial class RankingToolPanelItemExtensionView : UserControl, IToolPanelItemViewExtension
    {
        public RankingToolPanelItemExtensionView()
        {
            InitializeComponent();
        }
    
        #region IToolPanelItemViewExtension Members

            [Import(typeof(RankingToolPanelItemExtensionViewModel), AllowRecomposition = true)]
            public IToolPanelItemViewModelExtension  ViewModel
            {
	            get 
	            {
                    return this.DataContext as RankingToolPanelItemExtensionViewModel;
	            }
	            set 
	            {
                    this.DataContext = value;
	            }
            }

        #endregion
    }
}