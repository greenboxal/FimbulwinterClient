#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ObjectTextDisplay.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Math;
using Axiom.Overlays;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Attaches a label to a <see cref="MovableObject" />
    /// </summary>
    public class ObjectTextDisplay
    {
        #region Fields and Properties

        protected MovableObject parent;
        protected Camera camera;
        protected Overlay parentOverlay;
        protected OverlayElement parentText;
        protected OverlayElementContainer parentContainer;

        protected bool enabled;

        public bool IsEnabled
        {
            get { return this.enabled; }
            set
            {
                this.enabled = value;
                if (value)
                {
                    this.parentOverlay.Show();
                }
                else
                {
                    this.parentOverlay.Hide();
                }
            }
        }

        protected string text;

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                this.parentText.Text = this.text;
            }
        }

        #endregion Fields and Properties

        #region Construction and Destruction

        public ObjectTextDisplay(MovableObject p, Camera c, string shapeName)
        {
            this.parent = p;
            this.camera = c;
            this.enabled = false;
            this.text = "";

            // create an overlay that we can use for later

            // = Ogre.OverlayManager.getSingleton().create("shapeName");
            this.parentOverlay = OverlayManager.Instance.Create("shapeName");

            // (Ogre.OverlayContainer)(Ogre.OverlayManager.getSingleton().createOverlayElement("Panel", "container1"));
            this.parentContainer =
                (OverlayElementContainer) (OverlayElementManager.Instance.CreateElement("Panel", "container1", false));

            //parentOverlay.add2D(parentContainer);
            this.parentOverlay.AddElement(this.parentContainer);

            //parentText = Ogre.OverlayManager.getSingleton().createOverlayElement("TextArea", "shapeNameText");
            this.parentText = OverlayElementManager.Instance.CreateElement("TextArea", shapeName, false);

            this.parentText.SetDimensions(1.0f, 1.0f);

            //parentText.setMetricsMode(Ogre.GMM_PIXELS);
            this.parentText.MetricsMode = MetricsMode.Pixels;


            this.parentText.SetPosition(1.0f, 1.0f);


            this.parentText.SetParam("font_name", "Arial");
            this.parentText.SetParam("char_height", "25");
            this.parentText.SetParam("horz_align", "center");
            this.parentText.Color = new ColorEx(1.0f, 1.0f, 1.0f);
            //parentText.setColour(Ogre.ColourValue(1.0, 1.0, 1.0));


            this.parentContainer.AddChild(this.parentText);

            this.parentOverlay.Show();
        }

        #endregion Construction and Destruction

        public void Update()
        {
            if (!this.enabled)
            {
                return;
            }

            // get the projection of the object's AABB into screen space
            AxisAlignedBox bbox = this.parent.GetWorldBoundingBox(true);
            //new AxisAlignedBox(parent.BoundingBox.Minimum, parent.BoundingBox.Maximum);// GetWorldBoundingBox(true));


            //Ogre.Matrix4 mat = camera.getViewMatrix();
            Matrix4 mat = this.camera.ViewMatrix;
            //const Ogre.Vector3 corners = bbox.getAllCorners();
            Vector3[] corners = bbox.Corners;


            float min_x = 1.0f;
            float max_x = 0.0f;
            float min_y = 1.0f;
            float max_y = 0.0f;

            // expand the screen-space bounding-box so that it completely encloses
            // the object's AABB
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = corners[i];

                // multiply the AABB corner vertex by the view matrix to
                // get a camera-space vertex
                //corner = multiply(mat,corner);
                corner = mat*corner;

                // make 2D relative/normalized coords from the view-space vertex
                // by dividing out the Z (depth) factor -- this is an approximation
                float x = corner.x/corner.z + 0.5f;
                float y = corner.y/corner.z + 0.5f;

                if (x < min_x)
                {
                    min_x = x;
                }

                if (x > max_x)
                {
                    max_x = x;
                }

                if (y < min_y)
                {
                    min_y = y;
                }

                if (y > max_y)
                {
                    max_y = y;
                }
            }

            // we now have relative screen-space coords for the object's bounding box; here
            // we need to center the text above the BB on the top edge. The line that defines
            // this top edge is (min_x, min_y) to (max_x, min_y)

            //parentContainer->setPosition(min_x, min_y);
            this.parentContainer.SetPosition(1 - max_x, min_y); // Edited by alberts: This code works for me
            this.parentContainer.SetDimensions(max_x - min_x, 0.1f); // 0.1, just "because"
        }
    }
}