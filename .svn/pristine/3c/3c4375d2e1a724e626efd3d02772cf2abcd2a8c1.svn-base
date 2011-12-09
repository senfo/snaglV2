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
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(ZoomToolbarItemExtensionViewModel))]
    public class ZoomToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {
        private int index = 99;
        private string description = "Zoom the graph surface in and out";
        private bool isChecked = true;
        private bool isEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomToolbarItemExtensionViewModel" /> class
        /// </summary>
        public ZoomToolbarItemExtensionViewModel()
        {
            this.Name = "ZOOM";
        }

        #region IToolbarItemExtension Members

        /// <summary>
        /// Gets or sets the index
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                RaisePropertyChanged("Index");
            }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether or not the item is checked
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the toolbar control is enabled
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                RaisePropertyChanged("IsEnabled");
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

        #endregion

        #region IToolbarItemViewModelExtension Members

        public event EventHandler<EventArgs> ToolbarItemSelected;

        public ICommand ItemSelected
        {
            get { return null; }
        }

        #endregion
    }
}