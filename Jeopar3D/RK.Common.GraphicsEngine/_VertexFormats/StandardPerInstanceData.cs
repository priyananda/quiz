using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RK.Common.GraphicsEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StandardPerInstanceData
    {
        public static readonly int Size = Marshal.SizeOf(typeof(StandardPerInstanceData));

        //Standard elements for per instance data
        public Matrix4 InstanceTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardPerInstanceData" /> struct.
        /// </summary>
        /// <param name="instanceTransform">The transformation for this instance.</param>
        public StandardPerInstanceData(Matrix4 instanceTransform)
        {
            this.InstanceTransform = instanceTransform;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Instance transform: " + this.InstanceTransform;
        }
    }
}
