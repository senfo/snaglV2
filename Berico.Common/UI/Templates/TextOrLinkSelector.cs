using System;
using System.Collections.Generic;
using System.Windows;

namespace Berico.Common.UI.Templates
{
    /// <summary>
    /// Contains properties for text and link templates
    /// </summary>
    public class TextOrLinkSelector : DataTemplateSelector
    {
        #region Properties

        /// <summary>
        /// Gets or sets the link template
        /// </summary>
        public DataTemplate LinkTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text template
        /// </summary>
        public DataTemplate TextTemplate
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the TextOrLinkSelector class
        /// </summary>
        public TextOrLinkSelector()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines which DataTemplate to return based on whether or not the item is a URI
        /// </summary>
        /// <param name="item">Item being bound to the grid</param>
        /// <param name="container">The container</param>
        /// <returns>LinkTemplate if item is a URI, otherwise TextTemplate</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)item;

            if (Uri.IsWellFormedUriString(kvp.Value, UriKind.Absolute))
            {
                return LinkTemplate;
            }
            else
            {
                return TextTemplate;
            }
        }

        #endregion
    }
}
