using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Drawing3D.Resources;

namespace RK.Common.GraphicsEngine.Core
{
    public abstract class RenderPassBase : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPassBase" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public RenderPassBase(string name)
            : base(name)
        {

        }

        /// <summary>
        /// Applies this RenderPass (called before starting rendering first objects with it).
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        public abstract void Apply(RenderState renderState);

        /// <summary>
        /// Discards this RenderPass (called after rendering all objects of this pass).
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        public abstract void Discard(RenderState renderState);
    }
}
