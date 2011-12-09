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
using System.Windows;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Represents a control that allows the user to zoom in and out on the graph
    /// </summary>
    public class ZoomSliderViewModel : ViewModelBase
    {
        /// <summary>
        /// Stores the current value of the slider
        /// </summary>
        private double _currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomSliderViewModel"/> class
        /// </summary>
        public ZoomSliderViewModel()
        {
            SnaglEventAggregator.DefaultInstance.GetEvent<GraphResizedEvent>().Subscribe(OnGraphresized);
        }

        /// <summary>
        /// Gets or sets the sliders current value
        /// </summary>
        public double CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                RaisePropertyChanged("CurrentValue");
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="ICommand"/> that executes when the value of the slider changes.
        /// The <see cref="RelayCommand"/> zooms the graph in and out based on the value from the slider.
        /// </summary>
        public ICommand OnValueChanged
        {
            get
            {
                return new RelayCommand<RoutedPropertyChangedEventArgs<double>>(e => ViewModelLocator.GraphDataStatic.Zoom(e.NewValue));
            }
        }

        /// <summary>
        /// Handles the event where the graph is resized
        /// </summary>
        /// <param name="eventArgs">Any event arguments that might be passed</param>
        public virtual void OnGraphresized(EventArgs eventArgs)
        {
            CurrentValue = ViewModelLocator.GraphDataStatic.Scale;
        }
    }
}