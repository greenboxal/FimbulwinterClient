using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.IO;
using FimbulwinterClient.Core.IO.GRF;

namespace FimbulwinterClient.Core.IO
{
    public class GrfFileSystem : IFileSystem
    {
        public Grf Grf { get; private set; }

        public string Type
        {
            get { return "Grf"; }
        }

        public GrfFileSystem(string filename)
        {
            Grf = new Grf(filename);
        }

        public bool Load()
        {
            try
            {
                Grf.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Stream OpenStream(string name)
        {
            GrfFile file = (GrfFile)Grf.Files[name];

            return file != null ? new MemoryStream(Grf.GetDataFromFile(file)) : null;
        }

        public void Close()
        {
            Grf.Close();
        }
    }
}
