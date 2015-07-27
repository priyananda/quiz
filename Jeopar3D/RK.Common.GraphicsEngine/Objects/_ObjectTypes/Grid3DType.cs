using System.Collections.Generic;
using RK.Common;

namespace RK.Common.GraphicsEngine.Objects
{
    public class Grid3DType : ObjectType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grid3DType" /> class.
        /// </summary>
        public Grid3DType()
        {
            this.GenerateGround = true;
            this.LineSmallDevider = 25f;
            this.LineBigDevider = 100f;
        }

        /// <summary>
        /// Builds the structures.
        /// </summary>
        public override VertexStructure[] BuildStructure()
        {
            List<VertexStructure> result = new List<VertexStructure>();

            //Calculate parameters
            Vector3 firstCoordinate = new Vector3(
                -TilesX / 2f,
                0f,
                -TilesZ / 2f);
            float tileWidthX = this.TileWidth;
            float tileWidthZ = this.TileWidth;
            float fieldWidth = tileWidthX * TilesX;
            float fieldDepth = tileWidthZ * TilesZ;
            float fieldWidthHalf = fieldWidth / 2f;
            float fieldDepthHalf = fieldDepth / 2f;

            //Define lower ground structure
            VertexStructure lowerGround = new VertexStructure();
            if (this.GenerateGround)
            {
                lowerGround.BuildRect4V(
                    new Vector3(-fieldWidthHalf, -0.01f, -fieldDepthHalf),
                    new Vector3(fieldWidthHalf, -0.01f, -fieldDepthHalf),
                    new Vector3(fieldWidthHalf, -0.01f, fieldDepthHalf),
                    new Vector3(-fieldWidthHalf, -0.01f, fieldDepthHalf),
                    this.GroundColor);
                lowerGround.Material = this.GroundMaterial;
                result.Add(lowerGround);
            }

            //Define line structures
            VertexStructure genStructureDefaultLine = new VertexStructure();
            VertexStructure genStructureGroupLine = new VertexStructure();
            for (int actTileX = 0; actTileX < TilesX + 1; actTileX++)
            {
                Vector3 localStart = firstCoordinate + new Vector3(actTileX * tileWidthX, 0f, 0f);
                Vector3 localEnd = localStart + new Vector3(0f, 0f, tileWidthZ * TilesZ);

                VertexStructure targetStruture = actTileX % this.GroupTileCount == 0 ? genStructureGroupLine : genStructureDefaultLine;
                float devider = actTileX % this.GroupTileCount == 0 ? this.LineSmallDevider : this.LineBigDevider;
                targetStruture.BuildRect4V(
                    localStart - new Vector3(tileWidthX / devider, 0f, 0f),
                    localStart + new Vector3(tileWidthX / devider, 0f, 0f),
                    localEnd + new Vector3(tileWidthX / devider, 0f, 0f),
                    localEnd - new Vector3(tileWidthX / devider, 0f, 0f),
                    this.LineColor);
            }
            for (int actTileZ = 0; actTileZ < TilesZ + 1; actTileZ++)
            {
                Vector3 localStart = firstCoordinate + new Vector3(0f, 0f, actTileZ * tileWidthZ);
                Vector3 localEnd = localStart + new Vector3(tileWidthX * TilesX, 0f, 0f);

                VertexStructure targetStruture = actTileZ % this.GroupTileCount == 0 ? genStructureGroupLine : genStructureDefaultLine;
                float devider = actTileZ % this.GroupTileCount == 0 ? this.LineSmallDevider : this.LineBigDevider;
                targetStruture.BuildRect4V(
                    localStart + new Vector3(0f, 0f, tileWidthZ / devider),
                    localStart - new Vector3(0f, 0f, tileWidthZ / devider),
                    localEnd - new Vector3(0f, 0f, tileWidthZ / devider),
                    localEnd + new Vector3(0f, 0f, tileWidthZ / devider),
                    this.LineColor);
            }
            genStructureDefaultLine.Material = this.LineMaterial;
            genStructureGroupLine.Material = this.LineMaterial;
            if (genStructureDefaultLine.CountTriangles > 0) { result.Add(genStructureDefaultLine); }
            if (genStructureGroupLine.CountTriangles > 0) { result.Add(genStructureGroupLine); }

            //Return all generated structures
            return result.ToArray();
        }

        public string LineMaterial
        {
            get;
            set;
        }

        public string GroupLineMaterial
        {
            get;
            set;
        }

        public string GroundMaterial
        {
            get;
            set;
        }

        public int GroupTileCount
        {
            get;
            set;
        }

        public Color4 LineColor
        {
            get;
            set;
        }

        public Color4 GroundColor
        {
            get;
            set;
        }

        public float TileWidth
        {
            get;
            set;
        }

        public int TilesX
        {
            get;
            set;
        }

        public int TilesZ
        {
            get;
            set;
        }

        public bool GenerateGround
        {
            get;
            set;
        }

        public float LineSmallDevider
        {
            get;
            set;
        }

        public float LineBigDevider
        {
            get;
            set;
        }
    }
}