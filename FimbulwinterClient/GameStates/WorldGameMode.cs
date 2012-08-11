using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core;
using FimbulwinterClient.Core.Content;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using QuickFont;

namespace FimbulwinterClient.GameStates
{
    public class WorldGameState : GameState
    {
        public string WorldName { get; private set; }
        public Map World { get; private set; }

        public WorldGameState(string worldName)
        {
            WorldName = worldName;
        }

        public override void Setup()
        {
            World = ContentManager.Instance.Load<Map>(@"data\" + WorldName + ".gat");
        }

        public override void Update(FrameEventArgs e)
        {
            
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            
        }

        public override void Shutdown()
        {
            
        }
    }
}