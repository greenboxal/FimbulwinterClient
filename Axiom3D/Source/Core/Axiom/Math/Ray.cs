#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Ray.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Math.Collections;

#endregion Namespace Declarations

namespace Axiom.Math
{
    /// <summary>
    ///   Representation of a ray in space, ie a line with an origin and direction.
    /// </summary>
    public class Ray
    {
        #region Fields

        internal Vector3 origin;
        internal Vector3 direction;

        #endregion

        #region Constructors

        /// <summary>
        ///   Default constructor.
        /// </summary>
        public Ray()
        {
            this.origin = Vector3.Zero;
            this.direction = Vector3.UnitZ;
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="origin"> Starting point of the ray. </param>
        /// <param name="direction"> Direction the ray is pointing. </param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the position of a point t units along the ray.
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public Vector3 GetPoint(Real t)
        {
            return this.origin + (this.direction*t);
        }

        /// <summary>
        ///   Gets the position of a point t units along the ray.
        /// </summary>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public Vector3 this[Real t]
        {
            get { return this.origin + (this.direction*t); }
        }

        #endregion Methods

        #region Intersection Methods

        ///<summary>
        ///  Tests whether this ray intersects the given box.
        ///</summary>
        ///<param name="box"> </param>
        ///<returns> Struct containing info on whether there was a hit, and the distance from the origin of this ray where the intersect happened. </returns>
        public IntersectResult Intersects(AxisAlignedBox box)
        {
            return Utility.Intersects(this, box);
        }

        ///<summary>
        ///  Tests whether this ray intersects the given plane.
        ///</summary>
        ///<param name="plane"> </param>
        ///<returns> Struct containing info on whether there was a hit, and the distance from the origin of this ray where the intersect happened. </returns>
        public IntersectResult Intersects(Plane plane)
        {
            return Utility.Intersects(this, plane);
        }

        ///<summary>
        ///  Tests whether this ray intersects the given sphere.
        ///</summary>
        ///<param name="sphere"> </param>
        ///<returns> Struct containing info on whether there was a hit, and the distance from the origin of this ray where the intersect happened. </returns>
        public IntersectResult Intersects(Sphere sphere)
        {
            return Utility.Intersects(this, sphere);
        }

        ///<summary>
        ///  Tests whether this ray intersects the given PlaneBoundedVolume.
        ///</summary>
        ///<param name="volume"> </param>
        ///<returns> Struct containing info on whether there was a hit, and the distance from the origin of this ray where the intersect happened. </returns>
        public IntersectResult Intersects(PlaneBoundedVolume volume)
        {
            return Utility.Intersects(this, volume);
        }

        #endregion Intersection Methods

        #region Operator Overloads

        /// <summary>
        ///   Gets the position of a point t units along the ray.
        /// </summary>
        /// <param name="ray"> </param>
        /// <param name="t"> </param>
        /// <returns> </returns>
        public static Vector3 operator *(Ray ray, Real t)
        {
            return ray.origin + (ray.direction*t);
        }

        public static bool operator ==(Ray left, Ray right)
        {
            return left.direction == right.direction && left.origin == right.origin;
        }

        public static bool operator !=(Ray left, Ray right)
        {
            return left.direction != right.direction || left.origin != right.origin;
        }

        public override bool Equals(object obj)
        {
            return obj is Ray && this == (Ray) obj;
        }

        public override int GetHashCode()
        {
            return this.direction.GetHashCode() ^ this.origin.GetHashCode();
        }

        #endregion Operator Overloads

        #region Properties

        /// <summary>
        ///   Gets/Sets the origin of the ray.
        /// </summary>
        public Vector3 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        ///   Gets/Sets the direction this ray is pointing.
        /// </summary>
        /// <remarks>
        ///   A ray has no length, so the direction goes to infinity.
        /// </remarks>
        public Vector3 Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        #endregion
    }
}