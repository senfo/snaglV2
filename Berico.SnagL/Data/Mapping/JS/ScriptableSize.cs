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
    /// A scriptable version of the Size class
    /// </summary>
    [ScriptableType()]
    public class ScriptableSize
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Width
        /// </summary>
        [ScriptableMember()]
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        [ScriptableMember()]
        public double Height
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptableSize"/> class
        /// </summary>
        public ScriptableSize()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptableSize"/> class
        /// </summary>
        /// <param name="height">The height</param>
        /// <param name="width">The width</param>
        public ScriptableSize(double height, double width)
        {
            Height = height;
            Width = width;
        }

        #endregion
    }
}
