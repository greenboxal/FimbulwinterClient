using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Content.MapInternals
{
    public class Altitude
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

        public bool Load(Stream gnd)
        {
            BinaryReader br = new BinaryReader(gnd);
            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRAT")
                return false;

            majorVersion = br.ReadByte();
            minorVersion = br.ReadByte();

            if (majorVersion != 1 || minorVersion != 2)
                return false;

            _width = br.ReadInt32();
            _height = br.ReadInt32();

            _cells = new Cell[_width * _height];
            for (int i = 0; i < _cells.Length; i++)
            {
                Cell c = new Cell();

                c.Load(br);

                _cells[i] = c;
            }

            Map.OnReportStatus("Altitude v{0}.{1} status: {2}x{3} - {4} cells", majorVersion, minorVersion, _width, _height, _cells.Length);

            return true;
        }
    }
}
