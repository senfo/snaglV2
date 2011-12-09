using System.Windows;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// A structure containing properties that represent a nodes position
    /// </summary>
    public struct NodePosition
    {
        #region Properties

        /// <summary>
        /// Gets or sets the nodes coordinates
        /// </summary>
        public Point Coordinates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current angle
        /// </summary>
        public double CurrentAngle
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="NodePosition" /> structure
        /// </summary>
        /// <param name="coordinates">Specifies the nodes coordinates</param>
        public NodePosition(Point coordinates)
            : this(coordinates, double.NaN)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="NodePosition" /> structure
        /// </summary>
        /// <param name="coordinates">Specifies the nodes coordinates</param>
        /// <param name="currentAngel">Specifies the nodes current angle</param>
        public NodePosition(Point coordinates, double currentAngel)
            : this()
        {
            Coordinates = coordinates;
            CurrentAngle = currentAngel;
        }

        #endregion
    }
}
