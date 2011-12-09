using System.Collections.Generic;
using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Modularity;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// Contains methods for initializing instances of <see cref="LayoutBase"/> classes.
    /// </summary>
    public class LayoutManager
    {
        #region Fields

        /// <summary>
        /// Stores a reference to the last layout type used
        /// </summary>
        private LayoutBase _activeLayout;

        /// <summary>
        /// Stores the instances of layouts accessible by type name
        /// </summary>
        private Dictionary<string, LayoutBase> _layouts = new Dictionary<string, LayoutBase>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the active layout
        /// </summary>
        public LayoutBase ActiveLayout
        {
            get
            {
                return _activeLayout ?? DefaultLayout;
            }
            private set
            {
                _activeLayout = value;
            }
        }

        /// <summary>
        /// Stores the types that MEF imports. This property is required by MEF and shouldn't be used for access to instances.
        /// </summary>
        [ImportMany(typeof(LayoutBase), AllowRecomposition = true)]
        public List<LayoutBase> Catalog
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to the default layout type
        /// </summary>
        public LayoutBase DefaultLayout
        {
            get
            {
                return GetLayoutByName(InternalLayouts.Network);
            }
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="LayoutManager"/> class
        /// </summary>
        public static LayoutManager Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents an instance of the <see cref="LayoutManager"/> class from being instantiated
        /// </summary>
        private LayoutManager()
        {
            // Compose the parts of this class
            ExtensionManager.ComposeParts(this);

            foreach (LayoutBase layout in Catalog)
            {
                _layouts.Add(layout.LayoutName, layout);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an instance of the specified layout
        /// </summary>
        /// <param name="layout">The full name of the layout type for which to return</param>
        /// <returns>An instance of the requested LayoutBase class</returns>
        public LayoutBase GetLayoutByName(string layout)
        {
            ActiveLayout = _layouts[layout];

            return ActiveLayout;
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Private, nested class to implement a singleton
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Stores the singleton instance of the <see cref="LayoutManager"/> class
            /// </summary>
            internal static readonly LayoutManager instance = new LayoutManager();

            /// <summary>
            /// Initializes a new instance of the <see cref="Nested"/> class
            /// </summary>
            static Nested()
            {

            }
        }

        #endregion
    }
}
