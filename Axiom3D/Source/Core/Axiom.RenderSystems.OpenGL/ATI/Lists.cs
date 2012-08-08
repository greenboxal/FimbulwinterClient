#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Lists.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL.ATI
{
    public class TokenInstructionList : List<TokenInstruction>
    {
        public void Resize(int size)
        {
            TokenInstruction[] data = ToArray();
            TokenInstruction[] newData = new TokenInstruction[size];
            Array.Copy(data, 0, newData, 0, size);
            Clear();
            AddRange(newData);
        }
    }
}