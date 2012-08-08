#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: CompositionTargetPass.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Axiom.Core;
using Axiom.Configuration;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    ///<summary>
    ///  Object representing one pass or operation in a composition sequence. This provides a 
    ///  method to conviently interleave RenderSystem commands between Render Queues.
    ///</summary>
    public class CompositionTargetPass : DisposableObject
    {
        #region Fields and Properties

        ///<summary>
        ///  Parent technique
        ///</summary>
        protected CompositionTechnique parent;

        #region Parent Property

        /// <summary>
        ///   Gets Parent CompositionTechnique
        /// </summary>
        public CompositionTechnique Parent
        {
            get { return this.parent; }
        }

        #endregion

        #region InputMode Property

        ///<summary>
        ///  Input mode
        ///</summary>
        protected CompositorInputMode inputMode;

        ///<summary>
        ///  Input mode
        ///</summary>
        public CompositorInputMode InputMode
        {
            get { return this.inputMode; }
            set { this.inputMode = value; }
        }

        #endregion InputMode Property

        #region OutputName Property

        ///<summary>
        ///  (local) output texture
        ///</summary>
        protected string outputName;

        ///<summary>
        ///  (local) output texture
        ///</summary>
        public string OutputName
        {
            get { return this.outputName; }
            set { this.outputName = value; }
        }

        #endregion OutputName Property

        #region Passes Property

        ///<summary>
        ///  Passes
        ///</summary>
        protected List<CompositionPass> passes;

        ///<summary>
        ///  Passes
        ///</summary>
        public IList<CompositionPass> Passes
        {
            get { return this.passes; }
        }

        #endregion Passes Property

        #region OnlyInitial Property

        ///<summary>
        ///  This target pass is only executed initially after the effect
        ///  has been enabled.
        ///</summary>
        protected bool onlyInitial;

        ///<summary>
        ///  This target pass is only executed initially after the effect
        ///  has been enabled.
        ///</summary>
        public bool OnlyInitial
        {
            get { return this.onlyInitial; }
            set { this.onlyInitial = value; }
        }

        #endregion OnlyInitial Property

        #region VisibilityMask Property

        ///<summary>
        ///  Visibility mask for this render
        ///</summary>
        protected ulong visibilityMask;

        ///<summary>
        ///  Visibility mask for this render
        ///</summary>
        public ulong VisibilityMask
        {
            get { return this.visibilityMask; }
            set { this.visibilityMask = value; }
        }

        #endregion VisibilityMask Property

        #region LodBias Property

        ///<summary>
        ///  LOD bias of this render
        ///</summary>
        protected float lodBias;

        ///<summary>
        ///  LOD bias of this render
        ///</summary>
        public float LodBias
        {
            get { return this.lodBias; }
            set { this.lodBias = value; }
        }

        #endregion LodBias Property

        #region MaterialScheme Property

        ///<summary>
        ///  Material scheme name
        ///</summary>
        protected string materialScheme;

        ///<summary>
        ///  Material scheme name
        ///</summary>
        public string MaterialScheme
        {
            get { return this.materialScheme; }
            set { this.materialScheme = value; }
        }

        #endregion MaterialScheme Property

        #region ShadowsEnabled Property

        /// <summary>
        ///   Get's or Set's  whether shadows are enabled in this target pass.
        /// </summary>
        public bool ShadowsEnabled { get; set; }

        #endregion ShadowsEnabled Property

        ///<summary>
        ///  Determine if this target pass is supported on the current rendering device.
        ///</summary>
        public bool IsSupported
        {
            get
            {
                // A target pass is supported if all passes are supported
                foreach (CompositionPass pass in this.passes)
                {
                    if (!pass.IsSupported)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        #endregion Fields and Properties

        #region Constructors

        public CompositionTargetPass(CompositionTechnique parent)
        {
            this.parent = parent;
            this.inputMode = CompositorInputMode.None;
            this.passes = new List<CompositionPass>();
            this.onlyInitial = false;
            this.visibilityMask = 0xFFFFFFFF;
            this.lodBias = 1.0f;
            this.materialScheme = MaterialManager.DefaultSchemeName;
            ShadowsEnabled = true;

            if (Root.Instance.RenderSystem != null)
            {
                this.materialScheme = Root.Instance.RenderSystem.DefaultViewportMaterialScheme;
            }
        }

        #endregion Constructors

        #region Methods

        ///<summary>
        ///  Create a new pass, and return a pointer to it.
        ///</summary>
        public CompositionPass CreatePass()
        {
            CompositionPass t = new CompositionPass(this);
            this.passes.Add(t);
            return t;
        }

        public void RemovePass(int index)
        {
            Debug.Assert(index < this.passes.Count, "Index out of bounds.");
            this.passes[index].Dispose();
            this.passes[index] = null;
        }

        public void RemoveAllPasses()
        {
            for (int i = 0; i < this.passes.Count; i++)
            {
                this.passes[i].Dispose();
                this.passes[i] = null;
            }
        }

        #endregion Methods

        #region DisposableObject

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    return;
                    RemoveAllPasses();
                }
            }
            base.dispose(disposeManagedResources);
        }

        #endregion
    }
}