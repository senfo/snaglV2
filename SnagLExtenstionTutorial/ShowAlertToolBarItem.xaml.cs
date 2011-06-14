using System.ComponentModel.Composition;
using System.Windows.Controls;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using SnagLExtenstionTutorial.ViewModel;

namespace SnagLExtenstionTutorial
{
    /// <summary>
    /// Description for ShowAlertToolBarItem.
    /// </summary>
    [Export(typeof(IToolbarItemViewExtension))]
    public partial class ShowAlertToolBarItem : UserControl, IToolbarItemViewExtension
    {
        /// <summary>
        /// Initializes a new instance of the ShowAlertToolBarItem class.
        /// </summary>
        public ShowAlertToolBarItem()
        {
            InitializeComponent();
        }

        [Import(typeof(ShowAlertToolBarItemViewModel))]
        public IToolbarItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as IToolbarItemViewModelExtension;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}