using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.Screens
{
    public class LoadingScreen : IGameScreen
    {
        // Text
        private SpriteFont _font;
        private List<string> _log;

        // Progress
        private string _progressDot;
        private int _totalProgress;
        private float _progressX;

        // Map
        private string _mapName;
        private int _state;
        private Map _map;

        public event Action<Map> Loaded;

        public LoadingScreen(string mapname)
        {
            _font = ROClient.Singleton.GuiManager.Client.Content.Load<SpriteFont>("fb\\Gulim8b");
            _log = new List<string>();

            _progressDot = "";
            _totalProgress = 0;
            _progressX = _font.MeasureString("Loading.....").X + 10;

            _mapName = mapname;
            _state = 0;
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Begin();
            sb.DrawString(_font, "Loading" + _progressDot, new Vector2(10, 10), Color.White);
            sb.DrawString(_font, " " + _totalProgress + "%", new Vector2(_progressX, 10), Color.White);

            float y = 30;
            for (int i = _log.Count > 55 ? _log.Count - 55 : 0; i < _log.Count; i++)
            {
                Vector2 size = _font.MeasureString(_log[i]);

                sb.DrawString(_font, _log[i], new Vector2(10, y), Color.White);

                y += size.Y;
            }

            sb.End();
        }

        double _totalMS = 0;
        public virtual void Update(SpriteBatch sb, GameTime gameTime)
        {
            _totalMS += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_state == 0)
            {
                new Thread(_Load).Start();
                _state++;
            }
            else if (_state == 1)
            {
                if (_totalMS > 300)
                {
                    _totalMS -= 300;
                    _progressDot += ".";

                    if (_progressDot.Length > 5)
                    {
                        _progressDot = "";
                    }
                }
            }
            else if (_state == 2)
            {
                if (Loaded != null)
                    Loaded(_map);
            }
        }

        private void _Load()
        {
            Map.ReportStatus += Map_ReportStatus;
            Map.ReportProgress += Map_ReportProgress;

            _map = ROClient.Singleton.ContentManager.LoadContent<Map>(@"data\" + _mapName + ".gat");
            _state++;

            Map.ReportStatus -= Map_ReportStatus;
            Map.ReportProgress -= Map_ReportProgress;
        }

        private void Map_ReportProgress(int obj)
        {
            _totalProgress = obj;
        }

        private void Map_ReportStatus(string obj)
        {
            _log.Add(obj);
        }

        public virtual void Dispose()
        {
        }
    }
}
