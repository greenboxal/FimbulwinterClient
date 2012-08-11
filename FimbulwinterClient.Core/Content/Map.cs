using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core.Content.MapInternals;
using OpenTK;

namespace FimbulwinterClient.Core.Content
{
    public class Map
    {
        public Ground Ground { get; private set; }
        public Altitude Altitude { get; private set; }
        public World World { get; private set; }
        public Texture2D Lightmap { get; private set; }

        public Map()
        {
        }

        public bool Load(Stream gat, Stream gnd, Stream rsw)
        {
            Altitude = new Altitude();
            if (!Altitude.Load(gat))
                return false;

            Ground = new Ground();
            if (!Ground.Load(gnd))
                return false;

            World = new World(this);
            if (!World.Load(rsw))
                return false;

            return true;
        }

        private void BuildLightmaps()
        {
            // Nothing to be built
            /*if (_ground.Lightmaps.Length == 0)
                return;

            int w = (int)Math.Floor(Math.Sqrt(_ground.Lightmaps.Length));
            int h = (int)Math.Ceiling((float)_ground.Lightmaps.Length / w);

            Color[] color = new Color[8 * 8 * w * h];

            int x = 0, y = 0;
            for (int i = 0; i < _ground.Lightmaps.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int offset = y * w * 8 * 8 + j * w * 8 + x * 8;

                    for (int n = 0; n < 8; n++)
                    {
                        color[offset + n] = _ground.Lightmaps[i].Intensity[j * 8 + n];
                        color[offset + n].A = _ground.Lightmaps[i].Brightness[j * 8 + n];
                    }
                }

                y++;
                if (y >= h)
                {
                    y = 0;
                    x++;
                }
            }

            _lightmap = new Texture2D(_graphicsDevice, w * 8, h * 8, false, SurfaceFormat.Color);
            _lightmap.SetData(color);*/
        }

        public void Update(double elapsed)
        {
            //_world.UpdateWater(elapsed);
            //_world.UpdateModels(elapsed);
        }

        public void Draw(double elapsed, Matrix4 view, Matrix4 projection, Matrix4 world)
        {
            /*_effect.Parameters["View"].SetValue(view);
            _effect.Parameters["Projection"].SetValue(projection);
            _effect.Parameters["World"].SetValue(world);

            _effect.CurrentTechnique = _effect.Techniques["MapGround"];
            _ground.Draw(_effect);

            _effect.CurrentTechnique = _effect.Techniques["Water"];
            _world.DrawWater(_effect);

            _effect.CurrentTechnique = _effect.Techniques["Model"];
            _world.DrawModels(_effect);*/
        }
    }
}
