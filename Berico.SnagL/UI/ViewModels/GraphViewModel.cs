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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Berico.Common;
using Berico.Common.Diagnostics;
using Berico.Common.Events;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Layouts;
using Berico.SnagL.Infrastructure.Modularity;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ImageTools;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// This class represents the view model for the graph view
    /// </summary>
    public class GraphViewModel : ViewModelBase
    {

        // We should try and decouple this in the future.  I don't like
        // having a reference to a control on the view but I had to do
        // it to be able to directly create and add the edge views.
        private System.Windows.Controls.Canvas graphSurface;
        private System.Windows.Controls.Canvas graphViewPort;

        private double graphSurfaceHeight = double.NaN;
        private double graphSurfaceWidth = double.NaN;
        private Graph graphControl = null;

        private string _commandText = string.Empty;
        private bool hasData = false;
        private bool isPanMode = false;
        private bool isLeftButtonDown = false;
        private bool isNodeDragging = false;
        private bool isSelectingArea = false;
        private bool isMouseDownHandled = false;
        private Point selectionStartPosition;
        private Point lastMousePosition;
        private bool isPanning;
        private bool isZoomKeyPressed;
        private Point pan = new Point(0, 0);
        private double scale = 1;
        private double width = double.NaN;
        private double height = double.NaN;
        private Point currentTranslation = new Point();
        private double currentScale = 1;
        private NodeViewModelBase dragTargetNode;

        private System.Windows.Shapes.Rectangle selectionRectangle = null;

        /// <summary>
        /// Initializes a new instance of the GraphViewModel class
        /// </summary>
        public GraphViewModel()
        {
            GraphManager.Instance.DefaultGraphComponentsInstance.PropertyChanged += new System.EventHandler<PropertyChangedEventArgs<object>>(DefaultGraphComponentsInstance_PropertyChanged);
            //GraphManager.Instance.DefaultGraphComponentsInstance.Data.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Data_CollectionChanged);
            
            // Subscribe to desired events
            SnaglEventAggregator.DefaultInstance.GetEvent<DataLoadedEvent>().Subscribe(DataLoadedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<LayoutExecutedEvent>().Subscribe(LayoutExecutedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<ToolBarItemClickedEvent>().Subscribe(ToolbarItemClickedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeftButtonDownEvent>().Subscribe(NodeMouseLeftButtonDownEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeftButtonUpEvent>().Subscribe(NodeMouseLeftButtonUpEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodePositionAnimatedEvent>().Subscribe(NodePositionAnimatedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<EdgeViewModelAddedEvent>().Subscribe(EdgeViewModelAddedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<EdgeViewModelRemovedEvent>().Subscribe(EdgeViewModelRemovedEventHandler, false);
            
        }

        #region Properties

            /// <summary>
            /// Gets or sets the Height of the GraphSurface canvas
            /// </summary>
            public double GraphSurfaceHeight
            {
                get { return graphSurfaceHeight; }
                set
                {
                    graphSurfaceHeight = value;
                    RaisePropertyChanged("GraphSurfaceHeight");
                }
            }

            /// <summary>
            /// Gets or sets the Width of the GraphSurface canvas
            /// </summary>
            public double GraphSurfaceWidth
            {
                get { return graphSurfaceWidth; }
                set
                {
                    graphSurfaceWidth = value;
                    RaisePropertyChanged("GraphSurfaceWidth");
                }
            }

            /// <summary>
            /// Gets the center point of the GraphSurface
            /// </summary>
            public Point GraphSurfaceCenterPoint
            {
                get
                {
                    // double.NaN represents 'Auto'
                    if (double.IsNaN(this.graphSurfaceHeight) || double.IsNaN(this.graphSurfaceWidth))
                        return new Point(0, 0);
                    else
                        return new Point(this.graphSurfaceWidth / 2, this.graphSurfaceHeight / 2);
                }
            }

            /// <summary>
            /// Gets or sets whether the graph is in Pan mode or not
            /// </summary>
            /// <returns>true if the graph is in pan mode; otherwise false</returns>
            public bool IsPanMode
            {
                get { return this.isPanMode; }
                set { this.isPanMode = value; }
            }

            public Point Pan
            {
                get { return this.pan; }
                set
                {
                    this.pan = value;
                    RaisePropertyChanged("Pan");
                }
            }

            public double Height
            {
                get { return this.height; }
                set
                {
                    this.height = value;
                    RaisePropertyChanged("Height");
                }
            }

            public double Width
            {
                get { return this.width; }
                set
                {
                    this.width = value;
                    RaisePropertyChanged("Width");
                }
            }


            public double Scale
            {
                get { return currentScale; }
                set
                {
                    ScaleBy(value / currentScale);
                    //RaisePropertyChanged("Scale");
                }
            }

            /// <summary>
            /// Gets a collection of all the current node and edge view models
            /// </summary>
            public ObservableCollection<INodeShape> NodeViewModels
            {
                get { return GraphManager.Instance.DefaultGraphComponentsInstance.NodeViewModels; }
            }

            /// <summary>
            /// Gets or sets the type of node to show on the graph
            /// </summary>
            public NodeTypes NodeType { get; set; }

            /// <summary>
            /// Gets or sets whether there is any data present on the 
            /// graph or not
            /// </summary>
            public bool HasData
            {
                get { return this.hasData; }
                set
                {
                    this.hasData = value;
                    RaisePropertyChanged("HasData");
                }
            }

  #endregion

        #region Event Handlers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void DefaultGraphComponentsInstance_PropertyChanged(object sender, PropertyChangedEventArgs<object> e)
            {
                // Check if we are dealing with the 'Data' property
                if (e.PropertyName == "Data")
                {
                    // Unhook the event handler
                    (e.OldValue as GraphData).CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Data_CollectionChanged);

                    // Hook the event handler
                    (e.NewValue as GraphData).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Data_CollectionChanged);

                    // Update the HasData property if
                    if ((e.NewValue as GraphData).Order > 0)
                        HasData = true;
                    else
                        HasData = false;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                GraphData data = sender as GraphData;
                
                if (data.Count > 0)
                    HasData = true;
                else
                    HasData = false;
            }

            /// <summary>
            /// Handles the DataLoaded event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void DataLoadedEventHandler(DataLoadedEventArgs args)
            {
                //TODO:  HANDLE THIS BETTER
                //GraphManager.Instance.DefaultGraphComponentsInstance.Data.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Data_CollectionChanged);
                HasData = true;
                // Resize the graph to fit the window
                ResizeToFit();
            }

            /// <summary>
            /// Handles the LayoutCompleted event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void LayoutExecutedEventHandler(LayoutEventArgs args)
            {
                // Resize the graph to fit the window
                ResizeToFit();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            public void NodePositionAnimatedEventHandler(NodeViewModelEventArgs args)
            {
                // Resize the graph to fit the window
                ResizeToFit();
            }

            /// <summary>
            /// Handles the ToolbarItemClicked event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void ToolbarItemClickedEventHandler(ToolBarItemEventArgs args)
            {
                // TODO: USE CONSTANTS FOR NAME
                if (args.ToolBarItem.Name == "SELECTION")
                    this.isPanMode = false;
                else if (args.ToolBarItem.Name == "PAN")
                    this.isPanMode = true;
            }

            /// <summary>
            /// Handles the MouseLeftButtonUp event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void NodeMouseLeftButtonUpEventHandler(NodeViewModelMouseEventArgs<MouseButtonEventArgs> args)
            {
                // Only allow node manipulation if we are not in pan mode
                if (!isPanMode)
                {
                    NodeSelector nodeSelector = GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector;

                    // Check if we were dragging the node
                    if (isNodeDragging)
                    {
                        // Turn off node dragging
                        isNodeDragging = false;
                        dragTargetNode = null;
                    }
                    else
                    {
                        // Check if any nodes are currently selected
                        if (nodeSelector.AreAnyNodesSelected)
                        {
                            // Check if Control is pressed
                            if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
                            {
                                nodeSelector.InvertSelection();
                            }
                            else
                            {
                                // Check if multiple nodes are selected
                                if (nodeSelector.AreMultipleNodesSelected)
                                {
                                    // Check if target node is already selected
                                    if (nodeSelector.SelectedNodes.Contains(args.NodeViewModel))
                                    {
                                        // Check if Control is pressed
                                        if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                                        {
                                            nodeSelector.Unselect(args.NodeViewModel);
                                            return;
                                        }
                                        else
                                        {
                                            nodeSelector.UnselectAll();
                                        }
                                    }
                                    else // Target node is not selected
                                    {
                                        // Check if Control is pressed
                                        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                                        {
                                            nodeSelector.UnselectAll();
                                        }
                                    }
                                }
                                else // Only one node is selected
                                {
                                    // Check if the target node is already selected
                                    if (nodeSelector.SelectedNode.Equals(args.NodeViewModel))
                                    {
                                        nodeSelector.Unselect(nodeSelector.SelectedNode);
                                        return;
                                    }
                                    else // Target node is not selected
                                    {
                                        // Check if Control is pressed
                                        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                                        {
                                            nodeSelector.Unselect(nodeSelector.SelectedNode);
                                        }
                                    }
                                }
                            }
                        }

                        // Set the SELECTED state of the clicked node
                        nodeSelector.Select(args.NodeViewModel);
                    }
                }
            }

            /// <summary>
            /// Handles the MouseLeftButtonDown event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void NodeMouseLeftButtonDownEventHandler(NodeViewModelMouseEventArgs<MouseButtonEventArgs> args)
            {
                // Turn on node dragging if we are in node selection mode
                if (!isPanMode)
                {
                    isLeftButtonDown = true;
                    dragTargetNode = args.NodeViewModel;
                    isMouseDownHandled = true;
                }
            }

            /// <summary>
            /// Handles the EdgeViewModelRemoved event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void EdgeViewModelRemovedEventHandler(EdgeViewModelEventArgs args)
            {
                RemoveEdgeView(args.EdgeViewModel);
            }

            /// <summary>
            /// Handles the EdgeViewModelAdded event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void EdgeViewModelAddedEventHandler(EdgeViewModelEventArgs args)
            {
                AddEdgeView(args.EdgeViewModel);
            }

        #endregion

        #region Commands

            #region MouseLeftButtonDown

                /// <summary>
                /// Gets a command object that handles the MouseLeftButtonDown event
                /// </summary>
                public ICommand MouseLeftButtonDown
                {
                    get
                    {
                        return new RelayCommand<MouseButtonEventArgs>(e =>
                        {
                            if (isMouseDownHandled)
                            {
                                isMouseDownHandled = false;
                            }
                            else
                            {
                                isLeftButtonDown = true;
                                isZoomKeyPressed = (Keyboard.Modifiers & ModifierKeys.Shift) > 0;
                                isPanning = IsPanMode && !isZoomKeyPressed;
                                dragTargetNode = null;

                                lastMousePosition = e.GetPosition(graphViewPort);
                            }
                        });
                    }
                }

            #endregion

            #region MouseMove

                /// <summary>
                /// Gets a command object that handles the MouseMove event
                /// </summary>
                public ICommand MouseMove
                {
                    get
                    {
                        return new RelayCommand<DetailedEventInformation>(eventInfo =>
                            {
                                MouseEventArgs eventArgs = eventInfo.EventArgs as MouseEventArgs;
                                Graph graphViewControl = eventInfo.Sender as Graph;
                                Point currentMousePosition = eventArgs.GetPosition(graphViewControl.GraphViewPort);

                                // If the left button is down on a node, activate dragging
                                if (!isPanning && isLeftButtonDown && dragTargetNode != null && !isNodeDragging)
                                {
                                    isNodeDragging = true;
                                }
                                else if ((isZoomKeyPressed && !isSelectingArea) || !isPanning && isLeftButtonDown && dragTargetNode == null && !isSelectingArea)
                                {
                                    isSelectingArea = true;
                                    selectionStartPosition = currentMousePosition;
                                }

                                if (!isPanning && isNodeDragging && dragTargetNode != null)
                                {
                                    Point current = ViewportToCanvas(currentMousePosition);
                                    Point last = ViewportToCanvas(lastMousePosition);
                                    double deltaX = current.X - last.X;
                                    double deltaY = current.Y - last.Y;

                                    NodeSelector nodeSelector = GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector;

                                    // Check if multiple nodes are selected and that the node
                                    // being dragged is one of them
                                    if (nodeSelector.AreMultipleNodesSelected && nodeSelector.SelectedNodes.Contains(dragTargetNode))
                                    {
                                        // Loop over the selected nodes and relocated each one
                                        foreach (NodeViewModelBase nodeVM in nodeSelector.SelectedNodes)
                                        {
                                            nodeVM.Position = new Point(nodeVM.Position.X + deltaX, nodeVM.Position.Y + deltaY);
                                        }
                                    }
                                    else
                                    {
                                        // no nodes are currently selected
                                        dragTargetNode.Position = new Point(dragTargetNode.Position.X + deltaX, dragTargetNode.Position.Y + deltaY);
                                    }
                                }
                                else if (isPanning && !isZoomKeyPressed)
                                {
                                    double deltaX = currentMousePosition.X - lastMousePosition.X;
                                    double deltaY = currentMousePosition.Y - lastMousePosition.Y;

                                    //PanBy(deltaX, deltaY);
                                    MoveBy(deltaX, deltaY);
                                }
                                else if (isSelectingArea)
                                {
                                    if (selectionRectangle != null)
                                        graphViewPort.Children.Remove(selectionRectangle);
                                    else
                                    {
                                        selectionRectangle = new System.Windows.Shapes.Rectangle
                                        {
                                            Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue),
                                            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue),
                                            Opacity = 0.3
                                        };
                                    }

                                    Point position = eventArgs.GetPosition(graphViewControl);
                                    Point topLeft = position.GetTopLeft(selectionStartPosition);
                                    Point bottomRight = selectionStartPosition.GetBottomRight(position);
                                    double height = bottomRight.Y - topLeft.Y;
                                    double width = bottomRight.X - topLeft.X;

                                    selectionRectangle.Height = height;
                                    selectionRectangle.Width = width;
                                    selectionRectangle.SetValue(System.Windows.Controls.Canvas.TopProperty, topLeft.Y);
                                    selectionRectangle.SetValue(System.Windows.Controls.Canvas.LeftProperty, topLeft.X);

                                    graphViewPort.Children.Add(selectionRectangle);
                                }

                                lastMousePosition = currentMousePosition;
                            });
                    }
                }

            #endregion
 
            #region MouseLeftButtonUp

                /// <summary>
                /// Gets a command object that handles the MouseLeftButtonUp event
                /// </summary>
                public ICommand MouseLeftButtonUp
                {
                    get
                    {
                        return new RelayCommand<MouseButtonEventArgs>(e =>
                        {
                            if (isPanning)
                            {
                                isPanning = false;
                            }
                            else if (isSelectingArea && selectionStartPosition != lastMousePosition)
                            {
                                Point endPosition = ViewportToCanvas(lastMousePosition);
                                Point topLeft = ViewportToCanvas(selectionStartPosition).GetTopLeft(endPosition);
                                Point bottomRight = ViewportToCanvas(selectionStartPosition).GetBottomRight(endPosition);
                                NodeSelector nodeSelector = GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector;

                                if (isZoomKeyPressed)
                                {
                                    FitBoundingBox(topLeft, bottomRight);
                    
                                    // Indicate zoom key is no longer pressed
                                    isZoomKeyPressed = false;
                                }
                                else
                                {
                                    // Check if the Control key is being pressed
                                    if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                                    {
                                        nodeSelector.TurnOffSelection();
                                    }

                                    // Loop over all the node view models
                                    foreach (INodeShape nodeVM in GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModels())
                                    {
                                        // Check if the node view model is within our selection box
                                        if (nodeVM.Position.IsInBoundingBox(topLeft, bottomRight))
                                        {
                                            // Select this node view model
                                            nodeSelector.Select(nodeVM as NodeViewModelBase);
                                        }
                                    }
                                }

                                graphViewPort.Children.Remove(selectionRectangle);
                                isSelectingArea = false;
                                selectionRectangle = null;
                                selectionStartPosition = new Point(0, 0);
                            }

                            isLeftButtonDown = false;
                        });
                    }
                }

            #endregion

            #region MouseRightButtonDown

                /// <summary>
                /// Gets a command object that handles the MouseRightButtonDown event
                /// </summary>
                public ICommand MouseRightButtonDown
                {
                    get
                    {
                        return new RelayCommand<MouseButtonEventArgs>(e =>
                        {
                            // We must 'handle' the event or the Silverlight dialog will
                            // be displayed and MouseRightButtonUp won't be called.
                            (e as MouseButtonEventArgs).Handled = true;

                            SnaglEventAggregator.DefaultInstance.GetEvent<GraphMouseRightButtonDownEvent>().Publish(e);
                        });
                    }
                }

            #endregion

            #region MouseRightButtonUp

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

            #endregion

            #region MouseWheel

                /// <summary>
                /// Gets a command object that handles the MouseWheel event.
                /// In this case, the mouse wheel zooms the graph control in
                /// and out.
                /// </summary>
                public ICommand MouseWheel
                {
                    get
                    {
                        return new RelayCommand<DetailedEventInformation>(eventInfo =>
                        {
                            MouseWheelEventArgs eventArgs = eventInfo.EventArgs as MouseWheelEventArgs;
                            Graph graphViewControl = eventInfo.Sender as Graph;

                            Point graphCenter = new Point(this.width / 2, this.height / 2);
                            Point currentMousePosition = eventArgs.GetPosition(graphViewControl);

                            double offsetX = currentMousePosition.X - graphCenter.X;
                            double offsetY = currentMousePosition.Y - graphCenter.Y;
                            double zoomChange = 0.05 * Math.Sign(eventArgs.Delta);

                            //PanBy(-offsetX, -offsetY);
                            MoveBy(-offsetX, -offsetY);

                            if (this.Scale > 1)
                                zoomChange *= 4;

                            //ScaleBy(zoomChange);
                            this.Scale += zoomChange;

                            //PanBy(offsetX, offsetY);
                            MoveBy(offsetX, offsetY);

                            // Prevent HTML DOM event from firing
                            eventArgs.Handled = true;
                        });
                    }
                }

            #endregion

            #region GraphSurfaceLoaded

                /// <summary>
                /// Gets a command object that handles the Loaded event
                /// on the GraphSurface control.  We shouldn't need this
                /// in the long run.
                /// </summary>
                public ICommand GraphSurfaceLoaded
                {
                    get
                    {
                        return new RelayCommand<DetailedEventInformation>(e =>
                        {
                            if (graphSurface == null)
                            {
                                SnaglLoadedEventArgs args = new SnaglLoadedEventArgs
                                {
                                    GraphLoaded = true
                                };

                                // Save the instance of the GraphSurface cances.  We need
                                // this in order to create and manage the physical edge
                                // lines
                                graphSurface = e.Sender as System.Windows.Controls.Canvas;

                                // Fire the event that indicates that the graph surface
                                // has fully loaded
                                SnaglEventAggregator.DefaultInstance.GetEvent<SnaglLoadedEvent>().Publish(args);
                            }
                        });
                    }
                }

        
            #endregion

            #region GraphViewPortLoaded

                /// <summary>
                /// Gets a command object that handles the Loaded event
                /// on the GraphViewPort control.  We shouldn't need 
                /// this in the long run.
                /// </summary>
                public ICommand GraphViewPortLoaded
                {
                    get
                    {
                        return new RelayCommand<DetailedEventInformation>(e =>
                        {
                            if (graphViewPort == null)
                            {
                                // Save the instance of the GraphSurface cances.  We need
                                // this in order to create and manage the physical edge
                                // lines
                                graphViewPort = e.Sender as System.Windows.Controls.Canvas;
                            }
                        });
                    }
                }

            #endregion

            /// <summary>
            /// Gets the command that represents the KeyDown event of the Graph
            /// </summary>
            public ICommand GraphKeyDownCommand
            {
                get
                {
                    return new RelayCommand<KeyEventArgs>(e =>
                    {
                        switch (e.Key)
                        {
                            case Key.Escape:
                                GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector.TurnOffSelection();
                                break;
                            case Key.PageUp:
                                this.Scale += 0.05;
                                break;
                            case Key.PageDown:
                                this.Scale -= 0.05;
                                break;
                            case Key.Home:
                                ResizeToFit();
                                break;
                            case Key.Left:
                                MoveBy(-10, 0);
                                break;
                            case Key.Right:
                                MoveBy(10, 0);
                                break;
                            case Key.Up:
                                MoveBy(0, -10);
                                break;
                            case Key.Down:
                                MoveBy(0, 10);
                                break;
                        }
                    });
                }
            }

            public ICommand GraphControlLoadedCommand
            {
                get
                {
                    return new RelayCommand<DetailedEventInformation>(e =>
                    {
                        if (graphControl == null)
                        {
                            graphControl = e.Sender as Berico.SnagL.UI.Graph;
                            graphControl.Focus();
                            graphControl.MouseLeftButtonUp += (sender, args) => { graphControl.Focus(); };
                        }
                    });
                }
            }

            #region MouseDoubleClick

                /// <summary>
                /// Gets a command object that handles the MouseDoubleClick event
                /// </summary>
                public ICommand MouseDoubleClick
                {
                    get
                    {
                        return new RelayCommand<MouseEventArgs>(e =>
                        {
                            Point clickedPos = e.GetPosition(graphControl);
                            Point centerPos = new Point(graphControl.ActualWidth / 2D, graphControl.ActualHeight / 2D);

                            double offsetX = centerPos.X - clickedPos.X;
                            double offsetY = centerPos.Y - clickedPos.Y;

                            MoveBy(offsetX, offsetY);
                        });
                    }
                }

            #endregion

            #region SizeChanged

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
                            this.GraphSurfaceHeight = e.NewSize.Height;
                            this.GraphSurfaceWidth = e.NewSize.Width;

                            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                                ResizeToFit()
                            );

                        });
                    }
                }

            #endregion

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (NodeViewModels.Count > 0)
            {
                GraphManager.Instance.DefaultGraphComponentsInstance.Clear();
                this.graphSurface.Children.Clear();
            }
        }

        public ExtendedImage GraphToImage()
        {
            return graphViewPort.ToImage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void AddUIElementToGraph(UIElement element)
        {
            graphSurface.Children.Add(element);
        }

        public void RemoveUIElementFromGraph(UIElement element)
        {
            graphSurface.Children.Remove(element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointToTranslate"></param>
        /// <returns></returns>
        private Point ViewportToCanvas(Point pointToTranslate)
        {
            Point translatedPoint = new Point();

            translatedPoint.X = pointToTranslate.X / currentScale;
            translatedPoint.Y = pointToTranslate.Y / currentScale;
            translatedPoint.X -= currentTranslation.X;
            translatedPoint.Y -= currentTranslation.Y;

            return translatedPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizeToFit()
        {
            StopWatch stopWatch = StopWatch.StartNew();

            Point topLeft = new Point(long.MaxValue, long.MaxValue);
            Point bottomRight = new Point(long.MinValue, long.MinValue);

            // Loop through all nodes to get the bounding area
            foreach (NodeViewModelBase nodeVM in this.NodeViewModels)
            {
                if (double.IsNaN(nodeVM.Width) || double.IsNaN(nodeVM.Height))
                    continue;

                topLeft.X = System.Math.Min(topLeft.X, nodeVM.Position.X - nodeVM.Width);
                topLeft.Y = System.Math.Min(topLeft.Y, nodeVM.Position.Y - nodeVM.Height);
                bottomRight.X = System.Math.Max(bottomRight.X, nodeVM.Position.X + nodeVM.Width);
                bottomRight.Y = System.Math.Max(bottomRight.Y, nodeVM.Position.Y + nodeVM.Height);
            }

            // Zoom or pan the screen to ensure the nodes fit
            if (topLeft.X != long.MaxValue && topLeft.Y != long.MaxValue)
            {
                stopWatch.Stop();
                //System.Diagnostics.Debug.WriteLine("Determine bounding area:  {0} seconds", stopWatch.Elapsed.Seconds);
                //System.Diagnostics.Debug.WriteLine("TopLeft: {0}, {1}", topLeft.X, topLeft.Y);
                //System.Diagnostics.Debug.WriteLine("BottomRight: {0}, {1}", bottomRight.X, bottomRight.Y);

                stopWatch.Restart();
                //graphControl.FitBoundingBox(topLeft, bottomRight);
                FitBoundingBox(topLeft, bottomRight);
                //System.Diagnostics.Debug.WriteLine("Fit to Bounds:  {0} seconds", stopWatch.Elapsed.Seconds);
            }
        }

        /// <summary>
        /// Sizes the graph to fit within the specified points
        /// </summary>
        /// <param name="topLeft">Top left corner of the bounding box</param>
        /// <param name="bottomRight">Lower right corner of the bounding box</param>
        private void FitBoundingBox(Point topLeft, Point bottomRight)
        {
            // Rest the Zoom and Pan for a starting point
            //ResetZoomAndPan();

            //double width = bottomRight.X - topLeft.X;
            //double height = bottomRight.Y - topLeft.Y;

            //Point center = new Point((topLeft.X + bottomRight.X) / 2, (topLeft.Y + bottomRight.Y) / 2);
            //double scale = System.Math.Min(this.Width / width, this.Height / height);

            ////TODO: HANDLE MIN AND MAX FOR SCALING

            //// TODO:  COMBINE THIS WITH A VERSION OF PANBY (OR SOMETHING SIMILAR)
            //double deltaX = 0.0;
            //double deltaY = 0.0;

            //if (center.X >= 0)
            //    deltaX = (GraphSurfaceWidth / 2) - System.Math.Abs(center.X);
            //else
            //    deltaX = (GraphSurfaceWidth / 2) + System.Math.Abs(center.X);

            //if (center.Y >= 0)
            //    deltaY = (GraphSurfaceHeight / 2) - System.Math.Abs(center.Y);
            //else
            //    deltaY = (GraphSurfaceHeight / 2) + System.Math.Abs(center.Y);

            ////this.Pan = new Point(deltaX, deltaY);
            ////this.Pan = new Point(deltaX, deltaY);
            //PanBy(deltaX, deltaY);

            //// Save the current overall translation data
            ////this.currentTranslation.X += deltaX / currentScale;
            ////this.currentTranslation.Y += deltaY / currentScale;

            //// COMBINE THIS WITH A VERSION OF SCALEBY (OR SOMETHING SIMILAR)
            //this.Scale = scale;
            //this.currentScale = scale;

            //currentTranslation.X += (GraphSurfaceWidth / currentScale - GraphSurfaceWidth / (currentScale / scale)) / 2;
            //currentTranslation.Y += (GraphSurfaceHeight / currentScale - GraphSurfaceHeight / (currentScale / scale)) / 2;
            //ScaleBy(scale);

            //System.Diagnostics.Debug.WriteLine("PAN:   {0}", this.pan);
            //System.Diagnostics.Debug.WriteLine("SCALE: {0}", this.scale);

            ClearTransforms();
            double width = bottomRight.X - topLeft.X;
            double height = bottomRight.Y - topLeft.Y;

            Point center = new Point(
                                    (topLeft.X + bottomRight.X) / 2,
                                    (topLeft.Y + bottomRight.Y) / 2
                               );

            double scale = Math.Min(this.Width / width, this.Height / height);

            MoveBy(graphSurface.ActualWidth / 2 - center.X, graphSurface.ActualHeight / 2 - center.Y);
            ScaleBy(scale);
        }

        private void ClearTransforms()
        {
            System.Windows.Media.TransformGroup tg = graphSurface.RenderTransform as System.Windows.Media.TransformGroup;
            if (tg == null)
            {
                tg = new System.Windows.Media.TransformGroup();
                graphSurface.RenderTransform = tg;
            }

            tg.Children.Clear();

            currentScale = 1;
            currentTranslation = new Point();
        }

        public void MoveBy(double deltaX, double deltaY)
        {
            System.Windows.Media.TranslateTransform tt = new System.Windows.Media.TranslateTransform { X = deltaX, Y = deltaY };
            System.Windows.Media.TransformGroup tg = graphSurface.RenderTransform as System.Windows.Media.TransformGroup;
            tg.Children.Add(tt);

            graphSurface.RenderTransform = tg;

            currentTranslation.X += deltaX / currentScale;
            currentTranslation.Y += deltaY / currentScale;
        }

        //private void ResetZoomAndPan()
        //{
        //    // Reset transform values
        //    Scale = 1;
        //    Pan = new Point(0, 0);

        //    this.currentScale = 1;
        //    this.currentTranslation = new Point();
        //}

        //public void PanBy(double deltaX, double deltaY)
        //{
        //    // Pan the graph using the PAN property which
        //    // is bound to the translate transform
        //    this.Pan = new Point(this.pan.X + deltaX, this.pan.Y + deltaY);

        //    // Save the current overall translation data
        //    this.currentTranslation.X += deltaX / this.currentScale;
        //    this.currentTranslation.Y += deltaY / this.currentScale;
        //}

        public void ScaleBy(double scale)
        {
            double MIN_SCALE = 0.05;
            double MAX_SCALE = 4;

            double newScale = currentScale * scale;
            if (newScale < MIN_SCALE)
            {
                scale = MIN_SCALE / currentScale;
                newScale = currentScale * scale;
            }
            else if (newScale > MAX_SCALE)
            {
                scale = MAX_SCALE / currentScale;
                newScale = currentScale * scale;
            }

            System.Windows.Media.ScaleTransform st = new System.Windows.Media.ScaleTransform { ScaleX = scale, ScaleY = scale };
            (graphSurface.RenderTransform as System.Windows.Media.TransformGroup).Children.Add(st);
            currentScale = newScale;

            // scale>1 : up and to left
            // scale<1: down and to right
            // we are finding the difference the scale will cause in each dimension
            // (because scaling is done from the center)
            // and then compensating the translation by that
            currentTranslation.X += (graphSurface.ActualWidth / currentScale - graphSurface.ActualWidth / (currentScale / scale)) / 2;
            currentTranslation.Y += (graphSurface.ActualHeight / currentScale - graphSurface.ActualHeight / (currentScale / scale)) / 2;

            // Raise the GraphResizedEvent event
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                SnaglEventAggregator.DefaultInstance.GetEvent<GraphResizedEvent>().Publish(EventArgs.Empty)
                );
        }

        /// <summary>
        /// Zooms the graph surface in our out according to the <paramref name="scale"/>
        /// </summary>
        /// <param name="scale">A scale ranging from 0.05 to 4</param>
        public void Zoom(double scale)
        {
            Scale = scale;
        }

        //public void ScaleBy(double scaleChange)
        //{
        //    double MIN_SCALE = 0.01;
        //    double MAX_SCALE = 4;
        //    double newScale = this.currentScale + (this.currentScale * scaleChange);
        //    //double newScale = this.scale * scaleChange;

        //    //if (newScale < MIN_SCALE)
        //    //{
        //    //    proposedScale = MIN_SCALE / this.currentScale;
        //    //    newScale = this.currentScale * proposedScale;
        //    //    scaleChange = MIN_SCALE;
        //    //}
        //   // else if (newScale > MAX_SCALE)
        //    //{
        //    //    proposedScale = MAX_SCALE / this.currentScale;
        //    //    newScale = this.currentScale * proposedScale;
        //    //    scaleChange = MAX_SCALE;
        //    //}

        //    if (newScale < MIN_SCALE)
        //        return;

        //    if (newScale > MAX_SCALE)
        //        return;

        //    // Set the scale transform
        //    //this.Scale += proposedScale;
        //    this.Scale = newScale;
        //    this.currentScale = newScale;

        //    currentTranslation.X += (GraphSurfaceWidth / currentScale - GraphSurfaceWidth / (currentScale / scaleChange)) / 2;
        //    currentTranslation.Y += (GraphSurfaceHeight / currentScale - GraphSurfaceHeight / (currentScale / scaleChange)) / 2;
        //}

        /// <summary>
        /// Add an edge view (physical edge line)
        /// </summary>
        /// <param name="edgeViewModel">The view model for the edge view to be created</param>
        private void AddEdgeView(IEdgeViewModel edgeViewModel)
        {
            // Draw the physical edge (the line) on the graph surface
            edgeViewModel.DrawEdgeLine(graphSurface);
        }

        /// <summary>
        /// Remove an edge view (physical edge line)
        /// </summary>
        /// <param name="edgeViewModel">The view model for the edge view to be removed</param>
        private void RemoveEdgeView(IEdgeViewModel edgeViewModel)
        {
            // Remove the physical edge (the line) from the graph surface
            edgeViewModel.EraseEdgeLine(graphSurface);
        }
    }
}