using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace FimbulwinterClient.Core
{
    public class Camera
    {
        private Matrix _view;
        public Matrix View
        {
            get { return _view; }
        }

        private Matrix _projection;
        public Matrix Projection
        {
            get { return _projection; }
        }

        private Matrix _world;
        public Matrix World
        {
            get { return _world; }
        }

        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
        }

        private Vector3 _target;
        public Vector3 Target
        {
            get { return _target; }
        }

        private float _farPlane = 5000f;
        public float FarPlane
        {
            get { return _farPlane; }
        }

        private float _nearPlane = 100f;
        public float NearPlane
        {
            get { return _nearPlane; }
        }

        private float _yaw = 0f;
        public float Yaw
        {
            get { return _yaw; }
        }

        private float _pitch = 0f;
        public float Pitch
        {
            get { return _pitch; }
        }

        private Ray _mouseRay;
        public Ray MouseRay
        {
            get { return _mouseRay; }
        }

        private GraphicsDevice _device;
        public GraphicsDevice GraphicsDevice
        {
            get { return _device; }
        }

        public Vector3 Direction
        {
            get
            {
                return Vector3.Normalize(_target - _position);
            }
        }

        public Vector3 Up
        {
            get
            {
                return Vector3.Up;
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(Direction, Up));
            }
        }

        public Camera(Vector3 position, Vector3 target, Matrix world, float near, float far)
        {
            if (position == target) 
                target.Z += 10f;

            _position = position;
            _target = target;

            _device = SharedInformation.GraphicsDevice;
            _nearPlane = near;
            _farPlane = far;

            CalculateYawPitch();

            while (Math.Abs(_pitch) >= MathHelper.ToRadians(80))
            {
                _position.Z += 10;
                CalculateYawPitch();
            }

            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, _nearPlane, _farPlane);
            _view = Matrix.CreateLookAt(_position, _target, Up);
            _world = world;
        }

        protected virtual void CalculateYawPitch()
        {
            Vector3 dir = _target - _position;
            dir.Normalize();
            Vector3 m = dir; m.Y = _position.Y;

            _yaw = (float)Math.Atan2(dir.X, dir.Z);

            float len = (new Vector2(m.X, m.Z)).Length();
            _pitch = (float)Math.Atan2(dir.Y, len);
        }

        public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
        {
            Vector3 near = new Vector3(mousePosition, 0);
            Vector3 far = new Vector3(mousePosition, 1);

            near = viewport.Unproject(near, _projection, _view, _world);
            far = viewport.Unproject(far, _projection, _view, _world);

            return new Ray(near, Vector3.Normalize(far - near));
        }

        public virtual void UpdateMouseRay(Vector2 mousePos, Viewport viewport)
        {
            _mouseRay = this.GetMouseRay(mousePos, viewport);
        }

        public virtual void Update()
        {
            _view = Matrix.CreateLookAt(_position, _target, this.Up);
        }

        public virtual void MoveForward(float amount)
        {
            _position += amount * this.Direction;
            _target += amount * this.Direction;

            this.Update();
        }

        public virtual void Strafe(float amount)
        {
            _position += amount * this.Right;
            _target += amount * this.Right;

            this.Update();
        }

        public virtual void AddYaw(float angle)
        {
            _yaw += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(this.Up, angle));

            _target = _position + Vector3.Distance(_target, _position) * dir;
            CalculateYawPitch();

            this.Update();
        }

        public virtual void AddPitch(float angle)
        {
            if (Math.Abs(_pitch + angle) >= MathHelper.ToRadians(80)) return;
            _pitch += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(this.Right, angle));

            _target = _position + Vector3.Distance(_target, _position) * dir;
            CalculateYawPitch();

            this.Update();
        }

        public virtual void Levitate(float amount)
        {
            _position.Y += amount;
            _target.Y += amount;

            this.Update();
        }
    }
}
