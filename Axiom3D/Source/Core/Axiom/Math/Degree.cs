#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Degree.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
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
    ///   Wrapper class which indicates a given angle value is in Radian.
    /// </summary>
    /// <remarks>
    ///   Degree values are interchangeable with Radian values, and conversions
    ///   will be done automatically between them.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
#if !( XBOX || XBOX360 )
    [Serializable]
    public struct Degree : ISerializable, IComparable<Degree>, IComparable<Radian>, IComparable<Real>
#else
	public struct Degree : IComparable<Degree>, IComparable<Radian>, IComparable<Real>
#endif
    {
        private static readonly Real _degreesToRadians = Utility.PI/180.0f;

        public static readonly Degree Zero = Real.Zero;

        private Real _value;

        public Degree(Real r)
        {
            this._value = r;
        }

        public Degree(Degree d)
        {
            this._value = d._value;
        }

        public Degree(Radian r)
        {
            this._value = r.InDegrees;
        }

        public Radian InRadians
        {
            get { return this._value*_degreesToRadians; }
        }

        public static implicit operator Degree(Real value)
        {
            Degree retVal;
            retVal._value = value;
            return retVal;
        }

        public static implicit operator Degree(Radian value)
        {
            Degree retVal;
            retVal._value = value;
            return retVal;
        }

        public static implicit operator Degree(Numeric value)
        {
            Degree retVal;
            retVal._value = value;
            return retVal;
        }

        public static explicit operator Degree(int value)
        {
            Degree retVal;
            retVal._value = value;
            return retVal;
        }

        public static implicit operator Real(Degree value)
        {
            return value._value;
        }

        public static explicit operator Numeric(Degree value)
        {
            return value._value;
        }

        public static Degree operator +(Degree left, Real right)
        {
            return left._value + right;
        }

        public static Degree operator +(Degree left, Degree right)
        {
            return left._value + right._value;
        }

        public static Degree operator +(Degree left, Radian right)
        {
            return left + right.InDegrees;
        }

        public static Degree operator -(Degree r)
        {
            return -r._value;
        }

        public static Degree operator -(Degree left, Real right)
        {
            return left._value - right;
        }

        public static Degree operator -(Degree left, Degree right)
        {
            return left._value - right._value;
        }

        public static Degree operator -(Degree left, Radian right)
        {
            return left - right.InDegrees;
        }

        public static Degree operator *(Degree left, Real right)
        {
            return left._value*right;
        }

        public static Degree operator *(Real left, Degree right)
        {
            return left*right._value;
        }

        public static Degree operator *(Degree left, Degree right)
        {
            return left._value*right._value;
        }

        public static Degree operator *(Degree left, Radian right)
        {
            return left._value*right.InDegrees;
        }

        public static Degree operator /(Degree left, Real right)
        {
            return left._value/right;
        }

        public static bool operator <(Degree left, Degree right)
        {
            return left._value < right._value;
        }

        public static bool operator ==(Degree left, Degree right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(Degree left, Degree right)
        {
            return left._value != right._value;
        }

        public static bool operator >(Degree left, Degree right)
        {
            return left._value > right._value;
        }

        public override bool Equals(object obj)
        {
            return (obj is Degree && this == (Degree) obj);
        }

        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

#if !( XBOX || XBOX360 )

        #region ISerializable Implementation

        private Degree(SerializationInfo info, StreamingContext context)
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

        public int CompareTo(Degree other)
        {
            return this._value.CompareTo(other);
        }

        public int CompareTo(Radian other)
        {
            return this._value.CompareTo(other.InDegrees);
        }

        public int CompareTo(Real other)
        {
            return this._value.CompareTo(other);
        }

        #endregion
    }
}