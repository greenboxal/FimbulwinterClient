#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ICopyable.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

namespace Axiom.Core
{
    internal interface ICopyable<T>
    {
        void CopyTo(T dest);
    }
}