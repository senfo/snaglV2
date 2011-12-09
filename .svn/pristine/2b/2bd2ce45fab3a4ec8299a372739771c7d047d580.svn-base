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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Modularity.ToolPanel
{
    [Export(typeof(IToolPanelItemViewExtension))]
    public partial class LiveDataToolPanelItemExtension : UserControl, IToolPanelItemViewExtension
    {
        public LiveDataToolPanelItemExtension()
        {
            InitializeComponent();
        }

        [Import(typeof(LiveDataToolPanelItemExtensionViewModel), AllowRecomposition = true)]
        public IToolPanelItemViewModelExtension ViewModel
        {
            get
            {
                return this.DataContext as LiveDataToolPanelItemExtensionViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}