using System.ComponentModel.Composition;
using System.Windows.Controls;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using SnagLExtenstionTutorial.ViewModel;

namespace SnagLExtenstionTutorial
{
    [Export(typeof(IToolPanelItemViewExtension))]
    public partial class EventListingToolPanelItemExtensionView : UserControl, IToolPanelItemViewExtension
    {
        public EventListingToolPanelItemExtensionView()
        {
            InitializeComponent();
        }

        [Import(typeof(EventListingToolPanelItemExtensionViewModel))]
        public IToolPanelItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as EventListingToolPanelItemExtensionViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
