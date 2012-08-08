#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ParticleAffectorFactory.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.ParticleSystems
{
    ///<summary>
    ///  Abstract class defining the interface to be implemented by creators of ParticleAffector subclasses.
    ///</summary>
    ///<remarks>
    ///  Plugins or 3rd party applications can add new types of particle affectors  by creating
    ///  subclasses of the ParticleAffector class. Because multiple instances of these affectors may be
    ///  required, a factory class to manage the instances is also required. 
    ///  <p />
    ///  ParticleAffectorFactory subclasses must allow the creation and destruction of ParticleAffector
    ///  subclasses. They must also be registered with the ParticleSystemManager. All factories have
    ///  a name which identifies them, examples might be 'ForceVector', 'Attractor', or 'Fader', and these can be 
    ///  also be used from particle system scripts.
    ///</remarks>
    public abstract class ParticleAffectorFactory
    {
        #region Member variables

        protected AffectorList affectorList = new AffectorList();

        #endregion

        #region Constructors

        #endregion

        #region Properties

        ///<summary>
        ///  Returns the name of the factory, which identifies the affector type this factory creates.
        ///</summary>
        public abstract string Name { get; }

        ///<summary>
        ///  Creates a new affector instance.
        ///</summary>
        ///<remarks>
        ///  Subclasses MUST add a reference to the affectorList.
        ///</remarks>
        [OgreVersion(1, 7, 2)]
        public abstract ParticleAffector CreateAffector(ParticleSystem psys);

        ///<summary>
        ///  Destroys the affector referenced by the parameter.
        ///</summary>
        ///<param name="e"> The Affector to destroy. </param>
        public virtual void Destroy(ParticleAffector e)
        {
            // remove the affector from the list
            this.affectorList.Remove(e);
        }

        #endregion
    }
}