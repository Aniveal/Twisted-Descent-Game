using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns
{
    internal class ElectricColumn : ActivableColumn
    {
        public ElectricColumn(RopeGame game, World world, Vector2 center, float radius, Texture2D texture) : base(game, world, center, radius, texture)
        {

        }

        //TODO: remove this override once we have separate sprite for elec column
        public override void DrawFirst(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            Rectangle dstRec = camera.getScreenRectangle(_center.X - _radius, _center.Y - _radius, _radius * 2, _radius*2, true);
            
            batch.Draw(_columnTexture, dstRec, Color.Yellow);
        }
    }
}
