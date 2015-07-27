using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RK.Common
{
    public class Polygon
    {
        private Vector3[] m_vertices;
        private ReadOnlyCollection<Vector3> m_verticesPublic;
        private Lazy<Vector3> m_normal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D" /> class.
        /// </summary>
        public Polygon(params Vector3[] vertices)
        {
            if (vertices.Length < 3) { throw new CommonLibraryException("A plygon must at least have 4 vertices!"); }

            m_vertices = vertices;
            m_verticesPublic = new ReadOnlyCollection<Vector3>(m_vertices);
            
            //Define normal calculation method
            m_normal = new Lazy<Vector3>(() => Vector3.CalculateTriangleNormal(m_vertices[0], m_vertices[1], m_vertices[2]));
        }

        /// <summary>
        /// Flatterns this polygon.
        /// </summary>
        public Polygon2D Flattern()
        {
            //Inspired by implementation of the Helix Toolkit from codeplex (http://helixtoolkit.codeplex.com/)
            //Original sources:
            // http://forums.xna.com/forums/p/16529/86802.aspx
            // http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane

            //Calculate transform matrix
            Vector3 upVector = m_normal.Value;
            Vector3 right = Vector3.Cross(
                upVector, Math.Abs(upVector.X) > Math.Abs(upVector.Z) ? new Vector3(0, 0, 1) : new Vector3(1, 0, 0));
            Vector3 backward = Vector3.Cross(right, upVector);
            var m = new Matrix4(
                backward.X, right.X, upVector.X, 0, backward.Y, right.Y, upVector.Y, 0, backward.Z, right.Z, upVector.Z, 0, 0, 0, 0, 1);

            //Make first point origin
            var offs = Vector3.Transform(m_vertices[0], m);
            m.OffsetX = -offs.X;
            m.OffsetY = -offs.Y;

            //Calculate 2D surface
            Vector2[] resultVertices = new Vector2[m_vertices.Length];
            for (int loopVertex = 0; loopVertex < m_vertices.Length; loopVertex++)
            {
                var pp = Vector3.Transform(m_vertices[loopVertex], m);
                resultVertices[loopVertex] = new Vector2(pp.X, pp.Y);
            }

            return new Polygon2D(resultVertices);
        }

        /// <summary>
        /// Triangulates this surface using the cutting ears algorithm.
        /// </summary>
        public IEnumerable<ushort> TriangulateUsingCuttingEars()
        {
            Polygon2D surface2D = this.Flattern();
            return surface2D.TriangulateUsingCuttingEars();
        }

        /// <summary>
        /// Gets a collection containing all vertices.
        /// </summary>
        public ReadOnlyCollection<Vector3> Vertices
        {
            get { return m_verticesPublic; }
        }

        /// <summary>
        /// Gets the normal of this polygon.
        /// </summary>
        public Vector3 Normal
        {
            get { return m_normal.Value; }
        }
    }
}
