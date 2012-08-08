#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AttachmentPoint.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Math;
using Axiom.Core;
using Axiom.Collections;

#endregion Namespace Declarations

#region Ogre Synchronization Information

// <ogresynchronization>
//     <file name="TagPoint.h"   revision="1.10.2.2" lastUpdated="10/15/2005" lastUpdatedBy="DanielH" />
//     <file name="TagPoint.cpp" revision="1.12" lastUpdated="10/15/2005" lastUpdatedBy="DanielH" />
// </ogresynchronization>

#endregion

namespace Axiom.Animating
{
    public class AttachmentPoint
    {
        private readonly string name;
        private readonly string parentBone;
        private readonly Quaternion orientation;
        private readonly Vector3 position;

        public AttachmentPoint(string name, string parentBone, Quaternion orientation, Vector3 position)
        {
            this.name = name;
            this.parentBone = parentBone;
            this.orientation = orientation;
            this.position = position;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string ParentBone
        {
            get { return this.parentBone; }
        }

        public Quaternion Orientation
        {
            get { return this.orientation; }
        }

        public Vector3 Position
        {
            get { return this.position; }
        }
    }
}