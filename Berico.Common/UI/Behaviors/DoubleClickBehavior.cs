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
    /// This class represents a custom behavior that provides double-click
    /// functionality.
    /// </summary>
    public class DoubleClickBehvaior : Behavior<UIElement>
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// This method is executed when the behavior is attached to an object.  We
        /// use it to wire up our event handlers and set up the internal timer.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(DoubleClickSpeed);
        }

        /// <summary>
        /// This method is executed when the behavior is being detached.  We use it
        /// to unhook the MouseLeftButtonDown event.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
        }

        #region Properties

            #region DoubleClickSpeed

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.DoubleClickBehavior.DoubleClickSpeedProperty property.  This
                /// indicates the desired time, in seconds between clicks.
                /// </summary>
                public static readonly DependencyProperty DoubleClickSpeedProperty = DependencyProperty.Register("DoubleClickSpeed", typeof(double), typeof(DoubleClickBehvaior), new PropertyMetadata(0.25, new PropertyChangedCallback(OnDoubleClickSpeedChanged)));

                /// <summary>
                /// Handles when the DoubleClickSpeedProperty changes
                /// </summary>
                /// <param name="sender">The object whose double click property changed</param>
                /// <param name="args">The event data</param>
                private static void OnDoubleClickSpeedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
                {
                    DoubleClickBehvaior doubleClick = sender as DoubleClickBehvaior;
                    doubleClick.timer.Interval = TimeSpan.FromSeconds(doubleClick.DoubleClickSpeed);
                }

                /// <summary>
                /// Gets or sets the DoubleClickSpeed property for the Berico.LinkAnalysis.Assets.DoubleClickBehavior
                /// </summary>
                public double DoubleClickSpeed
                {
                    get { return (double)GetValue(DoubleClickSpeedProperty); }
                    set { SetValue(DoubleClickSpeedProperty, value); }
                }

            #endregion

            #region PassEventArgsToCommand

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.DoubleClickBehavior.PassEventArgsToCommandProperty property.  This
                /// indicates whether event arguments should be passed to the command when it is called.
                /// </summary>
                public static readonly DependencyProperty PassEventArgsToCommandProperty = DependencyProperty.Register("PassEventArgsToCommand", typeof(bool), typeof(DoubleClickBehvaior), new PropertyMetadata(true, null));

                /// <summary>
                ///  Gets or sets the PassEventArgsToCommand property for the Berico.LinkAnalysis.Assets.DoubleClickBehavior
                /// </summary>
                public bool PassEventArgsToCommand
                {
                    get { return (bool)GetValue(PassEventArgsToCommandProperty); }
                    set { SetValue(PassEventArgsToCommandProperty, value); }
                }

            #endregion

            #region Command

                /// <summary>
                /// Identifies the Berico.LinkAnalysis.Assets.DoubleClickBehavior.CommandProperty property.  This
                /// indicates the name of the command that should be called.
                /// </summary>
                public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DoubleClickBehvaior), new PropertyMetadata(null));

                /// <summary>
                /// Gets or sets the Command property for the Berico.LinkAnalysis.Assets.DoubleClickBehavior
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
            /// Handles the DispatchTimer.Tick event
            /// </summary>
            /// <param name="sender">The object where the event handler is attached</param>
            /// <param name="e">The event data</param>
            private void timer_Tick(object sender, EventArgs e)
            {
                timer.Stop();
            }

            /// <summary>
            /// Handles the MouseLeftButtonDown event
            /// </summary>
            /// <param name="sender">The object where the event handler is attached</param>
            /// <param name="e">The event data</param>
            private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                // Start the timer if it is enabled
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }
                else
                {
                    timer.Stop();
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
            }

        #endregion
    }
}