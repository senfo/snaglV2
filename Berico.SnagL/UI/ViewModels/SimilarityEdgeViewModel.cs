//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows.Media;
using System.Windows.Shapes;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;
using GalaSoft.MvvmLight.Threading;
using Berico.SnagL.UI;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Provides the code for an edge view that represents
    /// a similarity edge
    /// </summary>
    public class SimilarityEdgeViewModel : EdgeViewModelBase
    {

        /// <summary>
        /// Create a new instance of the Berico.LinkAnalysis.ViewModel.SimilarityEdgeViewModel
        /// class using the provided parent edge
        /// </summary>
        /// <param name="parentEdge">The parent edge for this edge view model</param>
        /// <param name="scope">Identifies the scope of this edge view model</param>
        public SimilarityEdgeViewModel(SimilarityDataEdge parentEdge, string scope) : base(parentEdge, scope) { }

        /// <summary>
        /// Performs initialization
        /// </summary>
        protected override void Initialize()
        {

            // Specify the style for the edge line
            //EdgeLine edgeLine = new EdgeLine(ParentEdge.Type)
            //{
            //    Opacity = 1,
            //    Color = new SolidColorBrush(Colors.Green),
            //    Thickness = 2,
            //    StrokeDashArray = new DoubleCollection { 2.0, 2.0 }
            //};

            this.EdgeLine = new EdgeLine(ParentEdge.Type, false);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AttributeValuePropertyChangedHandler(object sender, Common.Events.PropertyChangedEventArgs<object> e)
        {
            base.AttributeValuePropertyChangedHandler(sender, e);

            // If an attribute value has changed on the target or source node
            // we may need to recalculate the similarity for it (which
            // is used as the edges Weight)

            //TODO:  NEED A WAY TO KNOW WHAT ATTRIBUTES ARE BEING CLUSTERED ON AND ONLY RECALCULATE ONLY IF ONE OF THOSE CHANGED
            RecalculateWeight();
        }

        /// <summary>
        /// Recomputes the Weight for this edge.  This should only need
        /// need be executed, at this level, if the attribute values
        /// for the source or target node were changed.
        /// </summary>
        private void RecalculateWeight()
        {
            // Computer the similarity between the two nodes
            // that make up this edge
            //double similarity = AttributeSimilarityManager.Instance.ComputeNodeSimilarity(ParentEdge.Source, ParentEdge.Target);

            //JOSH:  WHAT IS THE PURPOSE OF THIS??
            //similarity = similarity * similarity * similarity;

            //JOSH:  WHAT IS THIS DOING?  NORMALIZING?
            //if (similarity < WEIGHT_THRESHOLD)
            //   similarity = 0;
            //else if (similarity > 1)
            //    similarity = 1;

            //JOSH: SHOULD THE SLIDER WEIGHT EFFECT THIS OR WILL THAT COME OUT IN THE ATTRIBUTE SIMILARITY CALCULATION??
            //(this.ParentEdge as SimilarityDataEdge).Weight = similarity;
        }

    }
}