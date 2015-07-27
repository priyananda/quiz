using System.Collections.Generic;
using RK.Common.GraphicsEngine.Core;

using DWrite = SharpDX.DirectWrite;

namespace RK.Common.GraphicsEngine.Objects
{
    public partial class VertexStructure
    {
        /// <summary>
        /// Builds the text geometry for the given string.
        /// </summary>
        /// <param name="stringToBuild">The string to build within the geometry.</param>
        public void BuildTextGeometry(string stringToBuild)
        {
            BuildTextGeometry(stringToBuild, TextGeometryOptions.Default);
        }

        /// <summary>
        /// Builds the text geometry for the given string.
        /// </summary>
        /// <param name="stringToBuild">The string to build within the geometry.</param>
        /// <param name="geometryOptions">Some configuration for geometry creation.</param>
        public void BuildTextGeometry(string stringToBuild, TextGeometryOptions geometryOptions)
        {
            DWrite.Factory writeFactory = GraphicsCore.Current.HandlerDWrite.Factory;

            //TODO: Cache font objects

            //Get DirectWrite font weight
            DWrite.FontWeight fontWeight = DWrite.FontWeight.Normal;
            switch (geometryOptions.FontWeight)
            {
                case FontGeometryWeight.Bold:
                    fontWeight = DWrite.FontWeight.Bold;
                    break;

                default:
                    fontWeight = DWrite.FontWeight.Normal;
                    break;
            }

            //Get DirectWrite font style
            DWrite.FontStyle fontStyle = DWrite.FontStyle.Normal;
            switch (geometryOptions.FontStyle)
            {
                case FontGeometryStyle.Italic:
                    fontStyle = DWrite.FontStyle.Italic;
                    break;

                case FontGeometryStyle.Oblique:
                    fontStyle = DWrite.FontStyle.Oblique;
                    break;

                default:
                    fontStyle = DWrite.FontStyle.Normal;
                    break;
            }

            //Create the text layout object
            DWrite.TextLayout textLayout = new DWrite.TextLayout(
                writeFactory, stringToBuild,
                new DWrite.TextFormat(
                    writeFactory, geometryOptions.FontFamily, fontWeight, DWrite.FontStyle.Normal, geometryOptions.FontSize),
                float.MaxValue, float.MaxValue);

            //Render the text using the vertex structure text renderer
            using (VertexStructureTextRenderer textRenderer = new VertexStructureTextRenderer(this, geometryOptions))
            {
                textLayout.Draw(textRenderer, 0f, 0f);
            }
        }

        /// <summary>
        /// Builds a plain polygon using the given coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates to build the polygon from.</param>
        public void BuildPlainPolygon(Vector3[] coordinates)
        {
            //Build the polygon
            Polygon polygon = new Polygon(coordinates);

            //Try to triangulate it
            IEnumerable<ushort> indices = polygon.TriangulateUsingCuttingEars();
            if (indices == null) { throw new CommonLibraryException("Unable to triangulate given polygon!"); }

            //Append all vertices
            ushort baseIndex = (ushort)m_vertices.Count;
            for (int loopCoordinates = 0; loopCoordinates < coordinates.Length; loopCoordinates++)
            {
                this.AddVertex(new Vertex(coordinates[loopCoordinates]));
            }

            //Append all indices
            using (IEnumerator<ushort> indexEnumerator = indices.GetEnumerator())
            {
                while (indexEnumerator.MoveNext())
                {
                    ushort index1 = indexEnumerator.Current;
                    ushort index2 = 0;
                    ushort index3 = 0;

                    if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                    if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                    this.AddTriangle((ushort)(index1 + baseIndex), (ushort)(index2 + baseIndex), (ushort)(index3 + baseIndex));
                }
            }
        }
    }
}