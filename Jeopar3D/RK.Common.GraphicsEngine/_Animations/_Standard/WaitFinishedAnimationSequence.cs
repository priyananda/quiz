namespace RK.Common.GraphicsEngine
{
    public class WaitFinishedAnimationSequence : AnimationSequence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitFinishedAnimationSequence" /> class.
        /// </summary>
        public WaitFinishedAnimationSequence()
            : base()
        {
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        /// <param name="updateState"></param>
        /// <param name="animationState"></param>
        protected override void OnCurrentTimeUpdated(Drawing3D.UpdateState updateState, AnimationState animationState)
        {
            if (animationState.RunningAnimationsIndex == 0) { base.NotifyAnimationFinished(); }
        }

        /// <summary>
        /// Is this animation a blocking animation?
        /// If true, all following animation have to wait for finish-event.
        /// </summary>
        public override bool IsBlocking
        {
            get { return true; }
        }
    }
}