/*
  In App.xaml:
  <Application.Resources>
      <vm:EventListingToolPanelItemExtensionViewModel xmlns:vm="clr-namespace:SnagLExtenstionTutorial"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;

namespace AnotherSnagLExtenstionTutorial.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the EventListingToolPanelItemExtensionViewModel in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:EventListingToolPanelItemExtensionViewModel xmlns:vm="clr-namespace:SnagLExtenstionTutorial"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    [PartMetadata("ID", "ToolPanelItemViewModelExtension"), Export(typeof(GuidListingToolPanelItemExtensionViewModel))]
    public class GuidListingToolPanelItemExtensionViewModel : ViewModelBase, IToolPanelItemViewModelExtension
    {
        #region Fields

        /// <summary>
        /// Stores a value that indicates whether the control is enabled
        /// </summary>
        private bool _isEnabled = true;

        /// <summary>
        /// Stores the tools description
        /// </summary>
        private string _description;

        /// <summary>
        /// Stores the name of the tool
        /// </summary>
        private string _toolName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GuidListingToolPanelItemExtensionViewModel class.
        /// </summary>
        public GuidListingToolPanelItemExtensionViewModel()
        {
            this.Index = 3;
            this.Description = "Another Example ToolPanel extension";
            this.ToolName = "Guid Lister";
            this.Events = new ObservableCollection<string>();

            for (int x = 0; x < 10; x++)
            {
                Events.Add(Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        /// <summary>
        /// Gets a references to a collection of events exposed
        /// </summary>
        public ObservableCollection<string> Events
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the index 
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tool is enabled
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Gets or sets the name of the tool
        /// </summary>
        public string ToolName
        {
            get
            {
                return _toolName;
            }
            set
            {
                _toolName = value;
                RaisePropertyChanged("ToolName");
            }
        }

        #endregion
    }
}