//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ToolPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ToolbarViewModel class.
        /// </summary>
        public ToolPanelViewModel()
        { }

        private ObservableCollection<AccordionItem> items = new ObservableCollection<AccordionItem>();
        public ObservableCollection<AccordionItem> Items
        {
            get { return this.items; }
            set
            {
                this.items = value;
                RaisePropertyChanged("Items");
            }
        }

        private Visibility visibility;
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }
    }
}