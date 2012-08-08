#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Sphere.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Math
{
    ///<summary>
    ///  A standard sphere, used mostly for bounds checking.
    ///</summary>
    ///<remarks>
    ///  A sphere in math texts is normally represented by the function
    ///  x^2 + y^2 + z^2 = r^2 (for sphere's centered on the origin). We store spheres
    ///  simply as a center point and a radius.
    ///</remarks>
    public sealed class Sphere
    {
        #region Protected member variables

        private Real radius;
        private Vector3 center;

        #endregion

        #region Constructors

        ///<summary>
        ///  Creates a unit sphere centered at the origin.
        ///</summary>
        public Sphere()
        {
            this.radius = 1.0f;
            this.center = Vector3.Zero;
        }

        /// <summary>
        ///   Creates an arbitrary spehere.
        /// </summary>
        /// <param name="center"> Center point of the sphere. </param>
        /// <param name="radius"> Radius of the sphere. </param>
        public Sphere(Vector3 center, Real radius)
        {
            this.center = center;
            this.radius = radius;
        }

        #endregion

        #region Properties

        ///<summary>
        ///  Gets/Sets the center of the sphere.
        ///</summary>
        public Vector3 Center
        {
            get { return this.center; }
            set { this.center = value; }
        }

        ///<summary>
        ///  Gets/Sets the radius of the sphere.
        ///</summary>
        public Real Radius
        {
            get { return this.radius; }
            set { this.radius = value; }
        }

        #endregion

        #region Intersection methods

        public static bool operator ==(Sphere sphere1, Sphere sphere2)
        {
            return sphere1.center == sphere2.center && sphere1.radius == sphere2.radius;
        }

        public static bool operator !=(Sphere sphere1, Sphere sphere2)
        {
            return sphere1.center != sphere2.center || sphere1.radius != sphere2.radius;
        }

        public override bool Equals(object obj)
        {
            return obj is Sphere && this == (Sphere) obj;
        }

        public override int GetHashCode()
        {
            return this.center.GetHashCode() ^ this.radius.GetHashCode();
        }


        ///<summary>
        ///  Tests for intersection between this sphere and another sphere.
        ///</summary>
        ///<param name="sphere"> Other sphere. </param>
        ///<returns> True if the spheres intersect, false otherwise. </returns>
        public bool Intersects(Sphere sphere)
        {
            return ((sphere.center - this.center).Length <= (sphere.radius + this.radius));
        }

        ///<summary>
        ///  Returns whether or not this sphere interects a box.
        ///</summary>
        ///<param name="box"> </param>
        ///<returns> True if the box intersects, false otherwise. </returns>
        public bool Intersects(AxisAlignedBox box)
        {
            return Utility.Intersects(this, box);
        }

        ///<summary>
        ///  Returns whether or not this sphere interects a plane.
        ///</summary>
        ///<param name="plane"> </param>
        ///<returns> True if the plane intersects, false otherwise. </returns>
        public bool Intersects(Plane plane)
        {
            return Utility.Intersects(this, plane);
        }

        ///<summary>
        ///  Returns whether or not this sphere interects a Vector3.
        ///</summary>
        ///<param name="vector"> </param>
        ///<returns> True if the vector intersects, false otherwise. </returns>
        public bool Intersects(Vector3 vector)
        {
            return (vector - this.center).Length <= this.radius;
        }

        #endregion Intersection methods
    }
}