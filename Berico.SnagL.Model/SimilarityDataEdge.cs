//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.Common;
using Berico.SnagL.Model.Attributes;

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Represents a relationship between two nodes that
    /// is based on how similar the two nodes are.  These
    /// edges are created during clustering.
    /// </summary>
    public class SimilarityDataEdge : DataEdge
    {
        // TODO:  THE VALUES BELOW SHOULD BE CONFIGURABLE
        private const double MINIMUM_SPRING_LENGTH = 75;
        private const double MAXIMUM_SPRING_LENGTH = 400;
        private const double MINIMUM_SPRING_STIFFNESS = 0.01;
        private const double MAXIMUM_SPRING_STIFFNESS = 1.0;
        // JOSH:  ANY IDEA WHAT THE ABOVE VALUES ARE FOR??

        private double weight;
        private double minimumSpringLength = MINIMUM_SPRING_LENGTH;
        private double maximumSpringLength = MAXIMUM_SPRING_LENGTH;
        private double minimumSpringStiffness = MINIMUM_SPRING_STIFFNESS;
        private double maximumSpringStiffness = MAXIMUM_SPRING_STIFFNESS;

        /// <summary>
        /// Initialzies a new instance of Berico.LinkAnalysis.Model.Edge with
        /// the provided source and target nodes.  The source and target nodes
        /// can not be null and can not be the same.
        /// </summary>
        /// <param name="initialWeight">The precalculated weight value</param>
        /// <param name="_source">The source Node for this edge</param>
        /// <param name="_target">The target Node for this edge</param>
        public SimilarityDataEdge(double initialWeight, INode _sourceNode, INode _targetNode)
            : base(_sourceNode, _targetNode)
        {
            // Set the initial weight.  This will be the similarity value that
            // was calculated before this edge was created
            this.weight = initialWeight;
        }

        /// <summary>
        /// Initialzies a new instance of Berico.LinkAnalysis.Model.Edge with
        /// the provided source and target nodes.  The source and target nodes
        /// can not be null and can not be the same.
        /// </summary>
        /// <param name="initialWeight">The precalculated weight value</param>
        /// <param name="_source">The source Node for this edge</param>
        /// <param name="_target">The target Node for this edge</param>
        /// <param name="_attributes">An existing AttributeCollection instance</param> 
        public SimilarityDataEdge(double initialWeight, INode _sourceNode, INode _targetNode, AttributeCollection _attributes)
            : base(_sourceNode, _targetNode, _attributes)
        {
            // Set the initial weight.  This will be the similarity value that
            // was calculated before this edge was created
            this.weight = initialWeight;
        }

        /// <summary>
        /// Gets the weight for this edge.  The weight is based on how
        /// similar the source and target nodes are to each other.
        /// </summary>
        [ExportableProperty("Weight")]
        public double Weight
        {
            get { return this.weight; }
        }

        /// <summary>
        /// Gets the spring stiffness, used by the force dirceted
        /// layout, for this edge
        /// </summary>
        public double SpringStiffness
        {
            get
            {
                //JOSH: WHAT IS THIS DOING?
                return (MaximumSpringStiffness - MinimumSpringStiffness) * Weight + MinimumSpringStiffness;
            }
        }

        /// <summary>
        /// Gets the spring length, used by the force directed
        /// layout, for this edge
        /// </summary>
        public double SpringLength
        {
            get
            {
                //JOSH: WHAT IS THIS DOING?
                return (MaximumSpringLength - MinimumSpringLength) * (1 - Weight) + MinimumSpringLength;
            }
        }

        /// <summary>
        /// Gets or sets the minimum spring length for this edge.  This value
        /// will effect the ForceDirected layout.
        /// </summary>
        public double MinimumSpringLength
        {
            get { return this.minimumSpringLength; }
            set
            {
                double oldValue = this.minimumSpringLength;
                this.minimumSpringLength = value;

                NotifyPropertyChanged("MinimumSpringLength", oldValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum spring length for this edge.  This value
        /// will effect the ForceDirected layout.
        /// </summary>
        public double MaximumSpringLength
        {
            get { return this.maximumSpringLength; }
            set
            {
                double oldValue = this.maximumSpringLength;
                this.maximumSpringLength = value;

                NotifyPropertyChanged("MaximumSpringLength", oldValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum spring stiffness.  This value
        /// will effect the ForceDirected layout.
        /// </summary>
        public double MinimumSpringStiffness
        {
            get { return this.minimumSpringLength; }
            set
            {
                double oldValue = this.minimumSpringLength;
                this.minimumSpringLength = value;

                NotifyPropertyChanged("MinimumSpringStiffness", oldValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum spring stiffness.  This value
        /// will effect the ForceDirected layout.
        /// </summary>
        public double MaximumSpringStiffness
        {
            get { return this.maximumSpringStiffness; }
            set
            {
                double oldValue = this.maximumSpringLength;
                this.maximumSpringStiffness = value;

                NotifyPropertyChanged("MaximumSpringStiffness", oldValue, value);
            }
        }

        /// <summary>
        /// Creates a new edge that is of the appropriate type.  The
        /// new edge will be a copy of the current edge but with a
        /// different source and target.
        /// </summary>
        /// <param name="source">The source Node</param>
        /// <param name="target">The target Node</param>
        /// <returns>a new edge</returns>
        public override IEdge Copy(INode source, INode target)
        {
            //TODO:  SHOULD BE UPDATED TO BE A MORE COMPLETE COPY

            SimilarityDataEdge newEdge = new SimilarityDataEdge(this.weight, source, target, Attributes)
            {
                ID = this.ID,
                Description = this.Description,
                DisplayValue = this.DisplayValue,
                MinimumSpringLength = this.minimumSpringLength,
                MaximumSpringLength = this.maximumSpringLength,
                MinimumSpringStiffness = this.minimumSpringStiffness,
                MaximumSpringStiffness = this.maximumSpringStiffness
            };

            return newEdge;
        }

    }

}