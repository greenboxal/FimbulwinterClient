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
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Core.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="Light">Lights</see>.
    /// </summary>
    public class LightList : List<Light>
    {
    }

    /// <summary>
    ///   Represents a collection of <see cref="Entity">Entities</see>
    /// </summary>
    public class EntityList : List<Entity>
    {
    }

    /// <summary>
    ///   Represents a collection of <see cref="SubEntity" /> objects
    /// </summary>
    /// <remarks>
    ///   The items are sorted by their implicit index, it is important that the order of  subentities in the collection maps to the order
    ///   of submeshes in a <see cref="SubMeshList" />
    /// </remarks>
    public class SubEntityList : List<SubEntity>
    {
    }

    /// <summary>
    ///   Represents a collection of <see cref="SubMesh">SubMeshes</see>
    /// </summary>
    /// <remarks>
    ///   The items are sorted by their implicit index, it is important that the order of  submeshes in the collection maps to the order
    ///   of subentities in a <see cref="SubEntityList" />
    /// </remarks>
    public class SubMeshList : List<SubMesh>
    {
    }

    public class MeshLodUsageList : List<MeshLodUsage>
    {
    }

    public class LodValueList : List<Real>
    {
    }

    public class IntList : List<int>
    {
        public void Resize(int size)
        {
            int[] data = ToArray();
            int[] newData = new int[size];
            Array.Copy(data, 0, newData, 0, size);
            Clear();
            AddRange(newData);
        }
    }

    public class RealList : List<Real>
    {
        public void Resize(int size)
        {
            Real[] data = ToArray();
            Real[] newData = new Real[size];
            Array.Copy(data, 0, newData, 0, size);
            Clear();
            AddRange(newData);
        }
    }
}