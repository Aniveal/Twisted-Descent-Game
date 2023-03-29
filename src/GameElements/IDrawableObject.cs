using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.GameElements
{
    internal interface IDrawableObject
    {
        //TODO: we will probably want to add the camera tranform as an argument to the draw method
        public void Draw(GameTime gameTime, SpriteBatch batch);
    }
}
