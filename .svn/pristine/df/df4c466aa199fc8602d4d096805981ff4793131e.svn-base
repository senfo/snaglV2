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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Berico.Common.Events;
using Berico.SnagL.Infrastructure.Configuration;
using Berico.SnagL.Infrastructure.Data.Searching;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using BERICO = Berico.Windows.Controls;

namespace Berico.SnagL.Infrastructure.Controls
{
    
    /// <summary>
    /// Represents a control that allows the user
    /// to conduct searches
    /// </summary>
    public class SearchToolViewModel : ViewModelBase
    {

        private BitmapImage captionImage;
        private string searchText = string.Empty;
        private BERICO.MenuItem selectedMenuItem = null;
        private ObservableCollection<BERICO.MenuItem> menuItems = new ObservableCollection<BERICO.MenuItem>();
        private SearchToolMode searchToolMode = SearchToolMode.Unknown;
        private SearchTool searchTool = null;
        private bool isAdvancedSearchVisible = true;
        private AdvancedSearchViewModel advancedOptions = null;

        /// <summary>
        /// Creates a new instance of the SearchToolViewModel class
        /// </summary>
        public SearchToolViewModel()
        {
            BitmapImage searchImage = new BitmapImage(new Uri("/Berico.SnagL;component/Resources/Icons/SnagL/Search.png", UriKind.Relative));
            BitmapImage filterImage = new BitmapImage(new Uri("/Berico.SnagL;component/Resources/Icons/SnagL/Filter.png", UriKind.Relative));
            BERICO.MenuItem menuItem;

            menuItem = new BERICO.MenuItem() { Header = "Search Nodes", Tag = "SEARCH", Icon = new Image() { Source = searchImage, Height = 15, Width = 15, Margin = new Thickness(2) } };
            MenuItems.Add(menuItem);

            this.selectedMenuItem = menuItem;
            this.CaptionImage = searchImage;
            this.searchToolMode = SearchToolMode.Find;

            SnaglEventAggregator.DefaultInstance.GetEvent<SnaglLoadedEvent>().Subscribe(SnaglLoadedEventHandler, false);

            menuItem = new BERICO.MenuItem() { Header = "Filter Nodes", Tag = "FILTER", Icon = new Image() { Source = filterImage, Height = 15, Width = 15, Margin = new Thickness(2) } };
            MenuItems.Add(menuItem);
        }

        #region Properties

            /// <summary>
            /// Gets or sets the Icon that is used as the search button's
            /// caption
            /// </summary>
            public BitmapImage CaptionImage
            {
                get { return this.captionImage; }
                set
                {
                    this.captionImage = value;
                    RaisePropertyChanged("CaptionImage");
                }
            }

            /// <summary>
            /// Gets or sets the text to be searched for.  This text is provided
            /// by the user and bound to the SearchTool view.
            /// </summary>
            public string SearchText
            {
                get { return this.searchText; }
                set
                {
                    this.searchText = value;
                    RaisePropertyChanged("SearchText");
                }
            }

            /// <summary>
            /// Gets or sets whether or not the Advanced Search tool is visible
            /// </summary>
            public bool IsAdvancedSearchVisible
            {
                get { return this.isAdvancedSearchVisible; }
                set
                {
                    this.isAdvancedSearchVisible = value;
                    RaisePropertyChanged("IsAdvancedSearchVisible");
                }
            }

            /// <summary>
            /// Gets or sets the currently selected item.  This value is bound
            /// directly to the view.
            /// </summary>
            public BERICO.MenuItem SelectedItem
            {
                get { return this.selectedMenuItem; }
                set
                {
                    this.selectedMenuItem = value;
                    RaisePropertyChanged("SelectedItem");
                }
            }

            /// <summary>
            /// Gets the SerachToolMode for this control, which indicates
            /// whether the tool will select or filter nodes
            /// </summary>
            public SearchToolMode SearchMode
            {
                get { return this.searchToolMode; }
                set { this.searchToolMode = value; }
            }

            /// <summary>
            /// Gets the collect of menu items that are displayed
            /// in the drop down potion of the SplitButton
            /// </summary>
            public ObservableCollection<BERICO.MenuItem> MenuItems
            {
                get { return this.menuItems; }
            }

        #endregion

        #region Commands

            /// <summary>
            /// Gets the command that should be executed when an item
            /// in the menu is selected
            /// </summary>
            public ICommand SelectionChangedCommand
            {
                get
                {
                    return new RelayCommand<SelectionChangedEventArgs>(e =>
                    {
                        // Cast the object that fired the event
                        BERICO.MenuItem selectedMenuItem = e.AddedItems[0] as BERICO.MenuItem;

                        // Set the image of the search button to the image for
                        // the selected menu item
                        CaptionImage = (selectedMenuItem.Icon as Image).Source as BitmapImage;

                        // Set the tool mode accordingly
                        if (selectedMenuItem.Tag.ToString() == "SEARCH")
                            this.searchToolMode = SearchToolMode.Find;
                        else
                            this.searchToolMode = SearchToolMode.Filter;

                    });
                }
            }

            /// <summary>
            /// Gets the command that represents the KeyDown event of
            /// the search textbox
            /// </summary>
            public ICommand KeyDownCommand
            {
                get
                {
                    return new RelayCommand<KeyEventArgs>(e =>
                    {
                        // Check if the Enter key was pressed
                        if (e.Key == Key.Enter)
                        {
                            // The Text binding isn't updated until the textbox loses focus
                            // so we force the binding to update in order to ensure the 
                            // value used is accurate
                            (e.OriginalSource as Microsoft.Windows.Controls.WatermarkedTextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();

                            // Excute the Button Click command in order to
                            // simulate the button being pressed
                            ButtonClickCommand.Execute(null);
                        }
                        else if (e.Key == Key.Escape)
                        {
                            (e.OriginalSource as Microsoft.Windows.Controls.WatermarkedTextBox).Text = "";
                            (e.OriginalSource as Microsoft.Windows.Controls.WatermarkedTextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();

                            // Excute the Button Click command in order to
                            // simulate the button being pressed
                            ButtonClickCommand.Execute(null);
                        }
                    });
                }
            }

            /// <summary>
            /// Gets the command That handles the event that the
            /// SearchTool has loaded
            /// </summary>
            public ICommand SearchToolLoadedCommand
            {
                get
                {
                    return new RelayCommand<DetailedEventInformation>(e =>
                    {
                        if (searchTool == null)
                        {
                            searchTool = e.Sender as SearchTool;
                            advancedOptions = searchTool.AdvancedSearchControl.DataContext as AdvancedSearchViewModel;
                        }
                    });
                }
            }

            /// <summary>
            /// Gets the command that handles the opening of
            /// the Advanced popup button
            /// </summary>
            public ICommand AdvancedOptionsButtonCheckedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        if (searchTool != null)
                        {
                            searchTool.SearchOptionsPopup.IsOpen = true;
                            searchTool.SearchOptionsPopup.UpdateLayout();

                            // Reposition the popup using the correctly calculated Horizontal and
                            // Vertical offsets
                            searchTool.SearchOptionsPanel.SizeChanged += new SizeChangedEventHandler(SearchOptionsPanel_SizeChanged);
                            searchTool.SearchOptionsPopup.HorizontalOffset = -(Math.Abs(searchTool.SearchControl.ActualWidth - searchTool.SearchOptionsPanel.ActualWidth)) + (searchTool.SearchOptionsButton.ActualWidth + 4);
                            searchTool.SearchOptionsPopup.VerticalOffset = searchTool.ActualHeight;

                            // Stop the animation, if it is running
                            // this.AdvancedOptionsSetAnimation.Stop();
                        }
                    });
                }
            }

            /// <summary>
            /// Gets the command that handles the closing of
            /// the Advanced popup button
            /// </summary>
            public ICommand AdvancedOptionsButtonUncheckedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {

                        if (searchTool != null)
                        {
                            // Close the SearchOptionsPopup, if it is open
                            if (searchTool.SearchOptionsPopup.IsOpen)
                                searchTool.SearchOptionsPopup.IsOpen = false;

                            // If the AdvancedSearch control has any criteria or the options were
                            // changed, we need to start the animation
                            // if ((AdvancedSearchControl.GetCriteria().Count > 0) || (AdvancedSearchControl.MaxNodes != Int32.MaxValue) || (AdvancedSearchControl.ShowNeighbors))
                            //    this.AdvancedOptionsSetAnimation.Begin();
                        }
                    });
                }
            }

            /// <summary>
            /// Gets the command that should be executed when the search button
            /// is clicked
            /// </summary>
            public ICommand ButtonClickCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        PerformAction();
                    });
                }
            }

        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// Handles the SizeChanged event of the SearchOptionsPanel (which
            /// is the portion of the popup that contains the actual advanced
            /// search controls.
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Events for the argument</param>
            private void SearchOptionsPanel_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                // Changed the offset to take into account the size change
                searchTool.SearchOptionsPopup.HorizontalOffset = -(Math.Abs(searchTool.SearchControl.ActualWidth - searchTool.SearchOptionsPanel.ActualWidth)) + (searchTool.SearchOptionsButton.ActualWidth + 4);
            }

            /// <summary>
            /// Handles the SnaglLoadedEvent
            /// </summary>
            /// <param name="args">The arguments for the events</param>
            public void SnaglLoadedEventHandler(SnaglLoadedEventArgs args)
            {
                IsAdvancedSearchVisible = ConfigurationManager.Instance.CurrentConfig.ApplicationMode == ApplicationMode.Evaluation ? true : false;
            }

        #endregion

        /// <summary>
        /// Performs the search action
        /// </summary>
        public void PerformAction()
        {
            // Get a collection of all criteria values
            List<SearchDescriptor> criteriaValues = advancedOptions.GetSearchDescriptors();

            // Determine if there are any valid criteria
            if (criteriaValues.Count == 0 && !string.IsNullOrEmpty(SearchText))
            {
                // Build SearchDescriptors for querying all attribute values
                criteriaValues.Add(new SearchDescriptor(null, SearchOperator.Contains, SearchText, false));
            }

            if (this.searchToolMode == SearchToolMode.Find)
                SearchManager.Instance.Find(criteriaValues, new SearchOptions(SearchAction.Select));
            else
                SearchManager.Instance.Find(criteriaValues, new SearchOptions(SearchAction.Filter));
        }

    }
}