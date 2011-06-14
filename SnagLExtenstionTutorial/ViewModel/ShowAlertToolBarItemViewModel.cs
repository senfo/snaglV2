using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SnagLExtenstionTutorial.ViewModel
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
    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(ShowAlertToolBarItemViewModel))]
    public class ShowAlertToolBarItemViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {
        /// <summary>
        /// Stores the description
        /// </summary>
        private string _description;
        private bool isEnabled = true;

        /// <summary>
        /// Initializes a new instance of the ShowAlertToolBarItemViewModel class.
        /// </summary>
        public ShowAlertToolBarItemViewModel()
        {
            Index = 51;
            Description = "Shows an alert";
            Name = "SHOWALERT";
        }

        /// <summary>
        /// Indicates the toolbar item has been selected
        /// </summary>
        public event EventHandler<EventArgs> ToolbarItemSelected;

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
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ICommand object the view binds with to handle user clicks
        /// </summary>
        public ICommand ItemSelected
        {
            get
            {
                return new RelayCommand(() =>
                {
                    OnToolbarItemSelected(EventArgs.Empty);
                });
            }
        }

        /// <summary>
        /// Event handler to handle when the button is selected
        /// </summary>
        /// <param name="e">Any event arguments that might be passed</param>
        protected virtual void OnToolbarItemSelected(EventArgs e)
        {
            MessageBox.Show("Hello from our sample button");

            if (ToolbarItemSelected != null)
            {
                ToolbarItemSelected(this, e);
            }
        }

        #region IToolbarItemViewModelExtension Members


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