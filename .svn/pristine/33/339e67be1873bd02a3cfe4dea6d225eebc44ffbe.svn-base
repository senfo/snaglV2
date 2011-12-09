//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using Berico.Common;
using Berico.SnagL.Infrastructure;
using Berico.SnagL.Infrastructure.Configuration;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Data.Mapping.JS;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Interop;
using Berico.SnagL.Infrastructure.Logging;
using Berico.SnagL.Infrastructure.Modularity;
using Berico.SnagL.Infrastructure.Modularity.Toolbar;
using Berico.SnagL.Infrastructure.Modularity.ToolPanel;
using Berico.SnagL.Infrastructure.Preferences;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// This class represents the view model for the SnagL application itself
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private const string PREFERENCE_DEFAULT_SIDEBAR_COLLAPSED = "false";
        private const string PREFERENCE_DEFAULT_SIDEBAR_WIDTH = "150";
        private const string PREFERENCE_DEFAULT_MAX_SIDEBAR_WIDTH = "300";

        Logger logger = Logger.GetLogger(typeof(MainViewModel));

        private double height;
        private double width;
        private bool isBusy = false;
        private bool isSideBarCollapsed;
        private bool isToolbarHidden = false;
        private bool isToolPanelHidden = false;
        private GridLength sideBarWidth;
        private GridLength sideBarMaxWidth = new GridLength(300);
        private SolidColorBrush graphLabelBackground = new SolidColorBrush(Colors.Green);
        private SolidColorBrush graphLabelForeground = new SolidColorBrush(Colors.White);
        private bool isGraphLabelEnabled = false;
        private string graphLabelText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Wire-up some event handlers
                Application.Current.Host.Content.Resized += new System.EventHandler(Content_Resized);
                Application.Current.Startup += new StartupEventHandler(Current_Startup);

                //logger.WriteLogEntry(LogLevel.DEBUG, "TEST", null, null);

            }
        }

        #region Properties

            /// <summary>
            /// Represents the application's Height.  The user interface is bound to 
            /// this value and this value is automatically updated when the browser
            /// is resized.
            /// </summary>
            private double Height
            {
                get { return height; }
                set { height = value; }
            }

            /// <summary>
            /// Represents the application's Width.  The user interface is bound to 
            /// this value and this value is automatically updated when the browser
            /// is resized.
            /// </summary>
            private double Width
            {
                get { return width; }
                set { width = value; }
            }

            /// <summary>
            /// Gets or sets whether or not the side bar panel is expanded or collapsed.
            /// Anytime this property changes, the state is saved by the PreferenceManager.
            /// </summary>
            public bool IsSidebarCollapsed
            {
                get { return this.isSideBarCollapsed; }
                set
                {
                    this.isSideBarCollapsed = value;
                
                    // TODO:  MAKE THIS A LITTLE SMARTER

                    // Save the state of the side bar panel to the preferences provider
                    PreferencesManager.Instance.SetPreference((typeof(MainViewModel)).ToString() + "_SideBarCollapsed", value.ToString());
                    RaisePropertyChanged("IsSidebarCollapsed");
                }
            }

            /// <summary>
            /// Gets or sets whether the tool bar is hidden
            /// </summary>
            public bool IsToolbarHidden
            {
                get { return this.isToolbarHidden; }
                set
                {
                    // Binding was not used because we could not
                    // bind to the Toolbar UserControl do to
                    // datacontext conflicts
                    this.isToolbarHidden = value;
                    ShowOrHideToolbar();
                }
            }

            /// <summary>
            /// Gets or sets whether the tool panel is hidden
            /// </summary>
            public bool IsToolPanelHidden
            {
                get { return this.isToolPanelHidden; }
                set
                {
                    // Binding was not used because we could not
                    // bind to the Toolbar UserControl do to
                    // datacontext conflicts
                    this.isToolPanelHidden = value;
                    ShowOrHideToolPanel();
                    RaisePropertyChanged("IsToolPanelHidden");
                }
            }

            /// <summary>
            /// Gets or sets the width of the side bar panel.  Anytime that this property
            /// changes, the state is saved by the PreferenceManager.
            /// </summary>
            public GridLength SideBarWidth
            {
                get { return this.sideBarWidth; }
                set 
                {
                    this.sideBarWidth = value;

                    // TODO:  MAKE THIS A LITTLE SMARTER

                    // Save the state of the side bar panel to the preferences provider
                    if (!IsToolPanelHidden)
                    {
                        PreferencesManager.Instance.SetPreference((typeof(MainViewModel)).ToString() + "_SideBarWidth", value.ToString());
                    }
                    RaisePropertyChanged("SideBarWidth");
                }
            }

            /// <summary>
            /// Gets or sets the maximum width of the side bar panel.  This is bound to
            /// the view and is only used to control the view when the toolpanel is not
            /// shown.
            /// </summary>
            public GridLength SideBarMaxWidth
            {
                get { return this.sideBarMaxWidth; }
                set
                {
                    this.sideBarMaxWidth = value;
                    RaisePropertyChanged("SideBarMaxWidth");
                }
            }

            /// <summary>
            /// Gets or sets whether or not the application itself is
            /// busy.  This property is bound to the BusyIndicator 
            /// conbtrol in the MainPage view.
            /// </summary>
            public bool IsBusy
            {
                get { return isBusy; }
                set
                {
                    isBusy = value;
                    RaisePropertyChanged("IsBusy");
                }
            }

            /// <summary>
            /// Gets or sets the background color of the graph label
            /// </summary>
            public SolidColorBrush GraphLabelBackground
            {
                get { return this.graphLabelBackground; }
                set
                {
                    this.graphLabelBackground = value;
                    RaisePropertyChanged("GraphLabelBackground");
                }
            }

            /// <summary>
            /// Gets or sets the foreground color of the graph label
            /// </summary>
            public SolidColorBrush GraphLabelForeground
            {
                get { return this.graphLabelForeground; }
                set
                {
                    this.graphLabelForeground = value;
                    RaisePropertyChanged("GraphLabelForeground");
                }
            }

            /// <summary>
            /// gets or sets whether the graph label is enabled
            /// </summary>
            public bool IsGraphLabelEnabled
            {
                get { return this.isGraphLabelEnabled; }
                set
                {
                    this.isGraphLabelEnabled = value;
                    RaisePropertyChanged("IsGraphLabelEnabled");
                }
            }

            /// <summary>
            /// Gets or sets the text for the graph label
            /// </summary>
            public string GraphLabelText
            {
                get { return this.graphLabelText; }
                set
                {
                    this.graphLabelText = value;
                    RaisePropertyChanged("GraphLabelText");
                }
            }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void ShowOrHideToolbar()
        {
            if (IsToolbarHidden)
                ViewModelLocator.ToolbarVMStatic.Visibility = Visibility.Collapsed;
            else
                ViewModelLocator.ToolbarVMStatic.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowOrHideToolPanel()
        {
            if (IsToolPanelHidden)
            {
                ViewModelLocator.ToolPanelVMStatic.Visibility = Visibility.Collapsed;
                SideBarMaxWidth = new GridLength(0);
                SideBarWidth = new GridLength(0);
            }
            else
            {
                ViewModelLocator.ToolPanelVMStatic.Visibility = Visibility.Visible;
                SideBarMaxWidth = new GridLength(300);
                SideBarWidth = PreferencesManager.Instance.GetPreference((typeof(MainViewModel)).ToString() + "_SideBarWidth", PREFERENCE_DEFAULT_SIDEBAR_WIDTH).ToGridLength();
            }
        }

        /// <summary>
        /// Handles the Startup event that is fired once the Silverlight application
        /// has completed its startup process.
        /// </summary>
        /// <param name="sender">The object (Silverlight application) that fire the event</param>
        /// <param name="e">The arguments for the event</param>
        private void Current_Startup(object sender, StartupEventArgs e)
        {
            // Initialize the configuration manager, passing in the key/value list of
            // configuration settings that was specified by the host.
            ConfigurationManager.Instance.Initialize(e.InitParams);

            // Create the scriptable object
            ScriptableGraph scriptableGraph = new ScriptableGraph();

            // Register our scriptable object
            System.Windows.Browser.HtmlPage.RegisterScriptableObject("SNAGL", scriptableGraph);
            HtmlPage.RegisterCreateableType("ScriptableAttributeMapData", typeof(ScriptableAttributeMapData));
            HtmlPage.RegisterCreateableType("ScriptableEdgeMapData", typeof(ScriptableEdgeMapData));
            HtmlPage.RegisterCreateableType("ScriptableIconNodeMapData", typeof(ScriptableIconNodeMapData));
            HtmlPage.RegisterCreateableType("ScriptablePoint", typeof(ScriptablePoint));
            HtmlPage.RegisterCreateableType("ScriptableSize", typeof(ScriptableSize));
            HtmlPage.RegisterCreateableType("ScriptableTextNodeMapData", typeof(ScriptableTextNodeMapData));

            // Initialize the extension manager
            ExtensionManager.Initialize();

            // Compose the parts for the GraphManager class
            ExtensionManager.ComposeParts(GraphManager.Instance);

            // Load the theme for the application
            LoadTheme();

            // Set the application to its previous state
            ApplyPreferences();

            // Load extensions
            LoadExtensions();

            // Load any external resources
            LoadExternalResources(ConfigurationManager.Instance.CurrentConfig.ExternalResources);

            // Listen for application wide events
            SnaglEventAggregator.DefaultInstance.GetEvent<TimeConsumingTaskExecutingEvent>().Subscribe(TimeConsumingTaskExecutingEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<TimeConsumingTaskCompletedEvent>().Subscribe(TimeConsumingTaskCompletedEventHandler, false);

            //logger.WriteLogEntry(LogLevel.DEBUG, "TEST", null, null);
        }

        /// <summary>
        /// Responsible for loading and applying the theme specified by the
        /// configuration settings
        /// </summary>
        private void LoadTheme()
        {
            if (ConfigurationManager.Instance.CurrentConfig.Theme != null)
            {
                //ResourceDictionary resourceDictionary = Application.Current.Resources as ResourceDictionary;
                foreach (string style in ConfigurationManager.Instance.CurrentConfig.Theme.Value.Split('|'))
                {
                    //TODO:  VALIDATION
                    //TODO:  ENSURE SAME STYLE ISN'T LOADED TWICE
                    //TODO:  HANDLE ERRORS

                    // Load the XAML file as a resource dictionary
                    XDocument xaml = XDocument.Load(style);
                    ResourceDictionary rd = XamlReader.Load(xaml.ToString()) as ResourceDictionary;

                    // Merge the new resource dictionary into the application
                    Application.Current.Resources.MergedDictionaries.Add(rd);
                }
            }
        }

        private void LoadExtensions()
        {
            string parentScope = GraphManager.Instance.DefaultGraphComponentsInstance.Scope;

            // Setup the ToolbarExtensionManager
            ToolbarExtensionManager.InitialSetup(ViewModelLocator.ToolbarVMStatic, parentScope);
            ToolbarExtensionManager toolbarManager = ToolbarExtensionManager.Instance;

            // Setup the ToolPanelExtensionManager
            ToolPanelExtensionManager.InitialSetup(ViewModelLocator.ToolPanelVMStatic, parentScope);
            ToolPanelExtensionManager toolPanelManager = ToolPanelExtensionManager.Instance;
        }

        /// <summary>
        /// Responsible for ensuring that the application is returned to the
        /// state that the user last left it.  The PreferenceManager is 
        /// responsible for loading the preferences that were previously
        /// saved.  If no preference is found, a default is applied.
        /// </summary>
        private void ApplyPreferences()
        {
            IsToolbarHidden = ConfigurationManager.Instance.CurrentConfig.IsToolbarHidden;
            IsToolPanelHidden = ConfigurationManager.Instance.CurrentConfig.IsToolPanelHidden;

            if (!IsToolPanelHidden)
            {
                IsSidebarCollapsed = bool.Parse(PreferencesManager.Instance.GetPreference((typeof(MainViewModel)).ToString() + "_SideBarCollapsed", PREFERENCE_DEFAULT_SIDEBAR_COLLAPSED));
                SideBarWidth = PreferencesManager.Instance.GetPreference((typeof(MainViewModel)).ToString() + "_SideBarWidth", PREFERENCE_DEFAULT_SIDEBAR_WIDTH).ToGridLength();
            }

            // Perform a little configuration here as well
            GraphLabel graphLabel = ConfigurationManager.Instance.CurrentConfig.GraphLabel;

            // Check if settings were provided for the graph label
            if (graphLabel == null)
                IsGraphLabelEnabled = false;
            else
            {
                // Set Background
                if (!String.IsNullOrWhiteSpace(graphLabel.Background))
                    GraphLabelBackground = Conversion.HexColorToBrush(graphLabel.Background);
                else
                    GraphLabelBackground = new SolidColorBrush(Colors.Green);

                // Set Foreground
                if (!String.IsNullOrWhiteSpace(graphLabel.Foreground))
                    GraphLabelForeground = Conversion.HexColorToBrush(graphLabel.Foreground);
                else
                    GraphLabelBackground = new SolidColorBrush(Colors.White);

                // Set Text
                if (!String.IsNullOrWhiteSpace(graphLabel.Text))
                    GraphLabelText = graphLabel.Text;
                else
                    GraphLabelText = "UNCLASSIFIED";

                IsGraphLabelEnabled = true;
            }
        }

        /// <summary>
        /// Loads any external resources that might be included
        /// </summary>
        private static void LoadExternalResources(IEnumerable<ExternalResource> externalResources)
        {
            if (externalResources != null && externalResources.Count() > 0)
            {
                XapLoader loader = new XapLoader();

                loader.XapLoaded += (s, e) =>
                {
                    RaiseSnaglLoadedEvent();
                };

                // Load the external resources
                loader.BeginLoadXap(externalResources.ToArray(), true);
            }
            else
            {
                RaiseSnaglLoadedEvent();
            }
        }

        /// <summary>
        /// Raises the SnaglLoadedEvent
        /// </summary>
        private static void RaiseSnaglLoadedEvent()
        {
            SnaglLoadedEventArgs args = new SnaglLoadedEventArgs
            {
                ExternalResourcesLoaded = true
            };

            // Fire the event that indicates that the graph surface
            // has fully loaded
            SnaglEventAggregator.DefaultInstance.GetEvent<SnaglLoadedEvent>().Publish(args);
        }

        /// <summary>
        /// Handles the resizing of the Silverlight plug-in
        /// </summary>
        /// <param name="sender">The object where the event handler is attached</param>
        /// <param name="e">The event data</param>
        private void Content_Resized(object sender, System.EventArgs e)
        {
            this.height = Application.Current.Host.Content.ActualHeight;
            this.width = Application.Current.Host.Content.ActualWidth;
        }

        public void TimeConsumingTaskExecutingEventHandler(TimeConsumingTaskEventArgs args)
        {
            //DispatcherHelper.UIDispatcher.BeginInvoke(() =>
            IsBusy = true;
                //);
            
            //System.Diagnostics.Debug.WriteLine("IsBusy = True");
        }

        public void TimeConsumingTaskCompletedEventHandler(TimeConsumingTaskEventArgs args)
        {
            //DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                IsBusy = false;
                //);

            //System.Diagnostics.Debug.WriteLine("IsBusy = False");
        }

        #region Commands

            /// <summary>
            /// Gets a command object that handles the SideBar being collapsed
            /// </summary>
            public ICommand SideBarCollapsed
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        // Change the user preference to reflect the current state of the SideBar
                        IsSidebarCollapsed = true;
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the SideBar being expanded
            /// </summary>
            public ICommand SideBarExpanded
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        // Change the user preference to reflect the current state of the SideBar
                        IsSidebarCollapsed = false;
                    });
                }
            }

        #endregion
    }
}