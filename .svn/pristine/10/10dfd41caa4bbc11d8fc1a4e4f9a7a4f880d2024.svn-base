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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Berico.Common.UI.Templates
{
    /// <summary>
    /// Represents a control whose underlying datatemplate 
    /// can change depending on the type specified by the
    /// EditorType field
    /// </summary>
    public class EditorTemplateSelector : UserControl
    {

        /// <summary>
        /// Creates a new instance of this control
        /// </summary>
        public EditorTemplateSelector()
        {
            DataTemplates = new Collection<KeyedDataTemplate>();
        }

        #region Properties

            /// <summary>
            /// Gets or sets the key that is used to select the template
            /// that should be used
            /// </summary>
        public static readonly DependencyProperty TargetSelectorKeyProperty = DependencyProperty.Register("TargetSelectorKey", typeof(string), typeof(EditorTemplateSelector), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTargetSelectorKeyPropertyChanged)));

            /// <summary>
            /// Gets or sets the key that is used to select the template
            /// that should be used
            /// </summary>
            public string TargetSelectorKey
            {
                get { return (string)GetValue(TargetSelectorKeyProperty); }
                set { SetValue(TargetSelectorKeyProperty, value); }
            }

            /// <summary>
            /// Gets or sets the key that is used to select the template
            /// that should be used
            /// </summary>
            public static readonly DependencyProperty DefaultSelectorKeyProperty = DependencyProperty.Register("DefaultSelectorKey", typeof(string), typeof(EditorTemplateSelector), new PropertyMetadata(string.Empty));

            /// <summary>
            /// Gets or sets the key that identifies the default
            /// data template to be used
            /// </summary>
            public string DefaultSelectorKey
            {
                get { return (string)GetValue(DefaultSelectorKeyProperty); }
                set { SetValue(DefaultSelectorKeyProperty, value); }
            }


            private static void OnTargetSelectorKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                EditorTemplateSelector s = d as EditorTemplateSelector;

                string value = (string)e.NewValue;
                s.OnTargetSelectorKeyChanged(value);
            }

            /// <summary>
            /// Gets or sets the collection of data templates 
            /// available to the selector
            /// </summary>
            public Collection<KeyedDataTemplate> DataTemplates { get; set; }

        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// 
            /// </summary>
            /// <param name="collapseMode"></param>
            protected virtual void OnTargetSelectorKeyChanged(string collapseMode)
            {
                KeyedDataTemplate targetTemplate = null;

                // Check if no DataTemplates were defined
                if (DataTemplates.Count == 0)
                    throw new ArgumentOutOfRangeException("DataTemplates", "No data templates were identified");

                // Check if no target key was provided
                if (string.IsNullOrEmpty(TargetSelectorKey))
                {
                    // Check if no default key was provided
                    if (!string.IsNullOrEmpty(DefaultSelectorKey))
                    {
                        // Attempt to get the default template
                        targetTemplate = DataTemplates.FirstOrDefault(temp => temp.SelectorKey.ToLower().Equals(DefaultSelectorKey.ToLower()));
                    }
                }
                else
                {
                    // Attempt to get the tspecified template
                    targetTemplate = DataTemplates.FirstOrDefault(temp => temp.SelectorKey.ToLower().Equals(TargetSelectorKey.ToLower()));
                }

                // Check if the template is null
                if (targetTemplate == null)
                    Content = null;
                else
                    Content = targetTemplate.LoadContent() as UIElement;
            }

        #endregion

    }
}
