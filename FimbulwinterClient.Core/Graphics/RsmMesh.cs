using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Extensions;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Graphics
{
    public class RsmMesh
    {
                private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _parentName;
        public string ParentName
        {
            get { return _parentName; }
        }

        private Texture2D[] _textures;
        public Texture2D[] Textures
        {
            get { return _textures; }
        }

        private float[] _matrix;
        public float[] Matrix
        {
            get { return _matrix; }
        }

        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
        }

        private Vector3 _position2;
        public Vector3 Position2
        {
            get { return _position2; }
        }

        private VertexBuffer _vertices;
        public VertexBuffer Vertices
        {
            get { return _vertices; }
        }

        private IndexBuffer[] _indexes;
        public IndexBuffer[] Indexes
        {
            get { return _indexes; }
        }

        private Tuple<Quaternion, int>[] _rotationFrames;
        public Tuple<Quaternion, int>[] RotationFrames
        {
            get { return _rotationFrames; }
        }

        private float _rotationAngle;
        public float RotationAngle
        {
            get { return _rotationAngle; }
        }

        private Vector3 _rotationAxis;
        public Vector3 RotationAxis
        {
            get { return _rotationAxis; }
        }

        private Vector3 _scale;
        public Vector3 Scale
        {
            get { return _scale; }
        }

        private Vector3[] _tempVertices;

        private AxisAlignedBox _boudingBox;
        public AxisAlignedBox BoudingBox
        {
            get { return _boudingBox; }
        }

        private RsmMesh _parent;
        public RsmMesh Parent
        {
            get { return _parent; }
        }

        private RsmMesh[] _children;
        public RsmMesh[] Children
        {
            get { return _children; }
        }

        private readonly RsmModel _owner;
        public RsmModel Owner
        {
            get { return _owner; }
        }

        public RsmMesh(RsmModel owner)
        {
            _owner = owner;
            _children = new RsmMesh[0];
        }

        public void Load(RsmModel owner, BinaryReader br, byte majorVersion, byte minorVersion)
        {
            _name = br.ReadCString(40);
            _parentName = br.ReadCString(40);

            _textures = new Texture2D[br.ReadInt32()];
            for (int i = 0; i < _textures.Length; i++)
            {
                _textures[i] = owner.Textures[br.ReadInt32()];
            }

            _matrix = new float[16];
            _matrix[0] = br.ReadSingle();
            _matrix[1] = br.ReadSingle();
            _matrix[2] = br.ReadSingle();
            _matrix[3] = 0;

            _matrix[4] = br.ReadSingle();
            _matrix[5] = br.ReadSingle();
            _matrix[6] = br.ReadSingle();
            _matrix[7] = 0;

            _matrix[8] = br.ReadSingle();
            _matrix[9] = br.ReadSingle();
            _matrix[10] = br.ReadSingle();
            _matrix[11] = 0;

            _matrix[12] = 0;
            _matrix[13] = 0;
            _matrix[14] = 0;
            _matrix[15] = 1;

            _position2.X = br.ReadSingle();
            _position2.Y = br.ReadSingle();
            _position2.Z = br.ReadSingle();

            _position.X = br.ReadSingle();
            _position.Y = br.ReadSingle();
            _position.Z = br.ReadSingle();

            _rotationAngle = br.ReadSingle();

            _rotationAxis.X = br.ReadSingle();
            _rotationAxis.Y = br.ReadSingle();
            _rotationAxis.Z = br.ReadSingle();

            _scale.X = br.ReadSingle();
            _scale.Y = br.ReadSingle();
            _scale.Z = br.ReadSingle();

            _tempVertices = new Vector3[br.ReadInt32()];
            for (int i = 0; i < _tempVertices.Length; i++)
            {
                _tempVertices[i].X = br.ReadSingle();
                _tempVertices[i].Y = br.ReadSingle();
                _tempVertices[i].Z = br.ReadSingle();
            }

            Vector2[] texcoords = new Vector2[br.ReadInt32()];
            for (int i = 0; i < texcoords.Length; i++)
            {
                if (majorVersion > 1 || (majorVersion == 1 && minorVersion >= 2))
                    br.ReadSingle();

                texcoords[i].X = br.ReadSingle();
                texcoords[i].Y = 1 - br.ReadSingle();
            }

            List<VertexPositionNormalTexture> ggvertices = new List<VertexPositionNormalTexture>();
            List<int>[] indexes = new List<int>[_textures.Length];

            for (int i = 0; i < _textures.Length; i++)
                indexes[i] = new List<int>();

            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int start = i * 3;

                Vector3 v1 = _tempVertices[br.ReadInt16()];
                Vector3 v2 = _tempVertices[br.ReadInt16()];
                Vector3 v3 = _tempVertices[br.ReadInt16()];

                Vector2 t1 = texcoords[br.ReadInt16()];
                Vector2 t2 = texcoords[br.ReadInt16()];
                Vector2 t3 = texcoords[br.ReadInt16()];

                int tex = br.ReadInt16();
                br.ReadInt16();
                br.ReadInt32();
                br.ReadInt32();

                Vector3 normal = Vector3.Cross(v1 - v3, v2 - v3);
                normal.Normalize();

                ggvertices.Add(new VertexPositionNormalTexture(v1, normal, t1));
                ggvertices.Add(new VertexPositionNormalTexture(v2, normal, t2));
                ggvertices.Add(new VertexPositionNormalTexture(v3, normal, t3));

                indexes[tex].Add(start + 0);
                indexes[tex].Add(start + 1);
                indexes[tex].Add(start + 2);
            }

            _rotationFrames = new Tuple<Quaternion, int>[br.ReadInt32()];
            for (int i = 0; i < _rotationFrames.Length; i++)
            {
                int time = br.ReadInt32();
                Quaternion q = new Quaternion();

                q.X = br.ReadSingle();
                q.Y = br.ReadSingle();
                q.Z = br.ReadSingle();
                q.W = br.ReadSingle();

                _rotationFrames[i] = new Tuple<Quaternion, int>(q, time);
            }

            Dispatcher.Instance.DispatchCoreTask(o =>
                {
                    _vertices = new VertexBuffer(VertexPositionNormalTexture.VertexDeclaration);
                    _vertices.SetData(ggvertices.ToArray(), BufferUsageHint.StaticDraw);

                    _indexes = new IndexBuffer[_textures.Length];
                    for (int i = 0; i < _textures.Length; i++)
                    {
                        if (indexes[i].Count > 0)
                        {
                            _indexes[i] = new IndexBuffer(DrawElementsType.UnsignedInt);
                            _indexes[i].SetData(indexes[i].ToArray(), BufferUsageHint.StaticDraw);
                        }
                    }
                });
        }

        private void MatrixMultVect(float[] m, Vector3 vin, ref Vector3 vout) 
        {
            vout.X = vin.X * m[0] + vin.Y * m[4] + vin.Z * m[8] + 1.0f * m[12];
            vout.Y = vin.X * m[1] + vin.Y * m[5] + vin.Z * m[9] + 1.0f * m[13];
            vout.Z = vin.X * m[2] + vin.Y * m[6] + vin.Z * m[10] + 1.0f * m[14];
        }

        private void CalculateBoundingBox()
        {
            _boudingBox = new AxisAlignedBox(new Vector3(999999), new Vector3(-999999));

            foreach (Vector3 ov in _tempVertices)
            {
                Vector3 v = Vector3.Zero;

                MatrixMultVect(_matrix, ov, ref v);

                if (Parent != null || Children.Length > 0)
                    v += _position2;

                _boudingBox += v;
            }

            foreach (RsmMesh m in _children)
            {
                m.CalculateBoundingBox();
                _boudingBox += m.BoudingBox;
            }

            _boudingBox.CalculateRangeAndOffset();
        }

        private double _lastTick;
        public void Draw(CommonShaderProgram shader, double elapsed)
        {
            if (!_owner.Loaded)
                return;

            GL.Scale(_scale);

            if (Parent == null)
            {
                if (Children.Length == 0)
                {
                    GL.Translate(0, -_boudingBox.Max.Y + _boudingBox.Offset.Y, 0);
                }
                else
                {
                    GL.Translate(-_boudingBox.Offset.X, -_boudingBox.Max.Y, -_boudingBox.Offset.Z);
                }
            }
            else
            {
                GL.Translate(Position);
            }

            if (RotationFrames.Length == 0)
            {
                GL.Rotate(MathHelper.RadiansToDegrees(RotationAngle), RotationAxis);
            }
            else
            {
                int current = 0;

                for (int i = 0; i < _rotationFrames.Length; i++)
                {
                    if (_rotationFrames[i].Item2 > _lastTick)
                    {
                        current = i - 1;
                        break;
                    }
                }

                if (current < 0)
                    current = 0;

                int next = current + 1;
                if (next >= _rotationFrames.Length)
                    next = 0;

                float interval = ((float)(_lastTick - _rotationFrames[current].Item2)) / ((_rotationFrames[next].Item2 - _rotationFrames[current].Item2));
                Quaternion q = Quaternion.Slerp(_rotationFrames[current].Item1, _rotationFrames[next].Item1, interval);
                q.Normalize();

                _lastTick += elapsed;
                while (_lastTick > _rotationFrames[_rotationFrames.Length - 1].Item2)
                    _lastTick -= _rotationFrames[_rotationFrames.Length - 1].Item2;

                float angle;
                Vector3 axis;
                q.ToAxisAngle(out axis, out angle);

                GL.Rotate(MathHelper.RadiansToDegrees(angle), axis);
            }

            GL.PushMatrix();

            if (Parent == null && Children.Length == 0)
            {
                GL.Translate(-_boudingBox.Offset.X, -_boudingBox.Offset.Y, -_boudingBox.Offset.Z);
            }
            else if (Parent != null || Children.Length > 0)
            {
                GL.Translate(_position2);
            }
            
            GL.MultMatrix(_matrix);

            shader.Begin();
            shader.SetAlpha(_owner.Alpha);
            _vertices.Bind();
            for (int i = 0; i < _textures.Length; i++)
            {
                shader.SetTexture(_textures[i]);
                _vertices.Render(BeginMode.Triangles, _indexes[i], _indexes[i].Count);
            }
            shader.End();

            GL.PopMatrix();

            foreach (RsmMesh t in _children)
            {
                GL.PushMatrix();
                t.Draw(shader, elapsed);
                GL.PopMatrix();
            }
        }
        
        public void CreateChildren(RsmMesh[] meshes)
        {
            foreach (RsmMesh m in meshes.Where(m => String.Compare(m.Name, _parentName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                _parent = m;
                break;
            }

            _children = meshes.Where(m => String.Compare(m.ParentName, _name, StringComparison.OrdinalIgnoreCase) == 0).ToArray();

            CalculateBoundingBox();
        }
    }
}
