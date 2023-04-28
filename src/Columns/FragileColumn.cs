using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns;

public class FragileColumn : ActivableColumn {
    private readonly Texture2D _brokenTexture;
    private bool _broken;

    public FragileColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D brokenTexture) : base(world, position, width, texture) {
        _brokenTexture = brokenTexture;
        _broken = false;
    }
    
    public FragileColumn(World world, Vector2 position, float width, Texture2D texture, Texture2D brokenTexture, bool isSpear) : base(world, position, width, texture, isSpear) {
        _brokenTexture = brokenTexture;
        _broken = false;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var screenRec = camera.getScreenRectangle(Position.X - SpriteSize.X / 2f, Position.Y, SpriteSize.X, SpriteSize.Y);
        screenRec.Y -= (int)(screenRec.Height * OcclusionHeightFactor);

        if (_broken)
            batch.Draw(_brokenTexture, screenRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor));
        else
            batch.Draw(ColumnTexture, screenRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor));
    }

    public void Break() {
        _broken = true;
        Body.Enabled = false;
    }
}