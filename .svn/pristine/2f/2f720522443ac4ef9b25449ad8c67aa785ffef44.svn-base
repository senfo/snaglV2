//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Controls.ViewModels
{
    using System;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// The ViewModel for the CustomMessageDialog control
    /// </summary>
    public class CustomMessageDialogViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Stores the title of the modal window
        /// </summary>
        private string _title;

        /// <summary>
        /// Stores the main text displayed on the modal window
        /// </summary>
        private string _text;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the main title on the modal window
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// Gets or sets the primary text displayed on the modal window
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        #endregion
    }
}
