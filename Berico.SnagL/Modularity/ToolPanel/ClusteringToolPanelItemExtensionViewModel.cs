﻿//-------------------------------------------------------------
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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Clustering;
using Berico.SnagL.Infrastructure.Controls;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Modularity.ToolPanel
{
    /// <summary>
    /// Represents the view model for the clustering tool panel
    /// </summary>
    [PartMetadata("ID", "ToolPanelItemViewModelExtension"), Export(typeof(ClusteringToolPanelItemExtensionViewModel))]
    public class  ClusteringToolPanelItemExtensionViewModel : ViewModelBase, IToolPanelItemViewModelExtension
    {

        private string toolName = string.Empty;
        private string description = string.Empty;
        private int index = 0;
        private bool isEnabled = false;
        private SimilarityClustering similarityClustering = null;
        private ClusteringToolMode mode = ClusteringToolMode.Simple;
        private ObservableCollection<AttributeSimilarityCriterionViewModel> similarityCriteria = new ObservableCollection<AttributeSimilarityCriterionViewModel>();
        private IList<double> thresholdValues = new List<double>();
        private double numberOfThresholds = 2;
        private bool isThresholdSliderEnabled = false;
        private double similarityThresholdValue = 2;
        private AttributeSimilarityCriterionViewModel selectedCriterion = null;
        private ObservableCollection<string> attributes = new ObservableCollection<string>();
        private bool clusteringEnabled = false;

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.ClusteringToolPanelItemExtensionViewModel
        /// </summary>
        public ClusteringToolPanelItemExtensionViewModel()
        {
            this.Index = 0;
            this.Description = "This tool provides clustering functionality";
            this.ToolName = "Clustering";
            this.similarityClustering = new SimilarityClustering();

            LoadAttributes();
            SetupCriteria();

            SnaglEventAggregator.DefaultInstance.GetEvent<Clustering.ClusteringCompletedEvent>().Subscribe(ClusteringCompletedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<Graph.Events.DataLoadedEvent>().Subscribe(DataLoadedEventHandler, false);
            AttributeSimilarityManager.Instance.ManagedAttributesChanged += new System.EventHandler<System.Collections.Specialized.NotifyCollectionChangedEventArgs>(Instance_ManagedAttributesChanged);
        }

        #region Properties

            /// <summary>
            /// Gets or sets the mode that the clustering tool is currently
            /// operating in
            /// </summary>
            public ClusteringToolMode Mode
            {
                get { return this.mode; }
                private set
                {
                    this.mode = value;
                    RaisePropertyChanged("Mode");
                }
            }

            /// <summary>
            /// Gets or sets whether clustering is enabled or not.  This
            /// is bound to the IsEnabled property of the clustering
            /// button on the view.
            /// </summary>
            public bool ClusteringEnabled
            {
                get { return this.clusteringEnabled; }
                private set
                {
                    this.clusteringEnabled = value;
                    RaisePropertyChanged("ClusteringEnabled");
                }
            }

            /// <summary>
            /// Gets or sets the AttributeSimilarityCriterionViewModel instances
            /// </summary>
            public ObservableCollection<AttributeSimilarityCriterionViewModel> SimilarityCriteria
            {
                get { return this.similarityCriteria; }
                private set { this.similarityCriteria = value; }
            }

            /// <summary>
            /// Gets the collection of visible attributes
            /// </summary>
            public ObservableCollection<string> Attributes
            {
                get { return this.attributes; }
                set { this.attributes = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool IsActive { get; private set; }

            /// <summary>
            /// Gets or sets
            /// </summary>
            public bool IsThresholdSliderEnabled
            {
                get { return this.isThresholdSliderEnabled; }
                private set
                {
                    this.isThresholdSliderEnabled = value;
                    RaisePropertyChanged("IsThresholdSliderEnabled");
                }
            }

            /// <summary>
            /// Gets or sets the number of threshold values
            /// </summary>
            public double NumberOfThresholds
            {
                get { return numberOfThresholds; }
                set
                {
                    numberOfThresholds = value;
                    RaisePropertyChanged("NumberOfThresholds");
                }
            }

            /// <summary>
            /// Gets or sets the currently selected AttributeSimilarityCriterionViewModel
            /// </summary>
            public AttributeSimilarityCriterionViewModel SelectedCriterion
            {
                get { return selectedCriterion; }
                set
                {
                    selectedCriterion = value;
                    RaisePropertyChanged("SelectedCriterion");
                }
            }

            /// <summary>
            /// Gets or sets the value from the threshold slider
            /// </summary>
            public double SimilarityThresholdValue
            {
                get { return this.similarityThresholdValue; }
                set
                {
                    this.similarityThresholdValue = value;
                    RaisePropertyChanged("SimilarityThresholdValue");
                }
            }

        #endregion

        /// <summary>
        /// Setup the criteria collection and the initial
        /// Criterion control
        /// </summary>
        private void SetupCriteria()
        {
            // Create initial clustering criterion and add it to the collection
            AttributeSimilarityCriterionViewModel newClusteringCriterion = new AttributeSimilarityCriterionViewModel(this) { IsEnabled = true, IsActive = false, IsSelected = true };
            SimilarityCriteria.Add(newClusteringCriterion);

            // Wire up event handlers for the Activated and Deactivated events of
            // the newly created clusdtering criterion control
            newClusteringCriterion.Activated += new EventHandler<EventArgs>(AttributeSimilarityCriterion_Activated);
            newClusteringCriterion.Deactivated += new EventHandler<EventArgs>(AttributeSimilarityCriterion_Deactivated);
        }

        /// <summary>
        /// Configures and adds new similarity panels to the control
        /// </summary>
        private void LoadAttributes()
        {
            // Clear the attributes collection
            Attributes.Clear();

            // Loop over the ManagedAttributes, adding each attribute
            // to the bound collection
            foreach (string attribute in AttributeSimilarityManager.Instance.ManagedAttributes)
            {
                Attributes.Add(attribute);
            }
        }

        /// <summary>
        /// Executes the clustering operation
        /// </summary>
        private void RunClustering()
        {
            RunClustering(double.NaN);
        }

        /// <summary>
        /// Executes the clustering operation using the
        /// propvided threshold value
        /// </summary>
        private void RunClustering(double threshold)
        {
            List<AttributeSimilarityDescriptor> descriptors = new List<AttributeSimilarityDescriptor>();

            // Get all similarity descriptors
            foreach (AttributeSimilarityCriterionViewModel criterionVM in SimilarityCriteria)
            {
                AttributeSimilarityDescriptor newDescriptor = criterionVM.GetSimilarityDescriptor();

                // Ensure that a new descriptor was returned
                if (newDescriptor != null)
                    descriptors.Add(newDescriptor);
            }

            // Ensure that we have some descriptors
            if (descriptors.Count > 0)
            {
                OnClusteringStarted(EventArgs.Empty);
                similarityClustering.PerformClustering(threshold, descriptors);
            }
        }

        /// <summary>
        /// Clears the current clustering operation
        /// </summary>
        private void ClearClustering()
        {
            similarityClustering.RemoveClustering();
            similarityThresholdValue = 0;
            IsThresholdSliderEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool AreAnyCriteriaDirty()
        {
            return SimilarityCriteria.Any(criterionVM => criterionVM.IsDirty);
        }

        #region Commands

            /// <summary>
            /// Gets a command object that performs clustering
            /// </summary>
            public ICommand ClusterCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        // Check if a cluster is currently active
                        if (IsActive)
                        {
                            // Check if we have any SimilarityCriteria.  Remember
                            // that there will always be one.
                            if (SimilarityCriteria.Count == 1)
                            {
                                // Turn clustering off
                                ClearClustering();
                            }
                            else // Reset the clustering
                            {
                                bool isDirty = AreAnyCriteriaDirty();

                                // Check if any criterion are dirty
                                if (isDirty)
                                {
                                    // Run clustering and ignore any set threshold
                                    // because the criterion are dirty
                                    RunClustering();
                                }
                                else
                                {
                                    // Determine if the threshold slider is enabled
                                    if (IsThresholdSliderEnabled)
                                    {
                                        // Run clustering using the set threshold value
                                        RunClustering(thresholdValues[(int)(SimilarityThresholdValue - 1)]);
                                    }
                                    else
                                        RunClustering();
                                }
                            }
                        }
                        else // No current clustering is active
                        {
                            RunClustering();
                        }
                    });
                }
            }

            /// <summary>
            /// Gets the command associated with
            /// the user clicking the "Simple" radio button
            /// </summary>
            public ICommand SimpleSelectedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        if (Mode != ClusteringToolMode.Simple)
                            Mode = ClusteringToolMode.Simple;
                    });
                }
            }

            /// <summary>
            /// Gets the command associated with
            /// the user clicking the "Advanced" radio button
            /// </summary>
            public ICommand AdvancedSelectedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        if (Mode != ClusteringToolMode.Advanced)
                            Mode = ClusteringToolMode.Advanced;
                    });
                }
            }

            /// <summary>
            /// Gets the command related to the user selecting a criterion
            /// from the criteria control list
            /// </summary>
            public ICommand SelectionChangedCommand
            {
                get
                {
                    return new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>((e) =>
                    {
                        AttributeSimilarityCriterionViewModel attributeSimilarityCriterionVM = null;
                        bool shouldUnselect = true;

                        // Check if any items are selected
                        if (e.AddedItems.Count > 0)
                        {
                            // We only allow for a single selection so we can
                            // just grab the first item
                            attributeSimilarityCriterionVM = e.AddedItems[0] as AttributeSimilarityCriterionViewModel;

                            // Check if the selected item is not active.  This will be the
                            // 'ADD' item and we don't want to flag it as selected
                            if (attributeSimilarityCriterionVM.IsActive == true)
                                attributeSimilarityCriterionVM.IsSelected = true;
                            else
                            {
                                shouldUnselect = false;
                                attributeSimilarityCriterionVM.IsSelected = false;
                            }
                        }

                        // Check if any items were unselected
                        if (e.RemovedItems.Count > 0)
                        {
                            // We only allow for a single selection so we can
                            // just grab the first item
                            attributeSimilarityCriterionVM = e.RemovedItems[0] as AttributeSimilarityCriterionViewModel;

                            if (shouldUnselect)
                                attributeSimilarityCriterionVM.IsSelected = false;
                        }
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the ValueChanged event
            /// for the fuuzy slider
            /// </summary>
            public ICommand ThresholdChangedCommand
            {
                get
                {
                    return new RelayCommand<System.Windows.RoutedPropertyChangedEventArgs<double>>(e =>
                    {
                        //similarityThresholdValue = e.NewValue;
                    });
                }
            }

        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// Handles the Deactivated event for any AttributeSimilarityCriterion control.  This event
            /// is fired when an AttributeSimilarityCriterion is made inactive, which means it should be
            /// removed.
            /// </summary>
            /// <param name="sender">The specific AttributeSimilarityCriterion instance that fired the event</param>
            /// <param name="e">Event arguments for the event</param>
            private void AttributeSimilarityCriterion_Deactivated(object sender, EventArgs e)
            {
                AttributeSimilarityCriterionViewModel criterion = sender as AttributeSimilarityCriterionViewModel;

                // Check if we have more than one AttributeSimilarityCriterion control
                if (SimilarityCriteria.Count > 1)
                {
                    // Remove the AttributeSimilarityCriterion from the collection
                    SimilarityCriteria.Remove(criterion);

                    // Remove event handlers
                    criterion.Activated -= new EventHandler<EventArgs>(AttributeSimilarityCriterion_Activated);
                    criterion.Deactivated -= new EventHandler<EventArgs>(AttributeSimilarityCriterion_Deactivated);
                }
                else
                {
                    // Disable clustering
                    ClusteringEnabled = false;

                    // Set the IsActive property to false which will trigger the
                    // underlying StoryBoard to disable the input controls.
                    criterion.IsActive = false;
                }
            }

            /// <summary>
            /// Handles the Activated event for any AttributeSimilarityCriterion control.  This event
            /// is fired when a AttributeSimilarityCriterion is made active, which mean that the input
            /// controls should be activated and a new (inactive) control should be created.
            /// However, if a control is not valid, a new control can't be added.
            /// </summary>
            /// <param name="sender">The specific AttributeSimilarityCriterion instance that fired the event</param>
            /// <param name="e">Event arguments for the event</param>
            private void AttributeSimilarityCriterion_Activated(object sender, EventArgs e)
            {
                bool addNew = true;
                AttributeSimilarityCriterionViewModel sourceCriterion = sender as AttributeSimilarityCriterionViewModel;

                // Check how many AttributeSimilarityCriterion controls we currently have
                if (SimilarityCriteria.Count > 1)
                {
                    // Loop through all AttributeSimilarityCriterion view models in our list
                    foreach (AttributeSimilarityCriterionViewModel criterion in SimilarityCriteria)
                    {
                        // If this criterion control is the one that fired the Activate
                        // event, just skip it
                        if (criterion == sourceCriterion) continue;

                        // Check if the AttributeSimilarityCriterion is currently active
                        if (criterion.IsActive)
                        {
                            // Validate the Criterion
                            criterion.Validate();

                            if (!criterion.IsValid)
                            {
                                // Set a flag that we don't actually want to create
                                // a new Criterion because this one was invalid.
                                addNew = false;
                                SelectedCriterion = criterion;
                            }
                        }
                    }
                }

                // Check if we are actually adding a new AttributeSimilarityCriterion control
                if (addNew)
                {
                    // Set the IsActive property to true which will trigger the
                    // underlying StoryBoard to enable the input controls.
                    sourceCriterion.IsActive = true;
                    sourceCriterion.IsSelected = true;

                    // Add the new AttributeSimilarityCriterion control
                    SetupCriteria();

                    ClusteringEnabled = true;
                }
                else
                    sourceCriterion.IsActive = false;

            }

            /// <summary>
            /// Handles the ManagedAttributesChanged event
            /// </summary>
            /// <param name="sender">The Attribute that has changed</param>
            /// <param name="e">The arguments for the event</param>
            private void Instance_ManagedAttributesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                LoadAttributes();
            }

            public event EventHandler<EventArgs> ClusteringStarted;
            public event EventHandler<ClusteringEventArgs> ClusteringFinished;
        
            /// <summary>
            /// Handles the ClusteringCompleted event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void ClusteringCompletedEventHandler(ClusteringCompletedEventArgs args)
            {
                IsActive = true;
                OnClusteringFinished(new ClusteringEventArgs(AttributeSimilarityManager.Instance.GetAttributesWithWeights(), args.SimilarityValues));

                // Ensure that clustering has been performed and that
                // Exact (if in simple mode) is not selected
                if (IsActive)
                {
                    // Save thresholds
                    if (args.SimilarityValues != null)
                    {
                        thresholdValues = args.SimilarityValues.OrderBy(value => value).ToList<double>();

                        // Check if we have more than one value returned
                        if (thresholdValues.Count > 1)
                        {
                            NumberOfThresholds = thresholdValues.Count;

                            // Update the currently set threshold value if no
                            // threshold was previously used
                            if (double.IsNaN(args.ThresholdUsed))
                                SimilarityThresholdValue = thresholdValues.Count;

                            IsThresholdSliderEnabled = true;
                        }
                        else
                            IsThresholdSliderEnabled = false;

                        // TODO:  NEED TO KNOW IF MULTIPLE ATTRIBUTES WERE SELECTED AND IF THEY WERE BOTH SET TO EXACT OR NOT
                    }
                }
                else
                {
                    thresholdValues.Clear();
                    IsThresholdSliderEnabled = false;
                }
            }

            /// <summary>
            /// Raises the ClusteringFinished event
            /// </summary>
            /// <param name="e">The event agruments</param>
            protected virtual void OnClusteringFinished(ClusteringEventArgs e)
            {
                EventHandler<ClusteringEventArgs> handler = this.ClusteringFinished;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Raises the ClusteringStarted event
            /// </summary>
            /// <param name="e">The event agruments</param>
            protected virtual void OnClusteringStarted(EventArgs e)
            {
                EventHandler<EventArgs> handler = this.ClusteringStarted;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            public void DataLoadedEventHandler(DataLoadedEventArgs args)
            {
                Graph.GraphComponents graph = Berico.SnagL.Infrastructure.Data.GraphManager.Instance.GetGraphComponents(args.Scope);

                if (graph != null && graph.Data.Order > 0)
                {
                    this.IsEnabled = true;
                }
                else
                    this.IsEnabled = false;
            }

        #endregion

        #region IToolPanelItemViewModelExtension Members

            public int Index
            {
                get { return this.index; }
                set
                {
                    this.index = value;
                }
            }

            public string Description
            {
                get { return this.description; }
                set
                {
                    this.description = value;
                    RaisePropertyChanged("Description");
                }
            }

            public string ToolName
            {
                get { return this.toolName; }
                set
                {
                    this.toolName = value;
                    RaisePropertyChanged("ToolName");
                }
            }

            public bool IsEnabled
            {
                get { return this.isEnabled; }
                set
                {
                    this.isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }

        #endregion
    }
}