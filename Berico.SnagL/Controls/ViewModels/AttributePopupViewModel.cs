﻿//-------------------------------------------------------------
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
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// A popup containing all attribute information for the node
    /// (or edge) that the cursor is currently over.
    /// </summary>
    public class AttributePopupViewModel : ViewModelBase, IDisposable
    {
        private readonly SnaglEventAggregator eventAggregator = SnaglEventAggregator.DefaultInstance;
        private bool isOpen = false;
        private double horizontalOffset = 0;
        private double verticalOffset = 0;
        private IEnumerable<KeyValuePair<string, string>> attributes = null;
        private DispatcherTimer openTimer;
        private DispatcherTimer closeTimer;
        private bool mouseLeftButtonDown = false;

        /// <summary>
        /// Creates a new instance of the attribute popup control
        /// </summary>
        public AttributePopupViewModel()
        {
            // Subscribe to appropriate events of nodes and edges
            this.eventAggregator.GetEvent<NodeMouseEnterEvent>().Subscribe(NodeMouseEnterEventHandler, false);
            this.eventAggregator.GetEvent<NodeMouseLeaveEvent>().Subscribe(NodeMouseLeaveEventHandler, false);
            this.eventAggregator.GetEvent<EdgeMouseEnterEvent>().Subscribe(EdgeMouseEnterEventHandler, false);
            this.eventAggregator.GetEvent<EdgeMouseLeaveEvent>().Subscribe(EdgeMouseLeaveEventHandler, false);
            this.eventAggregator.GetEvent<NodeMouseLeftButtonDownEvent>().Subscribe(NodeMouseLeftButtonDownEventHandler, false);
            this.eventAggregator.GetEvent<NodeMouseLeftButtonUpEvent>().Subscribe(NodeMouseLeftButtonUpEventHandler, false);
            this.eventAggregator.GetEvent<NodeMouseMoveEvent>().Subscribe(NodeMouseMoveEventHandler, false);

            // Setup the timers that will be responsible for opening
            // and closing the popup
            openTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(800) };
            openTimer.Tick += new EventHandler(OpenTimer_Tick);
            closeTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
            closeTimer.Tick += new EventHandler(CloseTimer_Tick);
        }

        /// <summary>
        /// Gets or sets whether the popup is open.  This
        /// property is bound to the view.
        /// </summary>
        public bool IsOpen
        {
            get { return this.isOpen; }
            set
            {
                this.isOpen = value;
                RaisePropertyChanged("IsOpen");
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset for the popup.  This
        /// property is bound to the view.
        /// </summary>
        public double HorizontalOffset
        {
            get { return this.horizontalOffset; }
            set
            {
                this.horizontalOffset = value;
                RaisePropertyChanged("HorizontalOffset");
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset for the popup.  This
        /// property is bound to the view.
        /// </summary>
        public double VerticalOffset
        {
            get { return this.verticalOffset; }
            set
            {
                this.verticalOffset = value;
                RaisePropertyChanged("VerticalOffset");
            }
        }

        /// <summary>
        /// Shows the popup with the specified attributes
        /// </summary>
        public void Show(IEnumerable<KeyValuePair<string, string>> attributes)
        {
            Attributes = attributes;

            IsOpen = false;

            // Open the popup
            this.closeTimer.Stop();
            this.openTimer.Start();
        }

        /// <summary>
        /// Closes the popup dialog
        /// </summary>
        public void Close()
        {
            openTimer.Stop();
            closeTimer.Start();
        }


        /// <summary>
        /// Handles the NodeMouseEnter event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void NodeMouseEnterEventHandler(NodeViewModelMouseEventArgs<MouseEventArgs> args)
        {
            // Positioning of the popup (the horizontal and vertical offsets) are
            // handled by a Behavior on the view.

            if (args.NodeViewModel.ParentNode != null && args.NodeViewModel.ParentNode.Attributes != null)
            {
                Show(args.NodeViewModel.ParentNode.Attributes.Select((record) => new KeyValuePair<string, string>(record.Key, record.Value.DisplayValue)));
            }
            else
            {
                Show(null);
            }
        }

        /// <summary>
        /// Handles the NodeMouseLeave event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void NodeMouseLeaveEventHandler(NodeViewModelMouseEventArgs<MouseEventArgs> args)
        {
            Close();
        }

        /// <summary>
        /// Handles the EdgeMouseEnter event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void EdgeMouseEnterEventHandler(EdgeViewModelMouseEventArgs<MouseEventArgs> args)
        {
            // Positioning of the popup (the horizontal and vertical offsets) are
            // handled by a Behavior on the view.

            if (args.EdgeViewModel.ParentEdge != null && (args.EdgeViewModel.ParentEdge is Model.DataEdge && (args.EdgeViewModel.ParentEdge as Model.DataEdge).Attributes != null))
            {
                Show((args.EdgeViewModel.ParentEdge as Model.DataEdge).Attributes.Select((record) => new KeyValuePair<string, string>(record.Key, record.Value.DisplayValue)));
            }
        }

        /// <summary>
        /// Handles the EdgeMouseLeave event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void EdgeMouseLeaveEventHandler(EdgeViewModelMouseEventArgs<MouseEventArgs> args)
        {
            // Close the popup
            this.openTimer.Stop();
            this.closeTimer.Start();
        }

        /// <summary>
        /// Handles the NodeMouseLeftButtonDown event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void NodeMouseLeftButtonDownEventHandler(NodeViewModelMouseEventArgs<MouseButtonEventArgs> args)
        {
            // Update the flag that indicates the state of the left
            // mouse button on the node
            this.mouseLeftButtonDown = true;
        }

        /// <summary>
        /// Handles the NodeMouseLeftButtonUp event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void NodeMouseLeftButtonUpEventHandler(NodeViewModelMouseEventArgs<MouseButtonEventArgs> args)
        {
            // Update the flag that indicates the state of the left
            // mouse button on the node
            this.mouseLeftButtonDown = false;
        }

        /// <summary>
        /// Handles the NodeMouseMoveEvent
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public void NodeMouseMoveEventHandler(NodeViewModelMouseEventArgs<MouseEventArgs> args)
        {
            // If the left mouse button is down while the mouse is being moved
            // we can assume the user is dragging the node and shouldn't show
            // the attribute popup
            if (this.mouseLeftButtonDown)
                if (IsOpen)
                    IsOpen = false;
                else
                    this.openTimer.Stop();
        }

        /// <summary>
        /// Gets or sets the collection of attributes to be displayed
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get { return this.attributes; }
            set
            {
                this.attributes = value;
                RaisePropertyChanged("Attributes");
            }
        }

        /// <summary>
        /// Handles the DispatchTImer.Tick event for the open timer
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The arguments for the event</param>
        private void OpenTimer_Tick(object sender, EventArgs e)
        {

            // Stop the close and open timers
            closeTimer.Stop();
            openTimer.Stop();

            // Ensure that there are Attributes to be displayed
            if (Attributes != null && Attributes.Count() > 0)
            {
                // Open the popup.  Keep in mind that the popup's position
                // is being handled by a behavior on the View
                IsOpen = true;
            }
        }

        /// <summary>
        /// Handles the DispatchTimer.Tick event for the close timer
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The arguments for the event</param>
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            this.closeTimer.Stop();
            IsOpen = false;
        }

        /// <summary>
        /// Gets a command object that handles the MouseEnter event
        /// </summary>
        public ICommand MouseEnterCommand
        {
            get
            {
                return new RelayCommand<MouseEventArgs>(e =>
                {
                    // We are inside the attribute popup so we don't
                    // want to close the popup
                    this.closeTimer.Stop();
                });
            }
        }

        /// <summary>
        /// Gets a command object that handles the MouseLeave event
        /// </summary>
        public ICommand MouseLeaveCommand
        {
            get
            {
                return new RelayCommand<MouseEventArgs>(e =>
                {
                    // We have left the attribute popup so we
                    // can close it
                    this.closeTimer.Start();
                });
            }
        }

    }
}