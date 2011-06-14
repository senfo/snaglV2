//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Infrastructure.Controls;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view models
            ////}
            ////else
            ////{
            ////    // Create run time view models
            ////}

            CreateMain();
            CreateGraphData();
            CreateToolbarVM();
            CreateToolPanelVM();
            CreateSearchToolVM();
            CreateAttributePopupVM();
            CreateAdvancedSearchVM();

        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ClearMain();
            ClearGraphData();
            ClearToolbarVM();
            ClearToolPanelVM();
            ClearSearchToolVM();
            ClearAttributePopupVM();
            ClearAdvancedSearchVM();
        }

        #region MainViewModel
            private static MainViewModel _main;

            /// <summary>
            /// Gets the Main property.
            /// </summary>
            public static MainViewModel MainStatic
            {
                get
                {
                    if (_main == null)
                    {
                        CreateMain();
                    }

                    return _main;
                }
            }

            /// <summary>
            /// Gets the Main property.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public MainViewModel Main
            {
                get
                {
                    return MainStatic;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the Main property.
            /// </summary>
            public static void ClearMain()
            {
                _main.Cleanup();
                _main = null;
            }

            /// <summary>
            /// Provides a deterministic way to create the Main property.
            /// </summary>
            public static void CreateMain()
            {
                if (_main == null)
                {
                    _main = new MainViewModel();
                }
            }

        #endregion

        #region GraphViewModel

            private static GraphViewModel graphData;

            /// <summary>
            /// Gets the ViewModelPropertyName property
            /// </summary>
            public static GraphViewModel GraphDataStatic
            {
                get
                {
                    if (graphData == null)
                    {
                        CreateGraphData();
                    }

                    return graphData;
                }
            }

            /// <summary>
            /// Gets the ViewModelPropertyName property
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public GraphViewModel GraphData
            {
                get
                {
                    return GraphDataStatic;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the ViewModelPropertyName property
            /// </summary>
            public static void ClearGraphData()
            {
                graphData.Cleanup();
                graphData = null;
            }

            /// <summary>
            /// Provides a deterministic way to create the ViewModelPropertyName property
            /// </summary>
            public static void CreateGraphData()
            {
                if (graphData == null)
                {
                    graphData = new GraphViewModel();
                }
            }

        #endregion

        #region ToolbarViewModel

            private static ToolbarViewModel toolbarVM;

            /// <summary>
            /// Gets the ViewModelPropertyName property
            /// </summary>
            public static ToolbarViewModel ToolbarVMStatic
            {
                get
                {
                    if (toolbarVM == null)
                    {
                        CreateToolbarVM();
                    }

                    return toolbarVM;
                }
            }

            /// <summary>
            /// Gets the ToolbarVM property
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public ToolbarViewModel ToolbarVM
            {
                get
                {
                    return toolbarVM;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the ToolbarVMStatic property
            /// </summary>
            public static void ClearToolbarVM()
            {
                toolbarVM.Cleanup();
                toolbarVM = null;
            }

            /// <summary>
            /// Provides a deterministic way to create the ToolbarVMStatic property
            /// </summary>
            public static void CreateToolbarVM()
            {
                if (toolbarVM == null)
                {
                    toolbarVM = new ToolbarViewModel();
                }
            }

        #endregion

        #region ToolPanelViewModel

            private static ToolPanelViewModel toolPanelVM;

            /// <summary>
            /// Gets the ViewModelPropertyName property
            /// </summary>
            public static ToolPanelViewModel ToolPanelVMStatic
            {
                get
                {
                    if (toolPanelVM == null)
                    {
                        CreateToolPanelVM();
                    }

                    return toolPanelVM;
                }
            }

            /// <summary>
            /// Gets the ToolPanelVM property
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public ToolPanelViewModel ToolPanelVM
            {
                get
                {
                    return toolPanelVM;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the ToolPanelVMStatic property
            /// </summary>
            public static void ClearToolPanelVM()
            {
                toolPanelVM.Cleanup();
                toolPanelVM = null;
            }

            /// <summary>
            /// Provides a deterministic way to create the ToolPanelVMStatic property
            /// </summary>
            public static void CreateToolPanelVM()
            {
                if (toolPanelVM == null)
                {
                    toolPanelVM = new ToolPanelViewModel();
                }
            }

        #endregion

        #region SearchToolViewModel

            private static SearchToolViewModel searchToolVM;

            /// <summary>
            /// Gets the SearchToolVMStatic property
            /// </summary>
            public static SearchToolViewModel SearchToolVMStatic
            {
                get
                {
                    if (searchToolVM == null)
                    {
                        CreateSearchToolVM();
                    }

                    return searchToolVM;
                }
            }

            /// <summary>
            /// Gets the SearchToolVM property
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public SearchToolViewModel SearchToolVM
            {
                get
                {
                    return searchToolVM;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the SearchToolVMStatic property
            /// </summary>
            public static void ClearSearchToolVM()
            {
                searchToolVM.Cleanup();
                searchToolVM = null;
            }

            /// <summary>
            /// Provides a deterministic way to create the SearchToolVMStatic property
            /// </summary>
            public static void CreateSearchToolVM()
            {
                if (searchToolVM == null)
                {
                    searchToolVM = new SearchToolViewModel();
                }
            }

        #endregion

        #region AttributePopupViewModel

            private static AttributePopupViewModel attributePopupVM;

            /// <summary>
            /// Gets the AttributePopupVMStatic property
            /// </summary>
            public static AttributePopupViewModel AttributePopupVMStatic
            {
                get
                {
                    if (attributePopupVM == null)
                    {
                        CreateAttributePopupVM();
                    }

                    return attributePopupVM;
                }
            }

            /// <summary>
            /// Gets the AttributePopupVM property
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public AttributePopupViewModel AttributePopupVM
            {
                get
                {
                    return attributePopupVM;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the AttributePopupVMStatic property
            /// </summary>
            public static void ClearAttributePopupVM()
            {
                attributePopupVM.Cleanup();
                attributePopupVM = null;
            }

            /// <summary>
            /// Provides a deterministic way to create the AttributePopupVMStatic property
            /// </summary>
            public static void CreateAttributePopupVM()
            {
                if (attributePopupVM == null)
                {
                    attributePopupVM = new AttributePopupViewModel();
                }
            }

        #endregion

        #region AdvancedSearchViewModel

            private static AdvancedSearchViewModel advancedSearchVM;

            /// <summary>
            /// Gets the AdvancedSearchVMStatic property
            /// </summary>
            public static AdvancedSearchViewModel AdvancedSearchVMStatic
            {
                get
                {
                    if (advancedSearchVM == null)
                    {
                        CreateAdvancedSearchVM();
                    }

                    return advancedSearchVM;
                }
            }

            /// <summary>
            /// Gets the AdvancedSearchVM property
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic",
                Justification = "This non-static member is needed for data binding purposes.")]
            public AdvancedSearchViewModel AdvancedSearchVM
            {
                get
                {
                    return advancedSearchVM;
                }
            }

            /// <summary>
            /// Provides a deterministic way to delete the AdvancedSearchVM static property
            /// </summary>
            public static void ClearAdvancedSearchVM()
            {
                advancedSearchVM.Cleanup();
                advancedSearchVM = null;
            }

            /// <summary>
            /// Provides a  way to create the AdvancedSearchVM property
            /// </summary>
            public static void CreateAdvancedSearchVM()
            {
                if (advancedSearchVM == null)
                {
                    advancedSearchVM = new AdvancedSearchViewModel();
                }
            }

        #endregion

    }
}