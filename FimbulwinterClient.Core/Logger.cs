using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Core
{
    public class Logger
    {
        public static int TabLevel { get; set; }

        private static List<string> _lines;
        public static List<string> Lines
        {
            get { return _lines; }
        }

        static Logger()
        {
            _lines = new List<string>();
        }

        public static void WriteLine(string format, params object[] args)
        {
            for (int i = 0; i < TabLevel; i++)
                format = "    " + format;

            lock (_lines)
            {
                _lines.Add(string.Format(format, args));

                if (_lines.Count > 40)
                    _lines.RemoveAt(0);
            }
        }
    }
}
