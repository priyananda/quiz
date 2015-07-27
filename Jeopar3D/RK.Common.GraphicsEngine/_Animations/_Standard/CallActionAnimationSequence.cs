using System;
using RK.Common.GraphicsEngine.Drawing3D;

namespace RK.Common.GraphicsEngine
{
    public class CallActionAnimationSequence : AnimationSequence
    {
        private Action m_actionToCall;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallActionAnimationSequence" /> class.
        /// </summary>
        public CallActionAnimationSequence(Action actionToCall)
            : base()
        {
            m_actionToCall = actionToCall;
        }

        /// <summary>
        /// Called each time the CurrentTime value gets updated.
        /// </summary>
        /// <param name="updateState"></param>
        /// <param name="animationState"></param>
        protected override async void OnCurrentTimeUpdated(UpdateState updateState, AnimationState animationState)
        {
            Scene sourceScene = this.TargetObject.Scene;
            if (sourceScene == null) { throw new InvalidOperationException("Unable to animate an object that is not assigned to a scene!"); }

            //Run action on start of next update pass
            await sourceScene.PerformBeforeUpdateAsync(() =>
            {
                try
                {
                    m_actionToCall();
                }
                finally
                {
                    base.NotifyAnimationFinished();
                }
            });
        }
    }
}