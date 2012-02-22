using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FimbulwinterClient.IO;
using FimbulwinterClient.Audio;

namespace FimbulwinterClient
{
    public class ROClient : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ROConfig cfg;
        ROContentManager contentManager;
        BGMManager bgmManager;
        EffectManager effectManager;

        public ROClient()
        {
            graphics = new GraphicsDeviceManager(this);

            cfg = new ROConfig();
            contentManager = new ROContentManager(this);
            bgmManager = new BGMManager(this, cfg);
            effectManager = new EffectManager(this, cfg);
        }

        protected override void Initialize()
        {
            base.Initialize();

            bgmManager.PlayBGM("01");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);



            base.Draw(gameTime);
        }
    }
}
