
//Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Drawing3D.Resources
{
    public abstract class MaterialResource : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialResource"/> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        public MaterialResource(string name)
            : base(name)
        {

        }

        /// <summary>
        /// Applies the material to the given render state.
        /// </summary>
        /// <param name="renderState">Current render state</param>
        /// <param name="applyMode">How should apply happen?</param>
        /// <param name="instancingMode">The instancing mode for which to apply the material.</param>
        public virtual void Apply(RenderState renderState, MaterialApplyMode applyMode, MaterialApplyInstancingMode instancingMode) { }

        /// <summary>
        /// Discards the material in current render state.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public virtual void Discard(RenderState renderState) { }

        /// <summary>
        /// Generates the requested input layout.
        /// </summary>
        /// <param name="inputElements">An array of InputElements describing vertex input structure.</param>
        /// <param name="instancingMode">Instancing mode for which to generate the input layout for.</param>
        public abstract D3D11.InputLayout GenerateInputLayout(D3D11.InputElement[] inputElements, MaterialApplyInstancingMode instancingMode);

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        /// <value></value>
        public override bool IsLoaded
        {
            get { return true; }
        }
    }
}
