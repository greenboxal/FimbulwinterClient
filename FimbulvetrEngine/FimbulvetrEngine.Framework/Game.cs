using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.IO;
using FimbulvetrEngine.Plugin;
using OpenTK;
using OpenTK.Graphics;

namespace FimbulvetrEngine.Framework
{
    public class Game : GameWindow
    {
        public static Game Instance { get; private set; }

        public Game()
        {
            if (Instance != null)
                throw new Exception("This class can have only one instance, use Game.Instance.");

            if (Vetr.Instance == null)
                new Vetr();

            Instance = this;
            Vetr.Instance.Window = this;
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
            FileSystemManager.Instance.LoadAll();

            ReadGameConfig();
        }

        protected void ReadGameConfig()
        {
            XElement window = Vetr.Instance.ConfiguartionRoot.Element("Window");

            if (window != null)
            {
                ClientSize = new Size(int.Parse((string)window.Attribute("Width") ?? "800"), int.Parse((string)window.Attribute("Height") ?? "600"));
                Title = (string)window.Attribute("Title") ?? "Ragnarök Online - Fimbulwinter Client";

                // TODO: What should I do here?
                X = 100;
                Y = 100;
            }
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
