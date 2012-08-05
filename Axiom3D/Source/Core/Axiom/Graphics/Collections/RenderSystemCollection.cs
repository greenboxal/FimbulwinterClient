#region LGPL License

// Axiom Graphics Engine Library
// Copyright � 2003-2011 Axiom Project Team
// 
// The overall design, and a majority of the core engine and rendering code 
// contained within this library is a derivative of the open source Object Oriented 
// Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
// Many thanks to the OGRE team for maintaining such a high quality project.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

#endregion

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
	/// Represents a collection of <see cref="RenderSystem">RenderSystems</see> that are sorted by name.
	/// </summary>
	public class RenderSystemCollection : AxiomCollection<RenderSystem>
	{
		#region Instance Methods

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The name of the <see cref="RenderSystem"/> to add.</param>
		/// <param name="item">A <see cref="RenderSystem"/>.</param>
		public new void Add( string key, RenderSystem item )
		{
			if ( !ContainsKey( key ) )
			{
				base.Add( key, item );
			}
			else
			{
				LogManager.Instance.Write( "{0} rendering system has already been registered by {1}, skipping {2}.", key,
				                           this[ key ].Name, item.Name );
			}
		}

		#endregion
	}
}