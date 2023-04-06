using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.GameElements
{
    public class TwoPhaseGameElement : IUpdatableObject, ITwoPhaseDrawableObject
    {

        public virtual void Initialize()
        {

        }
        public virtual void DrawFirst(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            
        }

        public virtual void DrawSecond(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }
    }
}
