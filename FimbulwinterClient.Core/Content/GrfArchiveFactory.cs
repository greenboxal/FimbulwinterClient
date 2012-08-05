using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRFSharp;
using System.IO;
using Axiom.FileSystem;

namespace FimbulwinterClient.Core.Content
{
    public class GrfArchiveFactory : ArchiveFactory
    {
        private static List<GRF> _grfFiles;
        public static List<GRF> GrfFiles
        {
            get { return GrfArchiveFactory._grfFiles; }
        }

        static GrfArchiveFactory()
        {
            _grfFiles = new List<GRF>();
        }
        
        public static void AddGrf(string file)
        {
            GRF grf = new GRF();

            grf.Open(file);

            _grfFiles.Add(grf);
        }

        public GrfArchiveFactory()
        {
            Type = "GrfFile";
        }

        public override Archive CreateInstance(string name)
        {
            return CreateInstance(name, null);
        }

        public override Archive CreateInstance(string name, Axiom.Collections.NameValuePairList parms)
        {
            for (int i = 0; i < _grfFiles.Count; i++)
            {
                GRFFile f = _grfFiles[i].GetFile(name);

                if (f != null)
                {
                    return new GrfArchive(_grfFiles[i], f);
                }
            }

            return null;
        }

        public override string Type
        {
            get
            {
                return base.Type;
            }
            protected set
            {
                base.Type = value;
            }
        }
    }
}
