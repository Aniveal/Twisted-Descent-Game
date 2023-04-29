using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns;

public class Column : DrawableGameElement {
    public Body Body;
    protected Vector2 Position;
    protected Texture2D ColumnTexture;
    protected float Width;
    protected World World;
    private bool isSpear;

    protected float ColumnDiameterFactor = 0.49f;
    protected float SpearDiameterFactor = 0.3f;
    protected float SpearOffsetFactor = 0.09f;
    protected float OcclusionHeightFactor = 0.78f;

    protected Vector2 SpriteSize;

    public Column(World world, Vector2 position, float width, Texture2D texture) {
        World = world;
        Position = position;
        Width = width;
        ColumnTexture = texture;
        Body = World.CreateCircle(Width * ColumnDiameterFactor / 2f, 0, Position);
        isSpear = false;

        SpriteSize = new Vector2(width, width * texture.Height / texture.Width);
    }

    public Column(World world, Vector2 position, float width, Texture2D texture, bool isSpear) {
        World = world;
        Position = position;
        Width = width;
        ColumnTexture = texture;
        this.isSpear = isSpear;

        SpriteSize = new Vector2(width, width * texture.Height / texture.Width);
        
        if (this.isSpear) {
            Vector2 center = new Vector2(Position.X + SpriteSize.X * SpearOffsetFactor, Position.Y);
            Body = World.CreateCircle(Width * SpearDiameterFactor / 2f, 0, center);
        } else {
            Body = World.CreateCircle(Width * ColumnDiameterFactor / 2f, 0, Position);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var screenRec =
            camera.getScreenRectangle(Position.X - SpriteSize.X / 2f, Position.Y, SpriteSize.X, SpriteSize.Y);
        screenRec.Y -= (int)(screenRec.Height * OcclusionHeightFactor);

        batch.Draw(ColumnTexture, screenRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
            camera.getLayerDepth(screenRec.Y + screenRec.Height * OcclusionHeightFactor));
    }
}