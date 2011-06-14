using System.Xml.Serialization;

namespace Berico.SnagL.Infrastructure.Configuration
{
    /// <summary>
    /// The live element
    /// </summary>
    [XmlType(AnonymousType = true, TypeName = "live")]
    public class Live
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value that indicates whether live should start automatically
        /// </summary>
        [XmlAttribute(AttributeName = "autostart")]
        public bool AutoStart
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Live"/> class
        /// </summary>
        public Live()
        {

        }

        #endregion
    }
}
