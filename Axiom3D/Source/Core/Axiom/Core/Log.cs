#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Log.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.IO;

#if SILVERLIGHT
using System.IO.IsolatedStorage;
#endif

#endregion Namespace Declarations

namespace Axiom.Core
{

    #region LogListenerEventArgs Class

    /// <summary>
    /// </summary>
    public class LogListenerEventArgs : EventArgs
    {
        /// <summary>
        ///   The message to be logged
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        ///   The message level the log is using
        /// </summary>
        public LogMessageLevel Level { get; private set; }

        /// <summary>
        ///   If we are printing to the console or not
        /// </summary>
        public bool MaskDebug { get; private set; }

        /// <summary>
        ///   the name of this log (so you can have several listeners for different logs, and identify them)
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        ///   This is called whenever the log recieves a message and is about to write it out
        /// </summary>
        /// <param name="message"> The message to be logged </param>
        /// <param name="lml"> The message level the log is using </param>
        /// <param name="maskDebug"> If we are printing to the console or not </param>
        /// <param name="logName"> the name of this log (so you can have several listeners for different logs, and identify them) </param>
        public LogListenerEventArgs(string message, LogMessageLevel lml, bool maskDebug, string logName)
        {
            Message = message;
            Level = lml;
            MaskDebug = maskDebug;
            LogName = logName;
        }
    }

    #endregion LogListenerEventArgs Class

    #region Log Class

    /// <summary>
    ///   Log class for writing debug/log data to files.
    /// </summary>
    public sealed class Log : DisposableObject
    {
        #region Fields

#if SILVERLIGHT
		private IsolatedStorageFile file;
#endif

        /// <summary>
        ///   File stream used for kepping the log file open.
        /// </summary>
        private readonly FileStream log;

        /// <summary>
        ///   Writer used for writing to the log file.
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        ///   Level of detail for this log.
        /// </summary>
        private LoggingLevel logLevel;

        /// <summary>
        ///   Debug output enabled?
        /// </summary>
        private readonly bool debugOutput;

        /// <summary>
        ///   LogMessageLevel + LoggingLevel > LOG_THRESHOLD = message logged.
        /// </summary>
        private const int LogThreshold = 4;

        private readonly string mLogName;

        #endregion Fields

        public event EventHandler<LogListenerEventArgs> MessageLogged;

        #region Constructors

        /// <summary>
        ///   Constructor.  Creates a log file that also logs debug output.
        /// </summary>
        /// <param name="fileName"> Name of the log file to open. </param>
        public Log(string fileName)
            : this(fileName, true)
        {
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="fileName"> Name of the log file to open. </param>
        /// <param name="debugOutput"> Write log messages to the debug output? </param>
        public Log(string fileName, bool debugOutput)
        {
            this.mLogName = fileName;
            MessageLogged = null;

            this.debugOutput = debugOutput;
            this.logLevel = LoggingLevel.Normal;

            if (fileName != null)
            {
                try
                {
#if !( ANDROID )

                    // create the log file, or open
#if SILVERLIGHT
					file = IsolatedStorageFile.GetUserStoreForApplication();
					log = file.OpenFile(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
#else
                    this.log = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
#endif

                    // get a stream writer using the file stream
                    this.writer = new StreamWriter(this.log);
                    this.writer.AutoFlush = true; //always flush after write
#endif
                }
                catch
                {
                }
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   Gets/Sets the level of the detail for this log.
        /// </summary>
        /// <value> </value>
        public LoggingLevel LogDetail
        {
            get { return this.logLevel; }
            set { this.logLevel = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Write a message to the log.
        /// </summary>
        /// <remarks>
        ///   Message is written with a LogMessageLevel of Normal, and debug output is not written.
        /// </remarks>
        /// <param name="message"> Message to write, which can include string formatting tokens. </param>
        /// <param name="substitutions"> When message includes string formatting tokens, these are the values to inject into the formatted string. </param>
        public void Write(string message, params object[] substitutions)
        {
            Write(LogMessageLevel.Normal, false, message, substitutions);
        }

        /// <summary>
        ///   Write a message to the log.
        /// </summary>
        /// <remarks>
        ///   Message is written with a LogMessageLevel of Normal, and debug output is not written.
        /// </remarks>
        /// <param name="maskDebug"> If true, debug output will not be written. </param>
        /// <param name="message"> Message to write, which can include string formatting tokens. </param>
        /// <param name="substitutions"> When message includes string formatting tokens, these are the values to inject into the formatted string. </param>
        public void Write(bool maskDebug, string message, params object[] substitutions)
        {
            Write(LogMessageLevel.Normal, maskDebug, message, substitutions);
        }

        /// <summary>
        ///   Write a message to the log.
        /// </summary>
        /// <param name="level"> Importance of this logged message. </param>
        /// <param name="maskDebug"> If true, debug output will not be written. </param>
        /// <param name="message"> Message to write, which can include string formatting tokens. </param>
        /// <param name="substitutions"> When message includes string formatting tokens, these are the values to inject into the formatted string. </param>
        public void Write(LogMessageLevel level, bool maskDebug, string message, params object[] substitutions)
        {
            if (IsDisposed)
            {
                return;
            }

            if (message == null)
            {
                throw new ArgumentNullException("The log message cannot be null");
            }
            if (((int) this.logLevel + (int) level) > LogThreshold)
            {
                return; //too verbose a message to write
            }

            // construct the log message
            if (substitutions != null && substitutions.Length > 0)
            {
                message = string.Format(message, substitutions);
            }

            // write the the debug output if requested
            if (this.debugOutput && !maskDebug)
            {
#if MONO
				if(System.Diagnostics.Debugger.IsAttached)
					System.Console.WriteLine( message );
				else
#endif
                System.Diagnostics.Debug.WriteLine(message);
            }

            if (this.writer != null && this.writer.BaseStream != null)
            {
                // prepend the current time to the message
                message = string.Format("[{0}] {1}", DateTime.Now.ToString("hh:mm:ss"), message);

                // write the message and flush the buffer
                lock (this.writer)
                    this.writer.WriteLine(message);
                //writer auto-flushes
            }

            FireMessageLogged(level, maskDebug, message);
        }

        private void FireMessageLogged(LogMessageLevel level, bool maskDebug, string message)
        {
            // Now fire the MessageLogged event
            if (MessageLogged != null)
            {
                LogListenerEventArgs args = new LogListenerEventArgs(message, level, maskDebug, this.mLogName);
                MessageLogged(this, args);
            }
        }

        #endregion Methods

        #region DisposableObject Members

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // Dispose managed resources.
                    try
                    {
                        if (this.writer != null)
                        {
                            this.writer.Close();
                        }

                        if (this.log != null)
                        {
                            this.log.Close();
                        }

#if SILVERLIGHT
				if (file != null)
					file.Dispose();
#endif
                    }
                    catch
                    {
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            base.dispose(disposeManagedResources);
        }

        #endregion DisposableObject Members
    }

    #endregion Log Class
}