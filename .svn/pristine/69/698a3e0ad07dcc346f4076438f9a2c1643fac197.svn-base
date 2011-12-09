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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Berico.Common;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// This class represents the base view model for all edge views
    /// </summary>
    public abstract class EdgeViewModelBase : ViewModelBase, IEdgeViewModel, IScopingContainer<string>
    {
        private IEdge parentEdge;
        private IEdgeLine edgeLine;
        private string scope = string.Empty;

        /// <summary>
        /// Initializes an edge.  This method must be implemented 
        /// by any parent classes.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Initializes a new instance of the EdgeViewModelBase class.
        /// </summary>
        /// <param name="edge">The Berico.LinkAnalysis.Model.Edge for this edge</param>
        /// <param name="scope">Identifies the scope of this edge view model</param>
        protected EdgeViewModelBase(IEdge edge, string _scope)
        {
            parentEdge = edge;
            scope = _scope;

            // Wire up event hanlders
            if (edge is DataEdge)
            {
                Node sourceNode = this.ParentEdge.Source as Node;
                Node targetNode = this.ParentEdge.Target as Node;

                sourceNode.Attributes.AttributeValuePropertyChanged += new EventHandler<Common.Events.PropertyChangedEventArgs<object>>(AttributeValuePropertyChangedHandler);
                sourceNode.Attributes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedHandler);
                targetNode.Attributes.AttributeValuePropertyChanged += new EventHandler<Common.Events.PropertyChangedEventArgs<object>>(AttributeValuePropertyChangedHandler);
                targetNode.Attributes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CollectionChangedHandler);

                edge.PropertyChanged += new EventHandler<Common.Events.PropertyChangedEventArgs<object>>(edge_PropertyChanged);
            }

            Initialize();

            // Wire up events for the EdgeLine
            edgeLine.LineMouseEnter += new EventHandler<System.Windows.Input.MouseEventArgs>(edgeLine_LineMouseEnter);
            edgeLine.LineMouseLeave += new EventHandler<System.Windows.Input.MouseEventArgs>(edgeLine_LineMouseLeave);
        }

        #region Properties

            #region IEdgeViewModel Properties

                /// <summary>
                /// Gets the parent Edge that the view and viewmodel represent
                /// </summary>
                public IEdge ParentEdge
                {
                    get { return parentEdge; }
                }

                /// <summary>
                /// Gets or sets the X property for the starting
                /// point of the line
                /// </summary>
                public double X1
                {
                    get { return this.edgeLine.X1; }
                    set { this.edgeLine.X1 = value; }
                }

                /// <summary>
                /// Gets or sets the Y property for the starting
                /// point of the line
                /// </summary>
                public double Y1
                {
                    get { return this.edgeLine.Y1; }
                    set { this.edgeLine.Y1 = value; }
                }

                /// <summary>
                /// Gets or sets the X property for the ending 
                /// point of the line
                /// </summary>
                public double X2
                {
                    get { return this.edgeLine.X2; }
                    set { this.edgeLine.X2 = value; }
                }

                /// <summary>
                /// Gets or sets the Y property for the ending
                /// point of the line
                /// </summary>
                public double Y2
                {
                    get { return this.edgeLine.Y2; }
                    set { this.edgeLine.Y2 = value; }
                }

                /// <summary>
                /// Gets or sets the edge's thickness
                /// </summary>
                [ExportableProperty("Thickness")]
                public double Thickness
                {
                    get { return this.edgeLine.Thickness; }
                    set { this.edgeLine.Thickness = value; }
                }

                /// <summary>
                /// Gets or sets the color of the edge
                /// </summary>
                [ExportableProperty("Color")]
                public Brush Color
                {
                    get { return this.edgeLine.Color; }
                    set { this.edgeLine.Color = value; }
                }

                /// <summary>
                /// Gets or sets the background color of the label
                /// </summary>
                [ExportableProperty("LabelBackgroundColor")]
                public Brush LabelBackgroundColor
                {
                    get { return this.edgeLine.LabelBackgroundColor; }
                    set { this.edgeLine.LabelBackgroundColor = value; }
                }

                /// <summary>
                /// Gets or sets the foreground color of the label
                /// </summary>
                [ExportableProperty("LabelForegroundColor")]
                public Brush LabelForegroundColor
                {
                    get { return this.edgeLine.LabelForegroundColor; }
                    set { this.edgeLine.LabelForegroundColor = value; }
                }

                /// <summary>
                /// Gets or sets the style (italics) of the label font
                /// </summary>
                [ExportableProperty("LabelFontStyle")]
                public FontStyle LabelFontStyle
                {
                    get { return this.edgeLine.LabelFontStyle; }
                    set { this.edgeLine.LabelFontStyle = value; }
                }

                /// <summary>
                /// Gets or sets the weight (boldness) of the label font
                /// </summary>
                [ExportableProperty("LabelFontWeight")]
                public FontWeight LabelFontWeight
                {
                    get { return this.edgeLine.LabelFontWeight; }
                    set { this.edgeLine.LabelFontWeight = value; }
                }

                /// <summary>
                /// Gets or sets whether the label text is underlined
                /// </summary>
                [ExportableProperty("LabelTextUnderline")]
                public bool LabelTextUnderline
                {
                    get { return this.edgeLine.LabelTextUnderline; }
                    set { this.edgeLine.LabelTextUnderline = value; }
                }

                /// <summary>
                /// Gets or sets the font used for the label text
                /// </summary>
                [ExportableProperty("LabelFont")]
                public FontFamily LabelFont
                {
                    get { return this.edgeLine.LabelFont; }
                    set { this.edgeLine.LabelFont = value; }
                }

                /// <summary>
                /// Gets or sets whether the edge is current visible or not
                /// </summary>
                public bool IsHidden { get; private set; }

                /// <summary>
                /// Gets or sets whether this edge is visible or not.  This 
                /// property is connected directly to the edge's Line.
                /// </summary>
                public virtual Visibility Visibility
                {
                    get { return this.edgeLine.Visibility; }
                    set
                    {
                        this.edgeLine.Visibility = value;

                        if (value == Visibility.Collapsed)
                            IsHidden = true;
                        else
                            IsHidden = false;
                    }
                }

                /// <summary>
                /// Gets the physical line for this edge
                /// </summary>
                public IEdgeLine EdgeLine
                {
                    get { return this.edgeLine; }
                    set { this.edgeLine = value; }
                }

            #endregion

            #region IScopingContainer Properties

                /// <summary>
                /// In this case, the scope is the parent GraphComponents
                /// instance that initially created the edge view model.
                /// This should never change during the lifetime of this
                /// object.
                /// </summary>
                public string Scope
                {
                    get { return this.scope; }
                    private set { this.scope = value; }
                }

            #endregion

        #endregion

        #region Events and EventHandlers

            /// <summary>
            /// Handles the event that the source or target's attribute 
            /// collection has changed
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            protected virtual void CollectionChangedHandler(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                // This handles the event that the source or target
                // node's attribute collection has changed
            }

            /// <summary>
            /// Handles the event that the source or target node's properties
            /// have changed
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            protected virtual void AttributeValuePropertyChangedHandler(object sender, Common.Events.PropertyChangedEventArgs<object> e)
            {
                // This handles the event that at least on of the source
                // or target node's properties have changed
            }

            /// <summary>
            /// Handles the EdgeLine.LineMouseLeave event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            void edgeLine_LineMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
            {
                SnaglEventAggregator.DefaultInstance.GetEvent<EdgeMouseLeaveEvent>().Publish(new EdgeViewModelMouseEventArgs<MouseEventArgs>(this, e, this.scope));
            }

            /// <summary>
            /// Handles the EdgeLine.LineMouseEnter event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            void edgeLine_LineMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
            {
                SnaglEventAggregator.DefaultInstance.GetEvent<EdgeMouseEnterEvent>().Publish(new EdgeViewModelMouseEventArgs<MouseEventArgs>(this, e, this.scope));
            }

            /// <summary>
            /// Handles the PropertyChanged event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            void edge_PropertyChanged(object sender, Common.Events.PropertyChangedEventArgs<object> e)
            {

                // Check if the edges DisplayValue property was changed
                if (e.PropertyName == "DisplayValue")
                    edgeLine.Text = e.NewValue as string;

            }

        #endregion

        #region External Methods

            /// <summary>
            /// Returns a new IEdgeViewModel that is a copy of the current
            /// edge view model but with the specified source and target
            /// nodes
            /// </summary>
            /// <param name="source">The source Node for this edge</param>
            /// <param name="target">The target Node for this edge</param>
            /// <returns>an edge view model</returns>
            public virtual IEdgeViewModel Copy(INode source, INode target)
            {

                // Create a copy of the parent edgeLine
                IEdge newEdge = this.ParentEdge.Copy(source, target);

                // Create a new edge view model based on the the type
                // of the new edge that was just created
                //EdgeViewModelBase oldEdgeVM = GetEdgeViewModel(newEdge);
                IEdgeViewModel newEdgeVM = null;

                //TODO: TRY AND UPDATE TO USE GENERICS

                if (this is SimilarityEdgeViewModel)
                {
                    newEdgeVM = new SimilarityEdgeViewModel(newEdge as SimilarityDataEdge, this.Scope)
                    {
                        Visibility = this.Visibility,
                        EdgeLine = new EdgeLine(ParentEdge.Type)
                        {
                            Opacity = this.EdgeLine.Opacity,
                            Color = this.EdgeLine.Color,
                            Thickness = this.EdgeLine.Thickness,
                            LabelForegroundColor = this.EdgeLine.LabelForegroundColor,
                            LabelBackgroundColor = this.EdgeLine.LabelBackgroundColor,
                            LabelFontStyle = this.EdgeLine.LabelFontStyle,
                            LabelFontWeight = this.EdgeLine.LabelFontWeight,
                            LabelTextUnderline = this.EdgeLine.LabelTextUnderline,
                            LabelFont = this.EdgeLine.LabelFont
                        }
                    };
                }
                else
                {
                    newEdgeVM = new StandardEdgeViewModel(newEdge, this.Scope)
                    {
                        Visibility = this.Visibility,
                        EdgeLine = new EdgeLine(ParentEdge.Type)
                        {
                            Opacity = this.EdgeLine.Opacity,
                            Color = this.EdgeLine.Color,
                            Thickness = this.EdgeLine.Thickness,
                            LabelForegroundColor = this.EdgeLine.LabelForegroundColor,
                            LabelBackgroundColor = this.EdgeLine.LabelBackgroundColor,
                            LabelFontStyle = this.EdgeLine.LabelFontStyle,
                            LabelFontWeight = this.EdgeLine.LabelFontWeight,
                            LabelTextUnderline = this.EdgeLine.LabelTextUnderline,
                            LabelFont = this.EdgeLine.LabelFont
                        }
                    };
                }

                return newEdgeVM;
            }

            /// <summary>
            /// This factory method creates and returns a new Berico.LinkAnalysis.ViewModel.
            /// EdgeViewModelBase instance based on the provided IEdge
            /// </summary>
            /// <param name="edge">The underlying data for the edge</param>
            /// <returns>a new Berico.LinkAnalysis.ViewModel.EdgeViewModelBase instance</returns>
            public static EdgeViewModelBase GetEdgeViewModel(IEdge edge, string scope)
            {

                // Create the appropriate viewmodel for the edge based on 
                // the type of edge that it is
                if (edge is SimilarityDataEdge)
                    return new SimilarityEdgeViewModel(edge as SimilarityDataEdge, scope);
                else if (CheckIfDuplexEdge(edge, scope))
                    return new DuplexEdgeViewModel(edge as Model.Edge, scope);
                else
                    return new StandardEdgeViewModel(edge as Model.Edge, scope);

            }

            /// <summary>
            /// Returns the hash code for this edge view model.  The hash
            /// is based off of the hash of the underlying Berico.LinkAnalysis.
            /// Model.Edge instance.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code</returns>
            public override int GetHashCode()
            {
                return this.parentEdge.GetHashCode();
            }

            /// <summary>
            /// Determines whether this instance of Berico.LinkAnalysis.ViewModel.EdgeViewModel
            /// and a specified object, which must also be a Berico.LinkAnalysis.ViewModel.EdgeViewModel
            /// object, have the same value.  This method calls the Equals method of the underlying
            /// Berico.LinkAnalysis.Model.Edge instance.
            /// </summary>
            /// <param name="obj">A System.Object</param>
            /// <returns>true if obj is a Berico.LinkAnalysis.ViewModel and its value is the 
            /// same as this instance; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                return this.parentEdge.Equals(obj);
            }

            public override void Cleanup()
            {
                base.Cleanup();
            }

            #region IEdgeViewModel Methods

                /// <summary>
                /// Draws the edge line (and all its parts) on the provided container
                /// </summary>
                /// <param name="container">The container that the edge will be added to</param>
                public void DrawEdgeLine(Canvas container)
                {
                    container.Children.Add(EdgeLine as UIElement);
                }

                /// <summary>
                /// Erases the edge line (and all its parts) from the provided container
                /// </summary>
                /// <param name="container">The container that the edge currently resides on</param>
                public void EraseEdgeLine(Canvas container)
                {
                    container.Children.Remove(EdgeLine as UIElement);
                }

            #endregion

        #endregion

        #region Internal Methods

            private static bool CheckIfDuplexEdge(IEdge edge, string scope)
            {
                //TODO:  DETERMINE IF THIS IS A DUPLEX EDGE
                return false;
            }

        #endregion

    }
}