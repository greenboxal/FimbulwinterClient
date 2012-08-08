#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: RenderSystemCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Graphics;
using Axiom.Collections;

#endregion

namespace Axiom.Graphics.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="RenderSystem">RenderSystems</see> that are sorted by name.
    /// </summary>
    public class RenderSystemCollection : AxiomCollection<RenderSystem>
    {
        #region Instance Methods

        /// <summary>
        ///   Adds the specified key.
        /// </summary>
        /// <param name="key"> The name of the <see cref="RenderSystem" /> to add. </param>
        /// <param name="item"> A <see cref="RenderSystem" /> . </param>
        public new void Add(string key, RenderSystem item)
        {
            if (!ContainsKey(key))
            {
                base.Add(key, item);
            }
            else
            {
                LogManager.Instance.Write("{0} rendering system has already been registered by {1}, skipping {2}.", key,
                                          this[key].Name, item.Name);
            }
        }

        #endregion
    }
}