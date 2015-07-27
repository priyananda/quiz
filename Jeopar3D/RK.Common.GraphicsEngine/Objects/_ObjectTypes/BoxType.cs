﻿namespace RK.Common.GraphicsEngine.Objects
{
    public class BoxType : ObjectType
    {
        private float m_width;
        private float m_heigth;
        private float m_depth;
        private string m_material;
        private Color4 m_color;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxType"/> class.
        /// </summary>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        /// <param name="depth">The depth of the box.</param>
        /// <param name="material">The material of the box.</param>
        public BoxType(float width, float height, float depth, string material)
        {
            m_width = width;
            m_heigth = height;
            m_depth = depth;
            m_material = material;
            m_color = Color4.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxType"/> class.
        /// </summary>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        /// <param name="depth">The depth of the box.</param>
        public BoxType(float width, float height, float depth)
            : this(width, height, depth, string.Empty)
        {
        }

        /// <summary>
        /// Builds the structure.
        /// </summary>
        public override VertexStructure[] BuildStructure()
        {
            VertexStructure[] result = new VertexStructure[1];

            //Build the box
            result[0] = new VertexStructure();
            result[0].Material = m_material;
            result[0].BuildCube24V(
                new Vector3(-m_width / 2f, -m_heigth / 2f, -m_depth / 2f),
                new Vector3(m_width, m_heigth, m_depth),
                m_color);

            return result;
        }

        public Color4 Color
        {
            get { return m_color; }
            set { m_color = value; }
        }
    }
}