#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ConfigOptionMap.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Collections.Generic;
using Axiom.Collections;
using Axiom.Configuration;

#endregion

namespace Axiom.Graphics.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="ConfigOption">ConfigOptions</see> that are sorted by key.
    /// </summary>
    public class ConfigOptionMap : AxiomCollection<ConfigOption> //Dictionary<string, ConfigOption>
    {
        public override void Add(ConfigOption item)
        {
            Add(item.Name, item);
        }
    }
}