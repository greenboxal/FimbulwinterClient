#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Particle.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.ParticleSystems
{
    public enum ParticleType
    {
        Emitter,
        Visual
    }

    ///<summary>
    ///  Class representing a single particle instance.
    ///</summary>
    public class Particle
    {
        #region Member variables

        /// Does this particle have it's own dimensions?
        public bool hasOwnDimensions;

        /// Personal width if mOwnDimensions == true
        public float width;

        /// Personal height if mOwnDimensions == true
        public float height;

        /// Current rotation value
        public float rotationInRadians;

        // Note the intentional public access to internal variables
        // Accessing via get/set would be too costly for 000's of particles
        /// World position
        public Vector3 Position = Vector3.Zero;

        /// Direction (and speed)
        public Vector3 Direction = Vector3.Zero;

        /// Current colour
        public ColorEx Color = ColorEx.White;

        /// <summary>
        ///   Time (in seconds) before this particle is destroyed.
        /// </summary>
        public float timeToLive;

        /// <summary>
        ///   Total Time to live, number of seconds of particles natural life
        /// </summary>
        public float totalTimeToLive;

        /// Parent ParticleSystem
        protected ParticleSystem parentSystem;

        /// Additional visual data you might want to associate with the Particle
        protected ParticleVisualData visual;

        protected ParticleType particleType = ParticleType.Visual;

        #endregion

        #region Properties

        public float RotationSpeed { get; set; }

        public ParticleType ParticleType
        {
            get { return this.particleType; }
            set { this.particleType = value; }
        }

        #endregion

        ///<summary>
        ///  Default constructor.
        ///</summary>
        public Particle()
        {
            this.timeToLive = 10;
            this.totalTimeToLive = 10;
            RotationSpeed = 0;
        }


        public void NotifyVisualData(ParticleVisualData vdata)
        {
            this.visual = vdata;
        }

        public void NotifyOwner(ParticleSystem owner)
        {
            this.parentSystem = owner;
        }

        public void SetDimensions(float width, float height)
        {
            this.hasOwnDimensions = true;
            this.width = width;
            this.height = height;
            this.parentSystem.NotifyParticleResized();
        }

        public void ResetDimensions()
        {
            this.hasOwnDimensions = false;
        }

        public bool HasOwnDimensions
        {
            get { return this.hasOwnDimensions; }
            set { this.hasOwnDimensions = value; }
        }

        public float Rotation
        {
            get { return this.rotationInRadians*Utility.DEGREES_PER_RADIAN; }
            set
            {
                this.rotationInRadians = value*Utility.RADIANS_PER_DEGREE;
                if (this.rotationInRadians != 0)
                {
                    this.parentSystem.NotifyParticleRotated();
                }
            }
        }

        public float Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        public float Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        public ParticleVisualData VisualData
        {
            get { return this.visual; }
        }
    }
}