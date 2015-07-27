namespace RK.Common.GraphicsEngine.Objects
{
    /// <summary>
    /// A Treangle inside a VertexStructure object
    /// </summary>
    public struct Triangle
    {
        public ushort Index1;
        public ushort Index2;
        public ushort Index3;

        /// <summary>
        /// Creates a new triangle
        /// </summary>
        /// <param name="index1">Index of the first vertex</param>
        /// <param name="index2">Index of the second vertex</param>
        /// <param name="index3">Index of the third vertex</param>
        public Triangle(ushort index1, ushort index2, ushort index3)
        {
            Index1 = index1;
            Index2 = index2;
            Index3 = index3;
        }

        /// <summary>
        /// Gets all edges defined by this triangle.
        /// </summary>
        /// <param name="sourceStructure">The source structure.</param>
        public Line[] GetEdges(VertexStructure sourceStructure)
        {
            return new Line[]
            {
                new Line(
                    sourceStructure.Vertices[this.Index1].Position,
                    sourceStructure.Vertices[this.Index2].Position),
                new Line(
                    sourceStructure.Vertices[this.Index2].Position,
                    sourceStructure.Vertices[this.Index3].Position),
                new Line(
                    sourceStructure.Vertices[this.Index3].Position,
                    sourceStructure.Vertices[this.Index1].Position)
            };
        }
    }
}