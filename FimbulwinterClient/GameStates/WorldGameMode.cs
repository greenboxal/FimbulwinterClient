using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.GameStates
{
    public class WorldGameState : GameState
    {
        public string WorldName { get; private set; }
        public Texture2D Texture { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public WorldGameState(string worldName)
        {
            WorldName = worldName;
        }

        public override void Setup()
        {
            SpriteBatch = new SpriteBatch();
            Texture = ContentManager.Instance.Load<Texture2D>(@"data\texture\rag_logo.bmp");
        }

        public override void Update(FrameEventArgs e)
        {
            
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            SpriteBatch.Begin();
            SpriteBatch.Draw(Texture, new Vector2(100, 100), new Vector2(100, 100), Color.Wheat);
            SpriteBatch.End();
        }

        public override void Shutdown()
        {
            
        }
    }
}