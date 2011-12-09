
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
using System.Windows.Controls;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(ResizeToolbarItemExtensionViewModel))]
    public class ResizeToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {

        private int index = 0;
        private string description = string.Empty;
        private bool isChecked = false;
        private bool isEnabled = true;
        //private OpenFileDialog openFileDialog = new OpenFileDialog();

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.ImportToolbarItemExtensionViewModel
        /// </summary>
        public ResizeToolbarItemExtensionViewModel()
        {
            this.index = 30;
            this.description = "Resize the graph to fit into the available space";
            this.isChecked = false;
            this.Name = "RESIZE";
        }

        #region IToolbarItemExtension Members

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

        #endregion

        #region IToolbarItemViewModelExtension Members

            public event EventHandler<EventArgs> ToolbarItemSelected;

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

        protected virtual void OnToolbarItemSelected(EventArgs e)
        {
            if (ToolbarItemSelected != null)
            {
                ToolbarItemSelected(this, e);
            }
        }
    }
}