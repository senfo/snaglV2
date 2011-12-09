//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Provides the contract for all toolbar item extension views.  A toolbar
    /// item extension represents an item (some control) on the toolbar.  
    /// This is primarily used by MEF for importing and exporting.
    /// </summary>
    public interface IToolPanelItemViewExtension
    {

        /// <summary>
        /// We are breaking the "no code in the view" rule here, but that is an often
        /// argued rule.  This is a minor code addition to the view that precludes the
        /// need for another locator.  In the future we can use another locator for
        /// this.
        /// </summary>
        IToolPanelItemViewModelExtension ViewModel { get; set; }

    }
}