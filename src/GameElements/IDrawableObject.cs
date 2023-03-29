﻿using Microsoft.Xna.Framework;
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
        public void Draw(GameTime gameTime, SpriteBatch batch);
    }
}
