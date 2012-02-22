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
        private List<GRF> m_grfFiles;
        public List<GRF> GrfFiles
        {
            get { return m_grfFiles; }
        }

        public ROFileSystem()
        {
            m_grfFiles = new List<GRF>();
        }

        public Stream LoadFile(string asset)
        {
            if (File.Exists(asset))
            {
                return new FileStream(asset, FileMode.Open);
            }
            else
            {
                for (int i = 0; i < m_grfFiles.Count; i++)
                {
                    GRFFile f = m_grfFiles[i].GetFile(asset);

                    if (f != null)
                    {
                        byte[] data = m_grfFiles[i].GetDataFromFile(f);

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

            m_grfFiles.Add(grf);
        }
    }
}
