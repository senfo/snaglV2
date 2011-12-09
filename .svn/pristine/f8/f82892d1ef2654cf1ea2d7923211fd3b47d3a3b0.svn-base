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


namespace Berico.SnagL.Infrastructure.Ranking
{
    /// <summary>
    /// Represents the data resulting from a ranking operation
    /// </summary>
    public class RankingData
    {
        /// <summary>
        /// Gets or sets the score
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ScoreText { get { return String.Format("{0:0.00}", Score); }}
    
        /// <summary>
        /// Gets or sets the number of nodes that had the score
        /// </summary>
        public int NodeCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[Score:" + Score + ", Count:" + NodeCount + "]";
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Node"/>
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            return Score.GetHashCode();
        }

        /// <summary>
        /// Determines whether this instance of Berico.LinkAnalysis.Model.Node and a
        /// specified object, which must also be a Berico.LinkAnalysis.Model.Node
        /// object, have the same value.  The main source for comparison is the nodes
        /// ID property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals(obj as RankingData);
        }

        /// <summary>
        /// Determines whether this instance and another specified Berico.LinkAnalysis.Model.Node
        /// object have the same value.  The main source for comparison is the nodes
        /// ID property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(RankingData obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetHashCode() != obj.GetHashCode())
                return false;

            return (Score.Equals(obj.Score));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.Node objects have the same value
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <returns>true if the value of leftHandSide is the same as the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator ==(RankingData leftHandSide, RankingData rightHandSide)
        {
            // Check if leftHandSide is null
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);

            return (leftHandSide.Equals(rightHandSide));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.Node objects have different values
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <returns>true if the value of lefthandside is different from the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator !=(RankingData leftHandSide, RankingData rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }
    }
}
