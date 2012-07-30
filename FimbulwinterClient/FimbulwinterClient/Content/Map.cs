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
        public static event Action<string> ReportStatus;
        public static event Action<int> ReportProgress;
        public static int TabLevel { get; set; }

        public static void OnReportStatus(string s, params object[] args)
        {
            for (int i = 0; i < TabLevel; i++)
                s = "    " + s;

            if (ReportStatus != null)
                ReportStatus(string.Format(s, args));
        }

        public static void OnReportProgress(int progress)
        {
            if (ReportProgress != null)
                ReportProgress(progress);
        }

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

        private Texture2D _lightmap;
        public Texture2D Lightmap
        {
            get { return _lightmap; }
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
            OnReportProgress(0);

            OnReportProgress(5);
            OnReportStatus("Loading altitude...");
            _altitude = new Altitude();
            if (!_altitude.Load(gat))
                return false;

            OnReportProgress(10);
            OnReportStatus("Loading ground...");
            _ground = new Ground(_graphicsDevice);
            if (!_ground.Load(gnd))
                return false;

            OnReportProgress(30);
            OnReportStatus("Loading world...");
            _world = new World(_graphicsDevice);
            if (!_world.Load(rsw, this))
                return false;

            OnReportProgress(90);
            OnReportStatus("Building lightmaps...");
            BuildLightmaps();

            OnReportProgress(99);

            _effect.Parameters["Lightmap"].SetValue(_lightmap);

            _effect.Parameters["AmbientColor"].SetValue(_world.LightInfo.Ambient);
            _effect.Parameters["DiffuseColor"].SetValue(_world.LightInfo.Diffuse);

            // FIXME: Where I put the light? O_O
            //_effect.Parameters["LightPosition"].SetValue(_world.LightInfo.Position);
            _effect.Parameters["LightPosition"].SetValue(new Vector3(-1000, 2000, -1000));

            OnReportProgress(100);

            return true;
        }

        private void BuildLightmaps()
        {
            int w = (int)Math.Floor(Math.Sqrt(_ground.Lightmaps.Length));
            int h = (int)Math.Ceiling((float)_ground.Lightmaps.Length / w);

            Color[] color = new Color[7 * 7 * w * h];

            int x = 0, y = 0;
            for (int i = 0; i < _ground.Lightmaps.Length; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int offset = y * w * 7 * 7 + j * w * 7 + x * 7;

                    for (int n = 0; n < 7; n++)
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

            _lightmap = new Texture2D(_graphicsDevice, w * 7, h * 7, false, SurfaceFormat.Color);
            _lightmap.SetData(color);
        }

        public void Update(GameTime gametime)
        {
            _world.UpdateWater(gametime);
            _world.UpdateModels(gametime);
        }

        public void Draw(GameTime gametime, Matrix view, Matrix projection, Matrix world)
        {
            _effect.Parameters["View"].SetValue(view);
            _effect.Parameters["Projection"].SetValue(projection);
            _effect.Parameters["World"].SetValue(world);

            _effect.CurrentTechnique = _effect.Techniques["MapGround"];
            _ground.Draw(_effect);

            _effect.CurrentTechnique = _effect.Techniques["Water"];
            _world.DrawWater(_effect);

            _world.DrawModels(view, projection, world);
        }
    }
}
