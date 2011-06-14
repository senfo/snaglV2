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
using Berico.SnagL.Infrastructure.Clustering;
using Berico.SnagL.Infrastructure.Events;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
    [PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(TreeToolbarItemExtensionViewModel))]
    public class TreeToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
    {

        private int index = 0;
        private string description = string.Empty;
        private bool isChecked = false;
        private bool isEnabled = true;

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.TreeToolbarItemExtensionViewModel
        /// </summary>
        public TreeToolbarItemExtensionViewModel()
        {
            this.index = 21;
            this.description = "Tree Layout:  Layout the graph in a hierarchical pattern (based on the nodes degree)";
            this.isChecked = false;
            this.Name = "TREE_LAYOUT";

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

        public bool IsChecked
        {
            get { return this.isChecked; }
            set
            {
                this.isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
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

            public string Name { get; set; }

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