using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace qvp.StateManagement
{
    /// <summary>
    /// Bounding box for all sprites
    /// </summary>
    public struct BoundingRectangle
    {
        /// <summary>
        /// X position of the bounding box
        /// </summary>
        public float X;

        /// <summary>
        /// Y position of the bounding box
        /// </summary>
        public float Y;

        /// <summary>
        /// Width of the bounding box
        /// </summary>
        public float Width;

        /// <summary>
        /// Height of the bounding box
        /// </summary>
        public float Height;

        /// <summary>
        /// Left side of the bounding box
        /// </summary>
        public float Left => X;

        /// <summary>
        /// Right side of the bounding box
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        /// Top side of the bounding box
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// Bottom side of the bounding box
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        /// Initialize the bounding rectangle with given properties (Using X,Y)
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Width of the bounding rectangle</param>
        /// <param name="height">Height of the bounding rectangle</param>
        public BoundingRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initialize the bounding rectangle with given properties (using Vector2)
        /// </summary>
        /// <param name="position">Position of the bounding rectangle</param>
        /// <param name="width">Width of the bounding rectangle</param>
        /// <param name="height">Height of the bounding rectangle</param>
        public BoundingRectangle(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Convert bounding rectangle to normal rectangle
        /// </summary>
        /// <returns>A new rectangle</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(
                (int)X,
                (int)Y,
                (int)Width,
                (int)Height
            );
        }

        /// <summary>
        /// Checks to see if a sprite has collided with another sprite
        /// </summary>
        /// <param name="other">other sprite</param>
        /// <returns></returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}
