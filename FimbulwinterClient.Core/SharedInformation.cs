using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core.Content;
using FimbulwinterClient.Core.Config;
using IrrlichtLime.Video;
using IrrlichtLime;
using IrrlichtLime.Scene;
using IrrlichtLime.GUI;

namespace FimbulwinterClient.Core
{
    public class SharedInformation
    {
        static SharedInformation()
        {
            ContentManager = new AdvancedContentManager();
        }

        public static void Initialize(IrrlichtDevice device)
        {
            Device = device;
            Graphics = device.VideoDriver;
            Scene = device.SceneManager;
            GUI = device.GUIEnvironment;
            Logger = device.Logger;
        }

        public static AdvancedContentManager ContentManager { get; set; }
        public static Configuration Config { get; set; }

        public static IrrlichtDevice Device { get; set; }
        public static VideoDriver Graphics { get; set; }
        public static SceneManager Scene { get; set; }
        public static GUIEnvironment GUI { get; set; }
        public static Logger Logger { get; set; }
    }
}
