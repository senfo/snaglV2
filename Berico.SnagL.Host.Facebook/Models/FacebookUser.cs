using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Berico.SnagL.Host.Facebook.Models
{
    /// <summary>
    /// Contains properties that represent a Facebook User
    /// </summary>
    [DataContract]
    public class FacebookUser : FacebookData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [DataMember(Name = "first_name")]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [DataMember(Name = "last_name")]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users birthday
        /// </summary>
        [DataMember(Name = "birthday")]
        public string Birthday
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hometown
        /// </summary>
        [DataMember(Name = "hometown")]
        public FacebookData Hometown
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the direct link to the users profile page
        /// </summary>
        [DataMember(Name="link")]
        public Uri Link
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users gender
        /// </summary>
        [DataMember(Name="gender")]
        public UserGender Gender
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to a collection of friends a FacebookUser has
        /// </summary>
        public Collection<FacebookUser> Friends
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FacebookUser class
        /// </summary>
        public FacebookUser()
        {
            Friends = new Collection<FacebookUser>();
        }

        #endregion
    }
}