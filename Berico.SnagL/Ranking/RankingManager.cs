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
using System.ComponentModel;
using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Modularity;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Model;
using GalaSoft.MvvmLight.Threading;

namespace Berico.SnagL.Infrastructure.Ranking
{
    /// <summary>
    /// Responsible for managing ranking algorithms and the 
    /// visualization of them
    /// </summary>
    public class RankingManager : IPartImportsSatisfiedNotification
    {

        #region Fields

            private static RankingManager _instance;
            private readonly static object SyncRoot = new object();
            
        #endregion

        /// <summary>
        /// Internal constructor
        /// </summary>
        private RankingManager() { }

        #region Properties

            /// <summary>
            /// Gets or sets the list of available ranking algorithms.  This
            /// property is maintained by the ExtensionManager and MEF.
            /// </summary>
            [ImportMany(typeof(IRanker), AllowRecomposition = true)]
            public List<IRanker> Rankers { get; set; }

            /// <summary>
            /// Gets the instance of the RankingManager class
            /// </summary>
            public static RankingManager Instance
            {
                get
                {
                    // Check if the instance is null
                    if (_instance == null)
                    {
                        lock (SyncRoot)
                        {
                            if (_instance == null)
                            {
                                _instance = new RankingManager();
                                _instance.Initialize();
                            }
                        }
                    }

                    return _instance;
                }
            }

        #endregion

        /// <summary>
        /// Performs ranking using the specified algorithm
        /// </summary>
        /// <param name="rankingAlgorithm">The algorithm that should be used for ranking</param>
        public void PerformRanking(IRanker rankingAlgorithm)
        {
            RankerBase ranker = rankingAlgorithm as RankerBase;

            // Validate the provided ranker
            if (ranker == null)
                throw new ArgumentNullException("rankingAlgorithm", "No valid ranker provided");

            // Execute the ranking algorithm.  If normalizeResults is true then
            // the returned results are normalized rather than just the raw values.
            ranker.CalculateNormalizedRankAsync(GraphManager.Instance.DefaultGraphComponentsInstance.Scope);
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            ExtensionManager.ComposeParts(this);
        }

        #region IPartImportsSatisfiedNotification Members

            /// <summary>
            /// 
            /// </summary>
            public void OnImportsSatisfied()
            { }

        #endregion

    }
}
