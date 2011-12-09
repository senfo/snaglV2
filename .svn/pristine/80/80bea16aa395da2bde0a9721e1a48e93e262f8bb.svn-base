using System.Windows;
using System.Windows.Controls;

namespace Berico.Common.UI.Templates
{
    /// <summary>
    /// Extends ContentControl to implement a data template selector similar to WPF
    /// </summary>
    public abstract class DataTemplateSelector : ContentControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DataTemplateSelector class
        /// </summary>
        public DataTemplateSelector()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Selects a template
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="container">The container</param>
        /// <returns>A null reference</returns>
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when the value of the content changes
        /// </summary>
        /// <param name="oldContent">Old content value</param>
        /// <param name="newContent">New content value</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }

        #endregion
    }
}
