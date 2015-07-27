﻿using System;

namespace RK.Common
{
    public struct Ray2D
    {
        public Vector2 Origin;
        public Vector2 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray2D" /> struct.
        /// </summary>
        public Ray2D(Vector2 origin, Vector2 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        /// <summary>
        /// Calculates the intersection point to the intersection seen from local origin position.
        /// </summary>
        /// <param name="other">The ray to calculate intersection with</param>
        public Tuple<bool, Vector2> Intersect(Ray2D other)
        {
            //Intersection method taken from http://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines
            
            float a1 = this.A;
            float b1 = this.B;
            float c1 = this.C;
            float a2 = other.A;
            float b2 = other.B;
            float c2 = other.C;

            //float delta = A1 * B2 - A2 * B1;
            //if (delta == 0)
            //    throw new ArgumentException("Lines are parallel");

            //float x = (B2 * C1 - B1 * C2) / delta;
            //float y = (A1 * C2 - A2 * C1) / delta;

            float delta = a1 * b2 - a2 * b1;
            if (delta == 0)
            {
                return Tuple.Create(false, Vector2.Empty);
            }

            float intersectionX = (b2 * c1 - b1 * c2) / delta;
            float intersectionY = (a1 * c2 - a2 * c1) / delta;

            return Tuple.Create(true, new Vector2(intersectionX, intersectionY));
        }

        /// <summary>
        /// Performs a equality check with a slight tolerance.
        /// </summary>
        /// <param name="otherRay">The other ray to check.</param>
        public bool EqualsWithTolerance(Ray2D otherRay)
        {
            return this.Origin.EqualsWithTolerance(otherRay.Origin) &&
                   this.Direction.EqualsWithTolerance(otherRay.Direction);
        }

        public float A
        {
            get 
            {
                Vector2 start = this.Origin;
                Vector2 end = this.Origin + Direction;
                return end.Y - start.Y; 
            }
        }

        public float B
        {
            get 
            {
                Vector2 start = this.Origin;
                Vector2 end = this.Origin + Direction;
                return start.X - end.X; 
            }
        }

        public float C
        {
            get 
            {
                Vector2 start = this.Origin;
                Vector2 end = this.Origin + Direction;
                return this.A * start.X + this.B * start.Y;
            }
        }
    }
}
