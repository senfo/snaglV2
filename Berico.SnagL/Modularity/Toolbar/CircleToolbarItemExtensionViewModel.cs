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
using GalaSoft.MvvmLight.Command;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Clustering;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(CircleToolbarItemExtensionViewModel))]
    public class CircleToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {

        private int index = 0;
        private string description = string.Empty;
        private bool isChecked = false;
        private bool isEnabled = true;

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.CircleToolbarItemExtensionViewModel
        /// </summary>
        public CircleToolbarItemExtensionViewModel()
        {
            this.index = 20;
            this.description = "Circle Layout:  Layout the graph in a circular pattern";
            this.isChecked = false;
            this.Name = "CIRCLE_LAYOUT";

            SnaglEventAggregator.DefaultInstance.GetEvent<Clustering.ClusteringCompletedEvent>().Subscribe(ClusteringCompletedEventHandler, false);
        }

        /// <summary>
        /// Handles the ClusteringCompleted event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void ClusteringCompletedEventHandler(ClusteringCompletedEventArgs args)
        {
            IsEnabled = !args.ClusteringActive;
        }

        protected virtual void OnToolbarItemSelected(EventArgs e)
        {
            if (ToolbarItemSelected != null)
            {
                ToolbarItemSelected(this, e);
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

        #region IToolbarItemViewModelExtension Members

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

            public event EventHandler<EventArgs> ToolbarItemSelected;

        #endregion

    }
}