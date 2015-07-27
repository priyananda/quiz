namespace RK.Common.GraphicsEngine.Objects
{
    /// <summary>
    /// Delegate used for accessing a tesselation function
    /// </summary>
    public delegate Vector3 TesselationFunction(float u, float v);

    /// <summary>
    /// Enumeration containing all components of texture coordinate
    /// </summary>
    public enum TextureCoordinateComponent
    {
        /// <summary>
        /// U component of a texture coordinate
        /// </summary>
        U,

        /// <summary>
        /// V component of a texture coordinate
        /// </summary>
        V
    }

    public enum FitToCuboidMode
    {
        MaintainAspectRatio,

        Stretch
    }

    public enum FitToCuboidOrigin
    {
        Center,

        LowerCenter
    }

    public enum TextGeometryAlignment
    {
        LowerLeft,

        MiddleCenter
    }

    public enum FontGeometryWeight
    {
        Bold,
        Normal
    }

    public enum FontGeometryStyle
    {
        Normal,
        Oblique,
        Italic
    }

    /// <summary>
    /// Some options for text geometry creation.
    /// </summary>
    public struct TextGeometryOptions
    {
        public static readonly TextGeometryOptions Default = new TextGeometryOptions()
        {
            FontSize = 20,
            FontFamily = "Sergoe UI",
            FontWeight = FontGeometryWeight.Normal,
            FontStyle = FontGeometryStyle.Normal,
            SimplificationFlatternTolerance = 0.1f,
            SurfaceMaterial = "TextSurfaceMaterial",
            VerticesScaleFactor = 0.05f,
            SurfaceVertexColor = Color4.White,
            MakeVolumetricText = true,
            VolumetricTextDepth = 0.5f,
            VolumetricSideMaterial = "TextSideMaterial",
            VolumetricSideSurfaceVertexColor = Color4.White,
            CalculateNormals = true,
            Alignment = TextGeometryAlignment.LowerLeft,
            VertexTransform = Matrix4.Identity
        };

        public string FontFamily;
        public int FontSize;
        public FontGeometryWeight FontWeight;
        public FontGeometryStyle FontStyle;
        public float VerticesScaleFactor;
        public float SimplificationFlatternTolerance;
        public string SurfaceMaterial;
        public bool GenerateCubesOnVertices;
        public Color4 SurfaceVertexColor;
        public Color4 VolumetricSideSurfaceVertexColor;
        public bool MakeVolumetricText;
        public float VolumetricTextDepth;
        public string VolumetricSideMaterial;
        public bool CalculateNormals;
        public TextGeometryAlignment Alignment;
        public Matrix4 VertexTransform;
    }
}