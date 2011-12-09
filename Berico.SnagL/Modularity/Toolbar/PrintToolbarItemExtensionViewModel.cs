//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows.Input;
    using Berico.SnagL.Infrastructure.Modularity.Contracts;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(PrintToolbarItemExtensionViewModel))]
    public class PrintToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {
        private int index;
        private string description = string.Empty;
        private bool isChecked = false;
        private bool isEnabled = true;

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.ImportToolbarItemExtensionViewModel
        /// </summary>
        public PrintToolbarItemExtensionViewModel()
        {
            this.index = 8;
            this.description = "Print the Graph";
            this.isChecked = false;
            this.Name = "PRINT_GRAPH";
        }

        protected virtual void OnToolbarItemSelected(EventArgs e)
        {
            if (ToolbarItemSelected != null)
            {
                ToolbarItemSelected(this, e);
            }
        }

        #region IToolbarItemViewModelExtension Members

        public event EventHandler<EventArgs> ToolbarItemSelected;

        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                RaisePropertyChanged("Description");
            }
        }

        public bool IsChecked
        {
            get { return this.isChecked; }
            set
            {
                this.isChecked = value;
                RaisePropertyChanged("IsChecked");
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

        public string Name { get; set; }

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

        #endregion
    }
}
