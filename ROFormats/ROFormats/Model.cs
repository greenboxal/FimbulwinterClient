using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ROFormats
{
    public class Model
    {
        public struct Texture
        {
            public string Name { get; set; }
        }

        public struct Vertex
        {
            private float[] values;
            public float[] Values
            {
                get { if (values == null) values = new float[3]; return values; }
            }

            public float X { get { return Values[0]; } set { Values[0] = value; } }
            public float Y { get { return Values[1]; } set { Values[1] = value; } }
            public float Z { get { return Values[2]; } set { Values[2] = value; } }

            public float this[int index]
            {
                get { return Values[index]; }
                set { Values[index] = value; }
            }

            public void Read(BinaryReader br)
            {
                X = br.ReadSingle();
                Y = br.ReadSingle();
                Z = br.ReadSingle();
            }
        }

        public struct TVertex
        {
            public uint Color { get; set; }
            public float U { get; set; }
            public float V { get; set; }

            public void Read(BinaryReader br, byte majorVersion, byte minorVersion)
            {
                if (majorVersion >= 1 && minorVersion >= 2)
                    Color = br.ReadUInt32();
                else
                    Color = 0xFFFFFFFF;

                U = br.ReadSingle();
                V = br.ReadSingle();
            }
        }

        public struct Face
        {
            public ushort[] VertexID { get; set; }
            public ushort[] TVertexID { get; set; }
            public ushort TexID { get; set; }
            public ushort Padding { get; set; }
            public int TwoSide { get; set; }
            public int SmoothGroup { get; set; }
        }

        public struct PosKeyFrame
        {
            public int Frame { get; set; }
            public float PX { get; set; }
            public float PY { get; set; }
            public float PZ { get; set; }

            public void Read(BinaryReader br)
            {
                Frame = br.ReadInt32();
                PX = br.ReadSingle();
                PY = br.ReadSingle();
                PZ = br.ReadSingle();
            }
        }

        public struct RotKeyFrame
        {
            public int Frame { get; set; }
            public float QX { get; set; }
            public float QY { get; set; }
            public float QZ { get; set; }
            public float QW { get; set; }

            public void Read(BinaryReader br)
            {
                Frame = br.ReadInt32();
                QX = br.ReadSingle();
                QY = br.ReadSingle();
                QZ = br.ReadSingle();
                QW = br.ReadSingle();
            }
        }

        public struct VolumeBox
        {
            public Vertex Size { get; set; }
            public Vertex Pos { get; set; }
            public Vertex Rot { get; set; }
            public int Flag { get; set; }

            public void Read(BinaryReader br, byte majorVersion, byte minorVersion)
            {
                Size.Read(br);
                Pos.Read(br);
                Rot.Read(br);

                if (majorVersion >= 1 && majorVersion >= 3)
                    Flag = br.ReadInt32();
            }
        }

        public struct BoundingBox
        {
            public Vertex Max;
            public Vertex Min;
            public Vertex Offset;
            public Vertex Range;
        }

        public class Node
        {
            public string Name { get; set; }
            public string ParentName { get; set; }
            public int[] Textures { get; set; }
            public float[] OffsetMT { get; set; }
            public Vertex Position { get; set; }
            public float RotAngle { get; set; }
            public Vertex RotAxis { get; set; }
            public Vertex Scale { get; set; }
            public Vertex[] Vertices { get; set; }
            public TVertex[] TVertices { get; set; }
            public Face[] Faces { get; set; }
            public PosKeyFrame[] PosKeyFrames { get; set; }
            public RotKeyFrame[] RotKeyFrames { get; set; }
            public bool IsMain { get; set; }
            public bool IsOnly { get; set; }

            public BoundingBox GetBoundingBox()
            {
                if (!boundingBoxCalculated)
                    CalcBoundingBox();

                return boundingBox;
            }

            public bool Load(BinaryReader br, byte majorVersion, byte minorVersion, bool main)
            {
                Name = br.ReadCString(40);
                ParentName = br.ReadCString(40);

                IsMain = main;

                int textureCount = br.ReadInt32();
                Textures = new int[textureCount];

                for (int i = 0; i < textureCount; i++)
                {
                    Textures[i] = br.ReadInt32();
                }

                OffsetMT = new float[12];
                for (int i = 0; i < 12; i++)
                {
                    OffsetMT[i] = br.ReadSingle();
                }

                Position.Read(br);
                RotAngle = br.ReadSingle();
                RotAxis.Read(br);
                Scale.Read(br);

                int vertexCount = br.ReadInt32();
                Vertices = new Vertex[vertexCount];

                for (int i = 0; i < vertexCount; i++)
                {
                    Vertices[i].Read(br);
                }

                int tvertexCount = br.ReadInt32();
                TVertices = new TVertex[tvertexCount];

                for (int i = 0; i < tvertexCount; i++)
                {
                    TVertices[i].Read(br, majorVersion, minorVersion);
                }

                int faceCount = br.ReadInt32();
                Faces = new Face[faceCount];

                for (int i = 0; i < faceCount; i++)
                {
                    Face f = new Face();

                    f.VertexID = new ushort[3];
                    f.TVertexID = new ushort[3];

                    for (int n = 0; n < 3; n++)
                        f.VertexID[n] = br.ReadUInt16();

                    for (int n = 0; n < 3; n++)
                        f.TVertexID[n] = br.ReadUInt16();

                    f.TexID = br.ReadUInt16();
                    f.Padding = br.ReadUInt16();
                    f.TwoSide = br.ReadInt32();

                    if (majorVersion >= 1 && minorVersion >= 2)
                    {
                        f.SmoothGroup = br.ReadInt32();
                    }
                    else
                    {
                        f.SmoothGroup = 0;
                    }

                    Faces[i] = f;
                }

                if (majorVersion >= 1 && minorVersion >= 5)
                {
                    int frameCount = br.ReadInt32();
                    PosKeyFrames = new PosKeyFrame[frameCount];

                    for (int i = 0; i < frameCount; i++)
                    {
                        PosKeyFrame pfk = new PosKeyFrame();

                        pfk.Read(br);

                        PosKeyFrames[i] = pfk;
                    }
                }

                int rotFrameCount = br.ReadInt32();
                RotKeyFrames = new RotKeyFrame[rotFrameCount];

                for (int i = 0; i < rotFrameCount; i++)
                {
                    RotKeyFrame rfk = new RotKeyFrame();

                    rfk.Read(br);

                    RotKeyFrames[i] = rfk;
                }

                return true;
            }

            private void MatrixMultVect(float[] M, Vertex Vin, ref Vertex Vout)
            {
                Vout[0] = Vin[0] * M[0] + Vin[1] * M[4] + Vin[2] * M[8] + 1.0f * M[12];
                Vout[1] = Vin[0] * M[1] + Vin[1] * M[5] + Vin[2] * M[9] + 1.0f * M[13];
                Vout[2] = Vin[0] * M[2] + Vin[1] * M[6] + Vin[2] * M[10] + 1.0f * M[14];
            }

            private bool boundingBoxCalculated;
            private BoundingBox boundingBox;

            public void CalcBoundingBox()
            {
                float[] transf = new float[16];
                transf[0] = OffsetMT[0];
                transf[1] = OffsetMT[1];
                transf[2] = OffsetMT[2];
                transf[3] = 0;

                transf[4] = OffsetMT[3];
                transf[5] = OffsetMT[4];
                transf[6] = OffsetMT[5];
                transf[7] = 0;

                transf[8] = OffsetMT[6];
                transf[9] = OffsetMT[7];
                transf[10] = OffsetMT[8];
                transf[11] = 0;

                transf[12] = 0;
                transf[13] = 0;
                transf[14] = 0;
                transf[15] = 1;

                boundingBox.Max[0] = boundingBox.Max[1] = boundingBox.Max[2] = -999999.0F;
                boundingBox.Min[0] = boundingBox.Min[1] = boundingBox.Min[2] = 999999.0F;

                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertex v = new Vertex();

                    MatrixMultVect(transf, Vertices[i], ref v);

                    for (int j = 0; j < 3; j++)
                    {
                        float f;

                        if (!IsOnly)
                            f = v[j] + transf[12 + j] + transf[9 + j];
                        else
                            f = v[j];

                        boundingBox.Min[j] = Math.Min(f, boundingBox.Min[j]);
                        boundingBox.Max[j] = Math.Max(f, boundingBox.Max[j]);
                    }
                }

                for (int i = 0; i < 3; i++)
                    boundingBox.Range[i] = (boundingBox.Max[i] + boundingBox.Min[i]) / 2.0f;

                boundingBoxCalculated = true;
            }
        }

        private int animLen;

        public int AnimLen
        {
            get { return animLen; }
            set { animLen = value; }
        }
        private int shadeType;

        public int ShadeType
        {
            get { return shadeType; }
            set { shadeType = value; }
        }
        private byte alpha;

        public byte Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        private Texture[] textures;

        public Texture[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }
        private string mainNode;

        public string MainNode
        {
            get { return mainNode; }
            set { mainNode = value; }
        }
        private Node[] nodes;

        public Node[] Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
        private VolumeBox[] volumeBoxes;

        public VolumeBox[] VolumeBoxes
        {
            get { return volumeBoxes; }
            set { volumeBoxes = value; }
        }

        protected byte minorVersion;
        protected byte majorVersion;

        public BoundingBox GetBoundingBox()
        {
            if (!boundingBoxCalculated)
                CalcBoundingBox();

            return boundingBox;
        }

        private bool boundingBoxCalculated;
        private BoundingBox boundingBox;

        public Model()
        {

        }

        public bool Load(Stream s)
        {
            BinaryReader br = new BinaryReader(s);

            string header = ((char)br.ReadByte()).ToString() + ((char)br.ReadByte()) + ((char)br.ReadByte()) + ((char)br.ReadByte());

            if (header != "GRSM")
                return false;

            majorVersion = br.ReadByte();
            minorVersion = br.ReadByte();

            if (!(majorVersion == 1 && (minorVersion >= 1 && minorVersion <= 5)))
                return false;

            animLen = br.ReadInt32();
            shadeType = br.ReadInt32();

            if (majorVersion >= 1 && minorVersion >= 4)
            {
                alpha = br.ReadByte();
            }
            else
            {
                alpha = 255;
            }

            br.ReadBytes(16);

            int textureCount = br.ReadInt32();
            textures = new Texture[textureCount];

            for (int i = 0; i < textureCount; i++)
            {
                Texture tex = new Texture();

                tex.Name = br.ReadCString(40);

                textures[i] = tex;
            }

            mainNode = br.ReadCString(40);

            int nodeCount = br.ReadInt32();
            nodes = new Node[nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                Node n = new Node();

                if (!n.Load(br, majorVersion, minorVersion, i == 0))
                    return false;

                nodes[i] = n;
            }

            if (FindNode(mainNode) == null)
                return false;
            
            if (nodes.Length == 1)
                nodes[0].IsOnly = true;

            if (majorVersion < 1 || minorVersion < 5)
            {
                Node mNode = FindNode(mainNode);

                int frameCount = br.ReadInt32();
                mNode.PosKeyFrames = new PosKeyFrame[frameCount];

                for (int i = 0; i < frameCount; i++)
                {
                    PosKeyFrame pfk = new PosKeyFrame();

                    pfk.Read(br);

                    mNode.PosKeyFrames[i] = pfk;
                }
            }

            int volumeCount = br.ReadInt32();
            volumeBoxes = new VolumeBox[volumeCount];

            for (int i = 0; i < volumeCount; i++)
            {
                VolumeBox vb = new VolumeBox();

                vb.Read(br, majorVersion, minorVersion);

                volumeBoxes[i] = vb;
            }

            CalcBoundingBox();

            return true;
        }

        private Node FindNode(string mainNode)
        {
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i].Name == mainNode)
                    return nodes[i];

            return null;
        }

        private void CalcBoundingBox()
        {
            Node mainNode = FindNode(MainNode);

            mainNode.CalcBoundingBox();
            for (int i = 0; i < nodes.Length; i++)
                if (nodes[i].ParentName == MainNode && nodes[i] != mainNode)
                    nodes[i].CalcBoundingBox();

            for (int i = 0; i < 3; i++)
            {
                boundingBox.Max[i] = mainNode.GetBoundingBox().Max[i];
                boundingBox.Min[i] = mainNode.GetBoundingBox().Min[i];

                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[j].ParentName == MainNode)
                    {
                        boundingBox.Max[i] = Math.Max(boundingBox.Max[i], nodes[j].GetBoundingBox().Max[i]);
                        boundingBox.Min[i] = Math.Min(boundingBox.Min[i], nodes[j].GetBoundingBox().Min[i]);
                    }
                }

                boundingBox.Range[i] = (boundingBox.Max[i] - boundingBox.Min[i]) / 2.0f;
            }

            boundingBoxCalculated = true;
        }
    }
}
