#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: NullProgram.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    public class NullProgram : HighLevelGpuProgram
    {
        internal NullProgram(ResourceManager creator, string name, ResourceHandle handle, string group)
            : this(creator, name, handle, group, false, null)
        {
        }

        [OgreVersion(1, 7, 2790)]
        internal NullProgram(ResourceManager creator, string name, ResourceHandle handle, string group, bool isManual,
                             IManualResourceLoader loader)
            : base(creator, name, handle, group, isManual, loader)
        {
        }

        [OgreVersion(1, 7, 2790)]
        protected override void LoadFromSource()
        {
        }

        [OgreVersion(1, 7, 2790)]
        protected override void CreateLowLevelImpl()
        {
        }

        [OgreVersion(1, 7, 2790, "might be unload()?")]
        protected override void UnloadHighLevelImpl()
        {
        }

        [OgreVersion(1, 7, 2790)]
        protected override void PopulateParameterNames(GpuProgramParameters parms)
        {
            // Skip the normal implementation
            // Ensure we don't complain about missing parameter names
            parms.IgnoreMissingParameters = true;
        }

        [OgreVersion(1, 7, 2790)]
        protected override void BuildConstantDefinitions()
        {
        }

        /// <summary>
        ///   Overridden from GpuProgram - never supported
        /// </summary>
        [OgreVersion(1, 7, 2790)]
        public override bool IsSupported
        {
            get { return false; }
        }
    };

    public class NullProgramFactory : HighLevelGpuProgramFactory
    {
        /// <summary>
        ///   Get the name of the language this factory creates programs for
        /// </summary>
        public override string Language
        {
            get { return HighLevelGpuProgramManager.NullLang; }
        }

        public override HighLevelGpuProgram CreateInstance(ResourceManager creator, string name, ulong handle,
                                                           string group,
                                                           bool isManual, IManualResourceLoader loader)
        {
            return new NullProgram(creator, name, handle, group, isManual, loader);
        }
    };
}