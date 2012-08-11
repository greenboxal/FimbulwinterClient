using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Core.Content.MapInternals
{
    public class Altitude
    {
        public class Cell
        {
            public float[] Height { get; private set; }
            public int Type { get; private set; }

            public void Load(BinaryReader br)
            {
                Height = new float[4];
                Height[0] = br.ReadSingle();
                Height[1] = br.ReadSingle();
                Height[2] = br.ReadSingle();
                Height[3] = br.ReadSingle();

                Type = br.ReadInt32();
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Cell[] Cells { get; private set; }

        protected byte MinorVersion;
        protected byte MajorVersion;

        public bool Load(Stream gnd)
        {
            BinaryReader br = new BinaryReader(gnd);
            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRAT")
                return false;

            MajorVersion = br.ReadByte();
            MinorVersion = br.ReadByte();

            if (MajorVersion != 1 || MinorVersion != 2)
                return false;

            Width = br.ReadInt32();
            Height = br.ReadInt32();

            Cells = new Cell[Width * Height];
            for (int i = 0; i < Cells.Length; i++)
            {
                Cell c = new Cell();

                c.Load(br);

                Cells[i] = c;
            }

            //Logger.WriteLine("Altitude v{0}.{1} status: {2}x{3} - {4} cells", MajorVersion, MinorVersion, _width, _height, _cells.Length);

            return true;
        }
    }
}