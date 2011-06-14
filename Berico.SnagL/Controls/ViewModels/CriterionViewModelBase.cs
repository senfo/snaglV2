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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Base class for Criterion view model classes
    /// </summary>
    /// <typeparam name="TParent">Indicates the type of the criterion's parent class</typeparam>
    public class CriterionViewModelBase<TParent> : ViewModelBase where TParent : ViewModelBase
    {
        private TParent parent = default(TParent);
        private Enum currentState;
        private bool isEnabled = false;
        private bool isActive = false;
        private bool isValid = false;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> Activated;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> Deactivated;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parent">The parent of this criterion</param>
        public CriterionViewModelBase(TParent _parent)
        {
            parent = _parent;
            parent.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ParentPropertyChanged);
        }



        #region Properties

            /// <summary>
            /// Gets the parent for this criterion
            /// </summary>
            public TParent Parent
            {
                get { return this.parent; }
            }

            /// <summary>
            /// Gets or sets whether or not this criterion is active
            /// </summary>
            public Boolean IsActive
            {
                get { return this.isActive; }
                set
                {
                    this.isActive = value;

                    // Change the state based on whether the criterion 
                    // is active or not
                    if (isActive)
                        CurrentState = CriterionState.Active;
                    else
                        CurrentState = CriterionState.Inactive;
                }
            }

            /// <summary>
            /// Gets or sets whether or not this criterion is enabled
            /// </summary>
            public bool IsEnabled
            {
                get
                {
                    return this.isEnabled;
                }
                set
                {
                    this.isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }

            /// <summary>
            /// Gets or sets the current state of the criterion control.  This 
            /// property
            /// </summary>
            public Enum CurrentState
            {
                get { return currentState; }
                protected set
                {
                    this.currentState = value;
                    RaisePropertyChanged("CurrentState");
                }
            }

            /// <summary>
            /// Gets whether the control is valid or not
            /// </summary>
            public bool IsValid
            {
                get { return isValid; }
                protected set
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }

        #endregion

        #region Commands

            /// <summary>
            /// Gets tht command for GotFocus event
            /// </summary>
            protected virtual ICommand GotFocusCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        CurrentState = SearchCriterionState.Focused;
                    });
                }
            }

            /// <summary>
            /// Gets tht command for the LostFocus event
            /// </summary>
            protected virtual ICommand LostFocusCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        CurrentState = SearchCriterionState.Unfocused;
                    });
                }
            }

            /// <summary>
            /// Gets the command for the Click event
            /// of the Add button
            /// </summary>
            public ICommand AddButtonClickedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        AddButtonClickedHandler();
                    });
                }
            }

            /// <summary>
            /// Gets the command for the Click event
            /// of the Remove button
            /// </summary>
            public ICommand RemoveButtonClickedCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        // Mark this criterion as inactive
                        this.IsActive = false;

                        // Raise the Deactivated event.
                        OnDeactivated(EventArgs.Empty);
                    });
                }
            }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AddButtonClickedHandler()
        {
            // Mark this criterion as active
            this.IsActive = true;

            // Raise the Activated event.
            OnActivated(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ParentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // No implementation at this level
        }

        #region Events

            /// <summary>
            /// Raises the Activated event.
            /// </summary>
            /// <param name="e">Contains event arguments.</param>
            protected virtual void OnActivated(EventArgs e)
            {
                EventHandler<EventArgs> handler = this.Activated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Raises the Deactivated event.
            /// </summary>
            /// <param name="e">Contains event arguments.</param>
            protected virtual void OnDeactivated(EventArgs e)
            {
                EventHandler<EventArgs> handler = this.Deactivated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

        #endregion

    }
}
