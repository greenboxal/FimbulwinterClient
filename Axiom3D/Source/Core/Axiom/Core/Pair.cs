#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: Pair.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   A simple container class for returning a pair of objects from a method call
    ///   (similar to std::pair).
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public class Pair<T> : IEquatable<Pair<T>>
    {
        private Axiom.Math.Tuple<T, T> data;

        public T First
        {
            get { return this.data.First; }
            set { this.data = new Axiom.Math.Tuple<T, T>(value, this.data.Second); }
        }

        public T Second
        {
            get { return this.data.Second; }
            set { this.data = new Axiom.Math.Tuple<T, T>(this.data.First, value); }
        }

        public Pair(T first, T second)
        {
            this.data = new Axiom.Math.Tuple<T, T>(first, second);
        }

        #region IEquatable<Pair<T>> Implementation

        public bool Equals(Pair<T> other)
        {
            return this.data.Equals(other.data);
        }


        public override bool Equals(object other)
        {
            if (other is Pair<T>)
            {
                return Equals((Pair<T>) other);
            }
            return false;
        }

        #endregion IEquatable<Pair<T>> Implementation

        #region System.Object Implementation

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }

        #endregion System.Object Implementation
    }
}