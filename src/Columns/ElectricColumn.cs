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

        public ElectricColumn(RopeGame game, World world, Vector2 center, float radius, ColumnTextures texture) : base(game, world, center, radius, texture)
        {

        }
    }
}
