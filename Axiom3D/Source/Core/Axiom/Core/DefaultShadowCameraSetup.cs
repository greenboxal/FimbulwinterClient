#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: DefaultShadowCameraSetup.cs 3226 2012-05-03 21:31:19Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Graphics;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Implements default shadow camera setup
    /// </summary>
    /// <remarks>
    ///   This implements the default shadow camera setup algorithm.  This is what might
    ///   be referred to as "normal" shadow mapping.
    /// </remarks>
    public class DefaultShadowCameraSetup : IShadowCameraSetup
    {
        /// <summary>
        ///   Gets a default implementation of a ShadowCamera.
        /// </summary>
        /// <see cref="IShadowCameraSetup.GetShadowCamera" />
        [OgreVersion(1, 7, 2)]
        public void GetShadowCamera(SceneManager sceneManager, Camera camera, Viewport viewport, Light light,
                                    Camera textureCamera, int iteration)
        {
            Vector3 pos, dir;
            Quaternion q;

            // reset custom view / projection matrix in case already set
            textureCamera.SetCustomViewMatrix(false);
            textureCamera.SetCustomProjectionMatrix(false);
            textureCamera.Near = light.DeriveShadowNearClipDistance(camera);
            textureCamera.Far = light.DeriveShadowFarClipDistance(camera);


            // get the shadow frustum's far distance
            Real shadowDist = light.ShadowFarDistance;
            if (shadowDist == 0.0f)
            {
                // need a shadow distance, make one up
                shadowDist = camera.Near*300;
            }
            Real shadowOffset = shadowDist*sceneManager.ShadowDirectionalLightTextureOffset;

            // Directional lights
            if (light.Type == LightType.Directional)
            {
                // set up the shadow texture
                // Set ortho projection
                textureCamera.ProjectionType = Projection.Orthographic;
                // set ortho window so that texture covers far dist
                textureCamera.SetOrthoWindow(shadowDist*2, shadowDist*2);

                // Calculate look at position
                // We want to look at a spot shadowOffset away from near plane
                // 0.5 is a litle too close for angles
                Vector3 target = camera.DerivedPosition + (camera.DerivedDirection*shadowOffset);

                // Calculate direction, which same as directional light direction
                dir = -light.DerivedDirection; // backwards since point down -z
                dir.Normalize();

                // Calculate position
                // We want to be in the -ve direction of the light direction
                // far enough to project for the dir light extrusion distance
                pos = target + dir*sceneManager.ShadowDirectionalLightExtrusionDistance;

                // Round local x/y position based on a world-space texel; this helps to reduce
                // jittering caused by the projection moving with the camera
                // Viewport is 2 * near clip distance across (90 degree fov)
                //~ Real worldTexelSize = (texCam->getNearClipDistance() * 20) / vp->getActualWidth();
                //~ pos.x -= fmod(pos.x, worldTexelSize);
                //~ pos.y -= fmod(pos.y, worldTexelSize);
                //~ pos.z -= fmod(pos.z, worldTexelSize);
                Real worldTexelSize = (shadowDist*2)/textureCamera.Viewport.ActualWidth;

                //get texCam orientation

                Vector3 up = Vector3.UnitY;
                // Check it's not coincident with dir
                if (Utility.Abs(up.Dot(dir)) >= 1.0f)
                {
                    // Use camera up
                    up = Vector3.UnitZ;
                }
                // cross twice to rederive, only direction is unaltered
                Vector3 left = dir.Cross(up);
                left.Normalize();
                up = dir.Cross(left);
                up.Normalize();
                // Derive quaternion from axes
                q = Quaternion.FromAxes(left, up, dir);

                //convert world space camera position into light space
                Vector3 lightSpacePos = q.Inverse()*pos;

                //snap to nearest texel
                lightSpacePos.x -= lightSpacePos.x%worldTexelSize; //fmod(lightSpacePos.x, worldTexelSize);
                lightSpacePos.y -= lightSpacePos.y%worldTexelSize; //fmod(lightSpacePos.y, worldTexelSize);

                //convert back to world space
                pos = q*lightSpacePos;
            }
                // Spotlight
            else if (light.Type == LightType.Spotlight)
            {
                // Set perspective projection
                textureCamera.ProjectionType = Projection.Perspective;
                // set FOV slightly larger than the spotlight range to ensure coverage
                Radian fovy = light.SpotlightOuterAngle*1.2;

                // limit angle
                if (fovy.InDegrees > 175)
                {
                    fovy = (Degree) (175);
                }
                textureCamera.FieldOfView = fovy;

                // Calculate position, which same as spotlight position
                pos = light.GetDerivedPosition();

                // Calculate direction, which same as spotlight direction
                dir = -light.DerivedDirection; // backwards since point down -z
                dir.Normalize();
            }
                // Point light
            else
            {
                // Set perspective projection
                textureCamera.ProjectionType = Projection.Perspective;
                // Use 120 degree FOV for point light to ensure coverage more area
                textureCamera.FieldOfView = 120.0f;

                // Calculate look at position
                // We want to look at a spot shadowOffset away from near plane
                // 0.5 is a litle too close for angles
                Vector3 target = camera.DerivedPosition + (camera.DerivedDirection*shadowOffset);

                // Calculate position, which same as point light position
                pos = light.GetDerivedPosition();

                dir = (pos - target); // backwards since point down -z
                dir.Normalize();
            }

            // Finally set position
            textureCamera.Position = pos;

            // Calculate orientation based on direction calculated above
            /*
            // Next section (camera oriented shadow map) abandoned
            // Always point in the same direction, if we don't do this then
            // we get 'shadow swimming' as camera rotates
            // As it is, we get swimming on moving but this is less noticeable

            // calculate up vector, we want it aligned with cam direction
            Vector3 up = cam->getDerivedDirection();
            // Check it's not coincident with dir
            if (up.dotProduct(dir) >= 1.0f)
            {
            // Use camera up
            up = cam->getUp();
            }
            */
            Vector3 up2 = Vector3.UnitY;

            // Check it's not coincident with dir
            if (Utility.Abs(up2.Dot(dir)) >= 1.0f)
            {
                // Use camera up
                up2 = Vector3.UnitZ;
            }

            // cross twice to rederive, only direction is unaltered
            Vector3 left2 = dir.Cross(up2);
            left2.Normalize();
            up2 = dir.Cross(left2);
            up2.Normalize();

            // Derive quaternion from axes
            q = Quaternion.FromAxes(left2, up2, dir);
            textureCamera.Orientation = q;
        }
    }
}