using System;

namespace FimbulwinterClient.Core.IO.GRF
{
    public class GRFEventArg : EventArgs
    {
        private readonly GrfFile _file;

        public GrfFile File
        {
            get { return _file; }
        }

        public GRFEventArg(GrfFile file)
        {
            _file = file;
        }
    }
}