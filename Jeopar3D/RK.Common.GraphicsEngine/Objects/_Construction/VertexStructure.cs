using System;
using System.Collections.Generic;

namespace RK.Common.GraphicsEngine.Objects
{
    public partial class VertexStructure
    {
        private string m_description;
        private List<ushort> m_indices;
        private MaterialProperties m_materialProperties;
        private Dictionary<Type, object> m_materialPropertiesExtended;
        private string m_name;
        private TriangleCollection m_triangleCollection;
        private VertexCollection m_vertexCollection;
        private List<Vertex> m_vertices;

        /// <summary>
        /// Creates a new Vertex structure object
        /// </summary>
        public VertexStructure()
            : this(0, 0)
        {
            m_materialProperties = new MaterialProperties();
        }

        /// <summary>
        /// Creates a new Vertex structure object
        /// </summary>
        public VertexStructure(int verticesCapacity, int triangleCapacity)
        {
            m_vertices = new List<Vertex>(verticesCapacity);
            m_indices = new List<ushort>(triangleCapacity * 3);

            m_vertexCollection = new VertexCollection(m_vertices);
            m_triangleCollection = new TriangleCollection(m_indices, m_vertices);

            m_name = string.Empty;
            m_description = string.Empty;
        }

        /// <summary>
        /// Generates a bounding box surrounding all given structures.
        /// </summary>
        /// <param name="structures">Structures to generate a bounding box for.</param>
        public static AxisAlignedBox GenerateBoundingBox(params VertexStructure[] structures)
        {
            return structures.GenerateBoundingBox();
        }

        /// <summary>
        /// Gets a vector to the bottom center of given structures.
        /// </summary>
        public static Vector3 GetBottomCenter(VertexStructure[] structures)
        {
            AxisAlignedBox box = GetBoundingBox(structures);
            return box.GetBottomCenter();
        }

        /// <summary>
        /// Gets a bounding box for given vertex structure array.
        /// </summary>
        /// <param name="structures">Array of structures.</param>
        public static AxisAlignedBox GetBoundingBox(VertexStructure[] structures)
        {
            AxisAlignedBox result = new AxisAlignedBox();

            if (structures != null)
            {
                for (int loop = 0; loop < structures.Length; loop++)
                {
                    if (structures[loop] != null)
                    {
                        result.MergeWith(structures[loop].GenerateBoundingBox());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a vector to the middle center of given structures.
        /// </summary>
        public static Vector3 GetMiddleCenter(VertexStructure[] structures)
        {
            AxisAlignedBox box = GetBoundingBox(structures);
            return box.GetMiddleCenter();
        }

        /// <summary>
        /// Adds all triangles of the given VertexStructure to this one.
        /// </summary>
        /// <param name="otherStructure">The structure to add to this one.</param>
        public void AddStructure(VertexStructure otherStructure)
        {
            ushort baseIndex = (ushort)m_vertices.Count;

            m_vertices.AddRange(otherStructure.Vertices);
            for (int loopIndex = 0; loopIndex < otherStructure.m_indices.Count; loopIndex++)
            {
                m_indices.Add((ushort)(baseIndex + otherStructure.m_indices[loopIndex]));
            }
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="vertices">The vertices to add.</param>
        public void AddPolygonByCuttingEars(IEnumerable<Vertex> vertices)
        {
            //Add vertices first
            List<ushort> indices = new List<ushort>();
            foreach (Vertex actVertex in vertices)
            {
                indices.Add(this.AddVertex(actVertex));
            }

            //Calculate cutting ears
            AddPolygonByCuttingEars(indices);
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="indices">The indices of the polygon's edges.</param>
        public void AddPolygonByCuttingEars(IEnumerable<ushort> indices)
        {
            AddPolygonByCuttingEarsInternal(new List<ushort>(indices));
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="vertices">The vertices to add.</param>
        public void AddPolygonByCuttingEarsAndCalculateNormals(IEnumerable<Vertex> vertices)
        {
            //Add vertices first
            List<ushort> indices = new List<ushort>();
            foreach (Vertex actVertex in vertices)
            {
                indices.Add(this.AddVertex(actVertex));
            }

            //Calculate cutting ears and normals
            AddPolygonByCuttingEarsAndCalculateNormals(indices);
        }

        /// <summary>
        /// Adds the given polygon using the cutting ears algorythm for triangulation.
        /// </summary>
        /// <param name="indices">The indices of the polygon's edges.</param>
        public void AddPolygonByCuttingEarsAndCalculateNormals(IEnumerable<ushort> indices)
        {
            //Add the triangles using cutting ears algorithm
            IEnumerable<ushort> addedIndices = AddPolygonByCuttingEarsInternal(new List<ushort>(indices));

            //Calculate all normals
            IEnumerator<ushort> indexEnumerator = addedIndices.GetEnumerator();
            while (indexEnumerator.MoveNext())
            {
                ushort index1 = indexEnumerator.Current;
                ushort index2 = 0;
                ushort index3 = 0;

                if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                this.CalculateNormalsForTriangle(new Triangle(index1, index2, index3));
            }
        }

        /// <summary>
        /// Adds a triangle
        /// </summary>
        /// <param name="index1">Index of the first vertex</param>
        /// <param name="index2">Index of the second vertex</param>
        /// <param name="index3">Index of the third vertex</param>
        public void AddTriangle(ushort index1, ushort index2, ushort index3)
        {
            m_indices.Add(index1);
            m_indices.Add(index2);
            m_indices.Add(index3);
        }

        /// <summary>
        /// Adds a triangle
        /// </summary>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        public void AddTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            m_indices.Add(AddVertex(v1));
            m_indices.Add(AddVertex(v2));
            m_indices.Add(AddVertex(v3));
        }

        /// <summary>
        /// Adds a triangle
        /// </summary>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        public void AddTriangleAndCalculateNormals(Vertex v1, Vertex v2, Vertex v3)
        {
            ushort index1 = AddVertex(v1);
            ushort index2 = AddVertex(v2);
            ushort index3 = AddVertex(v3);

            AddTriangleAndCalculateNormals(index1, index2, index3);
        }

        /// <summary>
        /// Adds a triangle and calculates normals for it.
        /// </summary>
        /// <param name="index1">Index of the first vertex</param>
        /// <param name="index2">Index of the second vertex</param>
        /// <param name="index3">Index of the third vertex</param>
        public void AddTriangleAndCalculateNormals(ushort index1, ushort index2, ushort index3)
        {
            AddTriangle(index1, index2, index3);
            CalculateNormalsForTriangle(new Triangle(index1, index2, index3));
        }

        /// <summary>
        /// Adds a vertex to the structure
        /// </summary>
        /// <param name="vertex"></param>
        public ushort AddVertex(Vertex vertex)
        {
            //Transform vertex on build-time
            if (m_buildTimeTransformEnabled)
            {
                if (m_buildTimeTransformFunc != null) { vertex = m_buildTimeTransformFunc(vertex); }
                else
                {
                    vertex.Position = Vector3.Transform(vertex.Position, m_buildTransformMatrix);
                    vertex.Normal = Vector3.TransformNormal(vertex.Normal, m_buildTransformMatrix);
                }
            }

            //Add the vertex and return the index
            m_vertices.Add(vertex);
            return (ushort)(m_vertices.Count - 1);
        }

        /// <summary>
        /// Builds a line list containing a line for each face binormal.
        /// </summary>
        public List<Vector3> BuildLineListForFaceBinormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageBinormal = Vector3.Normalize(Vector3.Average(vertex1.Binormal, vertex2.Binormal, vertex3.Binormal));
                Vector3 averagePosition = Vector3.Average(vertex1.Position, vertex2.Position, vertex3.Position);
                averageBinormal *= 0.2f;

                //Generate a line
                if (averageBinormal.Length > 0.1f)
                {
                    result.Add(averagePosition);
                    result.Add(averagePosition + averageBinormal);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a line list containing a line for each face normal.
        /// </summary>
        public List<Vector3> BuildLineListForFaceNormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageNormal = Vector3.Normalize(Vector3.Average(vertex1.Normal, vertex2.Normal, vertex3.Normal));
                Vector3 averagePosition = Vector3.Average(vertex1.Position, vertex2.Position, vertex3.Position);
                averageNormal *= 0.2f;

                //Generate a line
                if (averageNormal.Length > 0.1f)
                {
                    result.Add(averagePosition);
                    result.Add(averagePosition + averageNormal);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a line list containing a line for each face tangent.
        /// </summary>
        public List<Vector3> BuildLineListForFaceTangents()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Get average values for current face
                Vector3 averageTangent = Vector3.Normalize(Vector3.Average(vertex1.Tangent, vertex2.Tangent, vertex3.Tangent));
                Vector3 averagePosition = Vector3.Average(vertex1.Position, vertex2.Position, vertex3.Position);
                averageTangent *= 0.2f;

                //Generate a line
                if (averageTangent.Length > 0.1f)
                {
                    result.Add(averagePosition);
                    result.Add(averagePosition + averageTangent);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a list list containing a list for each vertex binormal.
        /// </summary>
        public List<Vector3> BuildLineListForVertexBinormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Vertex actVertex in m_vertices)
            {
                if (actVertex.Binormal.Length > 0.1f)
                {
                    result.Add(actVertex.Position);
                    result.Add(actVertex.Position + actVertex.Binormal * 0.2f);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a list list containing a list for each vertex normal.
        /// </summary>
        public List<Vector3> BuildLineListForVertexNormals()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Vertex actVertex in m_vertices)
            {
                if (actVertex.Normal.Length > 0.1f)
                {
                    result.Add(actVertex.Position);
                    result.Add(actVertex.Position + actVertex.Normal * 0.2f);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a list list containing a list for each vertex tangent.
        /// </summary>
        public List<Vector3> BuildLineListForVertexTangents()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Vertex actVertex in m_vertices)
            {
                if (actVertex.Tangent.Length > 0.1f)
                {
                    result.Add(actVertex.Position);
                    result.Add(actVertex.Position + actVertex.Tangent * 0.2f);
                }
            }

            return result;
        }

        /// <summary>
        /// Build a line list containing all lines for wireframe display.
        /// </summary>
        public List<Vector3> BuildLineListForWireframeView()
        {
            List<Vector3> result = new List<Vector3>();

            //Generate all lines
            foreach (Triangle actTriangle in this.Triangles)
            {
                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //first line (c)
                result.Add(vertex1.Position);
                result.Add(vertex2.Position);

                //second line (a)
                result.Add(vertex2.Position);
                result.Add(vertex3.Position);

                //third line (b)
                result.Add(vertex3.Position);
                result.Add(vertex1.Position);
            }

            return result;
        }

        /// <summary>
        /// Calculates normals for the given treeangle.
        /// </summary>
        public void CalculateNormalsForTriangle(Triangle actTriangle)
        {
            Vertex v1 = m_vertices[actTriangle.Index1];
            Vertex v2 = m_vertices[actTriangle.Index2];
            Vertex v3 = m_vertices[actTriangle.Index3];

            Vector3 normal = Vector3.CalculateTriangleNormal(v1.Geometry.Position, v2.Geometry.Position, v3.Geometry.Position);

            v1 = v1.Copy(v1.Geometry.Position, normal);
            v2 = v2.Copy(v2.Geometry.Position, normal);
            v3 = v3.Copy(v3.Geometry.Position, normal);

            m_vertices[actTriangle.Index1] = v1;
            m_vertices[actTriangle.Index2] = v2;
            m_vertices[actTriangle.Index3] = v3;
        }

        /// <summary>
        /// Calculates tangents for all vectors.
        /// </summary>
        public void CalculateTangentsAndBinormals()
        {
            for (int loop = 0; loop < this.CountTriangles; loop += 1)
            {
                Triangle actTriangle = this.Triangles[loop];

                //Get all vertices of current face
                Vertex vertex1 = m_vertices[actTriangle.Index1];
                Vertex vertex2 = m_vertices[actTriangle.Index2];
                Vertex vertex3 = m_vertices[actTriangle.Index3];

                //Perform some precalculations
                Vector2 w1 = vertex1.TexCoord;
                Vector2 w2 = vertex2.TexCoord;
                Vector2 w3 = vertex3.TexCoord;
                float x1 = vertex2.Position.X - vertex1.Position.X;
                float x2 = vertex3.Position.X - vertex1.Position.X;
                float y1 = vertex2.Position.Y - vertex1.Position.Y;
                float y2 = vertex3.Position.Y - vertex1.Position.Y;
                float z1 = vertex2.Position.Z - vertex1.Position.Z;
                float z2 = vertex3.Position.Z - vertex1.Position.Z;
                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;
                float r = 1f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                //Create the tangent vector (assumes that each vertex normal within the face are equal)
                Vector3 tangent = sdir - vertex1.Normal * Vector3.Dot(vertex1.Normal, sdir);
                tangent.Normalize();

                //Create the binormal using the tangent
                float tangentDir = (Vector3.Dot(Vector3.Cross(vertex1.Normal, sdir), tdir) >= 0.0f) ? 1f : -1f;
                Vector3 binormal = Vector3.Cross(vertex1.Normal, tangent) * tangentDir;

                //Seting binormals and tangents to each vertex of current face
                vertex1.Tangent = tangent;
                vertex1.Binormal = binormal;
                vertex2.Tangent = tangent;
                vertex2.Binormal = binormal;
                vertex3.Tangent = tangent;
                vertex3.Binormal = binormal;

                //Overtake changes made in vertex structures
                m_vertices[actTriangle.Index1] = vertex1;
                m_vertices[actTriangle.Index2] = vertex2;
                m_vertices[actTriangle.Index3] = vertex3;
            }
        }

        /// <summary>
        /// Recalculates all normals
        /// </summary>
        public void CalulateNormals()
        {
            foreach (Triangle actTriangle in m_triangleCollection)
            {
                CalculateNormalsForTriangle(actTriangle);
            }
        }

        /// <summary>
        /// Clones this object
        /// </summary>
        public VertexStructure Clone()
        {
            int vertexCount = m_vertices.Count;
            int indexCount = m_indices.Count;

            VertexStructure result = new VertexStructure(vertexCount, indexCount / 3);
            for (int loop = 0; loop < vertexCount; loop++)
            {
                result.m_vertices.Add(m_vertices[loop]);
            }
            for (int loop = 0; loop < indexCount; loop++)
            {
                result.m_indices.Add(m_indices[loop]);
            }
            result.m_materialProperties = m_materialProperties;
            result.m_description = m_description;
            result.m_name = m_name;

            return result;
        }

        /// <summary>
        /// Generates a boundbox around this structure
        /// </summary>
        public AxisAlignedBox GenerateBoundingBox()
        {
            Vector3 maximum = Vector3.MinValue;
            Vector3 minimum = Vector3.MaxValue;

            foreach (Vertex actVertex in m_vertices)
            {
                Vector3 actPosition = actVertex.Position;

                //Update minimum vector
                if (actPosition.X < minimum.X) { minimum.X = actPosition.X; }
                if (actPosition.Y < minimum.Y) { minimum.Y = actPosition.Y; }
                if (actPosition.Z < minimum.Z) { minimum.Z = actPosition.Z; }

                //Update maximum vector
                if (actPosition.X > maximum.X) { maximum.X = actPosition.X; }
                if (actPosition.Y > maximum.Y) { maximum.Y = actPosition.Y; }
                if (actPosition.Z > maximum.Z) { maximum.Z = actPosition.Z; }
            }

            return new AxisAlignedBox(minimum, maximum - minimum);
        }

        /// <summary>
        /// Gets extended material properties of the given type.
        /// </summary>
        /// <typeparam name="T">The type of properties to get.</typeparam>
        public T GetExtendedMaterialProperties<T>()
            where T : class
        {
            if (m_materialPropertiesExtended == null) { return null; }

            Type propertiesType = typeof(T);
            if (m_materialPropertiesExtended.ContainsKey(propertiesType)) { return m_materialPropertiesExtended[propertiesType] as T; }

            return null;
        }

        /// <summary>
        /// Gets an index array
        /// </summary>
        public ushort[] GetIndexArray()
        {
            return m_indices.ToArray();
        }

        /// <summary>
        /// Sets the extended material properties.
        /// </summary>
        /// <typeparam name="T">The type of properties to set.</typeparam>
        /// <param name="properties">The properties to set.</param>
        public void SetExtendedMaterialProperties<T>(T properties)
            where T : class
        {
            if (m_materialPropertiesExtended == null) { m_materialPropertiesExtended = new Dictionary<Type, object>(); }

            if (properties == null)
            {
                Type propertiesType = typeof(T);
                if (m_materialPropertiesExtended.ContainsKey(propertiesType)) { m_materialPropertiesExtended.Remove(propertiesType); }
                if (m_materialPropertiesExtended.Count == 0) { m_materialPropertiesExtended = null; }
            }
            else
            {
                m_materialPropertiesExtended[typeof(T)] = properties;
            }
        }

        /// <summary>
        /// Transforms positions and normals of all vertices using the given transform matrix
        /// </summary>
        /// <param name="transformMatrix"></param>
        public void TransformVertices(Matrix4 transformMatrix)
        {
            int length = m_vertices.Count;
            for (int loop = 0; loop < length; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(
                    Vector3.Transform(m_vertices[loop].Position, transformMatrix),
                    Vector3.TransformNormal(m_vertices[loop].Normal, transformMatrix));
            }
        }

        /// <summary>
        /// Gets an array with this object as a single item.
        /// </summary>
        public VertexStructure[] ToSingleItemArray()
        {
            return new VertexStructure[] { this };
        }

        /// <summary>
        /// Relocates all vertices by the given vector
        /// </summary>
        public void UpdateVerticesUsingRelocationBy(Vector3 relocateVector)
        {
            int length = m_vertices.Count;
            for (int loop = 0; loop < length; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(Vector3.Add(m_vertices[loop].Geometry.Position, relocateVector));
            }
        }

        public void UpdateVerticesUsingRelocationFunc(Func<Vector3, Vector3> calculatePositionFunc)
        {
            int length = m_vertices.Count;
            for (int loop = 0; loop < length; loop++)
            {
                m_vertices[loop] = m_vertices[loop].Copy(calculatePositionFunc(m_vertexCollection[loop].Position));
            }
        }

        private IEnumerable<ushort> AddPolygonByCuttingEarsInternal(IList<ushort> vertexIndices)
        {
            //Get all coordinates
            Vector3[] coordinates = new Vector3[vertexIndices.Count];
            for (int loop = 0; loop < vertexIndices.Count; loop++)
            {
                coordinates[loop] = m_vertices[loop].Position;
            }

            //Triangulate all data
            Polygon polygon = new Polygon(coordinates);
            IEnumerable<ushort> triangleIndices = polygon.TriangulateUsingCuttingEars();
            if (triangleIndices == null) { throw new CommonLibraryException("Unable to triangulate given polygon!"); }

            //Add all triangle data
            IEnumerator<ushort> indexEnumerator = triangleIndices.GetEnumerator();
            while (indexEnumerator.MoveNext())
            {
                ushort index1 = indexEnumerator.Current;
                ushort index2 = 0;
                ushort index3 = 0;

                if (indexEnumerator.MoveNext()) { index2 = indexEnumerator.Current; } else { break; }
                if (indexEnumerator.MoveNext()) { index3 = indexEnumerator.Current; } else { break; }

                this.AddTriangle(index3, index2, index1);
                this.AddTriangle(index1, index2, index3);
            }

            //Return found indices
            return triangleIndices;
        }

        /// <summary>
        /// Retrieves total count of all triangles within this structure
        /// </summary>
        public int CountTriangles
        {
            get { return m_indices.Count / 3; }
        }

        /// <summary>
        /// Retrieves total count of all vertices within this structure
        /// </summary>
        public int CountVertices
        {
            get { return m_vertices.Count; }
        }

        /// <summary>
        /// A short description for the use of this structure
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Is this structure empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return (m_vertices.Count == 0) && (m_indices.Count == 0); }
        }

        /// <summary>
        /// Gets the name of the material.
        /// </summary>
        public string Material
        {
            get { return m_materialProperties.Name; }
            set { m_materialProperties.Name = value; }
        }

        /// <summary>
        /// Gets or sets the material properties object.
        /// </summary>
        public MaterialProperties MaterialProperties
        {
            get { return m_materialProperties; }
        }

        /// <summary>
        /// The name of this structure
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                if (m_name == null) { m_name = string.Empty; }
            }
        }

        /// <summary>
        /// Retrieves a collection of triangles
        /// </summary>
        public TriangleCollection Triangles
        {
            get { return m_triangleCollection; }
        }

        /// <summary>
        /// Retrieves a collection of vertices
        /// </summary>
        public VertexCollection Vertices
        {
            get { return m_vertexCollection; }
        }

        /// <summary>
        /// Retrieves total count of all indexes within this structure
        /// </summary>
        internal int CountIndexes
        {
            get { return m_indices.Count; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all triangles of a VertexStructure object
        /// </summary>
        public class TriangleCollection : IEnumerable<Triangle>
        {
            private List<ushort> m_indices;
            private List<Vertex> m_vertices;

            /// <summary>
            ///
            /// </summary>
            internal TriangleCollection(List<ushort> indices, List<Vertex> vertices)
            {
                m_indices = indices;
                m_vertices = vertices;
            }

            /// <summary>
            ///
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(m_indices);
            }

            /// <summary>
            /// Adds a treangle to this vertex structure
            /// </summary>
            /// <param name="index1">Index of the first vertex</param>
            /// <param name="index2">Index of the second vertex</param>
            /// <param name="index3">Index of the third vertex</param>
            public int Add(ushort index1, ushort index2, ushort index3)
            {
                int result = m_indices.Count / 3;

                m_indices.Add(index1);
                m_indices.Add(index2);
                m_indices.Add(index3);

                return result;
            }

            /// <summary>
            /// Adds a treangle to this vertex structure
            /// </summary>
            /// <param name="triangle"></param>
            public int Add(Triangle triangle)
            {
                return this.Add(triangle.Index1, triangle.Index2, triangle.Index3);
            }

            /// <summary>
            ///
            /// </summary>
            public IEnumerator<Triangle> GetEnumerator()
            {
                return new Enumerator(m_indices);
            }

            /// <summary>
            /// Retrieves the triangle at the given index
            /// </summary>
            public Triangle this[int index]
            {
                get
                {
                    int startIndex = index * 3;
                    return new Triangle(m_indices[startIndex], m_indices[startIndex + 1], m_indices[startIndex + 2]);
                }
            }

            //*****************************************************************
            //*****************************************************************
            //*****************************************************************
            /// <summary>
            /// Enumerator of TriangleCollection
            /// </summary>
            private class Enumerator : IEnumerator<Triangle>
            {
                private List<ushort> m_indices;
                private int m_maxIndex;
                private int m_startIndex;

                /// <summary>
                ///
                /// </summary>
                public Enumerator(List<ushort> indices)
                {
                    m_startIndex = -3;
                    m_maxIndex = indices.Count - 3;
                    m_indices = indices;
                }

                /// <summary>
                ///
                /// </summary>
                public void Dispose()
                {
                    m_startIndex = -3;
                    m_indices = null;
                }

                /// <summary>
                ///
                /// </summary>
                public bool MoveNext()
                {
                    m_startIndex += 3;
                    return m_startIndex <= m_maxIndex;
                }

                /// <summary>
                ///
                /// </summary>
                public void Reset()
                {
                    m_startIndex = -3;
                    m_maxIndex = m_indices.Count - 3;
                }

                /// <summary>
                ///
                /// </summary>
                object System.Collections.IEnumerator.Current
                {
                    get { return new Triangle(m_indices[m_startIndex], m_indices[m_startIndex + 1], m_indices[m_startIndex + 2]); }
                }

                /// <summary>
                ///
                /// </summary>
                public Triangle Current
                {
                    get { return new Triangle(m_indices[m_startIndex], m_indices[m_startIndex + 1], m_indices[m_startIndex + 2]); }
                }
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Contains all vertices of a VertexStructure object
        /// </summary>
        public class VertexCollection : IEnumerable<Vertex>
        {
            private List<Vertex> m_vertices;

            /// <summary>
            ///
            /// </summary>
            internal VertexCollection(List<Vertex> vertices)
            {
                m_vertices = vertices;
            }

            /// <summary>
            ///
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return m_vertices.GetEnumerator();
            }

            /// <summary>
            /// Adds a vertex to the structure
            /// </summary>
            public void Add(Vertex vertex)
            {
                m_vertices.Add(vertex);
            }

            /// <summary>
            ///
            /// </summary>
            public IEnumerator<Vertex> GetEnumerator()
            {
                return m_vertices.GetEnumerator();
            }

            /// <summary>
            /// Returns the vertex at ghe given index
            /// </summary>
            public Vertex this[int index]
            {
                get { return m_vertices[index]; }
            }
        }
    }
}