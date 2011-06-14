using System.Collections.ObjectModel;

namespace Berico.SnagL.Host.Facebook.Models
{
    /// <summary>
    /// Contains properties to work with data returned by the friendId/likes Graph API call
    /// </summary>
    public class FacebookLikes
    {
        #region Properties

        /// <summary>
        /// Gets a reference to a collection containing FacebookLike objects
        /// </summary>
        public Collection<FacebookLike> Data
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Intializes a new instance of the FacebookLikes class
        /// </summary>
        public FacebookLikes()
        {
            Data = new Collection<FacebookLike>();
        }

        #endregion
    }
}