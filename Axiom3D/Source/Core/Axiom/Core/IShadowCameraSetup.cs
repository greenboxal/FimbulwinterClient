#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: IShadowCameraSetup.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Core
{
    ///<summary>
    ///  This class allows you to plug in new ways to define the camera setup when
    ///  rendering and projecting shadow textures.
    ///</summary>
    ///<remarks>
    ///  The default projection used when rendering shadow textures is a uniform
    ///  frustum. This is pretty straight forward but doesn't make the best use of
    ///  the space in the shadow map since texels closer to the camera will be larger,
    ///  resulting in 'jaggies'. There are several ways to distribute the texels
    ///  in the shadow texture differently, and this class allows you to override
    ///  that.
    ///  <para />
    ///  Axiom is provided with several alternative shadow camera setups, including
    ///  LiSPSM (<see name="LiSPSMShadowCameraSetup" />) and Plane Optimal
    ///  (<see name="PlaneOptimalShadowCameraSetup" />).
    ///  Others can of course be written to incorporate other algorithms. All you
    ///  have to do is instantiate one of these classes and enable it using
    ///  <see cref="SceneManager.ShadowCameraSetup" /> (global) or
    ///  <see cref="Light.CustomShadowCameraSetup" />
    ///  (per light). In both cases the instance is be deleted automatically when
    ///  no more references to it exist.
    ///  <para />
    ///  Shadow map matrices, being projective matrices, have 15 degrees of freedom.
    ///  3 of these degrees of freedom are fixed by the light's position.  4 are used to
    ///  affinely affect z values.  6 affinely affect u,v sampling.  2 are projective
    ///  degrees of freedom.  This class is meant to allow custom methods for
    ///  handling optimization.
    ///</remarks>
    public interface IShadowCameraSetup
    {
        /// <summary>
        ///   Gets a specific implementation of a ShadowCamera setup.
        /// </summary>
        /// <param name="sceneManager"> </param>
        /// <param name="camera"> </param>
        /// <param name="viewport"> </param>
        /// <param name="light"> </param>
        /// <param name="textureCamera"> </param>
        /// <param name="iteration"> </param>
        void GetShadowCamera(SceneManager sceneManager, Camera camera, Viewport viewport, Light light,
                             Camera textureCamera,
                             int iteration);
    };
}