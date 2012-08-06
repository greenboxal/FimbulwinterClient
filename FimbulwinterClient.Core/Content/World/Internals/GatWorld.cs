using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Axiom.Collections;
using Axiom.Core;

namespace FimbulwinterClient.Core.Content.World.Internals
{
    public class GatWorld : Resource
    {
        public class Cell
        {
            private float[] _height;
            public float[] Height
            {
                get { return _height; }
            }

            private int _type;
            public int Type
            {
                get { return _type; }
            }

            public void Load(BinaryReader br)
            {
                _height = new float[4];
                _height[0] = br.ReadSingle();
                _height[1] = br.ReadSingle();
                _height[2] = br.ReadSingle();
                _height[3] = br.ReadSingle();

                _type = br.ReadInt32();
            }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
        }

        private Cell[] _cells;
        public Cell[] Cells
        {
            get { return _cells; }
        }

        protected byte minorVersion;
        protected byte majorVersion;

        public GatWorld(ResourceManager parent, string name, ulong handle, string group, bool isManual, IManualResourceLoader loader, NameValuePairList createParams)
            : base(parent, name, handle, group, isManual, loader)
        {
        }

        public void Load(Stream gnd)
        {
            BinaryReader br = new BinaryReader(gnd);
            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRAT")
                throw new AxiomException("Invalid GAT header: {0}", header);

            majorVersion = br.ReadByte();
            minorVersion = br.ReadByte();

            if (majorVersion != 1 || minorVersion != 2)
                throw new AxiomException("Unknown GAT version {0}.{1}", majorVersion, minorVersion);

            _width = br.ReadInt32();
            _height = br.ReadInt32();

            _cells = new Cell[_width * _height];
            for (int i = 0; i < _cells.Length; i++)
            {
                Cell c = new Cell();

                c.Load(br);

                _cells[i] = c;
            }
        }

        protected override void load()
        {
            Stream stream = ResourceGroupManager.Instance.OpenResource(Name);
            Load(stream);
            stream.Close();
        }

        protected override void unload()
        {
            _width = 0;
            _height = 0;
            _cells = null;
        }
    }
}
