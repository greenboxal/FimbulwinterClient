#region SVN Version Information

// <file>
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Pair.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Math.Collections
{
    /// <summary>
    ///   A simple container class for returning a pair of objects from a method call 
    ///   (similar to std::pair, minus the templates).
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class Pair
    {
        public object first;
        public object second;

        public Pair(object first, object second)
        {
            this.first = first;
            this.second = second;
        }
    }
}