using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns; 

public class FragileColumn : ActivableColumn {
    private bool _broken;

    public FragileColumn(RopeGame game, World world, Vector2 center, float radius, Texture2D texture) : base(game,
        world, center, radius, texture) {
        _broken = false;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var dstRec = camera.getScreenRectangle(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2, true);
        if (_broken)
            //TODO: broken texture
            batch.Draw(ColumnTexture, dstRec, null, Color.Orange, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
        else
            batch.Draw(ColumnTexture, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(dstRec.Y + dstRec.Height));
    }

    public void Break() {
        _broken = true;
        Body.Enabled = false;
    }
}