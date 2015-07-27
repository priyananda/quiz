using System;

namespace RK.Common.GraphicsEngine.Drawing3D
{
    public delegate void Rendering3DHandler(object sender, Rendering3DArgs e);

    public delegate void Updating3DHandler(object sender, Updating3DArgs e);

    public enum SpacialTransformationType
    {
        /// <summary>
        /// Scaling, translation and Rotation using euler angles (pitch, yaw and roll).
        /// </summary>
        ScalingTranslationEulerAngles,

        /// <summary>
        /// Scaling, tranlation and Rotation using horizontal and vertical rotation values.
        /// </summary>
        ScalingTranslationHVAngles,

        ScalingTranslationQuaternion,

        /// <summary>
        /// Translation and Rotation using euler angles (pitch, yaw and roll).
        /// </summary>
        TranslationEulerAngles,

        /// <summary>
        /// Tranlation and Rotation using horizontal and vertical rotation values.
        /// </summary>
        TranslationHVAngles
    }

    public enum MaterialApplyInstancingMode
    {
        SingleObject,

        Instanced
    }

    /// <summary>
    /// EventArgs class for Rendering3DHandler.
    /// </summary>
    public class Rendering3DArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rendering3DArgs"/> class.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public Rendering3DArgs(RenderState renderState)
        {
            this.RenderState = renderState;
        }

        /// <summary>
        /// Gets the render state.
        /// </summary>
        /// <value>Gets the render state.</value>
        public RenderState RenderState
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// EventArgs class for Updating3DHandler.
    /// </summary>
    public class Updating3DArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Updating3DArgs"/> class.
        /// </summary>
        public Updating3DArgs(UpdateState updateState)
        {
            this.UpdateState = updateState;
        }

        /// <summary>
        /// Gets or sets the update state.
        /// </summary>
        public UpdateState UpdateState
        {
            get;
            private set;
        }
    }
}