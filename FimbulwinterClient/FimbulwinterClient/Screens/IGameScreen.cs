using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient
{
    public interface IGameScreen : IDisposable
    {
        void Draw(SpriteBatch sb, GameTime gameTime);
        void Update(SpriteBatch sb, GameTime gameTime);
    }
}
