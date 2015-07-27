namespace RK.Common.GraphicsEngine
{
    public class AnimationState
    {
        private int m_runningAnimationsIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationState" /> class.
        /// </summary>
        public AnimationState()
        {
            m_runningAnimationsIndex = 0;
        }

        /// <summary>
        /// Gets the index within the collection of running animations.
        /// </summary>
        public int RunningAnimationsIndex
        {
            get { return m_runningAnimationsIndex; }
            set { m_runningAnimationsIndex = value; }
        }
    }
}