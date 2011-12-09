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
using System.Windows.Input;
using Berico.Common.Events;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Berico.SnagL.Infrastructure.Clustering;
using System.Collections.Generic;
using System.Linq;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Represents a control that allows the user to specify a value
    /// (a weight) for a given attribute
    /// </summary>
    public class AttributeSimilarityPanelViewModel : ViewModelBase
    {
        private const string SIMILARITY_TOOLTIP_PREFIX = "Perform clustering based on a(n) ";
        private const string SIMILARITY_TOOLTIP_SUFFIX = " similarity measure";

        private string attributeName = string.Empty;
        private double attributeWeight = 0;
        private double currentValue = 100;
        private double previousValue = 0;
        private string currentState = string.Empty;
        private string similarityToolTip = string.Empty;
        private bool useExactSimilarity = false;
        private bool isPartOfCurrentClustering = false;
        private Modularity.ToolPanel.ClusteringToolPanelItemExtensionViewModel parent = null;

        /// <summary>
        /// Indicates that the attribute weight value has been changed
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs<double>> AttributeWeightChanged;

        /// <summary>
        /// Initializes a new instance of the AttributeSimilarityPanelViewModel class
        /// </summary>
        public AttributeSimilarityPanelViewModel(string _attributeName, Modularity.ToolPanel.ClusteringToolPanelItemExtensionViewModel _parent)
        {
            this.parent = _parent;
            this.previousValue = this.currentValue;
            this.attributeWeight = this.currentValue;

            AttributeName = _attributeName;
            UseExactSimilarity = true;
            SimilarityToolTip = Clustering.AttributeSimilarityManager.Instance.GetDefaultSimilarityMeasure(AttributeName).Name;

            // Wire up event handlers
            this.parent.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(parent_PropertyChanged);
            this.parent.ClusteringFinished += new EventHandler<ClusteringEventArgs>(parent_ClusteringFinished);

            // TODO:  REMOVE HANDLER WHEN THIS CONTROL IS REMOVED FROM PARENT
        }

        #region Properties

            /// <summary>
            /// Gets or sets the weight value for this panel
            /// </summary>
            public double AttributeWeight
            {
                get { return this.attributeWeight; }
                private set
                {
                    OnAttributeWeightChanged(new PropertyChangedEventArgs<double>("AttributeWeight", value, this.attributeWeight));
                    this.attributeWeight = value;
                }
            }

            /// <summary>
            /// Gets or sets the current Value of this panel
            /// </summary>
            public double Value
            {
                get { return this.currentValue; }
                private set
                {
                    this.currentValue = value;
                    RaisePropertyChanged("Value");
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
                    this.similarityToolTip = SIMILARITY_TOOLTIP_PREFIX + value + SIMILARITY_TOOLTIP_SUFFIX;
                    RaisePropertyChanged("SimilarityToolTip");
                }
            }

            /// <summary>
            /// Gets the parent ClusteringToolPanelItemExtensionViewModel for this control
            /// </summary>
            public Modularity.ToolPanel.ClusteringToolPanelItemExtensionViewModel Parent
            {
                get { return this.parent; }
            }

            /// <summary>
            /// Gets the current state (Simple or Advanced mode) of this panel
            /// </summary>
            public string CurrentState
            {
                get { return currentState; }
                private set
                {
                    this.currentState = value;
                    RaisePropertyChanged("CurrentState");
                }
            }

            /// <summary>
            /// Get or set the name of the attribute associated
            /// with this control
            /// </summary>
            public string AttributeName
            {
                get { return this.attributeName; }
                set
                {
                    this.attributeName = value;
                    RaisePropertyChanged("AttributeName");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool IsActive { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public bool IsPartOfPublicClustering { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public bool UseExactSimilarity
            {
                get { return useExactSimilarity; }
                set
                {
                    this.useExactSimilarity = value;
                    RaisePropertyChanged("UseExactSimilarity");

                    if (value)
                        Clustering.AttributeSimilarityManager.Instance.AddExactMatchAttribute(AttributeName);
                    else
                        Clustering.AttributeSimilarityManager.Instance.RemoveExactMatchAttribute(AttributeName);

                    //if (parent != null)
                    //{
                    //    if (!value && this.isPartOfCurrentClustering)
                    //        IsSimilaritySliderEnabled = true;
                    //    else
                    //        IsSimilaritySliderEnabled = false;

                    //}
                }
            }

        #endregion

        #region Commands

            /// <summary>
            /// Gets a command object that handles the Expanded event
            /// </summary>
            public ICommand ExpandedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        // When the panel is opened, we consider it enabled.  This
                        // means we must ensure that the slider is set back to any
                        // previous values.  A new slider defaults to 100%.
                        AttributeWeight = this.previousValue;
                        //this.parent.NumberOfOpenPanels++;
                        IsActive = true;
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the Collapsed event
            /// </summary>
            public ICommand CollapsedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        // When the panel closes, we consider it disabled.  This
                        // means that we want to set this panels weight to 0 since
                        // it is not currently selected.  We also need to save
                        // the current selected weight value.
                        this.previousValue = AttributeWeight;
                        //this.parent.NumberOfOpenPanels--;
                        AttributeWeight = 0;
                        IsActive = false;
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the ValueChanged event
            /// for the weight slider
            /// </summary>
            public ICommand WeightChangedCommand
            {
                get
                {
                    return new RelayCommand<System.Windows.RoutedPropertyChangedEventArgs<double>>(e =>
                    {
                        // The attribute weight value should reflect the
                        // slider's value
                        AttributeWeight = e.NewValue;
                    });
                }
            }

        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// Fires the AttributeWeightChanged event
            /// </summary>
            /// <param name="args">Arguments for the event</param>
            public virtual void OnAttributeWeightChanged(PropertyChangedEventArgs<double> args)
            {
                if (AttributeWeightChanged != null)
                {
                    AttributeWeightChanged(this, args);
                }
            }

            /// <summary>
            /// Handles the PropertyChanged event of the Parent control
            /// </summary>
            /// <param name="sender">The ClusteringToolPanelExtensionViewModel whose property changed</param>
            /// <param name="e">Arguments for the event</param>
            private void parent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                // Check if the "Mode" property was changed
                if (e.PropertyName == "Mode")
                {
                    //Change the controls state
                    if (parent.Mode == ClusteringToolMode.Simple)
                    {
                        if (CurrentState != "Simple")
                        {
                            CurrentState = "Simple";
                            UpdateAttributeWeights(ClusteringToolMode.Simple);
                        }
                    }
                    else if (parent.Mode == ClusteringToolMode.Advanced)
                    {
                        if (CurrentState != "Advanced")
                        {
                            CurrentState = "Advanced";
                            UpdateAttributeWeights(ClusteringToolMode.Advanced);
                        }
                    }
                    else
                    {
                        // Should never happen 
                    }
                }
            }

            /// <summary>
            /// Handles the ClusteringFinished event of the Parent control
            /// </summary>
            /// <param name="sender">The ClusteringToolPanelExtensionViewModel that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void parent_ClusteringFinished(object sender, ClusteringEventArgs e)
            {
                if (IsActive)
                    this.isPartOfCurrentClustering = true;
            }

        #endregion

        /// <summary>
        /// Updates the weight values, stored in the AttributeSimilarityManager, depending
        /// on the current mode of the parent clusterting tool
        /// </summary>
        private void UpdateAttributeWeights(ClusteringToolMode currentMode)
        {
            // Ensure this panel is active
            if (IsActive)
            {
                // Check if the tool is in 'Simple' mode
                if (currentMode == ClusteringToolMode.Simple)
                {
                    // Set the weight for this attribute to the default
                    // value of 100%
                    Clustering.AttributeSimilarityManager.Instance.SetAttributeWeight(this.AttributeName, 100d / 100d);
                } // Check if the tool is in 'Advanced' mode
                else if (currentMode == ClusteringToolMode.Advanced)
                {
                    // Set the weight for this attribute to the user-
                    // defined attribute weight value
                    Clustering.AttributeSimilarityManager.Instance.SetAttributeWeight(this.AttributeName, this.attributeWeight / 100d);
                }
            }
        }

    }
}