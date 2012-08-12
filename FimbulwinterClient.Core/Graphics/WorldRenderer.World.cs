using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulwinterClient.Core.Content.MapInternals;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Graphics
{
    public partial class WorldRenderer
    {
        public void LoadWorld(bool background)
        {
            foreach (World.ModelObject mo in Map.World.Models)
            {
                mo.SetModel(ContentManager.Instance.Load<RsmModel>(@"data\model\" + mo.ModelName, background));
            }
        }
    }
}
