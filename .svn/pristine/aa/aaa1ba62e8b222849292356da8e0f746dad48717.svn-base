using System.ComponentModel.Composition;
using System.Windows.Controls;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    /// <summary>
    /// The view for the zoom toolbar item
    /// </summary>
    [Export(typeof(IToolbarItemViewExtension))]
    public partial class ZoomToolbarItemExtensionView : UserControl, IToolbarItemViewExtension
    {
        public ZoomToolbarItemExtensionView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the view model view model for the control
        /// </summary>
        [Import(typeof(ZoomToolbarItemExtensionViewModel), AllowRecomposition = true)]
        public IToolbarItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as ZoomToolbarItemExtensionViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
