#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Radian.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#if AXIOM_REAL_AS_SINGLE || !( AXIOM_REAL_AS_DOUBLE )
using Numeric = System.Single;
#else
using Numeric = System.Double;
#endif
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion Namespace Declarations

namespace Axiom.Math
{
    /// <summary>
    ///   Wrapper class which indicates a given angle value is in Radians.
    /// </summary>
    /// <remarks>
    ///   Radian values are interchangeable with Degree values, and conversions
    ///   will be done automatically between them.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
#if !( XBOX || XBOX360 )
    [Serializable]
    public struct Radian : ISerializable, IComparable<Radian>, IComparable<Degree>, IComparable<Real>
#else
	public struct Radian : IComparable<Radian>, IComparable<Degree>, IComparable<Real>
#endif
    {
        private static readonly Real _radiansToDegrees = 180.0f/Utility.PI;

        public static readonly Radian Zero = Real.Zero;

        private Real _value;

        public Radian(Real r)
        {
            this._value = r;
        }

        public Radian(Radian r)
        {
            this._value = r._value;
        }

        public Radian(Degree d)
        {
            this._value = d.InRadians;
        }

        public Degree InDegrees
        {
            get { return this._value*_radiansToDegrees; }
        }

        public static implicit operator Radian(Real value)
        {
            Radian retVal;
            retVal._value = value;
            return retVal;
        }

        public static implicit operator Radian(Degree value)
        {
            Radian retVal;
            retVal._value = value;
            return retVal;
        }

        public static implicit operator Radian(Numeric value)
        {
            Radian retVal;
            retVal._value = value;
            return retVal;
        }

        public static explicit operator Radian(int value)
        {
            Radian retVal;
            retVal._value = value;
            return retVal;
        }

        public static implicit operator Real(Radian value)
        {
            return value._value;
        }

        public static explicit operator Numeric(Radian value)
        {
            return value._value;
        }

        public static Radian operator +(Radian left, Real right)
        {
            return left._value + right;
        }

        public static Radian operator +(Radian left, Radian right)
        {
            return left._value + right._value;
        }

        public static Radian operator +(Radian left, Degree right)
        {
            return left + right.InRadians;
        }

        public static Radian operator -(Radian r)
        {
            return -r._value;
        }

        public static Radian operator -(Radian left, Real right)
        {
            return left._value - right;
        }

        public static Radian operator -(Radian left, Radian right)
        {
            return left._value - right._value;
        }

        public static Radian operator -(Radian left, Degree right)
        {
            return left - right.InRadians;
        }

        public static Radian operator *(Radian left, Real right)
        {
            return left._value*right;
        }

        public static Radian operator *(Real left, Radian right)
        {
            return left*right._value;
        }

        public static Radian operator *(Radian left, Radian right)
        {
            return left._value*right._value;
        }

        public static Radian operator *(Radian left, Degree right)
        {
            return left._value*right.InRadians;
        }

        public static Radian operator /(Radian left, Real right)
        {
            return left._value/right;
        }

        public static bool operator <(Radian left, Radian right)
        {
            return left._value < right._value;
        }

        public static bool operator ==(Radian left, Radian right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(Radian left, Radian right)
        {
            return left._value != right._value;
        }

        public static bool operator >(Radian left, Radian right)
        {
            return left._value > right._value;
        }

        public override bool Equals(object obj)
        {
            return (obj is Radian && this == (Radian) obj);
        }

        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

#if !( XBOX || XBOX360 )

        #region ISerializable Implementation

        private Radian(SerializationInfo info, StreamingContext context)
        {
            this._value = (Real) info.GetValue("value", typeof (Real));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", this._value);
        }

        #endregion ISerializableImplementation

#endif

        #region IComparable<T> Members

        public int CompareTo(Radian other)
        {
            return this._value.CompareTo(other._value);
        }

        public int CompareTo(Degree other)
        {
            return this._value.CompareTo(other.InRadians);
        }

        public int CompareTo(Real other)
        {
            return this._value.CompareTo(other);
        }

        #endregion
    }
}