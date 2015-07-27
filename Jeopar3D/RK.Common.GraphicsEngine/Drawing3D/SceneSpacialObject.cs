namespace RK.Common.GraphicsEngine.Drawing3D
{
    public abstract class SceneSpacialObject : SceneObject
    {
        private SpacialTransformationType m_transformationType;
        private Vector3 m_position;
        private Vector3 m_rotation;
        private Vector2 m_rotationHW;
        private Quaternion m_rotationQuaternion;
        private Vector3 m_scaling;
        private Matrix4 m_transform;
        private bool m_transformParamsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneSpacialObject"/> class.
        /// </summary>
        public SceneSpacialObject()
        {
            m_transformationType = SpacialTransformationType.ScalingTranslationEulerAngles;
            m_position = Vector3.Empty;
            m_rotationHW = Vector2.Empty;
            m_rotation = Vector3.Empty;
            m_scaling = new Vector3(1f, 1f, 1f);
            m_transform = Matrix4.Identity;
            m_rotationQuaternion = Quaternion.Empty;
            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Zooms the camera into or out along the actual target-vector.
        /// </summary>
        /// <param name="dist">The Distance you want to zoom.</param>
        public void MoveForward(float dist)
        {
            Vector3 look = this.Look;

            m_position.X += dist * look.X;
            m_position.Y += dist * look.Y;
            m_position.Z += dist * look.Z;

            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Moves the object position.
        /// </summary>
        /// <param name="x">moving in x direction.</param>
        /// <param name="z">moving in z direction.</param>
        public void Move(float x, float z)
        {
            m_position.X += x;
            m_position.Z += z;

            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Moves the object up and down.
        /// </summary>
        public void UpDown(float points)
        {
            Vector3 up = this.Up;

            m_position.X = m_position.X + up.X * points;
            m_position.Y = m_position.Y + up.Y * points;
            m_position.Z = m_position.Z + up.Z * points;

            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Moves the object up and down.
        /// </summary>
        public void UpDownWithoutMoving(float points)
        {
            Vector3 up = this.Up;

            m_position.Y = m_position.Y + up.Y * points;

            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Straves the object.
        /// </summary>
        public void Strave(float points)
        {
            Vector3 right = this.Right;

            m_position.X = m_position.X + right.X * points;
            m_position.Y = m_position.Y + right.Y * points;
            m_position.Z = m_position.Z + right.Z * points;

            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Streaves the object.
        /// </summary>
        public void StraveAtPlane(float points)
        {
            Vector3 right = this.Right;

            m_position.X = m_position.X + right.X * points;
            m_position.Z = m_position.Z + right.Z * points;

            m_transformParamsChanged = true;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="updateState">Current update state.</param>
        protected override void UpdateInternal(UpdateState updateState)
        {
            //Calculates local transform matrix (transforms local space to world space)
            if (m_transformParamsChanged)
            {
                m_transformParamsChanged = false;

                switch (m_transformationType)
                {
                    case Drawing3D.SpacialTransformationType.ScalingTranslationEulerAngles:
                        m_transform =
                            Matrix4.Scaling(m_scaling) *
                            Matrix4.RotationYawPitchRoll(m_rotation.Y, m_rotation.X, m_rotation.Z) *
                            Matrix4.Translation(m_position) *
                            updateState.World.Top;
                        break;

                    case Drawing3D.SpacialTransformationType.ScalingTranslationHVAngles:
                        m_transform =
                            Matrix4.Scaling(m_scaling) *
                            Matrix4.RotationHV(m_rotationHW) *
                            Matrix4.Translation(m_position) *
                            updateState.World.Top;
                        break;

                    case Drawing3D.SpacialTransformationType.ScalingTranslationQuaternion:
                        m_transform =
                            Matrix4.Scaling(m_scaling) *
                            Matrix4.RotationQuaternion(m_rotationQuaternion) *
                            Matrix4.Translation(m_position) *
                            updateState.World.Top;
                        break;

                    case Drawing3D.SpacialTransformationType.TranslationEulerAngles:
                        m_transform =
                            Matrix4.RotationYawPitchRoll(m_rotation.Y, m_rotation.X, m_rotation.Z) *
                            Matrix4.Translation(m_position) *
                            updateState.World.Top;
                        break;

                    case Drawing3D.SpacialTransformationType.TranslationHVAngles:
                        m_transform =
                            Matrix4.RotationHV(m_rotationHW) *
                            Matrix4.Translation(m_position) *
                            updateState.World.Top;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets current position.
        /// </summary>
        public Vector3 Position
        {
            get { return m_position; }
            set
            {
                m_position = value;
                m_transformParamsChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets current rotation.
        /// </summary>
        public Vector3 Rotation
        {
            get { return m_rotation; }
            set
            {
                m_rotation = value;
                m_transformParamsChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets horizontal and vertical rotation values.
        /// </summary>
        public Vector2 RotationHV
        {
            get { return m_rotationHW; }
            set
            {
                m_rotationHW = value;
                m_transformParamsChanged = true;
            }
        }

        public Quaternion RotationQuaternion
        {
            get { return m_rotationQuaternion; }
            set
            {
                m_rotationQuaternion = value;
                m_transformParamsChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the used rotation type.
        /// </summary>
        public SpacialTransformationType TransformationType
        {
            get { return m_transformationType; }
            set
            {
                m_transformationType = value;
                m_transformParamsChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets current scaling.
        /// </summary>
        public Vector3 Scaling
        {
            get { return m_scaling; }
            set
            {
                m_scaling = value;
                m_transformParamsChanged = true;
            }
        }

        /// <summary>
        /// Gets a matrix that transforms local space to world space.
        /// </summary>
        public Matrix4 Transform
        {
            get { return m_transform; }
        }

        /// <summary>
        /// Gets the vector that looks up.
        /// </summary>
        public Vector3 Up
        {
            get
            {
                Vector3 result = new Vector3(0f, 1f, 0f);
                result.Transform(m_transform);
                return result;
            }
        }

        /// <summary>
        /// Gets the vector that looks into front.
        /// </summary>
        public Vector3 Look
        {
            get
            {
                Vector3 result = new Vector3(0f, 0f, 1f);
                result.Transform(m_transform);
                return result;
            }
        }

        /// <summary>
        /// Gets the vector that looks to the right.
        /// </summary>
        public Vector3 Right
        {
            get
            {
                Vector3 result = new Vector3(1f, 0f, 0f);
                result.Transform(m_transform);
                return result;
            }
        }
    }
}