using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using QuickFont;

namespace FimbulwinterClient
{
    public sealed class Camera
    {
        private Vector3 _position;
        private Vector3 _target;

        public Vector3 Position
        {
            get { return _position; }
            private set { _position = value; }
        }

        public Vector3 Target
        {
            get { return _target; }
            private set { _target = value; }
        }

        public float FarPlane { get; private set; }
        public float NearPlane { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        /*private Ray _mouseRay;
        public Ray MouseRay
        {
            get { return _mouseRay; }
        }*/

        public Vector3 Direction
        {
            get
            {
                return Vector3.Normalize(Target - Position);
            }
        }

        public Vector3 Up
        {
            get
            {
                return Vector3.UnitY;
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(Direction, Up));
            }
        }

        public Camera(Vector3 position, Vector3 target, float near, float far)
        {
            Pitch = 0f;
            Yaw = 0f;
            if (position == target)
                target.Z += 10f;

            Position = position;
            Target = target;

            NearPlane = near;
            FarPlane = far;

            CalculateYawPitch();

            while (Math.Abs(Pitch) >= MathHelper.DegreesToRadians(80))
            {
                _position.Z += 10;
                CalculateYawPitch();
            }
        }

        private void CalculateYawPitch()
        {
            Vector3 dir = Target - Position;
            dir.Normalize();
            Vector3 m = dir; m.Y = Position.Y;
            
            Yaw = (float)Math.Atan2(dir.X, dir.Z);

            float len = (new Vector2(m.X, m.Z)).Length;
            Pitch = (float)Math.Atan2(dir.Y, len);
        }

        /*public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
        {
            Vector3 near = new Vector3(mousePosition, 0);
            Vector3 far = new Vector3(mousePosition, 1);

            near = viewport.Unproject(near, _projection, _view, _world);
            far = viewport.Unproject(far, _projection, _view, _world);

            return new Ray(near, Vector3.Normalize(far - near));
        }

        public void UpdateMouseRay(Vector2 mousePos, Viewport viewport)
        {
            _mouseRay = this.GetMouseRay(mousePos, viewport);
        }*/

        public void Update()
        {
            Matrix4 view = Matrix4.LookAt(Position, Target, Up);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref view);
        }

        public void MoveForward(float amount)
        {
            Position += amount * Direction;
            Target += amount * Direction;
        }

        public void Strafe(float amount)
        {
            Position += amount * Right;
            Target += amount * Right;
        }

        public void AddYaw(float angle)
        {
            Yaw += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix4.CreateFromAxisAngle(Up, angle));

            Target = Position + (Target - Position).Length * dir;
            CalculateYawPitch();
        }

        public void AddPitch(float angle)
        {
            if (Math.Abs(Pitch + angle) >= MathHelper.DegreesToRadians(80)) return;
            Pitch += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix4.CreateFromAxisAngle(this.Right, angle));

            Target = Position + (Target - Position).Length * dir;
            CalculateYawPitch();
        }

        public void Levitate(float amount)
        {
            _position.Y += amount;
            _target.Y += amount;
        }
    }
}