using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.IO;
using FimbulwinterClient.Core.Content;
using FimbulwinterClient.Core.Content.Loaders;
using FimbulwinterClient.Core.Graphics;
using FimbulwinterClient.Core.IO;

namespace FimbulwinterClient.Core
{
    public class Initialization
    {
        public static void DoInit()
        {
            FileSystemManager.Instance.RegisterFileSystemFactory(new GrfFileSystemFactory());

            ContentManager.Instance.RegisterLoader<WorldRenderer>(new MapLoader());
            ContentManager.Instance.RegisterLoader<RsmModel>(new RsmModelLoader());
        }
    }
}