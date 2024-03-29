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
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Berico.Common;
using Berico.Common.Events;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Media.Animation;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Infrastructure.Preferences;
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// This class represents the base view model for all node views
    /// </summary>
    public class NodeViewModelBase : ViewModelBase, INodeShape
    {
        private readonly SnaglEventAggregator eventAggregator = SnaglEventAggregator.DefaultInstance;
        private const string STATE_NORMAL = "Normal";
        private Node parentNode;
        private Point position = new Point(0,0);
        private int zIndex = 2;
        private bool isHidden = false;
        private SolidColorBrush backgroundColor;
        private SolidColorBrush selectionColor;
        private double height = double.NaN;
        private double width = double.NaN;
        private NodeStates currentState = NodeStates.Normal;
        private Visibility backgroundVisibility = Visibility.Collapsed;
        private double scale = 1;

        private readonly SolidColorBrush PREFERENCE_DEFAULT_BACKGROUND_COLOR = Conversion.HexColorToBrush("#FF740000");
        private readonly SolidColorBrush PREFERENCE_DEFAULT_SELECTION_COLOR = Conversion.HexColorToBrush("#FF4A75A9");

        /// <summary>
        /// This factory method creates and returns a new Berico.LinkAnalysis.ViewModel.
        /// NodeViewModelBase instance based on the provided NodeType
        /// </summary>
        /// <param name="type">The type of node that should be created</param>
        /// <param name="node">The underlying data for the node</param>
        /// <returns>a new Berico.LinkAnalysis.ViewModel.NodeViewlModelBase instance</returns>
        public static NodeViewModelBase GetNodeViewModel(NodeTypes type, Node node, string scope)
        {

            // Create the appropriate viewmodel for the node based on
            // the provided type value
            switch (type)
            {
                case NodeTypes.Icon:
                    return new IconNodeViewModel(node, scope);
                case NodeTypes.Simple:
                    return null;
                case NodeTypes.Text:
                    return new TextNodeViewModel(node, scope);
                default:
                    return new TextNodeViewModel(node, scope);
            }

        }

        /// <summary>
        /// Initializes a new instance of the NodeViewModelBase class.
        /// </summary>
        public NodeViewModelBase(Node node, string _scope)
        {
            this.scope = _scope;
            this.parentNode = node;

            // Configure the node view
            //BackgroundColor = Conversion.HexColorToBrush(PreferencesManager.Instance.GetPreference((typeof(NodeViewModelBase)).ToString() + "_Background_Color", PREFERENCE_DEFAULT_BACKGROUND_COLOR.Color.ToString()));
            SelectionColor = Conversion.HexColorToBrush(PreferencesManager.Instance.GetPreference((typeof(NodeViewModelBase)).ToString() + "_Selection_Color", PREFERENCE_DEFAULT_SELECTION_COLOR.Color.ToString()));

            try
            {
                Modularity.ExtensionManager.ComposeParts(this);
            }
            catch (Exception)
            {
                this.scope = _scope;
            }

            // Randomly generate position for now.  This is only for
            // testing purposes and will be removed.
            // TODO:  REMOVE THIS AFTER LAYOUT ARE COMPLETED
            //Random rand = new Random();
            //System.Threading.Thread.Sleep(5);
            //Position = new Point(rand.Next(0, 1024), rand.Next(0, 768));
        }

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.ViewModel.NodeViewModalBase
        /// </summary>
        //public NodeViewModelBase() { }

        public void AnimateTo(Point targetPosition)
        {
            NodePositionAnimator animator = new NodePositionAnimator(this, targetPosition, 1000, 75);
            animator.FrameAnimated += new EventHandler(AnimatorFrameAnimated);
            animator.Completed += new EventHandler(AnimatorCompleted);
            animator.Begin();
        }

        private void AnimatorCompleted(object sender, EventArgs e)
        {
            OnAnimationCompleted(EventArgs.Empty);
        }

        private void AnimatorFrameAnimated(object sender, EventArgs e)
        {
 	        OnNodePositionAnimated(EventArgs.Empty);
        }

        #region properties

            /// <summary>
            /// Gets or sets the hiehgt of this node
            /// </summary>
            [ExportableProperty("Height")]
            public virtual double Height
            {
                get { return this.height; }
                set
                {
                    this.height = value;
                    RaisePropertyChanged("Height");
                }
            }

            /// <summary>
            /// Gets or sets the width of this node
            /// </summary>
            [ExportableProperty("Width")]
            public virtual double Width
            {
                get { return this.width; }
                set
                {
                    this.width = value;
                    RaisePropertyChanged("Width");
                }
            }

            /// <summary>
            /// Gets or sets the node's description
            /// </summary>
            public string Description
            {
                get { return this.parentNode.Description; }
                set
                {
                    this.parentNode.Description = value;
                    RaisePropertyChanged("Description");
                }
            }

            /// <summary>
            /// Gets or sets the DisplayValue for this node.  This value
            /// is shown on the nodes label (if it has one).
            /// </summary>
            public string DisplayValue
            {
                get { return this.parentNode.DisplayValue; }
                set
                {
                    this.parentNode.DisplayValue = value;
                    RaisePropertyChanged("DisplayValue");
                }
            }

            /// <summary>
            /// Gets or sets the position of this node on the
            /// graph
            /// </summary>
            [ExportableProperty("Position")]
            public virtual Point Position
            {
                get
                { 
                    return this.position;
                }
                set
                {
                    this.position = value;
                    RaisePropertyChanged("Position");

                    // Fire the NodeMoved event
                    OnNodeMoved(EventArgs.Empty);
                }
            }

            /// <summary>
            /// Gets or sets the ZIndex on this node.  A node should have a 
            /// higher ZIndex then an edge so it will appear above them.
            /// </summary>
            public int ZIndex
            {
                get { return this.zIndex; }
                set
                {
                    this.zIndex = value;
                    RaisePropertyChanged("ZIndex");
                }
            }

            /// <summary>
            /// Gets or sets whether this node is hidden or not
            /// </summary>
            [ExportableProperty("IsHidden")]
            public bool IsHidden
            {
                get { return this.isHidden; }
                set
                {
                    this.isHidden = value;
                    RaisePropertyChanged("IsHidden");
                }
            }

            /// <summary>
            /// Gets or sets the Background color for this node
            /// </summary>
            [ExportableProperty("BackgroundColor")]
            public SolidColorBrush BackgroundColor
            {
                get { return this.backgroundColor; }
                set
                {
                    this.backgroundColor = value;
                    RaisePropertyChanged("BackgroundColor");

                    // If the color is set to fully transparent, make
                    // the background hidden

                    if (value.Color.ToString() == "#00FFFFFF")
                        BackgroundVisibility = Visibility.Collapsed;
                    else
                        BackgroundVisibility = Visibility.Visible;

                    // The background color is managed by the PreferencesManager
                    //PreferencesManager.Instance.SetPreference((typeof(NodeViewModelBase)).ToString() + "_Background_Color", value.Color.ToString());
                }
            }

            /// <summary>
            /// Gets or sets the Selection color for this node
            /// </summary>
            [ExportableProperty("SelectionColor")]
            public SolidColorBrush SelectionColor
            {
                get
                {
                    return this.selectionColor;
                }
                set
                {
                    this.selectionColor = value;
                    RaisePropertyChanged("SelectionColor");

                    // The selection color is managed by the PreferencesManager
                    PreferencesManager.Instance.SetPreference((typeof(NodeViewModelBase)).ToString() + "_Selection_Color", value.Color.ToString());
                }
            }

            /// <summary>
            /// Gets whether the background should be visible
            /// </summary>
            public Visibility BackgroundVisibility
            {
                get { return this.backgroundVisibility; }
                private set
                {
                    this.backgroundVisibility = value;
                    RaisePropertyChanged("BackgroundVisibility");
                }
            }

            /// <summary>
            /// Gets or sets the underlying Berico.LinkAnalysis.Model.Node 
            /// object that contains the data for this node
            /// </summary>
            public Node ParentNode
            {
                get { return this.parentNode; }
                set { this.parentNode = value; }
            }
                

            /// <summary>
            /// Gets or sets the current state of this node.  This will be
            /// bound to the view to allow VisualStateManager to change states.
            /// </summary>
            public NodeStates CurrentState
            {
                get { return currentState; }
                set
                {
                    this.currentState = value;
                    RaisePropertyChanged("CurrentState");
                }
            }

            [ImportMany(typeof(IAction), RequiredCreationPolicy=CreationPolicy.Any, AllowRecomposition = true)]
            public List<IAction> Actions { get; set; }

            /// <summary>
            /// Gets or sets the amount this node is scaled.  A scale of
            /// 1 means no scale (or normal size).
            /// </summary>
            [ExportableProperty("Scale")]
            public double Scale
            {
                get { return this.scale; }
                set
                {
                    this.scale = value;
                    RaisePropertyChanged("Scale");
                }
            }

            public Point ScaleCenter
            {
                get { return new Point(Height/2, width/2); }
            }

        #endregion

        #region Commands

            /// <summary>
            /// Gets a command object that handles the SizeChanged event
            /// </summary>
            public ICommand SizeChangedCommand
            {
                get
                {
                    return new RelayCommand<SizeChangedEventArgs>(e =>
                    {
                        this.Height = e.NewSize.Height;
                        this.Width = e.NewSize.Width;
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the Loaded event
            /// </summary>
            public ICommand NodeLoadedCommand
            {
                get
                {
                    return new RelayCommand<DetailedEventInformation>(e =>
                    {
                        OnNodeLoaded(EventArgs.Empty);
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the MouseLeftButtonDown event
            /// </summary>
            public ICommand MouseLeftButtonDown
            {
                get
                {
                    return new RelayCommand<MouseButtonEventArgs>(e =>
                    {
                        // Fire NodeMouseLeftButtonUp event
                        OnNodeMouseLeftButtonDown(e);
                    });
                }
            }

            /// <summary>
            /// Gets the command that handles the MouseMove event for the view
            /// </summary>
            public ICommand MouseMoveCommand
            {
                get
                {
                    return new RelayCommand<MouseEventArgs>(e =>
                    {
                        OnNodeMouseMove(new NodeViewModelMouseEventArgs<MouseEventArgs>(this, e, this.scope));
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the MouseLeftButtonUp event
            /// </summary>
            public ICommand MouseLeftButtonUp
            {
                get
                {
                    return new RelayCommand<MouseButtonEventArgs>(e =>
                    {
                        // Fire NodeMouseLeftButtonUp event
                        OnNodeMouseLeftButtonUp(e);
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the MouseDoubleClick event
            /// </summary>
            public ICommand MouseDoubleClickCommand
            {
                get
                {
                    return new RelayCommand<MouseButtonEventArgs>(e =>
                    {
                        // Fire NodeMouseDoubleClick event
                        OnNodeMouseDoubleClick(e);
                    });
                }
            }
           
            /// <summary>
            /// Gets a command object that handles the MouseRightButtonDown event
            /// </summary>
            public ICommand MouseRightButtonDown
            {
                get
                {
                    return new RelayCommand<MouseEventArgs>(e =>
                    {
                        // We must 'handle' the event or the Silverlight dialog will
                        // be displayed and MouseRightButtonUp won't be called.
                        (e as MouseButtonEventArgs).Handled = true;

                        SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseRightButtonDownEvent>().Publish(new NodeViewModelMouseEventArgs<MouseButtonEventArgs>(this, e as MouseButtonEventArgs, this.scope));
                    });
                }
            }

            /// <summary>
            /// Gets a command object that handles the MouseRightButtonUp event
            /// </summary>
            public ICommand MouseRightButtonUp
            {
                get
                {
                    return new RelayCommand<MouseEventArgs>(e =>
                    {
                        //System.Windows.MessageBox.Show("MOUSE RIGHT BUTTON UP FIRED");
                    });
                }
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
                        OnNodeMouseEnter(new NodeViewModelMouseEventArgs<MouseEventArgs>(this, e, this.scope));
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
                        OnNodeMouseLeave(new NodeViewModelMouseEventArgs<MouseEventArgs>(this, e, this.scope));
                    });
                }
            }

        #endregion

        #region Events

            #region NodeLoaded

                /// <summary>
                /// Occurs when a node has loaded
                /// </summary>
                public event EventHandler<EventArgs> NodeLoaded;

                /// <summary>
                /// Raises the NodeLoaded event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnNodeLoaded(EventArgs e)
                {
                    EventHandler<EventArgs> handler = this.NodeLoaded;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }

            #endregion
        
            #region NodeMoved
                /// <summary>
                /// Occurrs when a node is moved
                /// </summary>
                private event EventHandler<EventArgs> nodeMoved;

                /// <summary>
                /// Occurrs when a node is moved
                /// </summary>
                public event EventHandler<EventArgs> NodeMoved
                {
                    add
                    {
                        // Ensure that the event handler is not null and the given object
                        // isn't already subscribed to it
                        if (this.nodeMoved == null || !this.nodeMoved.GetInvocationList().Contains(value))
                        {
                            // Add the object as subscriber
                            this.nodeMoved += value;
                        }
                    }
                    remove
                    {
                        // Remove the given object as a subscribed
                        this.nodeMoved -= value;
                    }
                }

                /// <summary>
                /// Raises the NodeMoved event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnNodeMoved(EventArgs e)
                {
                    EventHandler<EventArgs> handler = this.nodeMoved;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
            #endregion

            #region NodeMouseLeftButtonUp
                /// <summary>
                /// Occurrs when the left mouse button is released
                /// on a node
                /// </summary>
                private event MouseButtonEventHandler nodeMouseLeftButtonUp;

                /// <summary>
                /// Occurrs when the left mouse button is released
                /// on a node
                /// </summary>
                public event MouseButtonEventHandler NodeMouseLeftButtonUp
                {
                    add
                    {
                        // Ensure that the event handler is not null and the given object
                        // isn't already subscribed to it
                        if (this.nodeMouseLeftButtonUp == null || !this.nodeMouseLeftButtonUp.GetInvocationList().Contains(value))
                        {
                            // Add the object as subscriber
                            this.nodeMouseLeftButtonUp += value;
                        }
                    }
                    remove
                    {
                        // Remove the given object as a subscribed
                        this.nodeMouseLeftButtonUp -= value;
                    }
                }

                /// <summary>
                /// Raises the NodeMouseLeftButtonUp event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected void OnNodeMouseLeftButtonUp(MouseButtonEventArgs e)
                {
                    MouseButtonEventHandler handler = this.nodeMouseLeftButtonUp;
                    if (handler != null)
                    {
                        handler(this, e);
                    }

                    this.eventAggregator.GetEvent<NodeMouseLeftButtonUpEvent>().Publish(new NodeViewModelMouseEventArgs<MouseButtonEventArgs>(this, e, this.scope));
                }
            #endregion

            #region NodeMouseLeftButtonDown
                /// <summary>
                /// Occurrs when the left mouse button is pressed
                /// on a node
                /// </summary>
                private event MouseButtonEventHandler nodeMouseLeftButtonDown;

                /// <summary>
                /// Occurrs when the left mouse button is pressed
                /// on a node
                /// </summary>
                public event MouseButtonEventHandler NodeMouseLeftButtonDown
                {
                    add
                    {
                        // Ensure that the event handler is not null and the given object
                        // isn't already subscribed to it
                        if (this.nodeMouseLeftButtonDown == null || !this.nodeMouseLeftButtonDown.GetInvocationList().Contains(value))
                        {
                            // Add the object as subscriber
                            this.nodeMouseLeftButtonDown += value;
                        }
                    }
                    remove
                    {
                        // Remove the given object as a subscribed
                        this.nodeMouseLeftButtonDown -= value;
                    }
                }

                /// <summary>
                /// Raises the NodeMouseLeftButtonDown event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected void OnNodeMouseLeftButtonDown(MouseButtonEventArgs e)
                {
                    MouseButtonEventHandler handler = this.nodeMouseLeftButtonDown;
                    if (handler != null)
                    {
                        handler(this, e);
                    }

                    this.eventAggregator.GetEvent<NodeMouseLeftButtonDownEvent>().Publish(new NodeViewModelMouseEventArgs<MouseButtonEventArgs>(this, e, this.scope));
                }

            #endregion

            #region NodeMouseDoubleClick
                /// <summary>
                /// Raises the NodeMouseDoubleClick event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected void OnNodeMouseDoubleClick(MouseButtonEventArgs e)
                {
                    this.eventAggregator.GetEvent<NodeDoubleClickEvent>().Publish(new NodeViewModelMouseEventArgs<MouseButtonEventArgs>(this, e, this.scope));
                }
            #endregion

            #region NodePositionAnimated
                /// <summary>
                /// Occurrs when a node's position has been animated
                /// </summary>
                public event EventHandler<EventArgs> nodePositionAnimated;

                /// <summary>
                /// Occurrs when a node's position has been animated
                /// </summary>
                public event EventHandler<EventArgs> NodePositionAnimated
                {
                    add
                    {
                        // Ensure that the event handler is not null and the given object
                        // isn't already subscribed to it
                        if (this.nodePositionAnimated == null || !this.nodePositionAnimated.GetInvocationList().Contains(value))
                        {
                            // Add the object as subscriber
                            this.nodePositionAnimated += value;
                        }
                    }
                    remove
                    {
                        // Remove the given object as a subscribed
                        this.nodePositionAnimated -= value;
                    }
                }

                /// <summary>
                /// Raises the NodeMouseLeftButtonDown event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnNodePositionAnimated(EventArgs e)
                {
                    EventHandler<EventArgs> handler = this.nodePositionAnimated;
                    if (handler != null)
                    {
                        handler(this, e);
                    }

                    this.eventAggregator.GetEvent<NodePositionAnimatedEvent>().Publish(new NodeViewModelEventArgs(this, this.scope));
                }
            #endregion

            #region AnimationCompleted
                /// <summary>
                /// Occurs when node animation has completed
                /// </summary>
                public event EventHandler<EventArgs> animationCompleted;

                /// <summary>
                /// Occurs when node animation has completed
                /// </summary>
                public event EventHandler<EventArgs> AnimationCompleted
                {
                    add
                    {
                        // Ensure that the event handler is not null and the given object
                        // isn't already subscribed to it
                        if (this.animationCompleted == null || !this.animationCompleted.GetInvocationList().Contains(value))
                        {
                            // Add the object as subscriber
                            this.animationCompleted += value;
                        }
                    }
                    remove
                    {
                        // Remove the given object as a subscribed
                        this.animationCompleted -= value;
                    }
                }

                /// <summary>
                /// Raises the NodeMouseLeftButtonDown event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnAnimationCompleted(EventArgs e)
                {
                    EventHandler<EventArgs> handler = this.animationCompleted;
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
            #endregion

            #region NodeMouseEnter
                /// <summary>
                /// Raises the NodeMouseEnter event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnNodeMouseEnter(NodeViewModelMouseEventArgs<MouseEventArgs> args)
                {
                    this.eventAggregator.GetEvent<NodeMouseEnterEvent>().Publish(args);
                }
            #endregion

            #region NodeMouseLeave
                /// <summary>
                /// Raises the NodeMouseLeave event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnNodeMouseLeave(NodeViewModelMouseEventArgs<MouseEventArgs> args)
                {
                    this.eventAggregator.GetEvent<NodeMouseLeaveEvent>().Publish(args);
                }
            #endregion

            #region NodeMouseMove
                /// <summary>
                /// Raises the NodeMouseMove event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnNodeMouseMove(NodeViewModelMouseEventArgs<MouseEventArgs> args)
                {
                    this.eventAggregator.GetEvent<NodeMouseMoveEvent>().Publish(args);
                }
            #endregion

        #endregion

        #region IScopingContainer<string> Members

            private string scope = string.Empty;

            /// <summary>
            /// In this case, the scope is the parent GraphComponents
            /// instance that initially created the node view model.
            /// This should never change during the lifetime of this
            /// object.
            /// </summary>
            public string Scope
            {
                get { return this.scope; }
                private set { this.scope = value; }
            }

        #endregion

        #region INodeShape Members


            /// <summary>
            /// Gets the center point of this node.  This value is derrived from the 
            /// current Height and Width of the node and represents where an edge
            /// is attached.
            /// </summary>
            public Point CenterPoint
            {
                get
                {
                    // double.NaN represents 'Auto'
                    if (double.IsNaN(this.height) || double.IsNaN(this.width))
                        return position;
                    else
                        return new Point(position.X + (this.width / 2), position.Y + (this.height / 2));
                }
            }

        #endregion
    }
}