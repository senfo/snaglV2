using System.Windows;
using Berico.SnagL.ConfigurationEditor.ViewModel;

namespace Berico.SnagL.ConfigurationEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}
