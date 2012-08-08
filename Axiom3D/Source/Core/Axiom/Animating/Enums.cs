#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Enums.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion

namespace Axiom.Animating
{
    ///<summary>
    ///  Types of vertex animations
    ///</summary>
    public enum VertexAnimationType
    {
        /// No animation
        None,

        /// Morph animation is made up of many interpolated snapshot keyframes
        Morph,

        /// Pose animation is made up of a single delta pose keyframe
        Pose
    }

    ///<summary>
    ///  Identify which vertex data we should be sending to the renderer
    ///</summary>
    public enum VertexDataBindChoice
    {
        Original,
        SoftwareSkeletal,
        SoftwareMorph,
        HardwareMorph
    }

    ///<summary>
    ///  Do we do vertex animation in hardware or software?
    ///</summary>
    public enum VertexAnimationTargetMode
    {
        /// In software
        Software,

        /// In hardware
        Hardware
    }

    ///<summary>
    ///  Used to specify how animations are applied to a skeleton.
    ///</summary>
    public enum SkeletalAnimBlendMode
    {
        ///<summary>
        ///  Animations are applied by calculating a weighted average of all animations.
        ///</summary>
        Average,

        ///<summary>
        ///  Animations are applied by calculating a weighted cumulative total.
        ///</summary>
        Cumulative
    }
}