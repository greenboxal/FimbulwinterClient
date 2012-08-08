using System;

namespace Axiom.Framework.Exceptions
{
    /// <summary>
    ///   Interface for a display of error
    /// </summary>
    public interface IErrorDialog
    {
        /// <summary>
        ///   Causes the exception to be displayed on the screen
        /// </summary>
        /// <param name="exception"> The exception to display </param>
        void Show(Exception exception);
    }
}