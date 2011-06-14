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
using System.ComponentModel.Composition;
using System.Windows.Input;
using Berico.Common;
using Berico.SnagL.Infrastructure.Configuration;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Modularity.ToolPanel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    [PartMetadata("ID", "ToolPanelItemViewModelExtension"), Export(typeof(LiveDataToolPanelItemExtensionViewModel))]
    public class LiveDataToolPanelItemExtensionViewModel : ViewModelBase, IToolPanelItemViewModelExtension
    {
        #region Fields

        /// <summary>
        /// Stores the name of the LiveEnabled property
        /// </summary>
        private const string _liveEnabled = "LiveEnabled";

        /// <summary>
        /// Stores the visibility state for the play hover button
        /// </summary>
        private string _playHoverVisible = "Collapsed";

        /// <summary>
        /// Stores the visibility state for the pause hover button
        /// </summary>
        private string _pauseHoverVisible = "Collapsed";

        /// <summary>
        /// Stores the status
        /// </summary>
        private string _status;

        /// <summary>
        /// Stores a value that indicates whether or not the control is enabled
        /// </summary>
        private bool isEnabled = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not live data is active
        /// </summary>
        [ExportableProperty("LiveEnabled")]
        public bool LiveEnabled
        {
            get
            {
                return GraphManager.Instance.LiveEnabled;
            }
            set
            {
                GraphManager.Instance.LiveEnabled = value;
                RaisePropertyChanged(_liveEnabled);
            }
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
        /// Gets or sets the description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the tool
        /// </summary>
        public string ToolName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tool panel item is enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Gets or sets whether or not the play hover image is displayed
        /// </summary>
        [ExportableProperty("PlayHoverVisible")]
        public string PlayHoverVisible
        {
            get
            {
                return _playHoverVisible;
            }
            set
            {
                _playHoverVisible = value;
                RaisePropertyChanged("PlayHoverVisible");
            }
        }

        /// <summary>
        /// Gets or sets whether or not the pause hover image is displayed
        /// </summary>
        [ExportableProperty("PauseHoverVisible")]
        public string PauseHoverVisible
        {
            get
            {
                return _pauseHoverVisible;
            }
            set
            {
                _pauseHoverVisible = value;
                RaisePropertyChanged("PauseHoverVisible");
            }
        }

        /// <summary>
        /// Gets or sets the status message displayed
        /// </summary>
        [ExportableProperty("Status")]
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        /// <summary>
        /// Gets a command object that handles the MouseEnter event
        /// </summary>
        public ICommand MouseEnterCommand
        {
            get
            {
                return new RelayCommand<MouseEventArgs>(e =>
                {
                    OnNodeMouseEnter(this, e);
                });
            }
        }

        /// <summary>
        /// Gets a command object that handles the MouseEnter event
        /// </summary>
        public ICommand MouseLeaveCommand
        {
            get
            {
                return new RelayCommand<MouseEventArgs>(e =>
                {
                    OnNodeMouseLeave(this, e);
                });
            }
        }

        /// <summary>
        /// Gets a reference to an ICommand to provide functionality to the view for 
        /// </summary>
        public ICommand ChangeStatus
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!LiveEnabled)
                    {
                        LiveEnabled = true;
                    }
                    else
                    {
                        LiveEnabled = false;
                    }

                    // Update which image is displayed
                    UpdateVisibility();
                });
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveDataToolPanelItemExtensionViewModel"/> class.
        /// </summary>
        public LiveDataToolPanelItemExtensionViewModel()
        {
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataEnqueuedEvent>().Subscribe(LiveDataEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataDequeuedEvent>().Subscribe(LiveDataEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataStartedEvent>().Subscribe(LiveDataStartOrEndedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataEndedEvent>().Subscribe(LiveDataStartOrEndedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataLoadedEvent>().Subscribe(DataLoadedEventHandler, false);

            this.Index = 5;
            this.Description = "This tool provides live data functionality";
            this.IsEnabled = false;
            this.ToolName = "Live Data";
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the visibility status of the hover images
        /// </summary>
        private void UpdateVisibility()
        {
            if (LiveEnabled)
            {
                PauseHoverVisible = "Visible";
                PlayHoverVisible = "Collapsed";
            }
            else
            {
                PlayHoverVisible = "Visible";
                PauseHoverVisible = "Collapsed";
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the MouseEnter event
        /// </summary>
        /// <param name="sender">Whoever raised the event</param>
        /// <param name="e">Any event arguments that might be passed</param>
        private void OnNodeMouseEnter(object sender, MouseEventArgs e)
        {
            UpdateVisibility();
        }

        /// <summary>
        /// Handles the MouseLeave event
        /// </summary>
        /// <param name="sender">Whoever raised the event</param>
        /// <param name="e">Any event arguments that might be passed</param>
        private void OnNodeMouseLeave(object sender, MouseEventArgs e)
        {
            PlayHoverVisible = "Collapsed";
            PauseHoverVisible = "Collapsed";
        }

        /// <summary>
        /// Handles the LiveData event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void LiveDataEventHandler(LiveDataEventArgs args)
        {
            // Ensure that the Live tool panel is enabled
            if (!IsEnabled)
            {
                IsEnabled = true;
            }

            Status = String.Format("{0} item{1}", args.Count, args.Count != 1 ? "s" : String.Empty);
        }

        /// <summary>
        /// Handles the LiveDataStartedEvent and LiveDataEndedEvent events
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void LiveDataStartOrEndedEventHandler(EventArgs args)
        {
            RaisePropertyChanged(_liveEnabled);
        }

        /// <summary>
        /// Handles the DataLoaded event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void DataLoadedEventHandler(DataLoadedEventArgs args)
        {
            this.LiveEnabled = ConfigurationManager.Instance.CurrentConfig.LivePreferences.AutoStart;
        }

        #endregion
    }
}