using System.Xml.Serialization;

namespace RK.Common.GraphicsEngine.Core
{
    [XmlType]
    public class GraphicsConfiguration
    {
        /// <summary>
        /// Gets or sets the preferred antialiasing mode.
        /// </summary>
        [XmlAttribute]
        public AntialiasingMode Antialiasing
        {
            get;
            set;
        }
    }
}