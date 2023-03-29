using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.GameElements
{
    internal interface ITwoPhaseDrawableObject
    {
        public void DrawFirst(GameTime gameTime, SpriteBatch batch);

        public void DrawSecond(GameTime gameTime, SpriteBatch batch);
    }
}
