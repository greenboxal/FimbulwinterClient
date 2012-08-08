#region Namespace Declarations

using System;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Math
{
    ///<summary>
    ///  Represents two related values
    ///</summary>
    public class Tuple<A, B> : IEquatable<Tuple<A, B>>
    {
        #region Fields and Properties

        /// <summary>
        /// </summary>
        public readonly A First;

        /// <summary>
        /// </summary>
        public readonly B Second;

        #endregion Fields and Properties

        #region Construction and Destruction

        public Tuple(A first, B second)
        {
            this.First = first;
            this.Second = second;
        }

        #endregion Construction and Destruction

        #region IEquatable<Tuple<A,B>> Implementation

        public bool Equals(Tuple<A, B> other)
        {
            return this.First.Equals(other.First) && this.Second.Equals(other.Second);
        }

        public override bool Equals(object other)
        {
            if (other is Tuple<A, B>)
            {
                return Equals((Tuple<A, B>) other);
            }
            return false;
        }

        #endregion IEquatable<Tuple<A,B>> Implementation
    }

    /// <summary>
    ///   Represents three related values
    /// </summary>
    /// <typeparam name="A"> </typeparam>
    /// <typeparam name="B"> </typeparam>
    /// <typeparam name="C"> </typeparam>
    public struct Tuple<A, B, C> : IEquatable<Tuple<A, B, C>>
    {
        #region Fields and Properties

        /// <summary>
        /// </summary>
        public readonly A First;

        /// <summary>
        /// </summary>
        public readonly B Second;

        /// <summary>
        /// </summary>
        public readonly C Third;

        #endregion Fields and Properties

        #region Construction and Destruction

        public Tuple(A first, B second, C Third)
        {
            this.First = first;
            this.Second = second;
            this.Third = Third;
        }

        #endregion Construction and Destruction

        #region IEquatable<Tuple<A,B,C>> Implementation

        public bool Equals(Tuple<A, B, C> other)
        {
            return this.First.Equals(other.First) && this.Second.Equals(other.Second) && this.Third.Equals(other.Third);
        }

        public override bool Equals(object other)
        {
            if (other is Tuple<A, B, C>)
            {
                return Equals((Tuple<A, B, C>) other);
            }
            return false;
        }

        #endregion IEquatable<Tuple<A,B,C>> Implementation
    }

    /// <summary>
    ///   Represents four related values
    /// </summary>
    /// <typeparam name="A"> </typeparam>
    /// <typeparam name="B"> </typeparam>
    /// <typeparam name="C"> </typeparam>
    /// <typeparam name="D"> </typeparam>
    public struct Tuple<A, B, C, D> : IEquatable<Tuple<A, B, C, D>>
    {
        #region Fields and Properties

        /// <summary>
        /// </summary>
        public readonly A First;

        /// <summary>
        /// </summary>
        public readonly B Second;

        /// <summary>
        /// </summary>
        public readonly C Third;

        /// <summary>
        /// </summary>
        public readonly D Fourth;

        #endregion Fields and Properties

        #region Construction and Destruction

        public Tuple(A first, B second, C third, D fourth)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
            this.Fourth = fourth;
        }

        #endregion Construction and Destruction

        #region IEquatable<Tuple<A,B,C,D>> Implementation

        public bool Equals(Tuple<A, B, C, D> other)
        {
            return this.First.Equals(other.First) && this.Second.Equals(other.Second) && this.Third.Equals(other.Third);
        }

        public override bool Equals(object other)
        {
            if (other is Tuple<A, B, C, D>)
            {
                return Equals((Tuple<A, B, C, D>) other);
            }
            return false;
        }

        #endregion IEquatable<Tuple<A,B,C,D>> Implementation
    }
}