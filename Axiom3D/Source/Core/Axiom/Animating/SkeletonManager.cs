#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: SkeletonManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Collections;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.Animating
{
    /// <summary>
    ///   Summary description for SkeletonManager.
    /// </summary>
    public sealed class SkeletonManager : ResourceManager, ISingleton<SkeletonManager>
    {
        #region ISingleton<SkeletonManager> Implementation

        /// <summary>
        ///   Gets the singleton instance of this class.
        /// </summary>
        public static SkeletonManager Instance
        {
            get { return Singleton<SkeletonManager>.Instance; }
        }

        /// <summary>
        ///   Initializes the Skeleton Manager
        /// </summary>
        /// <param name="args"> </param>
        /// <returns> </returns>
        public bool Initialize(params object[] args)
        {
            return true;
        }

        #endregion ISingleton<SkeletonManager> Implementation

        #region Construction and Destruction

        /// <summary>
        ///   Internal constructor.  This class cannot be instantiated externally.
        /// </summary>
        public SkeletonManager()
        {
            LoadingOrder = 300.0f;
            ResourceType = "Skeleton";

            ResourceGroupManager.Instance.RegisterResourceManager(ResourceType, this);
        }

        #endregion Construction and Destruction

        #region ResourceManager Implementation

        /// <summary>
        ///   Creates a new skeleton object.
        /// </summary>
        protected override Resource _create(string name, ResourceHandle handle, string group, bool isManual,
                                            IManualResourceLoader loader, NameValuePairList createParams)
        {
            return new Skeleton(this, name, handle, group, isManual, loader);
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    ResourceGroupManager.Instance.UnregisterResourceManager(ResourceType);
                    Singleton<SkeletonManager>.Destroy();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.dispose(disposeManagedResources);
        }

        #endregion ResourceManager Implementation
    }
}