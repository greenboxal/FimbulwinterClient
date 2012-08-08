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
        public const string ArchiveType = "GrfFile";

        public override Archive CreateInstance(string name)
        {
            return CreateInstance(name, null);
        }

        public override Archive CreateInstance(string name, Axiom.Collections.NameValuePairList parms)
        {
            return new GrfArchive(new GRF(name), name);
        }

        public override string Type
        {
            get { return ArchiveType; }
            protected set { }
        }
    }
}