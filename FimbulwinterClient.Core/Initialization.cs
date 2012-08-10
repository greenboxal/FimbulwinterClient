using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.IO;
using FimbulwinterClient.Core.IO;

namespace FimbulwinterClient.Core
{
    public class Initialization
    {
        public static void DoInit()
        {
            FileSystemManager.Instance.RegisterFileSystemFactory(new GrfFileSystemFactory());
        }
    }
}