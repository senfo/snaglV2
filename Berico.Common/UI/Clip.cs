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
using System.Windows.Media;

namespace Berico.Common.UI
{
    /// <summary>
    /// 
    /// </summary>
    public static class Clip
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static bool GetToBounds(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(ToBoundsProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="clipContentsToBounds"></param>
        public static void SetToBounds(DependencyObject depObj, bool clipContentsToBounds)
        {
            depObj.SetValue(ToBoundsProperty, clipContentsToBounds);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ToBoundsProperty = DependencyProperty.RegisterAttached("ToBounds", typeof(bool), typeof(Clip), new PropertyMetadata(false, OnToBoundsPropertyChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnToBoundsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element != null)
            {
                ClipContentsToBounds(element);
                
                // The clippining geometry needs to be updated anytime the element
                // we are attached to resizes or is loaded
                element.Loaded += new RoutedEventHandler(element_Loaded);
                element.SizeChanged += new SizeChangedEventHandler(element_SizeChanged);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        private static void ClipContentsToBounds(FrameworkElement element)
        {
            if (GetToBounds(element))
            {
                element.Clip = new RectangleGeometry()
                {
                    Rect = new Rect(0, 0, element.ActualWidth, element.ActualHeight)
                };
            }
            else
            {
                element.Clip = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipContentsToBounds(sender as FrameworkElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void element_Loaded(object sender, RoutedEventArgs e)
        {
            ClipContentsToBounds(sender as FrameworkElement);
        }
    }

}