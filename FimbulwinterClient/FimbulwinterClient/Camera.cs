using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient
{
    public sealed class Camera
    {

        #region Fields

        private static float _elapsed;
        private static Camera _camera;
        private Matrix _proj;
        private Matrix _view;
        private Vector3 _lastMousePosition = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 _currentMousePosition = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 _right = new Vector3(1.0f, 0.0f, 0.0f);
        private Vector3 _up = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 _look;
        private Vector3 _position;

        #endregion


        #region Properties

        public static float ElapsedTime
        {
            get
            {
                return Camera._elapsed;
            }
            set
            {
                Camera._elapsed = value;
            }
        }

        public static Camera DefaultCamera
        {
            get
            {
                return Camera._camera;
            }
            set
            {
                Camera._camera = value;
            }
        }

        public Matrix View
        {
            get
            {
                return this._view;
            }
        }

        public Matrix Projection
        {
            get
            {
                return this._proj;
            }
        }

        public Vector3 Right
        {
            get
            {
                return this._right;
            }
            set
            {
                this._right = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }

        public Vector3 UpVector
        {
            get
            {
                return this._up;
            }
            set
            {
                this._up = value;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return this._look;
            }
            set
            {
                this._look = value;
            }
        }

        public float AspectRatio
        {
            get
            {
                return 800f / 600f;
            }
        }

        public float Fov
        {
            get
            {
                return MathHelper.PiOver4;
            }
        }

        public float NearClip
        {
            get
            {
                return 1f;
            }
        }

        public float FarClip
        {
            get
            {
                return 1000000f;
            }
        }
        #endregion


        #region Utils

        public static Matrix RotationAxis(Vector3 vec, float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            float t = 1 - c;
            Matrix m =
                    new Matrix(t * vec.X * vec.X + c, t * vec.X * vec.Y + s * vec.Z, t * vec.X * vec.Z + s * vec.Y, 0,
                                 t * vec.X * vec.Y - s * vec.Z, t * vec.Y * vec.Y + c, t * vec.Y * vec.Z + s * vec.X, 0,
                                 t * vec.X * vec.Z + s * vec.Y, t * vec.Y * vec.Z - s * vec.X, t * vec.Z * vec.Z + c, 0,
                                 0, 0, 0, 1);
            return Matrix.Transpose(m);
        }

        public static void Normalize(Plane p)
        {
            float length = p.Normal.Length();

            p.Normal.X *= length;
            p.Normal.Y *= length;
            p.Normal.Z *= length;
            p.D *= length;
        }

        #endregion


        #region Constructors

        public Camera(Vector3 eye, Vector3 target)
        {
            _proj = Matrix.CreatePerspectiveFieldOfView(this.Fov, this.AspectRatio, this.NearClip, this.FarClip);
            _position = eye;
            _look = target;
            _up = new Vector3(0.0f, 1.0f, 0.0f);
            _right = new Vector3(0.5f, 0.0f, 0.0f);
        }

        #endregion


        #region Movment

        public void Strafe(float units)
        {
            _position += _right * units * Camera.ElapsedTime;
        }

        public void Fly(float units)
        {
            _position += _up * units * Camera.ElapsedTime;
        }

        public void Land(float units)
        {
            _position -= _up * units * Camera.ElapsedTime;
        }

        public void Walk(float units)
        {
            _position += _look * units * Camera.ElapsedTime;
        }

        public void Pitch(float angle)
        {
            Matrix T = new Matrix();
            T = Camera.RotationAxis(_right, angle * Camera.ElapsedTime);

            _up = Vector3.Transform(_up, T);
            _look = Vector3.Transform(_look, T);
        }

        public void Yaw(float angle)
        {
            Matrix T = new Matrix();

            T = Camera.RotationAxis(_up, angle * Camera.ElapsedTime);

            _right = Vector3.Transform(_right, T);
            _look = Vector3.Transform(_look, T);
        }

        public void Roll(float angle)
        {
            Matrix T = new Matrix();
            T = Camera.RotationAxis(_look, angle * Camera.ElapsedTime);

            _right = Vector3.Transform(_right, T);
            _up = Vector3.Transform(_up, T);
        }

        public void RotateByMouse(Vector3 pos)
        {
            float deltaX = (float)pos.X * 0.005f;
            float deltaY = (float)pos.Y * 0.005f;

            Matrix R = Camera.RotationAxis(_right, deltaY);
            _look = Vector3.Transform(_look, R);
            _up = Vector3.Transform(_up, R);

            R = Matrix.CreateRotationY(deltaX);
            _look = Vector3.Transform(_look, R);
            _right = Vector3.Transform(_right, R);
        }

        #endregion


        #region Public methods

        public void Update()
        {
            Matrix v = new Matrix();

            _look.Normalize();

            _up = Vector3.Cross(_look, _right);
            _up.Normalize();

            _right = Vector3.Cross(_up, _look);

            float x = -Vector3.Dot(_right, _position);
            float y = -Vector3.Dot(_up, _position);
            float z = -Vector3.Dot(_look, _position);

            v.M11 = _right.X;
            v.M12 = _up.X;
            v.M13 = _look.X;
            v.M14 = 0.0f;

            v.M21 = _right.Y;
            v.M22 = _up.Y;
            v.M23 = _look.Y;
            v.M24 = 0.0f;

            v.M31 = _right.Z;
            v.M32 = _up.Z;
            v.M33 = _look.Z;
            v.M34 = 0.0f;

            v.M41 = x;
            v.M42 = y;
            v.M43 = z;
            v.M44 = 1.0f;

            this._view = v;

        }

        #endregion

    }

}
