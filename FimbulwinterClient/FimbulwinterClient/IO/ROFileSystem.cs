using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRFSharp;
using System.IO;

namespace FimbulwinterClient.IO
{
    public class ROFileSystem
    {
        private List<GRF> _grfFiles;
        public List<GRF> GrfFiles
        {
            get { return _grfFiles; }
        }

        public ROFileSystem()
        {
            _grfFiles = new List<GRF>();
        }

        public Stream LoadFile(string asset)
        {
            if (File.Exists(asset))
            {
                return new FileStream(asset, FileMode.Open);
            }
            else
            {
                for (int i = 0; i < _grfFiles.Count; i++)
                {
                    GRFFile f = _grfFiles[i].GetFile(asset);

                    if (f != null)
                    {
                        byte[] data = _grfFiles[i].GetDataFromFile(f);

                        return new MemoryStream(data);
                    }
                }
            }

            return null;
        }

        public void LoadGrf(string path)
        {
            if (!File.Exists(path))
                return;

            GRF grf = new GRF();
            grf.Open(path);

            _grfFiles.Add(grf);
        }
    }
}
