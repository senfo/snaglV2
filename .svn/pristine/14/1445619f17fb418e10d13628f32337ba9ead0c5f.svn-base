using System.ComponentModel.Composition;
using System.Windows.Controls;
using AnotherSnagLExtenstionTutorial.ViewModel;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace AnotherSnagLExtenstionTutorial
{
    [Export(typeof(IToolPanelItemViewExtension))]
    public partial class GuidListingToolPanelItemExtensionView : UserControl, IToolPanelItemViewExtension
    {
        public GuidListingToolPanelItemExtensionView()
        {
            InitializeComponent();
        }

        [Import(typeof(GuidListingToolPanelItemExtensionViewModel))]
        public IToolPanelItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as GuidListingToolPanelItemExtensionViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
