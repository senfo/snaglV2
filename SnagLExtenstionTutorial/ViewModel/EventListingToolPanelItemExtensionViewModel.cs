using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;

namespace SnagLExtenstionTutorial.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    [PartMetadata("ID", "ToolPanelItemViewModelExtension"), Export(typeof(EventListingToolPanelItemExtensionViewModel))]
    public class EventListingToolPanelItemExtensionViewModel : ViewModelBase, IToolPanelItemViewModelExtension
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

        /// <summary>
        /// Stores a collection of events that occur in SnagL
        /// </summary>
        private ObservableCollection<string> _events = new ObservableCollection<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EventListingToolPanelItemExtensionViewModel class.
        /// </summary>
        public EventListingToolPanelItemExtensionViewModel()
        {
            this.Index = 4;
            this.Description = "Example ToolPanel extension";
            this.ToolName = "Event Lister";

            // Subscribe to some events
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeftButtonUpEvent>().Subscribe(OnNodeMouseLeftButtonUp, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeftButtonDownEvent>().Subscribe(OnNodeMouseLeftButtonDown, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseEnterEvent>().Subscribe(OnNodeMouseEnter, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeaveEvent>().Subscribe(OnNodeMouseLeave, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseMoveEvent>().Subscribe(OnNodeMouseMove, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.UI.TimeConsumingTaskCompletedEvent>().Subscribe(OnTimeConsumingTaskCompleted, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.UI.TimeConsumingTaskExecutingEvent>().Subscribe(OnTimeConsumingTaskExecuting, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.Infrastructure.Layouts.LayoutExecutedEvent>().Subscribe(OnLayoutExecuted, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.Infrastructure.Layouts.LayoutExecutingEvent>().Subscribe(OnLayoutExecuting, false);
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
            get { return _events; }
            private set { _events = value; }
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

        #region Event Handlers

        /// <summary>
        /// Event handler to handle when a node is clicked
        /// </summary>
        /// <param name="eventArgs">Any event arguments that might be passed</param>
        public void OnNodeMouseLeftButtonUp(NodeViewModelMouseEventArgs<MouseButtonEventArgs> eventArgs)
        {
            Events.Add(string.Format("NodeMouseLeftButtonUp - {0}", eventArgs.NodeViewModel.ParentNode.ID));
        }

        public void OnNodeMouseLeftButtonDown(NodeViewModelMouseEventArgs<MouseButtonEventArgs> eventArgs)
        {
            Events.Add(string.Format("NodeMouseLeftButtonDown - {0}", eventArgs.NodeViewModel.ParentNode.ID));
        }

        public void OnNodeMouseEnter(NodeViewModelMouseEventArgs<MouseEventArgs> eventArgs)
        {
            Events.Add(string.Format("NodeMouseEnter - {0}", eventArgs.NodeViewModel.ParentNode.ID));
        }

        /// <summary>
        /// Event handler to handle when the mouse is no longer hovering over a node
        /// </summary>
        /// <param name="eventArgs">Any event arguments that might be passed</param>
        public void OnNodeMouseLeave(NodeViewModelMouseEventArgs<MouseEventArgs> eventArgs)
        {
            Events.Add(string.Format("NodeMouseLeave - {0}", eventArgs.NodeViewModel.ParentNode.ID));
        }

        public void OnNodeMouseMove(NodeViewModelMouseEventArgs<MouseEventArgs> eventArgs)
        {
            Events.Add(string.Format("NodeMoved - {0}", eventArgs.NodeViewModel.ParentNode.ID));
        }

        public void OnTimeConsumingTaskCompleted(Berico.SnagL.UI.TimeConsumingTaskEventArgs eventArgs)
        {
            Events.Add("Time Consuming task completed");
        }

        public void OnTimeConsumingTaskExecuting(Berico.SnagL.UI.TimeConsumingTaskEventArgs eventArgs)
        {
            Events.Add("Time Consuming task started");
        }

        public void OnLayoutExecuted(Berico.SnagL.Infrastructure.Layouts.LayoutEventArgs eventArgs)
        {
            Events.Add(string.Format("Layout completed - {0}", eventArgs.LayoutName));
        }

        public void OnLayoutExecuting(Berico.SnagL.Infrastructure.Layouts.LayoutEventArgs eventArgs)
        {
            Events.Add(string.Format("Layout started - {0}", eventArgs.LayoutName));
        }

        /// <summary>
        /// Cleans resources used by the SnagLExtenstionTutorial.ViewModel.EventListingToolPanelItemExtensionViewModel
        /// </summary>
        public override void Cleanup()
        {
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeftButtonUpEvent>().Unsubscribe(OnNodeMouseLeftButtonUp);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeftButtonDownEvent>().Unsubscribe(OnNodeMouseLeftButtonDown);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseEnterEvent>().Unsubscribe(OnNodeMouseEnter);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeaveEvent>().Unsubscribe(OnNodeMouseLeave);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseMoveEvent>().Unsubscribe(OnNodeMouseMove);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.UI.TimeConsumingTaskCompletedEvent>().Unsubscribe(OnTimeConsumingTaskCompleted);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.UI.TimeConsumingTaskExecutingEvent>().Unsubscribe(OnTimeConsumingTaskExecuting);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.Infrastructure.Layouts.LayoutExecutedEvent>().Unsubscribe(OnLayoutExecuted);
            SnaglEventAggregator.DefaultInstance.GetEvent<Berico.SnagL.Infrastructure.Layouts.LayoutExecutingEvent>().Unsubscribe(OnLayoutExecuting);

            base.Cleanup();
        }

        #endregion
   }
}