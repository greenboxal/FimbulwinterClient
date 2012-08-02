using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.Core.Content;

namespace FimbulwinterClient.Core
{
    public class SharedInformation
    {
        public static void Initialize(IServiceProvider serviceProvider, GraphicsDevice graphicsDevice)
        {
            ContentManager = new AdvancedContentManager(serviceProvider, graphicsDevice);
        }

        public static AdvancedContentManager ContentManager { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }
    }
}
