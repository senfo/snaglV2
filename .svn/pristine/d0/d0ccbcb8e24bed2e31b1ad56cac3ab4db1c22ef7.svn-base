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
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;

namespace Berico.Common.Modularity
{

    /// <summary>
    /// Represents a custom ComposablePartCatalog that returns a filtered list of
    /// ComposablePartDefinition objects.  This provides a means to reduce the number
    /// of parts that would typically be returned by a catalog.
    /// </summary>
    public class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly ComposablePartCatalog _inner;
        private readonly INotifyComposablePartCatalogChanged _innerNotifyChange;
        private readonly IQueryable<ComposablePartDefinition> _partsQuery;

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.SnagL.Modularity.FilteredCatalog
        /// class.  The specified ComposablePartCatalog will use the specified  expression to 
        /// perform a LINQ Where query in order to filter the parts in the catalog.
        /// </summary>
        /// <param name="inner">The ComposablePartCatalog that will be filtered</param>
        /// <param name="expression">A Func delegate</param>
        public FilteredCatalog(ComposablePartCatalog inner,
                               Expression<Func<ComposablePartDefinition, bool>> expression)
        {
            _inner = inner;
            _innerNotifyChange = inner as INotifyComposablePartCatalogChanged;
            _partsQuery = inner.Parts.Where(expression);
        }

        /// <summary>
        /// Gets the part definitions that are contained in the filtered catalog
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _partsQuery;
            }
        }

        /// <summary>
        /// Occurs when a System.ComponentModel.Composition.Primitives.ComposablePartCatalog
        /// has changed
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changed += value;
            }
            remove
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changed -= value;
            }
        }

        /// <summary>
        /// Occurs when a System.ComponentModel.Composition.Primitives.ComposablePartCatalog
        /// is changing
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changing += value;
            }
            remove
            {
                if (_innerNotifyChange != null)
                    _innerNotifyChange.Changing -= value;
            }
        }
    }

}