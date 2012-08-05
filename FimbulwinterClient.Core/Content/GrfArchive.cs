using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.FileSystem;
using GRFSharp;
using System.IO;
using System.Collections;

namespace FimbulwinterClient.Core.Content
{
    public class GrfArchive : Archive
    {
        private GRF _grf;
        public GRF Grf
        {
            get { return _grf; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
        }

        public GrfArchive(GRF grf, string name)
            : base(name, GrfArchiveFactory.ArchiveType)
        {
            _grf = grf;
        }

        public override void Load()
        {
            _grf.Open();
        }

        public override Stream Open(string fileName, bool readOnly)
        {
            if (!_grf.IsOpen)
                Load();

            GRFFile file = _grf.GetFile(fileName);

            if (file == null)
                return null;

            return new MemoryStream(_grf.GetDataFromFile(file));
        }

        public override bool Exists(string fileName)
        {
            if (!_grf.IsOpen)
                Load();

            return _grf.GetFile(fileName) != null;
        }

        public override List<string> Find(string pattern, bool recursive)
        {
            List<string> files = new List<string>();

            if (pattern == "*" && recursive == true)
            {
                foreach (DictionaryEntry file in _grf.Files)
                    files.Add((string)file.Key);
            }
            else
            {
                throw new NotImplementedException();
            }

            return files;
        }

        public override FileInfoList FindFileInfo(string pattern, bool recursive)
        {
            return new FileInfoList();
        }

        public override bool IsCaseSensitive
        {
            get { return false; }
        }

        public override bool IsMonitorable
        {
            get { return false; }
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
            _grf.Close();
        }
    }
}
