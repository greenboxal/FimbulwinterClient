using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Content.MapInternals;

namespace FimbulwinterClient.Content
{
    public class Map
    {
        private Ground _ground;
        public Ground Ground
        {
            get { return _ground; }
        }

        private Altitude _altitude;
        public Altitude Altitude
        {
            get { return _altitude; }
        }

        private World _world;
        public World World
        {
            get { return _world; }
        }

        private Texture2D _shadowLightmap;
        public Texture2D ShadowLightmap
        {
            get { return _shadowLightmap; }
        }

        private Texture2D _colorLightmap;
        public Texture2D ColorLightmap
        {
            get { return _colorLightmap; }
        }

        private Effect _effect;
        public Effect Effect
        {
            get { return _effect; }
        }

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public Map(GraphicsDevice gd)
        {
            _graphicsDevice = gd;
            _effect = new BasicEffect(_graphicsDevice);

            _effect = ROClient.Singleton.Content.Load<Effect>(@"Ragnarok");
            _effect.CurrentTechnique = _effect.Techniques["MapGround"];
        }

        public bool Load(Stream gat, Stream gnd, Stream rsw)
        {
            _ground = new Ground(_graphicsDevice);

            if (!_ground.Load(gnd))
                return false;

            _altitude = new Altitude();
            if (!_altitude.Load(gat))
                return false;

            _world = new World(_graphicsDevice);
            if (!_world.Load(rsw, this))
                return false;

            _effect.Parameters["AmbientColor"].SetValue(_world.LightInfo.Ambient);
            _effect.Parameters["DiffuseColor"].SetValue(_world.LightInfo.Diffuse);
            
            // FIXME: Where I put the light? O_O
            //_effect.Parameters["LightPosition"].SetValue(_world.LightInfo.Position);
            _effect.Parameters["LightPosition"].SetValue(new Vector3(-1000, 2000, -1000));

            BuildLightmaps();

            return true;
        }

        private void BuildLightmaps()
        {
            int w = (int)Math.Floor(Math.Sqrt(_ground.Lightmaps.Length));
            int h = (int)Math.Ceiling((float)_ground.Lightmaps.Length / w);

            byte[] shadow = new byte[7 * 7 * w * h];
            byte[] color = new byte[7 * 7 * w * h * 4];

            int x = 0, y = 0;
            for (int i = 0; i < _ground.Lightmaps.Length; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int offset = y * w * 7 * 7 + j * w * 7 + x * 7;

                    Buffer.BlockCopy(_ground.Lightmaps[i].Brightness, j * 8, shadow, offset, 7);

                    for (int n = 0; n < 7; n++)
                    {
                        color[offset * 4 + n * 4 + 0] = _ground.Lightmaps[i].Intensity[j * 8 + n * 3 + 0];
                        color[offset * 4 + n * 4 + 1] = _ground.Lightmaps[i].Intensity[j * 8 + n * 3 + 1];
                        color[offset * 4 + n * 4 + 2] = _ground.Lightmaps[i].Intensity[j * 8 + n * 3 + 2];
                        color[offset * 4 + n * 4 + 3] = 255;
                    }
                }

                y++;
                if (y >= h)
                {
                    y = 0;
                    x++;
                }
            }

            _shadowLightmap = new Texture2D(_graphicsDevice, w * 7, h * 7, false, SurfaceFormat.Alpha8);
            _shadowLightmap.SetData(shadow);
            _effect.Parameters["ShadowLightmap"].SetValue(_shadowLightmap);

            _colorLightmap = new Texture2D(_graphicsDevice, w * 7, h * 7, false, SurfaceFormat.Color);
            _colorLightmap.SetData(color);
            _effect.Parameters["ColorLightmap"].SetValue(_colorLightmap);
        }

        public void Update(GameTime gametime)
        {
            _world.Update(gametime);
        }

        public void Draw(GameTime gametime, Matrix view, Matrix projection, Matrix world)
        {
            _effect.Parameters["View"].SetValue(view);
            _effect.Parameters["Projection"].SetValue(projection);
            _effect.Parameters["World"].SetValue(world);

            _effect.CurrentTechnique = _effect.Techniques["MapGround"];
            _ground.Draw(_effect);

            _effect.CurrentTechnique = _effect.Techniques["Water"];
            _world.Draw(_effect);
        }
    }
}
