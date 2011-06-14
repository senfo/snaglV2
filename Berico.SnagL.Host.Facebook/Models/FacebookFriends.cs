using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Berico.SnagL.Host.Facebook.Models
{
    /// <summary>
    /// Contains properties that represent a collection of friends
    /// </summary>
    [DataContract]
    public class FacebookFriends
    {
        #region Properties

        /// <summary>
        /// Gets a reference to a collection of friends
        /// </summary>
        [DataMember(Name = "data")]
        public Collection<FacebookUser> Friends
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FacebookFriends class
        /// </summary>
        public FacebookFriends()
        {
            Friends = new Collection<FacebookUser>();
        }

        #endregion
    }
}
