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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Infrastructure.Ranking;
using Berico.SnagL.Infrastructure.Ranking.Visualization;
using Berico.SnagL.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Modularity.ToolPanel
{
    /// <summary>
    /// Represents the view model for the clustering tool panel
    /// </summary>
    [PartMetadata("ID", "ToolPanelItemViewModelExtension"), Export(typeof(RankingToolPanelItemExtensionViewModel))]
    public class RankingToolPanelItemExtensionViewModel : ViewModelBase, IToolPanelItemViewModelExtension
    {

        #region Fields

            private string _toolName = string.Empty;
            private string _description = string.Empty;
            private bool _isEnabled;
            private RankingManager _rankingManager;
            private ObservableCollection<RankingData> _scores = new ObservableCollection<RankingData>();
            private IRanker _selectedRanker;
            private VisualizationOptions _visualizationOptions = VisualizationOptions.Both;
            private bool _isActive;
            private ColorVisualizer _colorVisualizer;
            private ScaleVisualizer _scaleVisualizer;

        #endregion

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Extensions.RankingToolPanelItemExtensionViewModel
        /// </summary>
        public RankingToolPanelItemExtensionViewModel()
        {
            Index = 2;
            Description = "This tool provides ranking functionality";
            ToolName = "Ranking and Scoring";
            SelectedRanker = Rankers[0];

            SnaglEventAggregator.DefaultInstance.GetEvent<DataLoadedEvent>().Subscribe(DataLoadedEventHandler, false);
        }

        #region Properties

            /// <summary>
            /// Gets or sets all the scores currently computed by
            /// the selected ranking algorithm
            /// </summary>
            public ObservableCollection<RankingData> Scores
            {
                get { return _scores; }
                set
                {
                    _scores = value;
                    RaisePropertyChanged("Scores");
                }
            }

            /// <summary>
            /// Gets or sets the currently selected ranking algorithm
            /// </summary>
            public IRanker SelectedRanker
            {
                get { return _selectedRanker; }
                set
                {
                    _selectedRanker = value;
                    RaisePropertyChanged("SelectedRanker");
                }
            }

            /// <summary>
            /// Gets or sets the currently selected VisualizationOption
            /// </summary>
            public VisualizationOptions VisualizationOption
            {
                get { return _visualizationOptions; }
                set
                {
                    _visualizationOptions = value;
                    RaisePropertyChanged("VisualizationOption");
                }
            }

            /// <summary>
            /// Gets a list of all the available ranking algorithms
            /// </summary>
            public List<IRanker> Rankers
            {
                get { return RankingManager.Instance.Rankers; }
            }

            /// <summary>
            /// Gets or sets whether the ranking is currently active
            /// </summary>
            public bool IsActive
            {
                get { return _isActive; }
                set
                {
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }

            #region IToolPanelItemViewModelExtension Members

                /// <summary>
                /// Gets or sets the index for this tool panel
                /// </summary>
                public int Index { get; set; }

                /// <summary>
                /// Gets or sets the description for this tool panel tool
                /// </summary>
                public string Description
                {
                    get { return _description; }
                    set
                    {
                        _description = value;
                        RaisePropertyChanged("Description");
                    }
                }

                /// <summary>
                /// Gets or sets the name of this tool
                /// </summary>
                public string ToolName
                {
                    get { return _toolName; }
                    set
                    {
                        _toolName = value;
                        RaisePropertyChanged("ToolName");
                    }
                }

                /// <summary>
                /// Gets or sets whether this tool is enabled or not
                /// </summary>
                public bool IsEnabled
                {
                    get { return _isEnabled; }
                    set
                    {
                        _isEnabled = value;
                        RaisePropertyChanged("IsEnabled");
                    }
                }

            #endregion

        #endregion

        #region Commands

            /// <summary>
            /// Gets or sets the command that handles when the user pressing
            /// the Rank button
            /// </summary>
            public ICommand RankGraphCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        if (_rankingManager == null)
                            _rankingManager = RankingManager.Instance;

                        // Check if there are any rankers available
                        if (_rankingManager.Rankers == null || _rankingManager.Rankers.Count == 0)
                            throw new ArgumentException("No rankers available");

                        // Wire up the RankingCompleted event
                        SelectedRanker.RankingCompleted += RankingCompletedHandler;

                        // Perform the ranking asynchronously
                        _rankingManager.PerformRanking(SelectedRanker);
                    });
                }
            }

            private void RankingCompletedHandler(object sender, RankingEventArgs e)
            {
                // Remove event handler for currently selected ranker
                SelectedRanker.RankingCompleted -= RankingCompletedHandler;

                GraphComponents graph = GraphManager.Instance.DefaultGraphComponentsInstance;
                List<RankingData> data = new List<RankingData>();

                if (_colorVisualizer == null)
                    _colorVisualizer = new ColorVisualizer(e.Results.Values.Min(), e.Results.Values.Max());
                else
                    _colorVisualizer.Reset(e.Results.Values.Min(), e.Results.Values.Max());

                _scaleVisualizer = new ScaleVisualizer();

                foreach (INode node in e.Results.Keys)
                {
                    RankingData rankingData = new RankingData { Score = e.Results[node], NodeCount = 1 };

                    if (data.Contains(rankingData))
                        data[data.IndexOf(rankingData)].NodeCount += 1;
                    else
                        data.Add(rankingData);

                    NodeViewModelBase nodeVM = graph.GetNodeViewModel(node) as NodeViewModelBase;

                    if (nodeVM != null)
                    {
                        if ((VisualizationOption & VisualizationOptions.Color) == VisualizationOptions.Color)
                        {
                            _colorVisualizer.Visualize(nodeVM, e.Results[node]);
                        }
                        else
                        {
                            _colorVisualizer.ClearVisualization(nodeVM);
                        }

                        if ((VisualizationOption & VisualizationOptions.Scale) == VisualizationOptions.Scale)
                        {
                            _scaleVisualizer.Visualize(nodeVM, e.Results[node]);
                        }
                        else
                            _scaleVisualizer.ClearVisualization(nodeVM);
                    }
                }

                Scores = new ObservableCollection<RankingData>(data.OrderBy(rankData => rankData.Score));
                IsActive = true;
            }

            /// <summary>
            /// Gets or sets the command that handles when the user pressing
            /// the Clear button
            /// </summary>
            public ICommand ClearRankCommand
            {
                get
                {
                    return new RelayCommand(() =>
                    {
                        foreach (INodeShape nodeShape in GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModels())
                        {
                            NodeViewModelBase nodeVM = nodeShape as NodeViewModelBase;

                            // Check if a color was saved for this node
                            if (nodeVM != null)
                            {
                                _colorVisualizer.ClearVisualization(nodeVM);
                                _scaleVisualizer.ClearVisualization(nodeVM);
                            }
                        }

                        IsActive = false;
                        _colorVisualizer.Clear();
                    });
                }
            }
 
        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// Handles the DataLoaded event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void DataLoadedEventHandler(DataLoadedEventArgs args)
            {
                GraphComponents graph = GraphManager.Instance.GetGraphComponents(args.Scope);

                if (graph != null && graph.Data.Order > 0)
                {
                    IsEnabled = true;
                }
                else
                    IsEnabled = false;
            }

        #endregion

    }
}