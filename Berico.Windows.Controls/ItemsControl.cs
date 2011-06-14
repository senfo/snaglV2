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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Berico.Windows.Controls
{

    /// <summary>
    /// Enhances the System.Windows.Controls.ItemsControl class.  The ItemsControl class
    /// Represents a control that can be used to present a collection of items
    /// </summary>
    public class ItemsControl : System.Windows.Controls.ItemsControl
    {

        /// <summary>
        /// Initializes a new instance of the ItemsControl class.
        /// </summary>
        public ItemsControl() : base() { }

        /// <summary>
        /// Returns whether the ItemsControl has any items.
        /// </summary>
        public bool HasItems
        {
            get
            {
                if (this.Items.Count > 0)
                    return true;
                else
                    return false;
            }
        }

    }
}