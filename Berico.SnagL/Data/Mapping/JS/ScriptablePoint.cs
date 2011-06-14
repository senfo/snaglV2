//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Mapping.JS
{
    using System.Windows.Browser;

    /// <summary>
    /// A scriptable version of the Point class
    /// </summary>
    [ScriptableType()]
    public class ScriptablePoint
    {
        #region Properties

        /// <summary>
        /// Gets or sets the X coordinate
        /// </summary>
        [ScriptableMember()]
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y coordinate
        /// </summary>
        [ScriptableMember()]
        public double Y
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptablePoint"/> class
        /// </summary>
        public ScriptablePoint()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptablePoint"/> class
        /// </summary>
        /// <param name="x">Specifies the X coordinate</param>
        /// <param name="y">Specifies the Y coordinate</param>
        public ScriptablePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion
    }
}
