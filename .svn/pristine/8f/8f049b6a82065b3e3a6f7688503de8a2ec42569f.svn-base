using System;
using System.Runtime.Serialization;

namespace Berico.SnagL.Host.Facebook.Models
{
    /// <summary>
    /// Contains properties that represent a Facebook like
    /// </summary>
    [DataContract]
    public class FacebookLike : FacebookData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        [DataMember(Name = "category")]
        public string Category
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date/time the object was created
        /// </summary>
        [DataMember(Name = "created_time")]
        public DateTime Created
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Intializes a new instance of the FacebookLike class
        /// </summary>
        public FacebookLike()
        {
        }

        #endregion
    }
}