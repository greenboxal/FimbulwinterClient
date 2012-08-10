using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.Plugin;
using OpenTK;

namespace FimbulvetrEngine.Framework
{
    public class Game : GameWindow
    {
        public Game()
        {
            if (Vetr.Instance == null) 
                new Vetr();
        }

        protected override void OnLoad(EventArgs e)
        {
            ReadConfiguration();
            Initialize();
            LoadContent();
        }

        protected override void OnUnload(EventArgs e)
        {
            UnloadContent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Shutdown(e);
        }

        protected virtual void ReadConfiguration()
        {
            Vetr.Instance.ReadConfiguration();
            PluginManager.Instance.LoadPlugins();
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void LoadContent()
        {

        }

        protected virtual void UnloadContent()
        {

        }

        protected virtual void Shutdown(CancelEventArgs e)
        {

        }
    }
}
