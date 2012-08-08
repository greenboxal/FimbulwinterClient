#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ParticleEmitterFactory.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using Axiom.Collections;
using Axiom.ParticleSystems.Collections;

#endregion Namespace Declarations

namespace Axiom.ParticleSystems
{
    ///<summary>
    ///  Abstract class defining the interface to be implemented by creators of ParticleEmitter subclasses.
    ///</summary>
    ///<remarks>
    ///  Plugins or 3rd party applications can add new types of particle emitters by creating
    ///  subclasses of the ParticleEmitter class. Because multiple instances of these emitters may be
    ///  required, a factory class to manage the instances is also required. 
    ///  <p />
    ///  ParticleEmitterFactory subclasses must allow the creation and destruction of ParticleEmitter
    ///  subclasses. They must also be registered with the ParticleSystemManager. All factories have
    ///  a name which identifies them, examples might be 'Point', 'Cone', or 'Box', and these can be 
    ///  also be used from XML particle system scripts.
    ///</remarks>
    public abstract class ParticleEmitterFactory
    {
        #region Member variables

        protected EmitterList emitterList = new EmitterList();

        #endregion

        #region Constructors

        #endregion

        #region Properties

        ///<summary>
        ///  Returns the name of the factory, which identifies which type of emitter this factory creates.
        ///</summary>
        public abstract string Name { get; }

        #endregion

        #region Methods

        ///<summary>
        ///  Creates a new instance of an emitter.
        ///</summary>
        ///<remarks>
        ///  Subclasses must add newly created emitters to the emitterList.
        ///</remarks>
        ///<returns> </returns>
        public abstract ParticleEmitter Create(ParticleSystem ps);

        ///<summary>
        ///  Destroys the emitter referenced by the parameter.
        ///</summary>
        ///<param name="emitter"> </param>
        public virtual void Destroy(ParticleEmitter emitter)
        {
            // remove the emitter from the list
            this.emitterList.Remove(emitter);
        }

        #endregion
    }
}