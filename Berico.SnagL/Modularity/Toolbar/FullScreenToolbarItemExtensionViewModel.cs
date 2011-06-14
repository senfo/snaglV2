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
using System.Windows;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(FullScreenToolbarItemExtensionViewModel))]
    public class FullScreenToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {

        private int index = 0;
        private string description = string.Empty;
        private bool isEnabled = true;

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.FullScreenToolbarItemExtensionViewModel
        /// </summary>
        public FullScreenToolbarItemExtensionViewModel()
        {
            this.index = 31;
            this.description = "Toggle full screen mode";
            this.Name = "FULL_SCREEN";
        }

        protected virtual void OnToolbarItemSelected(EventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;

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
