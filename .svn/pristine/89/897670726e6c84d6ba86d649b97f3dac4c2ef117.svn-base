//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows.Interactivity;
using System.Windows.Threading;
using System;
using System.Windows;
using System.Windows.Input;

namespace Berico.Common.UI.Behaviors
{
    /// <summary>
    /// This class is a custom behavior that is used to bind the LayoutUpdated
    /// event to a Command delegate.  Typically we would use an event Trigger
    /// and the EventToCommand but it doesn't seem to work with the LayoutUpdated
    /// event.
    /// </summary>
    public class LayoutUpdatedHandlerBehavior : Behavior<UIElement>
    {

        /// <summary>
        /// This method is executed when the behavior is attached to an object.  We
        /// use it to wire up our event handler for the LayoutUpdated event.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            (AssociatedObject as FrameworkElement).LayoutUpdated += new EventHandler(LayoutChangedBehavior_LayoutUpdated);
        }

        /// <summary>
        /// This method is executed when the behavior is being detached.  We use it
        /// to unhook the LayoutUpdated event.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            (AssociatedObject as FrameworkElement).LayoutUpdated -= new EventHandler(LayoutChangedBehavior_LayoutUpdated);
        }

        #region Properties

            #region PassEventArgsToCommand

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.LayoutUpdatedHandlerBehavior.PassEventArgsToCommandProperty property.  This
                /// indicates whether event arguments should be passed to the command when it is called.
                /// </summary>
                public static readonly DependencyProperty PassEventArgsToCommandProperty = DependencyProperty.Register("PassEventArgsToCommand", typeof(bool), typeof(LayoutUpdatedHandlerBehavior), new PropertyMetadata(true, null));

                /// <summary>
                ///  Gets or sets the PassEventArgsToCommand property for the Berico.LinkAnalysis.Assets.LayoutUpdatedHandlerBehavior
                /// </summary>
                public bool PassEventArgsToCommand
                {
                    get { return (bool)GetValue(PassEventArgsToCommandProperty); }
                    set { SetValue(PassEventArgsToCommandProperty, value); }
                }

            #endregion

            #region Command

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.LayoutUpdatedHandlerBehavior.CommandProperty property.  This
                /// indicates the name of the command that should be called.
                /// </summary>
                public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(LayoutUpdatedHandlerBehavior), new PropertyMetadata(null));

                /// <summary>
                /// Gets or sets the Command property for the Berico.LinkAnalysis.Assets.LayoutUpdatedHandlerBehavior
                /// </summary>
                public ICommand Command
                {
                    get { return (ICommand)GetValue(CommandProperty); }
                    set { SetValue(CommandProperty, value); }
                }

            #endregion

        #endregion

        #region Events

            /// <summary>
            /// Handles the LayoutUpdated event
            /// </summary>
            /// <param name="sender">The object where the event handler is attached</param>
            /// <param name="e">The event data</param>
            private void LayoutChangedBehavior_LayoutUpdated(object sender, EventArgs e)
            {
                if (Command != null)
                {
                    // Call the Command (providing event arguments
                    // if configured to do so)
                    if (PassEventArgsToCommand)
                        Command.Execute(e);
                    else
                        Command.Execute(null);
                }
            }

        #endregion

    }
}