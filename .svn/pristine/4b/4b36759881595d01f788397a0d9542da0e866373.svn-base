using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Berico.SnagL.Infrastructure.Modularity.ToolPanel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using System.Collections.ObjectModel;
using Berico.SnagL.Infrastructure.Clustering;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Represents a similarity criterion used to build 
    /// instructions for clustering
    /// </summary>
    public class AttributeSimilarityCriterionViewModel : CriterionViewModelBase<ClusteringToolPanelItemExtensionViewModel>
    {
        private bool isSelected = false;
        private string errorMessage = string.Empty;
        private ClusteringToolMode currentMode;
        private string selectedAttribute = string.Empty;
        private ISimilarityMeasure selectedSimilarityMeasure = null;
        private ObservableCollection<ISimilarityMeasure> availableSimilarityMeasures = new ObservableCollection<ISimilarityMeasure>();
        private double weight = 100;
        private bool useExactSimilarity = true;
        private string similarityToolTip = string.Empty;
        private bool isPartOfActiveClustering = false;
        private AttributeSimilarityDescriptor previousDescriptor = null;

        /// <summary>
        /// Creates a new AttributeSimilarityCriterionViewModel instance
        /// using the provided parent instance
        /// </summary>
        /// <param name="_parent">The ClusteringToolPanelItemExtensionViewModel instance
        /// that owns this criterion</param>
        public AttributeSimilarityCriterionViewModel(ClusteringToolPanelItemExtensionViewModel _parent)
            : base(_parent)
        {
            IsValid = true;
            this.currentMode = _parent.Mode;
            _parent.ClusteringFinished += new EventHandler<ClusteringEventArgs>(ClusteringFinishedHandler);
        }

        #region Properties

            /// <summary>
            /// Gets or sets whether this criterion is
            /// currently selected or not
            /// </summary>
            public bool IsSelected
            {
                get { return this.isSelected; }
                set { this.isSelected = value; UpdateState(); }
            }

            /// <summary>
            /// Gets or sets whether this criterion is aprt of 
            /// the currently active cluster
            /// </summary>
            public bool IsPartOfActiveClustering
            {
                get { return this.isPartOfActiveClustering; }
                private set { this.isPartOfActiveClustering = value; }
            }

            /// <summary>
            /// Gets or sets an error message for this criterion.
            /// This value is bound to the validation tooltip.
            /// </summary>
            public string ErrorMessage
            {
                get { return this.errorMessage; }
                set
                {
                    this.errorMessage = value;
                    RaisePropertyChanged("ErrorMessage");
                }
            }

            /// <summary>
            /// Gets or sets the attribute that is currently selected.
            /// This property is bound to the attribute combobox.
            /// </summary>
            public string SelectedAttribute
            {
                get { return this.selectedAttribute; }
                set
                {
                    this.selectedAttribute = value;
                    RaisePropertyChanged("SelectedAttribute");
                }
            }

            /// <summary>
            /// Gets or sets the attribute that is currently selected.
            /// This property is bound to the attribute combobox.
            /// </summary>
            public ISimilarityMeasure SelectedSimilarityMeasure
            {
                get { return this.selectedSimilarityMeasure; }
                set
                {
                    this.selectedSimilarityMeasure = value;
                    RaisePropertyChanged("SelectedSimilarityMeasure");
                }
            }

            /// <summary>
            /// Gets the collection of available similarity measures
            /// </summary>
            public ObservableCollection<ISimilarityMeasure> AvailableSimilarityMeasures
            {
                get { return this.availableSimilarityMeasures; }
                set { this.availableSimilarityMeasures = value; }
            }

            /// <summary>
            /// Gets or sets the current weight for this 
            /// similarity criterion
            /// </summary>
            public double Weight
            {
                get { return this.weight; }
                private set
                {
                    this.weight = value;
                    RaisePropertyChanged("Weight");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string SimilarityToolTip
            {
                get { return this.similarityToolTip; }
                private set
                {
                    this.similarityToolTip = value;
                    RaisePropertyChanged("SimilarityToolTip");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool UseExactSimilarity
            {
                get { return this.useExactSimilarity; }
                set
                {
                    this.useExactSimilarity = value;
                    RaisePropertyChanged("UseExactSimilarity");
                }
            }

            /// <summary>
            /// Determines if this criterion is dirty (new to the currently active
            /// clustering) or false (part of the currently active clustering)
            /// </summary>
            /// <returns>true if this criterion is dirty; otherwise false</returns>
            public bool IsDirty
            {
                get
                {
                    if (!IsActive)
                        return false;

                    AttributeSimilarityDescriptor currentDescriptor = GetSimilarityDescriptor();

                    // Check if this criterion is part of the active clustering
                    // and if the current descriptor matches 
                    if (IsPartOfActiveClustering && (previousDescriptor.Equals(currentDescriptor) && currentDescriptor != null))
                        return false;
                    else
                        return true;
                }
            }

        #endregion

        /// <summary>
        /// Updates the state of the criterion control
        /// </summary>
        private void UpdateState()
        {
            // ENsure the criterion is active
            if (IsActive)
            {
                // Check if the criterion is currently selected
                if (IsSelected)
                {
                    // The state will be different depending on if the 
                    // tool mose is set to Simple or Advanced
                    CurrentState = (currentMode == ClusteringToolMode.Simple ? SimilarityCriterionState.SimpleFocused : SimilarityCriterionState.AdvancedFocused);
                }
                else
                {
                    // The state will be different depending on if the 
                    // tool mose is set to Simple or Advanced
                    CurrentState = (currentMode == ClusteringToolMode.Simple ? SimilarityCriterionState.SimpleUnfocused : SimilarityCriterionState.AdvancedUnfocused);

                    // Since this criterion is being unselected we need to validate it
                    Validate();
                }
            }
        }

        /// <summary>
        /// Validate this criterion control
        /// </summary>
        public void Validate()
        {
            IsValid = true;

            if (IsValid && !IsActive)
                return;

            // Check that an attribute is selected
            if (SelectedAttribute == string.Empty)
            {
                ErrorMessage = "A target attribute must be selected for this criterion";
                IsValid = false;
            }

            // There are additional tests if the control
            // is in adavnced mode
            if (currentMode == ClusteringToolMode.Advanced)
            {
                // Check that a similarity measure was selected
                if (SelectedSimilarityMeasure == null)
                {
                    ErrorMessage = "A similarity measure must be selected for this criterion";
                    IsValid = false;
                }
            }
        }

        /// <summary>
        /// Constructs a valid and appropriate AttributeSimilarityDescriptor
        /// for this criterion.  If this criterion is not valid, null is 
        /// returned.
        /// </summary>
        /// <returns></returns>
        public AttributeSimilarityDescriptor GetSimilarityDescriptor()
        {
            // Validate the control
            Validate();

            // Check if the this AttributeSimilarityCriterionViewModel instance
            // is valid.  Valid means that all required input controls have values.
            if (IsValid && IsActive)
            {
                ISimilarityMeasure selectedMeasure = null;
                double currentWeight = 0.0;

                // Check if the tool is in simple mode or not
                if (currentMode == ClusteringToolMode.Simple)
                {
                    // Check if 'Exact' is selected
                    if (UseExactSimilarity)
                    {
                        // Set the similarity measure to an ExactMatchSimilarityMeasure
                        selectedMeasure = AttributeSimilarityManager.Instance.GetSimilarityMeasureInstance(typeof(Berico.SnagL.Infrastructure.Similarity.ExactMatchSimilarityMeasure).FullName);
                    }
                    else
                    {
                        // Set the similarity measure to the default for
                        // the selected
                        selectedMeasure = AttributeSimilarityManager.Instance.GetDefaultSimilarityMeasure(SelectedAttribute);
                    }

                    // Set current weight to 1 (no weight)
                    currentWeight = 1d;
                }
                else
                {
                    // Set the similarity measure to the currently selected one
                    selectedMeasure = SelectedSimilarityMeasure;

                    // Set the current weight to the weight set on the slider
                    currentWeight = Weight / 100d;
                }

                // Create a new AttributeSimilarityDescriptor instance using
                // the values from the input controls
                return new AttributeSimilarityDescriptor(SelectedAttribute,
                                                         selectedMeasure,
                                                         currentWeight);
            }
            else
            {
                // Return null if this AttributeSimilarityDescriptor instance
                // was invalid
                return null;
            }

        }

        /// <summary>
        /// Handles the add button being clicked
        /// </summary>
        protected override void AddButtonClickedHandler()
        {
            // Validate the currently selected criterion
            //if (Parent.SelectedCriterion != null && Parent.SelectedCriterion.IsActive)
            //    Parent.SelectedCriterion.Validate();

            //if ((Parent.SelectedCriterion != null && Parent.SelectedCriterion.IsValid) || (Parent.SelectedCriterion == null && IsActive == false))
            //{
                // Raise the Activated event.
                OnActivated(EventArgs.Empty);

                // Ensure that the clustering tool knows that
                // this Criterion was selected.  This property
                // is bound to the ListBox to ensure everything
                // behaves appropriately.
                Parent.SelectedCriterion = this;
            //}
        }

        #region Events and Event Handlers

            /// <summary>
            /// Handles property changes in the parent Clustering Tool
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            protected override void ParentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                // Check if the "Mode" property was changed
                if (e.PropertyName == "Mode")
                {
                    //Change the controls state
                    if (Parent.Mode == ClusteringToolMode.Simple && currentMode != ClusteringToolMode.Simple)
                        currentMode = ClusteringToolMode.Simple;
                    else if (Parent.Mode == ClusteringToolMode.Advanced && currentMode != ClusteringToolMode.Advanced)
                    {
                        currentMode = ClusteringToolMode.Advanced;
                    }
                    else // Should never happen
                    { }

                    // Ensure the state is updated
                    UpdateState();
                }
            }

            /// <summary>
            /// Handles the ClusteringFinished event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            private void ClusteringFinishedHandler(object sender, ClusteringEventArgs e)
            {
                if (IsActive && IsValid)
                {
                    IsPartOfActiveClustering = true;
                    previousDescriptor = GetSimilarityDescriptor();
                }
            }

        #endregion

        #region Commands

            /// <summary>
            /// Gets the command that handles the selected item, in the
            /// attribute similarity combobox, changing
            /// </summary>
            public ICommand SelectedAttributeChangedCommand
            {
                get
                {
                    return new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>((e) =>
                    {
                        ISimilarityMeasure defaultSimilarityMeasure = null;

                        // Check if any items are selected
                        if (e.AddedItems.Count > 0)
                        {

                            AvailableSimilarityMeasures.Clear();

                            foreach (ISimilarityMeasure similarityMeasure in AttributeSimilarityManager.Instance.GetValidSimilarityMeasures(e.AddedItems[0] as string))
                            {
                                AvailableSimilarityMeasures.Add(similarityMeasure);
                            }

                            // Get the default similarity measure to be used for this attribute
                            defaultSimilarityMeasure = AttributeSimilarityManager.Instance.GetDefaultSimilarityMeasure(e.AddedItems[0] as string);

                            // Ensure that a default similarity measure was found
                            if (defaultSimilarityMeasure != null)
                            {
                                // Select the item that is the default
                                SelectedSimilarityMeasure = defaultSimilarityMeasure;
                            }
                            else
                            {
                                // Ths shouldn't happen because the GetDefaultSimilarityMeasure
                                // should always return something
                                selectedSimilarityMeasure = AvailableSimilarityMeasures[0];
                            }
                        }

                        // Check if any items were unselected
                        if (e.RemovedItems.Count > 0)
                        {

                        }

                    });
                }
            }

        #endregion
    }
}
