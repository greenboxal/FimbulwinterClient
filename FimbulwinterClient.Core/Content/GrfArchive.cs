using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.FileSystem;
using GRFSharp;
using System.IO;

namespace FimbulwinterClient.Core.Content
{
    public class GrfArchive : Archive
    {
        private GRF _grf;
        public GRF Grf
        {
            get { return _grf; }
        }

        private GRFFile _file;
        public GRFFile File
        {
            get { return _file; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
        }

        public GrfArchive(GRF grf, GRFFile file)
            : base(file.Name, "GrfFile")
        {
            _file = file;
        }

        public override void Load()
        {
            _data = _grf.GetDataFromFile(_file);
        }

        public override Stream Open(string filename, bool readOnly)
        {
            if (_data == null)
                Load();

            return new MemoryStream(_data);
        }

        public override bool Exists(string fileName)
        {
            throw new NotImplementedException();
        }

        public override List<string> Find(string pattern, bool recursive)
        {
            throw new NotImplementedException();
        }

        public override FileInfoList FindFileInfo(string pattern, bool recursive)
        {
            throw new NotImplementedException();
        }

        public override bool IsCaseSensitive
        {
            get { throw new NotImplementedException(); }
        }

        public override List<string> List(bool recursive)
        {
            throw new NotImplementedException();
        }

        public override FileInfoList ListFileInfo(bool recursive)
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
