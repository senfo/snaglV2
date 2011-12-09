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
    /// This class is a custom behavior that is used to bind the SizeChanged
    /// event to a Command delegate.  Typically we would use an event Trigger
    /// and the EventToCommand but it doesn't seem to work with the SizeChanged
    /// event.
    /// </summary>
    public class SizeChangedHandlerBehavior : Behavior<UIElement>
    {

        /// <summary>
        /// This method is executed when the behavior is attached to an object.  We
        /// use it to wire up our event handlers and set up the internal timer.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            (AssociatedObject as FrameworkElement).SizeChanged += new SizeChangedEventHandler(SizeChangedBehavior_SizeChanged);
        }

        /// <summary>
        /// This method is executed when the behavior is being detached.  We use it
        /// to unhook the MouseLeftButtonDown event.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            (AssociatedObject as FrameworkElement).SizeChanged -= new SizeChangedEventHandler(SizeChangedBehavior_SizeChanged);
        }

        #region Properties

            #region PassEventArgsToCommand

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.SizeChangedHandlerBehavior.PassEventArgsToCommandProperty property.  This
                /// indicates whether event arguments should be passed to the command when it is called.
                /// </summary>
                public static readonly DependencyProperty PassEventArgsToCommandProperty = DependencyProperty.Register("PassEventArgsToCommand", typeof(bool), typeof(SizeChangedHandlerBehavior), new PropertyMetadata(true, null));

                /// <summary>
                ///  Gets or sets the PassEventArgsToCommand property for the Berico.LinkAnalysis.Assets.SizeChangedHandlerBehavior
                /// </summary>
                public bool PassEventArgsToCommand
                {
                    get { return (bool)GetValue(PassEventArgsToCommandProperty); }
                    set { SetValue(PassEventArgsToCommandProperty, value); }
                }

            #endregion

            #region Command

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.SizeChangedHandlerBehavior.CommandProperty property.  This
                /// indicates the name of the command that should be called.
                /// </summary>
                public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SizeChangedHandlerBehavior), new PropertyMetadata(null));

                /// <summary>
                /// Gets or sets the Command property for the Berico.LinkAnalysis.Assets.SizeChangedHandlerBehavior
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
            /// Handles the SizeChanged event
            /// </summary>
            /// <param name="sender">The object where the event handler is attached</param>
            /// <param name="e">The event data</param>
            void SizeChangedBehavior_SizeChanged(object sender, SizeChangedEventArgs e)
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