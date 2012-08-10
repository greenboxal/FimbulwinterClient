using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.IO;
using GRFSharp;

namespace FimbulwinterClient.Core.IO
{
    public class GrfFileSystem : IFileSystem
    {
        public GRF Grf { get; private set; }

        public string Type
        {
            get { return "Grf"; }
        }

        public GrfFileSystem(string filename)
        {
            Grf = new GRF(filename);
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
            return (from GRFFile file in Grf.Files where string.Compare(file.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0 select new MemoryStream(Grf.GetDataFromFile(file))).FirstOrDefault();
        }

        public void Close()
        {
            Grf.Close();
        }
    }
}
