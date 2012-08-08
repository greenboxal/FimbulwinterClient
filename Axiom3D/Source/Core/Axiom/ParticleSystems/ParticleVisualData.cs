#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ParticleVisualData.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.ParticleSystems
{
    /// <summary>
    ///   Abstract class containing any additional data required to be associated
    ///   with a particle to perform the required rendering.
    /// </summary>
    /// <remarks>
    ///   Because you can specialise the way that particles are renderered by supplying
    ///   custom ParticleSystemRenderer classes, you might well need some additional 
    ///   data for your custom rendering routine which is not held on the default particle
    ///   class. If that's the case, then you should define a subclass of this class, 
    ///   and construct it when asked in your custom ParticleSystemRenderer class.
    /// </remarks>
    public abstract class ParticleVisualData
    {
    };
}