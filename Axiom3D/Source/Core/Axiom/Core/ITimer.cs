#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ITimer.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Core
{
    ///<summary>
    ///  Describes the interface for a platform independent timer.
    ///</summary>
    public interface ITimer
    {
        #region Methods

        ///<summary>
        ///  Resets this timer.
        ///</summary>
        ///<remarks>
        ///  This must be called first before using the timer.
        ///</remarks>
        void Reset();

        #endregion Methods

        #region Properties

        ///<summary>
        ///  Returns microseconds since initialization or last reset.
        ///</summary>
        long Microseconds { get; }

        ///<summary>
        ///  Returns milliseconds since initialization or last reset.
        ///</summary>
        long Milliseconds { get; }

        #endregion Properties
    }
}