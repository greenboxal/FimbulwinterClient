#region SVN Version Information

// <file>
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Collections.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.ParticleSystems
{
    public class EmitterList : List<ParticleEmitter>
    {
    }

    public class AffectorList : List<ParticleAffector>
    {
    }

    public class ParticleList : List<Particle>
    {
    }
}