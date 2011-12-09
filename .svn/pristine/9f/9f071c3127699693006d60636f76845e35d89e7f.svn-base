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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Berico.SnagL.Infrastructure.Data.Searching;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// A control that represents a criterion used for advanced
    /// searches and filters
    /// </summary>
    public class SearchCriterionViewModel : CriterionViewModelBase<AdvancedSearchViewModel>
    {

        private string value = string.Empty;
        private string errorMessage = string.Empty;
        private string selectedAttribute = string.Empty;
        private string selectedOperator = SearchOperator.Equals.ToString();
        private string selectorKey = "PLAIN_TEXT";

        /// <summary>
        /// Initializes a new instance of SearchCriterionViewModel
        /// setting the provided AdvancedSearchViewModel as its
        /// parent
        /// </summary>
        /// <param name="parent"></param>
        public SearchCriterionViewModel(AdvancedSearchViewModel _parent) : base(_parent)
        {
            IsValid = true;
        }

        #region Properties

            /// <summary>
            /// Gets or sets the value for this criterion.  This property
            /// is bound to the Text property of the value TextBox on
            /// the SearchCriterion's view.
            /// </summary>
            public string Value
            {
                get { return this.value; }
                set
                {
                    this.value = value;
                    RaisePropertyChanged("Value");
                }
            }

            /// <summary>
            /// Gets or sets the currently selected attribute.  This property
            /// is bound to the SelectedValue property of the Attribute
            /// ComboBox on the SearchCriterion's view.
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
            /// Gets or sets the currently selected operator.  This property
            /// is bound to the SelectedItem property of the Operator
            /// ComboBox on the SearchCriterion's view.
            /// </summary>
            public string SelectedOperator
            {
                get { return this.selectedOperator; }
                set
                {
                    this.selectedOperator = value;
                    RaisePropertyChanged("SelectedOperator");
                }
            }

            /// <summary>
            /// Gets or sets the key for the data template that
            /// should be used for the Editor control
            /// </summary>
            public string SelectorKey
            {
                get { return this.selectorKey; }
                set
                {
                    this.selectorKey = value;
                    RaisePropertyChanged("SelectorKey");
                }
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
                        //TODO:  UPDATE EDITOR USED DEPENDING ON ATTRIBUTE SelectedOperator

                        //TODO:  GET DETAILS ON ATTRIBUTE SELECTED
                        //TODO:  DETERMINE BASE TYPE

                        SelectorKey = "PLAIN_TEXT";
                    });
                }
            }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            SelectedOperator = SearchOperator.Contains.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SearchDescriptor GetSearchDescriptor()
        {
            // Validate the control
            Validate();

            // Check if the this SearchCriterion instance is valid.  Valid
            // means that all required input controls have values.
            if (IsValid)
            {
                // Create a new CriterionValue instance using the values from the 
                // input controls
                return new SearchDescriptor(SelectedAttribute,
                                            (SearchOperator)Enum.Parse(typeof(SearchOperator), SelectedOperator, true),
                                            Value,
                                            true);
            }
            else
            {
                // Return null if this SearchCriterion instance
                // was invalid.
                return null;
            }
        }

        /// <summary>
        /// Determines if the criterion is valid or not
        /// </summary>
        public void Validate()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(SelectedAttribute))
                sb.AppendLine("No target attribute was selected");

            if (string.IsNullOrEmpty(Value))
                sb.AppendLine("No value was specified");

            if (sb.Length > 0)
            {
                IsValid = false;
                ErrorMessage = sb.ToString();
            }
            else
            {
                IsValid = true;
                ErrorMessage = string.Empty;
            }
        }        

    }
}
