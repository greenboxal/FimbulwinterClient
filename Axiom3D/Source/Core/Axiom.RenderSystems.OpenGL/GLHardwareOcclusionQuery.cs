#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GLHardwareOcclusionQuery.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;
using Tao.OpenGl;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Summary description for GLHardwareOcclusionQuery.
    /// </summary>
    public class GLHardwareOcclusionQuery : HardwareOcclusionQuery
    {
        private const string GL_ARB_occlusion_query = "GL_ARB_occlusion_query";
        private const string GL_NV_occlusion_query = "GL_NV_occlusion_query";
        private const string GL_Version_1_5 = "1.5";

        private readonly BaseGLSupport _glSupport;

        ///<summary>
        ///  Number of fragments returned from the last query.
        ///</summary>
        private int lastFragmentCount;

        ///<summary>
        ///  Id of the GL query.
        ///</summary>
        private int queryId;

        private readonly bool isSupportedARB;
        private readonly bool isSupportedNV;

        internal GLHardwareOcclusionQuery(BaseGLSupport glSupport)
        {
            this._glSupport = glSupport;
            this.isSupportedARB = this._glSupport.CheckMinVersion(GL_Version_1_5) ||
                                  this._glSupport.CheckExtension(GL_ARB_occlusion_query);
            this.isSupportedNV = this._glSupport.CheckExtension(GL_NV_occlusion_query);

            if (this.isSupportedNV)
            {
                Gl.glGenOcclusionQueriesNV(1, out this.queryId);
            }
            else if (this.isSupportedARB)
            {
                Gl.glGenQueriesARB(1, out this.queryId);
            }
        }

        #region HardwareOcclusionQuery Members

        public override void Begin()
        {
            if (this.isSupportedNV)
            {
                Gl.glBeginOcclusionQueryNV(this.queryId);
            }
            else if (this.isSupportedARB)
            {
                Gl.glBeginQueryARB(Gl.GL_SAMPLES_PASSED_ARB, this.queryId);
            }
        }

        public override void End()
        {
            if (this.isSupportedNV)
            {
                Gl.glEndOcclusionQueryNV();
            }
            else if (this.isSupportedARB)
            {
                Gl.glEndQueryARB(Gl.GL_SAMPLES_PASSED_ARB);
            }
        }

        public override bool PullResults(out int NumOfFragments)
        {
            // note: flush doesn't apply to GL

            // default to returning a high count.  will be set otherwise if the query runs
            NumOfFragments = 100000;

            if (this.isSupportedNV)
            {
                Gl.glGetOcclusionQueryivNV(this.queryId, Gl.GL_PIXEL_COUNT_NV, out NumOfFragments);
                return true;
            }
            else if (this.isSupportedARB)
            {
                Gl.glGetQueryObjectivARB(this.queryId, Gl.GL_QUERY_RESULT_ARB, out NumOfFragments);
                return true;
            }

            return false;
        }

        public override bool IsStillOutstanding()
        {
            int available = 0;

            if (this.isSupportedNV)
            {
                Gl.glGetOcclusionQueryivNV(this.queryId, Gl.GL_PIXEL_COUNT_AVAILABLE_NV, out available);
            }
            else if (this.isSupportedARB)
            {
                Gl.glGetQueryivARB(this.queryId, Gl.GL_QUERY_RESULT_AVAILABLE_ARB, out available);
            }

            return available == 0;
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (this.isSupportedNV)
            {
                Gl.glDeleteOcclusionQueriesNV(1, ref this.queryId);
            }
            else if (this.isSupportedARB)
            {
                Gl.glDeleteQueriesARB(1, ref this.queryId);
            }
            base.dispose(disposeManagedResources);
        }

        #endregion HardwareOcclusionQuery Members
    }
}