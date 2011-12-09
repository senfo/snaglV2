//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Berico.SnagL.Infrastructure.Graph;

namespace Berico.SnagL.Infrastructure.Controls
{
    
    /// <summary>
    /// A custom ItemsControl class that overcomes an issue with ItemsControl
    /// that prevents you from setting properties (Left, Top and ZIndex)
    /// on DataTemplate items against a canvas in the ItemsPanel template.
    /// </summary>
    public class GraphItemsControl : ItemsControl
    {
        /// <summary>
        /// Prepares the specified element to display the specified item.  Additional
        /// functionality has been added to ensure that the specified element has it's
        /// canvas properties set correctly.
        /// </summary>
        /// <param name="element">The element used to display the specified item</param>
        /// <param name="item">The item to display</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            FrameworkElement contentItem = element as FrameworkElement;

            if (!(item is EdgeViewModelBase))
            {
                Binding leftBinding = new Binding("Position.X");
                Binding topBinding = new Binding("Position.Y");
                Binding zIndexBinding = new Binding("ZIndex");

                contentItem.SetBinding(Canvas.LeftProperty, leftBinding);
                contentItem.SetBinding(Canvas.TopProperty, topBinding);
                contentItem.SetBinding(Canvas.ZIndexProperty, zIndexBinding);
            }

            base.PrepareContainerForItemOverride(element, item);
        }

    }
}