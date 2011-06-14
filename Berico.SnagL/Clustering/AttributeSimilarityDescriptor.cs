using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// Provides a description, of a given similarity criterion
    /// control, that is used for clustering
    /// </summary>
    public class AttributeSimilarityDescriptor
    {
        /// <summary>
        /// Creaes a new instance of AttributeSimilarityDescriptor using
        /// the provided property values
        /// </summary>
        public AttributeSimilarityDescriptor(string attributeName, ISimilarityMeasure similarityMeasure, double weight)
        {
            AttributeName = attributeName;
            SimilarityMeasure = similarityMeasure;
            Weight = weight;
        }

        /// <summary>
        /// 
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ISimilarityMeasure SimilarityMeasure { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}:{1}:{2}]", AttributeName, SimilarityMeasure.ToString(), Weight.ToString());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals(obj as AttributeSimilarityDescriptor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(AttributeSimilarityDescriptor obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetHashCode() != obj.GetHashCode())
                return false;

            return (this.ToString().Equals(obj.ToString()));
        }
    }
}
