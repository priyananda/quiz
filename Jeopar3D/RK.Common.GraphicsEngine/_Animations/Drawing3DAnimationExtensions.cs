using System;

namespace RK.Common.GraphicsEngine
{
    public static class Drawing3DAnimationExtensions
    {
        /// <summary>
        /// Moves current object by the given move vector.
        /// </summary>
        /// <param name="sequenceBuilder">AnimationSequenceBuilder building the animation.</param>
        /// <param name="moveVector">The move vector.</param>
        /// <param name="animationTime">Total time for the animation.</param>
        public static AnimationSequenceBuilder Move3DBy(this AnimationSequenceBuilder sequenceBuilder, Vector3 moveVector, TimeSpan animationTime)
        {
            sequenceBuilder.Add(
                new Move3DByAnimationSequence(moveVector, animationTime));
            return sequenceBuilder;
        }

        /// <summary>
        /// Scales current object by the given move vector.
        /// </summary>
        /// <param name="sequenceBuilder">AnimationSequenceBuilder building the animation.</param>
        /// <param name="moveVector">The scale vector.</param>
        /// <param name="animationTime">Total time for the animation.</param>
        public static AnimationSequenceBuilder Scale3DBy(this AnimationSequenceBuilder sequenceBuilder, Vector3 scaleVector, TimeSpan animationTime)
        {
            sequenceBuilder.Add(
                new Scale3DByAnimationSequence(scaleVector, animationTime));
            return sequenceBuilder;
        }

        public static AnimationSequenceBuilder RotateHVBy(this AnimationSequenceBuilder sequenceBuilder, Vector2 rotateVector, TimeSpan animationTime)
        {
            sequenceBuilder.Add(
                new RotateHVByAnimationSequence(rotateVector, animationTime));
            return sequenceBuilder;
        }
    }
}