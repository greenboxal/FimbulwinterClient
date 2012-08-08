#region SVN Version Information

// <file>
//     <id value="$Id: Timer.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

#endregion Namespace Declarations

namespace Axiom.Core
{
    ///<summary>
    ///  Encapsulates the functionality of the platform's highest resolution timer available.
    ///</summary>
    ///<remarks>
    ///  based on an vb.net implementation by createdbyx as posted in SourceForge Tracker #: [1612705]
    ///</remarks>
    public class Timer : ITimer
    {
        #region Private Fields

        private readonly Stopwatch _timer = new Stopwatch();

        #endregion Private Fields

        #region Methods

        /// <summary>
        ///   Start this instance's timer.
        /// </summary>
        public void Start()
        {
            this._timer.Start();
        }

        #endregion Methods

        #region Public Properties

        /// <summary>
        ///   Gets a <see cref="System.Int64" /> representing the 
        ///   current tick count of the timer.
        /// </summary>
        public long Count
        {
            get { return this._timer.ElapsedTicks; }
        }

        /// <summary>
        ///   Gets a <see cref="System.Int64" /> representing the 
        ///   frequency of the counter in ticks-per-second.
        /// </summary>
        public long Frequency
        {
            get { return Stopwatch.Frequency; }
        }

        /// <summary>
        ///   Gets a <see cref="System.Boolean" /> representing whether the 
        ///   timer has been started and is currently running.
        /// </summary>
        public bool IsRunning
        {
            get { return this._timer.IsRunning; }
        }

        /// <summary>
        ///   Gets a <see cref="System.Double" /> representing the 
        ///   resolution of the timer in seconds.
        /// </summary>
        public float Resolution
        {
            get { return ((float) 1.0/Frequency); }
        }

        /// <summary>
        ///   Gets a <see cref="System.Int64" /> representing the 
        ///   tick count at the start of the timer's run.
        /// </summary>
        public long StartCount
        {
            get { return 0; }
        }

        #endregion Public Properties

        #region ITimer Members

        ///<summary>
        ///  Reset this instance's timer.
        ///</summary>
        public void Reset()
        {
            // reset by restarting the timer
            this._timer.Reset();
            this._timer.Start();
        }

        public long Microseconds
        {
            get { return this._timer.ElapsedMilliseconds/10; }
        }

        public long Milliseconds
        {
            get { return this._timer.ElapsedMilliseconds; }
        }

        #endregion
    }
}