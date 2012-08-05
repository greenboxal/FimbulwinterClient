using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core;
using System.IO;
using FimbulwinterClient.GameModes;
using Axiom.Core;
using Axiom.Framework;
using Axiom.FileSystem;
using FimbulwinterClient.Core.Content;

namespace FimbulwinterClient
{
    public class Ragnarok : Game
    {
        public static Ragnarok Instance { get; private set; }

        private SceneNode _gameMode;
        public SceneNode GameMode
        {
            get { return _gameMode; }
        }

        public Ragnarok()
        {
            Instance = this;
        }

        /*public void Run()
        {
            using (var engine = new Root("Ragnarök.log"))
            {
                engine.RenderSystem = engine.RenderSystems[0];
                
                using (var renderWindow = engine.Initialize(true, "Ragnarök Online"))
                {
                    SharedInformation.Engine = engine;
                    SharedInformation.Window = renderWindow;

                    Initialize();
                    CreateScene();

                    engine.FrameRenderingQueued += OnRenderFrame;
                    engine.StartRendering();

                    OnUnload();
                }
            }
        }*/

        public void OnLoad()
        {
            ArchiveManager.Instance.AddArchiveFactory(new GrfArchiveFactory());
            ResourceGroupManager.Instance.AddResourceLocation("data.grf", "GrfFile");

            ResourceGroupManager.Instance.InitializeAllResourceGroups();
            /*try
            {
                Stream s = ArchiveManager.Instance.Load(@"data\fb\config\config.xml", "File");
                SharedInformation.Config = Configuration.FromStream(s);
                s.Close();
            }
            catch
            {
                SharedInformation.Config = new Configuration();
            }

            SharedInformation.Config.ReadConfig();

            ChangeMap("prontera");*/
        }

        public void OnRenderFrame(object sender, FrameEventArgs e)
        {

        }

        public override void CreateScene()
        {
            
        }

        public void OnUnload()
        {

        }

        public void ChangeMap(string mapname)
        {
            //ChangeGameMode(new LoadingGameMode(mapname));
        }

        public void ChangeGameMode(SceneNode gameMode)
        {
            if (_gameMode != null)
            {
                _gameMode.RemoveAndDestroyAllChildren();
            }

            _gameMode = gameMode;
        }

        public void Dispose()
        {
            
        }
    }
}
