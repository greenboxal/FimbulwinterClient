using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRFSharp;
using System.IO;

namespace FimbulwinterClient.Core.Content
{
    public class GrfFileSystem : IFileSystem
    {
        private static List<GRF> _grfFiles;
        public static List<GRF> GrfFiles
        {
            get { return GrfFileSystem._grfFiles; }
        }

        static GrfFileSystem()
        {
            _grfFiles = new List<GRF>();
        }

        public static void AddGrf(string file)
        {
            GRF grf = new GRF();

            grf.Open(file);

            _grfFiles.Add(grf);
        }

        public Stream Load(string filename)
        {
            for (int i = 0; i < _grfFiles.Count; i++)
            {
                GRFFile f = _grfFiles[i].GetFile(filename);

                if (f != null)
                {
                    byte[] data = _grfFiles[i].GetDataFromFile(f);

                    return new MemoryStream(data);
                }
            }

            return null;
        }
    }
}
