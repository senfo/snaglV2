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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Berico.SnagL.Infrastructure.Data.Searching;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Berico.Common.Events;
using System.Linq;
using Berico.SnagL.Infrastructure.Data.Attributes;
using System.Collections.Specialized;
using System.Collections.Generic;
using Berico.Common;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Provides the functionality for the AdvancedSerach control.  The
    /// ViewModelLocator creates an instance of this class which is 
    /// referenced by the control itself and allows binding.
    /// </summary>
    public class AdvancedSearchViewModel : ViewModelBase
    {
        private ObservableCollection<SearchCriterionViewModel> searchCriteria = new ObservableCollection<SearchCriterionViewModel>();
        private ObservableCollection<string> attributes = new ObservableCollection<string>();
        private ObservableCollection<string> operators = new ObservableCollection<string>();
        private SearchCriterionViewModel selectedCriterion = null;
        private string scope = string.Empty;

        /// <summary>
        /// Initializes a new instance of the AdvancedSearchViewModel class
        /// </summary>
        public AdvancedSearchViewModel()
        {
            scope = Data.GraphManager.Instance.DefaultGraphComponentsInstance.Scope;

            // Setup a listener for the AttributeListUpdated event
            Data.Attributes.GlobalAttributeCollection.GetInstance(this.scope).AttributeListUpdated += new EventHandler<Data.Attributes.AttributeEventArgs>(GlobalAttributeCollection_AttributeListUpdated);
            
            // Get a list of all the attributes
            Attributes = new ObservableCollection<string>(Data.Attributes.GlobalAttributeCollection.GetInstance(scope).GetAttributes().Where(attribute => attribute.Visible).Select(attribute => attribute.Name));

            // Get a list of all valid operators
            Operators = new ObservableCollection<string>(ExtensionMethods.GetNames(typeof(SearchOperator)));
            
            // Make sure that we have one criterion and that it is disabled 
            // and inactive.
            SearchCriterionViewModel newSearchCriterion = new SearchCriterionViewModel(this) { IsEnabled = true, IsActive = false };
            SearchCriteria.Add(newSearchCriterion);

            // Wire up event handlers for the Activated and Deactivated events of
            // the newly created SearchCriterion control
            newSearchCriterion.Activated += new EventHandler<EventArgs>(SearchCriterion_Activated);
            newSearchCriterion.Deactivated += new EventHandler<EventArgs>(SearchCriterion_Deactivated);
        }

        /// <summary>
        /// Gets or sets the currently selected SearchCriterionViewModel
        /// </summary>
        public SearchCriterionViewModel SelectedCriterion
        {
            get { return selectedCriterion; }
            set
            {
                selectedCriterion = value;
                RaisePropertyChanged("SelectedCriterion");
            }
        }

        /// <summary>
        /// Gets a collection of SearchCriterion controls
        /// </summary>
        public ObservableCollection<SearchCriterionViewModel> SearchCriteria
        {
            get { return this.searchCriteria; }
            private set { this.searchCriteria = value; }
        }

        /// <summary>
        /// Gets the collection of visible attributes
        /// </summary>
        public ObservableCollection<string> Attributes
        {
            get { return this.attributes; }
            private set { this.attributes = value; }
        }

        /// <summary>
        /// Gets the collection of search operators
        /// </summary>
        public ObservableCollection<string> Operators
        {
            get { return this.operators; }
            private set { this.operators = value; }
        }

        #region Events and Event Handlers

            /// <summary>
            /// Handles the AttributeListUpdated event, which is fired anytime
            /// the GlobalAttributeCollection changes.
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The attributes associated with the event</param>
            private void GlobalAttributeCollection_AttributeListUpdated(object sender, AttributeEventArgs e)
            {
                // We are only concerned about Visible attributes
                if (e.Attribute != null && !e.Attribute.Visible)
                    return;

                // Ensure changes to the global attribute collection 
                // are reflected by the internal attribute list
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    this.attributes.Add(e.Attribute.Name);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    this.attributes.Remove(e.Attribute.Name);
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    // Not something we need to handle here
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    // The global collection was cleared so we need to clear
                    // the local managed list
                    this.attributes.Clear();
                }
                else
                {
                    // We shouldn't get here unless the action was incorrectly
                    // specified for the event
                    throw new ArgumentException("No valid action was specified for the AttributeListUpdated event", "Action");
                }
            }

            /// <summary>
            /// Handles the Deactivated event for any SearchCriterion control.  This event
            /// is fired when a SearchControl is made inactive, which means it should be
            /// removed.
            /// </summary>
            /// <param name="sender">The specific SearchCriterion instance that fired the event</param>
            /// <param name="e">Event arguments for the event</param>
            private void SearchCriterion_Deactivated(object sender, EventArgs e)
            {
                SearchCriterionViewModel criterion = sender as SearchCriterionViewModel;

                // Check if we have more than one SearchCriterion controls
                if (SearchCriteria.Count > 1)
                {
                    // Remove the SearchCriterion from the collection
                    SearchCriteria.Remove(criterion);

                    // Remove event handlers
                    criterion.Activated -= new EventHandler<EventArgs>(SearchCriterion_Activated);
                    criterion.Deactivated -= new EventHandler<EventArgs>(SearchCriterion_Deactivated);
                }
                else
                {
                    // Set the IsActive property to false which will trigger the
                    // underlying StoryBoard to disable the input controls.
                    criterion.IsActive = false;
                }
            }

            /// <summary>
            /// Handles the Activated event for any SearchCriterion control.  This event
            /// is fired when a SearchCriterion is made active, which mean that the input
            /// controls should be activated and a new (inactive) control should be created.
            /// However, if a control is not valid, a new control can't be added.
            /// </summary>
            /// <param name="sender">The specific SearchCriterion instance that fired the event</param>
            /// <param name="e">Event arguments for the event</param>
            private void SearchCriterion_Activated(object sender, EventArgs e)
            {
                bool addNew = true;
                SearchCriterionViewModel sourceCriterion = sender as SearchCriterionViewModel;

                // Check how many SearchCriterion controls we currently have
                if (SearchCriteria.Count > 1)
                {
                    // Loop through all SearchCriterion view models in our list
                    foreach (SearchCriterionViewModel criterion in SearchCriteria)
                    {
                        // If this criterion control is the one that fired the Activate
                        // event, just skip it
                        if (criterion == sourceCriterion) continue;

                        // Check if the SearchCriterion is currently active
                        if (criterion.IsActive)
                        {
                            // Validate the Search Criterion
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

                // Check if we are actually adding a new SearchCritrion control
                if (addNew)
                {
                    // Set the IsActive property to true which will trigger the
                    // underlying StoryBoard to enable the input controls.
                    sourceCriterion.IsActive = true;

                    // Make sure that we have one criterion and that it is disabled 
                    // and inactive.
                    SearchCriterionViewModel newSearchCriterion = new SearchCriterionViewModel(this) { IsEnabled = true, IsActive = false };
                    SearchCriteria.Add(newSearchCriterion);
                    SelectedCriterion = sourceCriterion;

                    // Wire up event handlers for the Activated and Deactivated events of
                    // the newly created SearchCriterion control
                    newSearchCriterion.Activated += new EventHandler<EventArgs>(SearchCriterion_Activated);
                    newSearchCriterion.Deactivated += new EventHandler<EventArgs>(SearchCriterion_Deactivated);

                }
                else
                    sourceCriterion.IsActive = false;

            }

        #endregion

        /// <summary>
        /// Returns a collection of SearchDescriptor instances for every
        /// valid SearchCriterion in the SearchCriteria collection
        /// </summary>
        public List<SearchDescriptor> GetSearchDescriptors()
        {
            List<SearchDescriptor> searchDescriptors = new List<SearchDescriptor>();

            // Loop over all the SearcCriteria
            foreach (SearchCriterionViewModel searchCriterionVM in SearchCriteria)
            {
                if (searchCriterionVM.IsActive && searchCriterionVM.IsEnabled)
                {
                    // Get the vlaue for the current criterion
                    SearchDescriptor descriptor = searchCriterionVM.GetSearchDescriptor();
                    
                    // If the value is not null, add it to our collection
                    if (descriptor != null)
                        searchDescriptors.Add(descriptor);
                }
            }

            // Return all valid criterion values
            return searchDescriptors;
        }

    }
}
