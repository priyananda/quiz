using System.Windows.Media;

namespace RK.Common.GraphicsEngine.Objects
{
    public class WpfMaterialProperties
    {
        private Brush m_wpfBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfMaterialProperties" /> class.
        /// </summary>
        public WpfMaterialProperties()
        {
        }

        /// <summary>
        /// Gets or sets the brush object.
        /// </summary>
        public Brush WpfBrush
        {
            get
            {
                if (m_wpfBrush != null) { return m_wpfBrush; }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            set
            {
                m_wpfBrush = value;
            }
        }
    }
}