using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns; 

internal class ElectricColumn : ActivableColumn {
    public ElectricColumn(World world, Vector2 position, float width, Texture2D texture) : base(world, position, width, texture) { }
    
    public ElectricColumn(World world, Vector2 position, float width, Texture2D texture, bool isSpear) : base(world, position, width, texture, isSpear) { }
}