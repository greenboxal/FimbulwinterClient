#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: RenderPriorityGroupList.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Collections;

#endregion Namespace Declarations

namespace Axiom.Graphics.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="RenderPriorityGroup" /> objects sorted by priority.
    /// </summary>
    public class RenderPriorityGroupList : AxiomSortedCollection<ushort, RenderPriorityGroup>
    {
    }
}