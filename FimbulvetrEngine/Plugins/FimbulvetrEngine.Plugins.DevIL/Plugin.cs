using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.Plugin;
using Tao.DevIl;

namespace FimbulvetrEngine.Plugins.DevIL
{
    public class Plugin : IPlugin
    {
        public string Name
        {
            get { return "DevIL Image Loader Plugin"; }
        }

        public bool Initialize()
        {
            Il.ilInit();
            Ilut.ilutRenderer(Ilut.ILUT_OPENGL);

            TextureManager.Instance.RegisterTextureLoader(new DevILTextureLoader());

            return true;
        }

        public void Shutdown()
        {
            Il.ilShutDown();
        }
    }
}
