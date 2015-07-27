using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RK.Common
{
    public struct AxisAlignedBox2D
    {
        public static readonly AxisAlignedBox2D Empty = new AxisAlignedBox2D(Vector2.Empty, Vector2.Empty);

        public Vector2 Location;
        public Vector2 Size;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisAlignedBox2D" /> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="size">The size.</param>
        public AxisAlignedBox2D(Vector2 location, Vector2 size)
        {
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Pos: " + this.Location.ToString() + "; Size: " + this.Size.ToString();
        }

        /// <summary>
        /// Is this box contained by the given one?
        /// </summary>
        /// <param name="otherOne"></param>
        public bool IsContainedBy(AxisAlignedBox2D otherOne)
        {
            Vector2 thisMinimum = this.Location;
            Vector2 thisMaximum = this.Location + this.Size;
            Vector2 otherMinimum = otherOne.Location;
            Vector2 otherMaximum = otherOne.Location + otherOne.Size;

            return (otherMinimum.X <= thisMinimum.X) &&
                   (otherMinimum.Y <= thisMinimum.Y) &&
                   (otherMaximum.X >= thisMaximum.X) &&
                   (otherMaximum.Y >= thisMaximum.Y);
        }

        /// <summary>
        /// Is the given smaller box contained by the given bigger one?
        /// </summary>
        /// <param name="smallerBox"></param>
        /// <param name="biggerBox"></param>
        public static bool IsContainedBy(AxisAlignedBox2D smallerBox, AxisAlignedBox2D biggerBox)
        {
            return smallerBox.IsContainedBy(biggerBox);
        }

        /// <summary>
        /// Is this box empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Location.IsEmpty() && this.Size.IsEmpty(); }
        }
    }
}
